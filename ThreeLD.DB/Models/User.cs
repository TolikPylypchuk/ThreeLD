using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNet.Identity.EntityFramework;

using ThreeLD.DB.Properties;

namespace ThreeLD.DB.Models
{
	public class User : IdentityUser
	{
		[Display(
			Name = "UserUserNameDisplayName",
			ResourceType = typeof(Resources))]
		[Required(
			ErrorMessageResourceName = "UserUserNameRequired",
			ErrorMessageResourceType = typeof(Resources))]
		public override string UserName
		{
			get => base.UserName;
			set => base.UserName = value;
		}

		[Display(
			Name = "UserEmailDisplayName",
			ResourceType = typeof(Resources))]
		[Required(
			ErrorMessageResourceName = "UserEmailRequired",
			ErrorMessageResourceType = typeof(Resources))]
		public override string Email
		{
			get => base.UserName;
			set => base.UserName = value;
		}

		[Display(
			Name = "UserFirstNameDisplayName",
			ResourceType = typeof(Resources))]
		[Required(
			ErrorMessageResourceName = "UserFirstNameRequired",
			ErrorMessageResourceType = typeof(Resources))]
		public string FirstName { get; set; }

		[Display(
			Name = "UserLastNameDisplayName",
			ResourceType = typeof(Resources))]
		[Required(
			ErrorMessageResourceName = "UserLastNameRequired",
			ErrorMessageResourceType = typeof(Resources))]
		public string LastName { get; set; }

		public virtual ICollection<Event> BookmarkedEvents { get; set; } =
			new HashSet<Event>();

		public virtual ICollection<Preference> Preferences { get; set; } =
			new HashSet<Preference>();

        public virtual ICollection<Notification>
            IncomingNotifications { get; set; } =
                new HashSet<Notification>();

        public virtual ICollection<Notification> 
            OutcomingNotifications { get; set; } =
                new HashSet<Notification>();
    }
}
