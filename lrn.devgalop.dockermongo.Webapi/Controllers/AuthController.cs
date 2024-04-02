using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lrn.devgalop.dockermongo.Core.Interfaces;
using lrn.devgalop.dockermongo.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace lrn.devgalop.dockermongo.Webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserManagementService _userService;

        public AuthController(IUserManagementService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody]LoginRequest request)
        {
            try
            {
                var response = await _userService.LoginAsync(request);
                if(!response.IsSucceed) throw new Exception(response.ErrorMessage);
                Response.Cookies.Append("jwtRefreshToken",response.RefreshToken,new CookieOptions()
                {
                    HttpOnly = true,
                    Expires = response.RefreshTokenExpiration,
                    IsEssential = true
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}