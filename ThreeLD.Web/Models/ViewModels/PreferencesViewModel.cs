using System.Collections.Generic;
using ThreeLD.DB.Models;

namespace ThreeLD.Web.Models.ViewModels
{
    public class PreferencesViewModel
    {
        public IEnumerable<Preference> Preferences { get; set; }
        public IEnumerable<string> Categories { get; set; }
    }
}