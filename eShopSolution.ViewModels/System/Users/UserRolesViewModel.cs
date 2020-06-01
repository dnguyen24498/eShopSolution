using System.Collections.Generic;

namespace eShopSolution.ViewModels.System.Users
{
    public class UserRolesViewModel
    {
        public UserRolesViewModel()
        {
            IsRolesMember = new List<string>();
            NoneRolesMember = new List<string>();
        }
        public string UserName { get; set; }
        public IList<string> IsRolesMember { get; set; }
        public IList<string> NoneRolesMember { get; set; }
    }
}   