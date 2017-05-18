using System.Collections.Generic;

using ThreeLD.DB.Models;

namespace ThreeLD.Web.Models.ViewModels
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public string SelectedCategory { get; set; }
    }
}
