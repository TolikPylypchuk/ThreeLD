﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        [ExcludeFromCodeCoverage]
        public ActionResult Index()
        {
            return RedirectToAction(nameof(this.ViewEvents));
        }

        [HttpGet]
        public ViewResult ViewEvents()
        {
            /*if (HttpContext.User.Identity.IsAuthenticated)
            {
                return this.View(nameof(Index), "Home");
            }*/

            return this.View(this.events.GetAll()
                .Where(e => e.IsApproved == true));
        }

        [HttpPost]
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
            
            return this.View(nameof(this.ViewEvents), result);
        }
    }
}