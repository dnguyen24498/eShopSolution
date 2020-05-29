using System.Threading.Tasks;
using eShopSolution.AdminApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShopSolution.AdminApp.Controllers
{
    public class RoleController:Controller
    {
        private IUserApiClient _userApiClient;

        public RoleController(IUserApiClient userApiClient){
            _userApiClient=userApiClient;
        }

         public async Task<ViewResult> Index(){
            var result=await _userApiClient.GetRoles();
            if(result.IsSuccess) return View(result.ResultObj);
            return View();
         }
    }
}