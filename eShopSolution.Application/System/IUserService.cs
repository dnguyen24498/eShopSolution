using System.Threading.Tasks;
using eShopSolution.ViewModels.System.Users;

namespace eShopSolution.Application.System
{
    public interface IUserService
    {
        Task<string> Login(LoginUserRequest request);
        Task<bool> Register(RegisterUserRequest request);
    }
}