using System.Threading.Tasks;
using eShopSolution.AdminApp.Services;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace eShopSolution.AdminApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserApiClient _userApiClient;

        public UserController(IUserApiClient userApiClient){
            _userApiClient=userApiClient;
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
        public async Task<IActionResult> Index(int pageIndex){
            var pagingRequest=new PagingRequestBase(){
              PageIndex=1,
              PageSize= 10
            };
            var result = await _userApiClient.GetUserPaging(pagingRequest);
            if(result!=null && result.IsSuccess) return View(result.ResultObj.Items);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UpdateUserRoles(string userName)
        {
            var result = await _userApiClient.GetUserRolesByUserName(userName);
            if(result!=null && result.IsSuccess) return View(result.ResultObj);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRoles(UserRolesViewModel request)
        {
            if (ModelState.IsValid)
            {
                var result = await _userApiClient.UpdateUserRoles(request);
                if (result.IsSuccess) return RedirectToAction(nameof(Index));
            }
            return View(ModelState);
        }
        


    }
}