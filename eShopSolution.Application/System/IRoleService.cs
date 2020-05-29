using System.Collections.Generic;
using System.Threading.Tasks;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Roles;

namespace eShopSolution.Application.System
{
    public interface IRoleService
    {
         Task<ApiResult<List<AppRole>>> GetRoles();
         Task<ApiResult<AppRole>> CreateRole(CreateRoleRequest request);

    }
}