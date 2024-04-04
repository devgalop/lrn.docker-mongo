using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using lrn.devgalop.dockermongo.Infrastructure.Data.Interfaces;
using lrn.devgalop.dockermongo.Infrastructure.Security.JWT.Interfaces;
using lrn.devgalop.dockermongo.Infrastructure.Security.JWT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace lrn.devgalop.dockermongo.Infrastructure.Security.JWT.Middleware
{
    public class JwtAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITokenFactoryService _tokenFactoryService;
        private readonly TokenConfiguration _tokenConfiguration;
        private readonly IRepository _repository;

        public JwtAuthenticationMiddleware(
            RequestDelegate next,
            ITokenFactoryService tokenFactoryService,
            TokenConfiguration tokenConfiguration,
            IRepository repository)
        {
            _next = next;
            _tokenFactoryService = tokenFactoryService;
            _tokenConfiguration = tokenConfiguration;
            _repository = repository;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var token = httpContext.Request.Headers["Authorization"]
                                .FirstOrDefault()?
                                .Split(" ")
                                .Last();
            var refreshToken = httpContext.Request.Cookies
                                .Where(c => c.Key == "jwtRefreshToken")
                                .Select(c => c.Value)
                                .FirstOrDefault();
            if (token is not null && refreshToken is not null)
            {
                TokenValidationParameters tokenParams = new()
                {
                    //Remember configurations applied in Extensions
                    ValidateAudience = _tokenConfiguration.ValidateAudience,
                    ValidateIssuer = _tokenConfiguration.ValidateIssuer,
                    ValidateLifetime = _tokenConfiguration.ValidateLifeTime,
                    ValidateIssuerSigningKey = _tokenConfiguration.ValidateIssuerSigningKey,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(_tokenConfiguration.GetSigningKey(_tokenConfiguration.SecretKey)),
                };
                var validationResponse = _tokenFactoryService.ValidateToken(token, tokenParams);
                if(!validationResponse.IsSucceed)
                {
                    // TODO: Use claim user to search user in database and compare refresh token
                    var claimsFoundResponse = _tokenFactoryService.GetClaimsFromExpiredToken(token, tokenParams);
                    if(!claimsFoundResponse.IsSucceed)
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    };
                    var claimUser = claimsFoundResponse.Claims.Where(c => c.Type=="user").Select(c => c.Value).FirstOrDefault();
                    if(string.IsNullOrEmpty(claimUser))
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    };
                    var userResponse = await _repository.GetUserAsync(claimUser);
                    if(!userResponse.IsSucceed || userResponse.Result is null)
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    };
                    if(userResponse.Result.Auth is null 
                    || userResponse.Result.Auth.RefreshToken != refreshToken 
                    || userResponse.Result.Auth.ExpirationRefreshToken.ToLocalTime() < DateTime.Now)
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }; 
                }
            }
            await _next(httpContext);
        }

    }
}