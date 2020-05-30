using System.Threading.Tasks;
using eShopSolution.AdminApp.Services;
using eShopSolution.ViewModels.System.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShopSolution.AdminApp.Controllers
{
    [Authorize]
    public class RoleController:Controller
    {
        private IRoleApiClient _roleApiClient;

        public RoleController(IRoleApiClient roleApiClient){
            _roleApiClient=roleApiClient;
        }

         public async Task<ViewResult> Index(){
            var result=await _roleApiClient.GetRoles();
            if(result==null) return View("Views/Errors/401.cshtml");
            if(result.IsSuccess && result.ResultObj!=null) return View(result.ResultObj);
            return View();
         }

         [HttpGet]
         public ViewResult Create(){
             return View();
         }

         [HttpPost]
         public async Task<IActionResult> Create(CreateRoleRequest request){
             if(ModelState.IsValid){
                 var result=await _roleApiClient.CreateRole(request);
                 if(result!=null && result.IsSuccess==true) return RedirectToAction(nameof(Index));
                 else return View(nameof(Create));
             }
             return View(ModelState);
         }

         [HttpPost]
         public async Task<IActionResult> Delete(string id){
             if(!string.IsNullOrEmpty(id)){
                 var result=await _roleApiClient.DeleteRole(id);
             }
             return RedirectToAction(nameof(Index));
         }
    }
}