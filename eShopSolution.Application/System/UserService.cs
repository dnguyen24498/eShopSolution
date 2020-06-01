using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NotImplementedException = System.NotImplementedException;

namespace eShopSolution.Application.System
{
    public class UserService:IUserService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly IConfiguration configuration;

        public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
            ;
        }

        public async Task<ApiResult<PagedResult<UserViewModel>>> GetUserPaging(PagingRequestBase request)
        {
            if(request==null) return new ApiErrorResult<PagedResult<UserViewModel>>("Nhập sai dữ liệu");
            var users= await userManager.Users.Skip((request.PageIndex-1)*request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

            List<UserViewModel> listUser=new List<UserViewModel>();
            UserViewModel newUser;
            foreach(var user in users){
                newUser=new UserViewModel(){
                    Dob=user.Dob,
                    Email=user.Email,
                    FirstName=user.FirstName,
                    LastName=user.LastName,
                    UserName=user.UserName,
                    Roles=await userManager.GetRolesAsync(user)
                };
                listUser.Add(newUser);
            }   
            PagedResult<UserViewModel> result=new PagedResult<UserViewModel>(){
                TotalRecord=listUser.Count,
                Items= listUser
            };
            return new ApiSuccessResult<PagedResult<UserViewModel>>(result);
        }

        public async Task<ApiResult<UserRolesViewModel>> GetUserRoles(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user == null) return new ApiErrorResult<UserRolesViewModel>("Không tìm thấy người dùng");
            IEnumerable<string> isRoleMembers = await userManager.GetRolesAsync(user) as List<string>;
            IEnumerable<string> allRole = await roleManager.Roles.Select(x => x.Name).ToListAsync();
            var userRolesVM = new UserRolesViewModel()
            {
                UserName = userName,
                IsRolesMember = isRoleMembers as IList<string>,
                NoneRolesMember = allRole.Except(isRoleMembers).ToList() as IList<string>
            };
            return new ApiSuccessResult<UserRolesViewModel>(userRolesVM);
        }

        public async Task<ApiResult<string>> Login(LoginUserRequest request)
        {
            var user = await userManager.FindByNameAsync(request.UserName);
            if (user == null) return null;
            var logging = await signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true);
            if (!logging.Succeeded) return null;

            var roles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Name, request.UserName),
            }.Union(roles.Select(item=>new Claim(ClaimTypes.Role,item)));
            //Encode claim
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Tokens:Key"]));
            var keyEncoded = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            var token = new JwtSecurityToken(configuration["Tokens:Issuer"],
                configuration["Tokens:Issuer"],
                claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: keyEncoded);
            return new ApiSuccessResult<string>(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public async Task<ApiResult<bool>> Register(RegisterUserRequest request)
        {
            var user = await userManager.FindByNameAsync(request.UserName);
            var email = await userManager.FindByEmailAsync(request.Email);
            if (user != null || email != null) return new ApiErrorResult<bool>("Không tìm thấy người dùng.");
            user=new AppUser()
            {
                Dob = request.Dob,
                Email = request.Email,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                LastName = request.LastName,
                FirstName = request.FirstName,
            };
            var result = await userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Đăng ký không thành công");
        }

        public async Task<ApiResult<int>> UpdateUserRoles(UserRolesViewModel request)
        {
            var user = await userManager.FindByNameAsync(request.UserName);
            if(user==null) return new ApiErrorResult<int>("Không tìm thấy user");
            foreach(var r in request.IsRolesMember){
                await userManager.RemoveFromRoleAsync(user,r);
            }
            foreach(var r in request.NoneRolesMember){
                await userManager.AddToRoleAsync(user,r);
            }
            return new ApiSuccessResult<int>(request.NoneRolesMember.Count+request.IsRolesMember.Count);
        }
    }
}