using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;

namespace ThreeLD.Web.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        IRepository<Event> events;

        public UserController(IRepository<Event> events)
        {
            this.events = events;
        }

        [HttpGet]
        public ViewResult ProposeEvent()
        {
            return View(new Event());
        }

        [HttpPost]
        public ActionResult ProposeEvent(Event newEvent)
        {
            if (newEvent != null)
            {
                this.events.Add(newEvent);
                this.events.Save();
            }

            return this.RedirectToAction("");
        }
    }
}