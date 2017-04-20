using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;

namespace ThreeLD.Web.Controllers
{
    [Authorize(Roles = "Guest")]
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
            return this.View(this.events.GetAll().ToList<Event>());
        }

        [HttpGet]
        public ViewResult FilterEventsByCategory(string[] category)
        {
            List<Event> result = new List<Event>();

            for (int i = 0; i < category.Length; i++)
            {
                var currentEvents = this.events.GetAll().Where(e => e.Category == category[i]).Select(e => e).ToList<Event>();

                if (currentEvents != null)
                {
                    for (int j = 0; j < currentEvents.Count; j++)
                    {
                        result.Add(currentEvents[j]);
                    }
                }

                currentEvents = null;
            }
            
            return this.View(result);
        }

        [HttpGet]
        public ViewResult FilterEventsByDateTime(DateTime dateFrom,
            DateTime dateTo)
        {
            List<Event> result = new List<Event>();
            var currentEvents = this.events.GetAll().
                Where(e => (DateTime.Compare(e.DateTime, dateFrom) >= 0 && 
                DateTime.Compare(e.DateTime, dateTo) <= 0)).Select(e => e);

            if (currentEvents != null)
            {
                result = currentEvents.ToList<Event>();
            }

            return this.View(result);
        }
    }
}