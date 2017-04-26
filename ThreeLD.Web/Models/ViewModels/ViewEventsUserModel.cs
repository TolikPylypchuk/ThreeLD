using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using ThreeLD.DB.Models;

namespace ThreeLD.Web.Models.ViewModels
{
	public class ViewEventsUserModel
	{
		[Required]
		public Dictionary<Event, bool> Events { get; set; }
	}
}
