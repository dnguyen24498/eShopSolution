using System.Collections.Generic;
using System.Linq;
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
               Name=request.Name,
               Description=request.Description
           };
           var result=await _roleManager.CreateAsync(role);
           if(result.Succeeded) return new ApiSuccessResult<AppRole>(role);
           return new ApiErrorResult<AppRole>("Có lỗi khi tạo role");

        }

        public async Task<ApiResult<string>> DeleteRole(string roleId)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(x=>x.Id.ToString().Equals(roleId));
            if(role!=null){
               var result = await _roleManager.DeleteAsync(role);
               if(result.Succeeded) return new ApiSuccessResult<string>(roleId);
               return new ApiErrorResult<string>("Có lỗi khi xóa");
            }
            return new ApiErrorResult<string>("Không tìm thấy role");

        }

        public async Task<ApiResult<List<AppRole>>> GetRoles()
        {
            var roles= await _roleManager.Roles.ToListAsync();
            if(roles!=null) return new ApiSuccessResult<List<AppRole>>(roles);
            return new ApiSuccessResult<List<AppRole>>(new List<AppRole>());
        }
    }
}