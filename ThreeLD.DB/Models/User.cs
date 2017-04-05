using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ThreeLD.DB.Models
{
	public class User : IdentityUser
	{
		[Required(ErrorMessage = "The first name of the user is required.")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "The last name of the user is required.")]
		public string LastName { get; set; }

		public virtual ICollection<Event> BookmarkedEvents { get; set; } =
			new HashSet<Event>();

		public virtual ICollection<Preference> Preferences { get; set; } =
			new HashSet<Preference>();
	}
}
