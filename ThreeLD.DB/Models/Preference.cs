using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThreeLD.DB.Models
{
	[Table(nameof(AppDbContext.Preferences))]
	public class Preference : EntityBase
	{
		[Required(ErrorMessage = "Specify the user.")]
		public string UserId { get; set; }

		[Required(ErrorMessage = "The category of the preference is required.")]
		public string Category { get; set; }

		[ForeignKey(nameof(UserId))]
		public User User { get; set; }
	}
}
