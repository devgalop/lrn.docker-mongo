using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lrn.devgalop.dockermongo.Core.Interfaces;
using lrn.devgalop.dockermongo.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lrn.devgalop.dockermongo.Webapi.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserManagementService _userService;

        public UserController(IUserManagementService userService)
        {
            _userService = userService;
        }

        [HttpPost, Authorize(Policy = "AdminRolePolicy")]
        public async Task<IActionResult> InsertUser([FromBody]InsertUserRequest request)
        {
            try
            {
                var response = await _userService.InsertUserAsync(request);
                if(!response.IsSucceed) throw new Exception(response.ErrorMessage);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{username}"), Authorize(Policy = "BasicRolePolicy")]
        public async Task<IActionResult> GetUser(string username)
        {
            try
            {
                var response = await _userService.GetUserAsync(username);
                if(!response.IsSucceed) throw new Exception(response.ErrorMessage);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("disable/{username}"), Authorize(Policy = "AdminRolePolicy")]
        public async Task<IActionResult> DisableUserAsync(string username)
        {
            try
            {
                if(string.IsNullOrEmpty(username)) throw new ArgumentNullException($"Username cannot be null or empty");
                var response = await _userService.DisableUserAsync(username);
                if(!response.IsSucceed) throw new Exception($"User cannot be deactivated. {response.ErrorMessage}");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("revoke/{username}"), Authorize(Policy = "AdminRolePolicy")]
        public async Task<IActionResult> RevokeAccessAsync(string username)
        {
            try
            {
                if(string.IsNullOrEmpty(username)) throw new ArgumentNullException($"Username cannot be null or empty");
                var response = await _userService.RevokeAccessAsync(username);
                if(!response.IsSucceed) throw new Exception($"User access could not be removed. {response.ErrorMessage}");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("pwd"), Authorize(Policy = "BasicRolePolicy")]
        public async Task<IActionResult> UpdatePassword([FromBody]ModifyPasswordRequest request)
        {
            try
            {
                var response = await _userService.ModifyPasswordAsync(request);
                if(!response.IsSucceed) throw new Exception(response.ErrorMessage);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }
}