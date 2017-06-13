using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Mvc;

using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;
using ThreeLD.Web.Models.ViewModels;

namespace ThreeLD.Web.Controllers
{
    public class GuestController : Controller
    {
        private IRepository<Event> events;

        public GuestController(IRepository<Event> events)
        {
            this.events = events;
        }

        [ExcludeFromCodeCoverage]
        public ActionResult Index()
        {
            return RedirectToAction(nameof(this.ViewEvents));
        }

        [HttpGet]
        public ViewResult ViewEvents()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return this.View(nameof(Index), "Home");
            }

            var categories =
                this.events.GetAll()
                            .Where(e => e.IsApproved)
                            .Select(e => e.Category)
                            .Distinct()
                            .ToList();

            Dictionary<string, bool> dict = new Dictionary<string, bool>();

            foreach(var i in categories)
            {
                dict.Add(i, true);
            }

            var model = new FilterEventsModel()
            {
                Events = this.events.GetAll()
                    .Where(e => e.IsApproved)
                    .OrderBy(e => e.DateTime).ToList(),

                Categories = dict
            };

            return this.View(model);
        }
        
        [HttpPost]
        private ViewResult ViewEvents(
            string categories, DateTime? start, DateTime? end)
        {
            var result = new List<Event>();

            var categoriesArray = categories.Split(',');

            foreach (string category in categoriesArray)
            {
                var currentEvents = this.events.GetAll()
                    .Where(e => e.IsApproved)
                    .Where(e => e.Category == category)
                    .Where(e =>
                        (start == null ||
                         DateTime.Compare(e.DateTime, start.Value) >= 0) &&
                        (end == null ||
                         DateTime.Compare(e.DateTime, end.Value) <= 0))
                    .ToList();

                result.AddRange(currentEvents);
            }

            var categoriesAll =
                this.events.GetAll()
                            .Where(e => e.IsApproved)
                            .Select(e => e.Category)
                            .Distinct()
                            .ToList();

            Dictionary<string, bool> dict = new Dictionary<string, bool>();

            foreach (var i in categoriesAll)
            {
                dict.Add(i, categoriesArray.Contains(i));
            }

            var model = new FilterEventsModel()
            {
                Events = result,
                Categories = dict
            };

            return this.View(nameof(this.ViewEvents), model);
        }

        [HttpPost]
        public ViewResult ViewEvents(ICollection<string> categories)
        {
            string result = string.Empty;
            if(categories != null)
            {
                foreach (string i in categories)
                {
                    result += i + ",";
                }
            }
            
            return ViewEvents(result, null, null);
        }
    }
}