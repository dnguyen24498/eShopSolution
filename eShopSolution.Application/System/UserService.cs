using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Identity;
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
        public async Task<string> Login(LoginUserRequest request)
        {
            var user = await userManager.FindByNameAsync(request.UserName);
            if (user == null) return null;
            var logging = await signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true);
            if (!logging.Succeeded) return null;

            var roles = userManager.GetRolesAsync(user);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Role, string.Join(";", roles)),
                new Claim(ClaimTypes.Name, request.UserName),
            };
            //Encode claim
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Tokens:Key"]));
            var keyEncoded = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            var token = new JwtSecurityToken(configuration["Tokens:Issuer"],
                configuration["Tokens:Issuer"],
                claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: keyEncoded);
            return "Bearer "+new JwtSecurityTokenHandler().WriteToken(token) ;
        }

        public async Task<bool> Register(RegisterUserRequest request)
        {
            var user = await userManager.FindByNameAsync(request.UserName);
            var email = await userManager.FindByEmailAsync(request.Email);
            if (user != null || email != null) return false;
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
                return true;
            }

            return false;
        }
    }
}