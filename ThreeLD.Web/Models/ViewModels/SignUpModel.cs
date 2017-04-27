using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ThreeLD.Web.Models.ViewModels
{
	[ExcludeFromCodeCoverage]
	public class SignUpModel
	{
		[Required]
		public string UserName { get; set; }

		[Required]
		public string FirstName { get; set; }

		[Required]
		public string LastName { get; set; }

		[Required]
		public string Email { get; set; }

		[Required]
		public string Password { get; set; }
	}
}
