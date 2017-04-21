using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;

namespace ThreeLD.Web.Controllers
{
    public class GuestController : Controller
    {
        private IRepository<Event> events;

        public GuestController(IRepository<Event> events)
        {
            this.events = events;
        }

        [HttpGet]
        public ViewResult ViewEvents()
        {
            return this.View(this.events.GetAll()
                .Where(e => e.IsApproved == true));
        }

        [HttpGet]
        public ViewResult FilterEvents(
            string categories, DateTime? start, DateTime? end)
        {
            List<Event> result = new List<Event>();

            var categoriesArray = categories.Split(',');

            foreach (string category in categoriesArray)
            {
                var currentEvents = this.events.GetAll()
                    .Where(e => e.IsApproved == true)
                    .Where(e => e.Category == category)
                    .Where(e =>
                        (start == null ||
                         DateTime.Compare(e.DateTime, start.Value) >= 0) &&
                        (end == null ||
                         DateTime.Compare(e.DateTime, end.Value) <= 0))
                    .ToList();
                
                    foreach (Event currentEvent in currentEvents)
                    {
                        result.Add(currentEvent);
                    }
            }
            
            return this.View(result);
        }
    }
}