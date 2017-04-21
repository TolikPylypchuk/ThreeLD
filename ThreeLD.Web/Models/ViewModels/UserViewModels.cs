using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using ThreeLD.DB.Models;

namespace ThreeLD.Web.Models.ViewModels
{
	public class CreateModel
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

	public class LoginModel
	{
		[Required]
		public string UserName { get; set; }
		[Required]
		public string Password { get; set; }
	}

	public class RoleEditModel
	{
		public AppRole Role { get; set; }
		public IEnumerable<User> Members { get; set; }
		public IEnumerable<User> NonMembers { get; set; }
	}

	public class RoleModificationModel
	{
		[Required]
		public string RoleName { get; set; }
		public string[] IdsToAdd { get; set; }
		public string[] IdsToDelete { get; set; }
	}
}
