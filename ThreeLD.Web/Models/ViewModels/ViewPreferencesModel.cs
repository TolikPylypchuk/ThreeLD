using System.Collections.Generic;

using ThreeLD.DB.Models;

namespace ThreeLD.Web.Models.ViewModels
{
    public class ViewPreferencesModel
    {
        public Dictionary<string, List<Event>> EventsByPreferences { get; set; }
    }
}