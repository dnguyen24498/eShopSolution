using System.Threading.Tasks;
using eShopSolution.Application.System;
using eShopSolution.ViewModels.Common;
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
        
        [HttpGet]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> GetUserPaging([FromQuery] PagingRequestBase request){
            var returnObj=await userService.GetUserPaging(request);
            if(returnObj.IsSuccess) return Ok(returnObj);
            return BadRequest(returnObj.Message);
        }

        [HttpPost("roles")]
        [Authorize(Roles="admin")]
        public async Task<IActionResult> UpdateUserRoles([FromBody] UserRolesViewModel request){
            var result=await userService.UpdateUserRoles(request);
            if(result.IsSuccess) return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpGet("roles/{userName}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUserRolesByUserName(string userName)
        {
            var result = await userService.GetUserRoles(userName);
            if (result != null && result.IsSuccess) return Ok(result);
            return BadRequest(result.Message);
        }
    }
}