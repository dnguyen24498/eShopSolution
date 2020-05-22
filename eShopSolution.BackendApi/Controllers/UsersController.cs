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
        public async Task<IActionResult> Login([FromForm] LoginUserRequest request)
        {
            if (!ModelState.IsValid) return BadRequest();
            var token = await userService.Login(request);
            if (token!=null) return Ok(token);
            return BadRequest();
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterUserRequest request)
        {
            var isSuccess = await userService.Register(request);
            if (isSuccess) return Ok();
            return BadRequest();
        }
    }
}