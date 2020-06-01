using System.Threading.Tasks;
using eShopSolution.Application.System;
using Microsoft.AspNetCore.Mvc;
using eShopSolution.ViewModels.System.Roles;
using Microsoft.AspNetCore.Authorization;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class RolesController:ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService){
            _roleService=roleService;
        }
        [HttpGet]
        public async Task<IActionResult> GetRoles(){
            var result= await _roleService.GetRoles();
            if(result.IsSuccess) return Ok(result);
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            if(request==null || string.IsNullOrEmpty(request.Name)) return BadRequest();
            var result=await _roleService.CreateRole(request);
            if(result.IsSuccess) return Ok(result.ResultObj);
            return BadRequest(result.Message);
        }

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRole(string roleId){
            var result=await _roleService.DeleteRole(roleId);
            if(result!=null && result.IsSuccess) return Ok(result.ResultObj);
            return BadRequest(result.Message);
        }
    }
}