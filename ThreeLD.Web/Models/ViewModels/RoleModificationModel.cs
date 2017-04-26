using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ThreeLD.Web.Models.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class RoleModificationModel
    {
        [Required]
        public string RoleName { get; set; }
        public string[] IdsToAdd { get; set; }
        public string[] IdsToDelete { get; set; }
    }
}