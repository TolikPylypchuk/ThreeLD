using System.Collections.Generic;

using ThreeLD.DB.Models;

namespace ThreeLD.Web.Models.ViewModels
{
    public class ViewPreferencesModel
    {
        public Dictionary<string, Dictionary<Event, bool>>
			EventsByPreferences { get; set; }
    }
}