using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lrn.devgalop.dockermongo.Infrastructure.Data.Models;

namespace lrn.devgalop.dockermongo.Infrastructure.Data.Interfaces
{
    public interface IRepository
    {
        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="user">User model</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<BaseResponse> InsertAsync(User user, CancellationToken ct = default);

        /// <summary>
        /// Returns a user found in the database
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<UserResponse> GetUserAsync(string username, CancellationToken ct = default);

        /// <summary>
        /// Update a user's generated tokens and TOTP code
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="auth">Authentication model (refresh token and totp code)</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<BaseResponse> UpdateAuthAsync(string username, UserAuth auth, CancellationToken ct = default);

        /// <summary>
        /// Remove the refresh token or totp code associated with a user.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<BaseResponse> RevokeAuthAsync(string username, CancellationToken ct = default);

        /// <summary>
        /// Non-removal user deactivation
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<BaseResponse> DisableUserAsync(string username, CancellationToken ct = default);

        /// <summary>
        /// Update user password
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="passwordHashed">Password encrypted</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<BaseResponse> ChangePassword(string username, string passwordHashed, CancellationToken ct = default);
    }
}