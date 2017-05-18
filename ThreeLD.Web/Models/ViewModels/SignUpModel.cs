using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

using ThreeLD.Web.Properties;

namespace ThreeLD.Web.Models.ViewModels
{
	[ExcludeFromCodeCoverage]
	public class SignUpModel
	{
		[Display(
			Name = "SignUpUserNameDisplayName",
			ResourceType = typeof(Resources))]
		[Required(
			ErrorMessageResourceName = "SignUpUserNameRequired",
			ErrorMessageResourceType = typeof(Resources))]
		public string UserName { get; set; }

		[Display(
			Name = "SignUpFirstNameDisplayName",
			ResourceType = typeof(Resources))]
		[Required(
			ErrorMessageResourceName = "SignUpFirstNameRequired",
			ErrorMessageResourceType = typeof(Resources))]
		public string FirstName { get; set; }

		[Display(
			Name = "SignUpLastNameDisplayName",
			ResourceType = typeof(Resources))]
		[Required(
			ErrorMessageResourceName = "SignUpLastNameRequired",
			ErrorMessageResourceType = typeof(Resources))]
		public string LastName { get; set; }

		[Display(
			Name = "SignUpEmailDisplayName",
			ResourceType = typeof(Resources))]
		[Required(
			ErrorMessageResourceName = "SignUpEmailRequired",
			ErrorMessageResourceType = typeof(Resources))]
		[EmailAddress(
			ErrorMessageResourceName = "SignUpEmailInvalid",
			ErrorMessageResourceType = typeof(Resources))]
		public string Email { get; set; }

		[Display(
			Name = "SignUpPasswordDisplayName",
			ResourceType = typeof(Resources))]
		[Required(
			ErrorMessageResourceName = "SignUpPasswordRequired",
			ErrorMessageResourceType = typeof(Resources))]
		[UIHint("password")]
		public string Password { get; set; }
	}
}
