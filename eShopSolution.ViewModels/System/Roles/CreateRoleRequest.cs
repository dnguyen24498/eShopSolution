using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace eShopSolution.ViewModels.System.Roles
{
    public class CreateRoleRequest
    {
        [Required(ErrorMessage ="Tên role là bắt buộc")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}