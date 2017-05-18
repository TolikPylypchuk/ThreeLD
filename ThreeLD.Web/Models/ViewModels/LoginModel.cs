using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

using ThreeLD.Web.Properties;

namespace ThreeLD.Web.Models.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class LoginModel
    {
		[Display(
			Name = "LoginUserNameDisplayName",
			ResourceType = typeof(Resources))]
        [Required(
			ErrorMessageResourceName = "LoginUserNameRequired",
			ErrorMessageResourceType = typeof(Resources))]
        public string UserName { get; set; }

		[Display(
			Name = "LoginPasswordDisplayName",
			ResourceType = typeof(Resources))]
		[Required(
			ErrorMessageResourceName = "LoginPasswordRequired",
			ErrorMessageResourceType = typeof(Resources))]
		[UIHint("password")]
        public string Password { get; set; }
    }
}
