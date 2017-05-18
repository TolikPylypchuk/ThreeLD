using System.Collections.Generic;

using ThreeLD.DB.Models;

namespace ThreeLD.Web.Models.ViewModels
{
    public class FilterEventsModel
    {
        public List<Event> Events { get; set; }
        public Dictionary<string, bool> Categories { get; set; }
    }
}