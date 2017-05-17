using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ThreeLD.DB.Properties;

namespace ThreeLD.DB.Models
{
	[Table(nameof(AppDbContext.Preferences))]
	public class Preference : EntityBase
	{
		public string UserId { get; set; }

		[Display(
			Name = "PreferenceCategoryDisplayName",
			ResourceType = typeof(Resources))]
		[Required(
			ErrorMessageResourceName = "PreferenceCategoryRequired",
			ErrorMessageResourceType = typeof(Resources))]
		public string Category { get; set; }

		[ForeignKey(nameof(UserId))]
		public virtual User User { get; set; }
	}
}
