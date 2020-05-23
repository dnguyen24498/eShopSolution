using System.Threading.Tasks;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Users;

namespace eShopSolution.Application.System
{
    public interface IUserService
    {
        Task<ApiResult<string>> Login(LoginUserRequest request);
        Task<ApiResult<bool>> Register(RegisterUserRequest request);
    }
}