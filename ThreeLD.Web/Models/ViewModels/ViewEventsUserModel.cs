using System.Collections.Generic;

using ThreeLD.DB.Models;

namespace ThreeLD.Web.Models.ViewModels
{
    public class ViewEventsUserModel
	{
		public Dictionary<Event, bool> Events { get; set; }
	}
}
