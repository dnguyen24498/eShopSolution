using System.Collections.Generic;
using System.Threading.Tasks;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace eShopSolution.Application.System
{
    public class RoleService : IRoleService
    {
        private RoleManager<AppRole> _roleManager;

        public RoleService(RoleManager<AppRole> roleManager){
            _roleManager=roleManager;
        }
        public async Task<ApiResult<AppRole>> CreateRole(CreateRoleRequest request)
        {
           var role=new AppRole(){
               Name=request.Name
           };
           var result=await _roleManager.CreateAsync(role);
           if(result.Succeeded) return new ApiSuccessResult<AppRole>(role);
           return new ApiErrorResult<AppRole>("Có lỗi khi tạo role");

        }

        public async Task<ApiResult<List<AppRole>>> GetRoles()
        {
            var roles= await _roleManager.Roles.ToListAsync();
            if(roles!=null) return new ApiSuccessResult<List<AppRole>>(roles);
            return new ApiSuccessResult<List<AppRole>>(new List<AppRole>());
        }
    }
}