using System.Collections.Generic;
using System.Threading.Tasks;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Roles;
using eShopSolution.ViewModels.System.Users;

namespace eShopSolution.AdminApp.Services
{
    public interface IUserApiClient
    {
        Task<ApiResult<string>> Login(LoginUserRequest request);

        Task<ApiResult<bool>> Register(RegisterUserRequest request);

        Task<ApiResult<PagedResult<UserViewModel>>> GetUserPaging(PagingRequestBase request);

        Task<ApiResult<int>> UpdateUserRoles(UserRolesViewModel request);

        Task<ApiResult<UserRolesViewModel>> GetUserRolesByUserName(string userName);

    }
}