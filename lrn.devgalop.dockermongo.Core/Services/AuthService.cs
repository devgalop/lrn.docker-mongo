using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using lrn.devgalop.dockermongo.Core.Interfaces;
using lrn.devgalop.dockermongo.Core.Models;
using lrn.devgalop.dockermongo.Infrastructure.Data.Interfaces;
using lrn.devgalop.dockermongo.Infrastructure.Data.Models;
using lrn.devgalop.dockermongo.Infrastructure.Security.EncryptDecrypt.Interfaces;
using lrn.devgalop.dockermongo.Infrastructure.Security.EncryptDecrypt.Models;
using lrn.devgalop.dockermongo.Infrastructure.Security.JWT.Interfaces;
using lrn.devgalop.dockermongo.Infrastructure.Security.JWT.Models;
using Microsoft.IdentityModel.Tokens;

namespace lrn.devgalop.dockermongo.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository _repository;
        private readonly IAesCryptService _cryptService;
        private readonly AesCryptType _cryptConfig;
        private readonly ITokenFactoryService _tokenFactory;
        private readonly TokenConfiguration _tokenConfiguration;

        public AuthService(
            IRepository repository,
            IAesCryptService cryptService,
            AesCryptType cryptConfig,
            ITokenFactoryService tokenFactory,
            TokenConfiguration tokenConfiguration
            )
        {
            _repository = repository;
            _cryptService = cryptService;
            _cryptConfig = cryptConfig;
            _tokenFactory = tokenFactory;
            _tokenConfiguration = tokenConfiguration;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                List<ValidationResult> validationResults = new();
                if(!Validator.TryValidateObject(request,new ValidationContext(request), validationResults, true))
                {
                    string errors = string.Join(",", validationResults.Where(r => !string.IsNullOrEmpty(r.ErrorMessage)).Select(r => r.ErrorMessage));
                    throw new Exception($"Invalid model. {errors}");
                }
                
                var userResponse = await _repository.GetUserAsync(request.Username);
                if(!userResponse.IsSucceed || userResponse.Result is null) throw new Exception($"Could not find the user '{request.Username}' in database. {userResponse.ErrorMessage}");
                
                var cryptResponse = _cryptService.Encrypt(request.Password, _cryptConfig);
                if(!cryptResponse.IsSucceed || string.IsNullOrEmpty(cryptResponse.Text)) throw new Exception($"Password encryption failed. {cryptResponse.ErrorMessage}");

                var userFound = userResponse.Result;
                if(userFound.Password != cryptResponse.Text) throw new Exception($"Username or password are incorrect.");

                
                List<ClaimRequest> claims = new()
                {
                    new(){ Type = "user", Value = userFound.Username },
                    new(){ Type = "role", Value = userFound.RoleId.ToString()},
                };
                var tokenResponse = _tokenFactory.GenerateToken(_tokenConfiguration.SecretKey, claims);
                if(!tokenResponse.IsSucceed || tokenResponse.Token is null) throw new Exception($"Could not create a token. {tokenResponse.ErrorMessage}");   
                
                var refreshTokenResponse = _tokenFactory.GenerateRefreshToken(60);
                if(!refreshTokenResponse.IsSucceed || refreshTokenResponse.Token is null)throw new Exception($"Could not create a refresh token. {refreshTokenResponse.ErrorMessage}");

                var updateResponse = await _repository.UpdateAuthAsync(userFound.Username, new()
                {
                    RefreshToken = refreshTokenResponse.Token,
                    ExpirationRefreshToken = refreshTokenResponse.Expiration
                });
                if(!updateResponse.IsSucceed)throw new Exception($"User token cannot be saved. {updateResponse.ErrorMessage}");

                return new()
                {
                    IsSucceed = true,
                    Token = tokenResponse.Token,
                    TokenExpiration = tokenResponse.Expiration,
                    RefreshToken = refreshTokenResponse.Token,
                    RefreshTokenExpiration = refreshTokenResponse.Expiration
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    IsSucceed = false,
                    ErrorMessage = ex.Message,
                    ErrorDescription = ex.ToString()
                };
            }
        }

        public async Task<AuthResponse> RefreshTokenAsync(string token, string refreshToken)
        {
            try
            {
                if(string.IsNullOrEmpty(token)) throw new Exception("Invalid token");
                if(string.IsNullOrEmpty(refreshToken)) throw new Exception("Invalid refresh token");

                var signingKey = new SymmetricSecurityKey(_tokenConfiguration.GetSigningKey(_tokenConfiguration.SecretKey));
                var tokenValidationParams = new TokenValidationParameters()
                {
                    ValidateAudience = _tokenConfiguration.ValidateAudience,
                    ValidateIssuer = _tokenConfiguration.ValidateIssuer,
                    ValidateLifetime = _tokenConfiguration.ValidateLifeTime,
                    ValidateIssuerSigningKey = _tokenConfiguration.ValidateIssuerSigningKey,
                    IssuerSigningKey = signingKey,
                    ClockSkew = TimeSpan.Zero
                };
                var claimsFromTokenResponse = _tokenFactory.GetClaimsFromExpiredToken(token, tokenValidationParams);
                if (!claimsFromTokenResponse.IsSucceed) throw new Exception("Couldn't extract claims from token");

                var claimsFromToken = claimsFromTokenResponse.Claims;
                var claimUser = claimsFromToken.Where(c => c.Type == "user").Select(c => c.Value).FirstOrDefault();
                var claimRole = claimsFromToken.Where(c => c.Type == "role").Select(c => c.Value).FirstOrDefault();
                if (string.IsNullOrEmpty(claimUser)|| string.IsNullOrEmpty(claimRole)) throw new Exception("Invalid token");

                var userResponse = await _repository.GetUserAsync(claimUser);
                if(!userResponse.IsSucceed || userResponse.Result is null) throw new Exception("Invalid token");
                var userFound = userResponse.Result;
                var role = Enum.GetName(typeof(ERole), userFound.RoleId);
                if(userFound.Username != claimUser 
                    || role != claimRole) throw new Exception("Invalid token");

                List<ClaimRequest> claims = new()
                {
                    new(){ Type = "user", Value = claimUser },
                    new(){ Type = "role", Value = claimRole},
                };
                var tokenResponse = _tokenFactory.GenerateToken(_tokenConfiguration.SecretKey, claims);
                if(!tokenResponse.IsSucceed || tokenResponse.Token is null) throw new Exception($"Could not create a token. {tokenResponse.ErrorMessage}");   
                
                var refreshTokenResponse = _tokenFactory.GenerateRefreshToken(60);
                if(!refreshTokenResponse.IsSucceed || refreshTokenResponse.Token is null)throw new Exception($"Could not create a refresh token. {refreshTokenResponse.ErrorMessage}");

                var updateResponse = await _repository.UpdateAuthAsync(claimUser, new()
                {
                    RefreshToken = refreshTokenResponse.Token,
                    ExpirationRefreshToken = refreshTokenResponse.Expiration
                });
                if(!updateResponse.IsSucceed)throw new Exception($"User token cannot be saved. {updateResponse.ErrorMessage}");

                return new()
                {
                    IsSucceed = true,
                    Token = tokenResponse.Token,
                    TokenExpiration = tokenResponse.Expiration,
                    RefreshToken = refreshTokenResponse.Token,
                    RefreshTokenExpiration = refreshTokenResponse.Expiration
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    IsSucceed = false,
                    ErrorMessage = ex.Message,
                    ErrorDescription = ex.ToString()
                };
            }
        }
    }
}