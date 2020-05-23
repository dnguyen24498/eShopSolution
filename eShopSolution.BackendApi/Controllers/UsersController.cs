using System.Threading.Tasks;
using eShopSolution.Application.System;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
        {
            if (!ModelState.IsValid) return BadRequest();
            var resultObj = await userService.Login(request);
            if (resultObj.IsSuccess) return Ok(resultObj);
            return BadRequest(resultObj);
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            var returnObj = await userService.Register(request);
            if (returnObj.IsSuccess) return Ok(returnObj);
            return BadRequest(returnObj);
        }
    }
}