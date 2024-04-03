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

namespace lrn.devgalop.dockermongo.Core.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IRepository _repository;
        private readonly IAesCryptService _cryptService;
        private readonly AesCryptType _cryptConfig;
        private readonly ITokenFactoryService _tokenFactory;
        private readonly TokenConfiguration _tokenConfiguration;

        public UserManagementService(
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

        public async Task<Models.BaseResponse> InsertUserAsync(InsertUserRequest request)
        {
            try
            {
                var validationResults = new List<ValidationResult>();
                if(!Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true))
                {
                    string errors = string.Join(",", validationResults.Where(r => !string.IsNullOrEmpty(r.ErrorMessage)).Select(r => r.ErrorMessage));
                    throw new Exception($"Invalid user model. Errors: {errors}");
                }
                var cryptResponse = _cryptService.Encrypt(request.Password, _cryptConfig);
                if(!cryptResponse.IsSucceed || string.IsNullOrEmpty(cryptResponse.Text)) throw new Exception($"Password encryption failed. {cryptResponse.ErrorMessage}");
                var passwordHashed = cryptResponse.Text;

                var userFoundResponse = await _repository.GetUserAsync(request.Username);
                if(userFoundResponse.Result is not null) throw new Exception($"Username {request.Username} already exists");

                var insertResponse = await _repository.InsertAsync(new User()
                {
                    Username = request.Username,
                    Password = passwordHashed,
                    RegistrationDate = request.RegistrationDate,
                    UpdateDate = request.UpdateDate,
                    Status = request.Status,
                    RoleId = ERole.BASIC_AUTH
                });
                if(!insertResponse.IsSucceed) throw new Exception($"User cannot be added to database. {insertResponse.ErrorMessage}");
                return new()
                {
                    IsSucceed = true
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

        public async Task<BasicUserResponse> GetUserAsync(string username)
        {
            try
            {
                if(string.IsNullOrEmpty(username))throw new ArgumentNullException("Username cannot be null or empty");
                var response = await _repository.GetUserAsync(username);
                if(!response.IsSucceed || response.Result is null) throw new Exception($"Could not find user '{username}' in repository. {response.ErrorMessage}");
                return new()
                {
                    IsSucceed = true,
                    Username = response.Result.Username,
                    RegistrationDate = response.Result.RegistrationDate,
                    Status = response.Result.Status,
                    Role = response.Result.RoleId.ToString()
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

        public async Task<Models.BaseResponse> DisableUserAsync(string username)
        {
            try
            {
                if(string.IsNullOrEmpty(username))throw new ArgumentNullException("Username cannot be null or empty");
                var response = await _repository.GetUserAsync(username);
                if(!response.IsSucceed || response.Result is null) throw new Exception($"User {username} does not exist. {response.ErrorMessage}");
                var disableResponse = await _repository.DisableUserAsync(username);
                if(!disableResponse.IsSucceed) throw new Exception($"An unexpected error ocurred during user deactivation. {disableResponse.ErrorMessage}"); 
                return new()
                {
                    IsSucceed = true
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

        public async Task<Models.BaseResponse> RevokeAccessAsync(string username)
        {
            try
            {
                if(string.IsNullOrEmpty(username))throw new ArgumentNullException("Username cannot be null or empty");
                var response = await _repository.GetUserAsync(username);
                if(!response.IsSucceed || response.Result is null) throw new Exception($"User {username} does not exist. {response.ErrorMessage}");
                var revokeResponse = await _repository.RevokeAuthAsync(username);
                return new()
                {
                    IsSucceed = true
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

        public async Task<Models.BaseResponse> ModifyPasswordAsync(ModifyPasswordRequest request)
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
                
                var cryptResponse = _cryptService.Encrypt(request.ActualPassword, _cryptConfig);
                if(!cryptResponse.IsSucceed || string.IsNullOrEmpty(cryptResponse.Text)) throw new Exception($"Password encryption failed. {cryptResponse.ErrorMessage}");

                var userFound = userResponse.Result;
                if(userFound.Password != cryptResponse.Text) throw new Exception($"Username or password are incorrect.");

                cryptResponse = _cryptService.Encrypt(request.NewPassword, _cryptConfig);
                if(!cryptResponse.IsSucceed || string.IsNullOrEmpty(cryptResponse.Text)) throw new Exception($"Password encryption failed. {cryptResponse.ErrorMessage}");

                var response = await _repository.ChangePassword(request.Username, cryptResponse.Text);
                if(!response.IsSucceed) throw new Exception($"Password could not be updated. {response.ErrorMessage}");
   
                return new()
                {
                    IsSucceed = true
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