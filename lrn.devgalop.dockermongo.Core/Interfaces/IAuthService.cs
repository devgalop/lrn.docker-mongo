using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lrn.devgalop.dockermongo.Core.Models;

namespace lrn.devgalop.dockermongo.Core.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Generate Jwt token and refresh token by logging into the application
        /// </summary>
        /// <param name="request">Login information</param>
        /// <returns></returns>
        Task<AuthResponse> LoginAsync(LoginRequest request);

        /// <summary>
        /// Generate new token and refresh token
        /// </summary>
        /// <param name="token">User jwt token</param>
        /// <param name="refreshToken">User refresh token</param>
        /// <returns></returns>
        Task<AuthResponse> RefreshTokenAsync(string token, string refreshToken);
    }
}