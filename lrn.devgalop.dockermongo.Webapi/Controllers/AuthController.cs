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
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost,AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody]LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
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

        [HttpPost("refresh"), AllowAnonymous]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var token = Request.Headers["Authorization"]
                                .FirstOrDefault()?
                                .Split(" ")
                                .Last();
                if(token is null) throw new Exception("User token is invalid");
                var refreshToken = Request.Cookies
                                        .Where(c => c.Key == "jwtRefreshToken")
                                        .Select(c => c.Value)
                                        .FirstOrDefault();
                if(refreshToken is null) throw new Exception("Refresh token is invalid");
                
                var tokenResponse = await _authService.RefreshTokenAsync(token, refreshToken);
                if(!tokenResponse.IsSucceed)throw new Exception(tokenResponse.ErrorMessage);
                Response.Cookies.Append("jwtRefreshToken",tokenResponse.RefreshToken,new CookieOptions()
                {
                    HttpOnly = true,
                    Expires = tokenResponse.RefreshTokenExpiration,
                    IsEssential = true
                });
                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}