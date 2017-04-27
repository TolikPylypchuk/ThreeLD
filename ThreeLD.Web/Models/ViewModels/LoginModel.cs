using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ThreeLD.Web.Models.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class LoginModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}