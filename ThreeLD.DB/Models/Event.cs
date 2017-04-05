using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThreeLD.DB.Models
{
	[Table(nameof(AppDbContext.Events))]
	public class Event : EntityBase
	{
		[Required(ErrorMessage = "The name of the event is required.")]
		public string Name { get; set; }

		[Required(ErrorMessage = "The date and time of the event is required.")]
		public DateTime DateTime { get; set; }

		[Required(ErrorMessage = "The duration of the event is required.")]
		public TimeSpan Duration { get; set; }

		[Required(ErrorMessage = "The address of the event is required.")]
		public string Address { get; set; }

		[Required(ErrorMessage = "The URL of the event is required.")]
		[RegularExpression(@"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$")]
		public string Url { get; set; }

		[Required(ErrorMessage = "The description of the event is required.")]
		public string Description { get; set; }

		[Required(ErrorMessage = "The category of the event is required.")]
		public string Category { get; set; }

		public virtual ICollection<User> BookmarkedBy { get; set; } =
			new HashSet<User>();
	}
}
