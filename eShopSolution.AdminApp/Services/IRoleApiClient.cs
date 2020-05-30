using System.Collections.Generic;
using System.Threading.Tasks;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Roles;

namespace eShopSolution.AdminApp.Services
{
    public interface IRoleApiClient
    {
         Task<ApiResult<List<AppRole>>> GetRoles();
        
         Task<ApiResult<AppRole>> CreateRole(CreateRoleRequest request);

         Task<ApiResult<string>> DeleteRole(string roleId);
    }
}