using System;
using System.Collections.Generic;

namespace eShopSolution.ViewModels.System.Users
{
    public class UserViewModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Dob { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
        
    }
}