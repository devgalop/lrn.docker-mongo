using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lrn.devgalop.dockermongo.Core.Models;

namespace lrn.devgalop.dockermongo.Core.Interfaces
{
    public interface IUserManagementService
    {
        /// <summary>
        /// Create a user
        /// </summary>
        /// <param name="request">User information</param>
        /// <returns></returns>
        Task<BaseResponse> InsertUserAsync(InsertUserRequest request);
        
        /// <summary>
        /// Find a user by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns></returns>
        Task<BasicUserResponse> GetUserAsync(string username);

        /// <summary>
        /// Generate Jwt token and refresh token by logging into the application
        /// </summary>
        /// <param name="request">Login information</param>
        /// <returns></returns>
        Task<AuthResponse> LoginAsync(LoginRequest request);

        /// <summary>
        /// Unable user without remove
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns></returns>
        Task<BaseResponse> DisableUserAsync(string username);

        /// <summary>
        /// Remove refresh token associated
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns></returns>
        Task<BaseResponse> RevokeAccessAsync(string username);

        /// <summary>
        /// Update password
        /// </summary>
        /// <param name="request">Update information</param>
        /// <returns></returns>
        Task<Models.BaseResponse> ModifyPasswordAsync(ModifyPasswordRequest request);
    }
}