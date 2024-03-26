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
    public class UserController : ControllerBase
    {
        private readonly IUserManagementService _userService;

        public UserController(IUserManagementService userService)
        {
            _userService = userService;
        }

        [HttpPost]
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
                return BadRequest(ex);
            }
        }
    }
}