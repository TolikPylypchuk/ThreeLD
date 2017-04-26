using System.Linq;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using ThreeLD.DB.Infrastructure;
using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;
using ThreeLD.Web.Models.ViewModels;
using System;

namespace ThreeLD.Web.Controllers
{
    [Authorize(Roles = "User, Editor")]
    public class UserController : Controller
    {
        private IRepository<Preference> preferences;
        private IRepository<Event> events;

        public UserController(
            IRepository<Preference> preferences,
            IRepository<Event> events)
        {
            this.preferences = preferences;
            this.events = events;
        }

        private AppUserManager UserManager
            => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

        [HttpGet]
        [Authorize(Roles ="User")]
        public ViewResult ViewEvents()
        {
            var events = this.events.GetAll().Where(e => e.IsApproved == true);

            User currentUser =
                this.UserManager.FindById(User.Identity.GetUserId());

            ViewEventsUserModel model = new ViewEventsUserModel();
            foreach (Event e in events)
            {
                model.Events.Add(
                    e, currentUser.BookmarkedEvents.Any(ev => ev.Id == e.Id));
            }

            ViewBag.ReturnURL = "/User/ViewEvents";

            return this.View(model);
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public ViewResult ProposeEvent()
        {
            ViewBag.Action = "Propose";

            return this.View("EditEvent", new Event());
        }

        [HttpPost]
        public ActionResult ProposeEvent(Event newEvent)
        {
            if (newEvent != null && newEvent.IsApproved == false)
            {
                this.events.Add(newEvent);
                this.events.Save();
            }

            return this.RedirectToAction("ViewEvents", "Guest");
        }

        [HttpPost]
        public ActionResult AddBookmark(int eventId)
        {
            User currentUser =
                this.UserManager.FindById(User.Identity.GetUserId());
            Event chosenEvent = this.events.GetById(eventId);

            chosenEvent.BookmarkedBy.Add(currentUser);
            this.events.Save();

            currentUser.BookmarkedEvents.Add(chosenEvent);

            this.TempData["message"] =
                $"Event {chosenEvent.Name} has been bookmarked.";
            
            return this.RedirectToAction("ViewEvents", "Guest");
        }

        [HttpPost]
        public ActionResult RemoveBookmark(int eventId, string returnURL)
        {
            User currentUser =
                this.UserManager.FindById(User.Identity.GetUserId());
            Event chosenEvent = this.events.GetById(eventId);

            chosenEvent.BookmarkedBy.Remove(currentUser);
            int res = this.events.Save();

            currentUser.BookmarkedEvents.Remove(chosenEvent);

            if (res != 0)
            {
                this.TempData["message"] =
                    $"Bookmark on event {chosenEvent.Name} " +
                    $"has been removed.";
            }

            if (String.IsNullOrEmpty(returnURL))
            {
                return this.RedirectToAction(nameof(ViewEvents));
            }

            return this.Redirect(returnURL);
        }
        
        public ActionResult Profile()
        {
            ViewBag.ReturnURL = "/User/Profile";

            User currentUser =
                this.UserManager.FindById(User.Identity.GetUserId());

            return View(new ProfileViewModel { User = currentUser });
        }

        [HttpGet]
        public ViewResult ViewPreferences()
        {
            return this.View(this.preferences.GetAll()
                .Where(p => p.UserId == User.Identity.GetUserId()).ToArray());
        }

        [HttpPost]
        public ActionResult AddPreference(string preferenceCategory)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(nameof(ViewPreferences));
            }

	        Preference newPreference = new Preference
	        {
		        UserId = this.User.Identity.GetUserId(),
		        Category = preferenceCategory
	        };

	        this.preferences.Add(newPreference);
            this.preferences.Save();

            this.TempData["message"] =
                $"Preference with category {newPreference.Category} " +
                $"has been created.";

            return this.RedirectToAction(nameof(this.ViewPreferences));
        }

        [HttpPost]
        public ActionResult RemovePreference(int id)
        {
            this.preferences.Delete(id);
            int res = this.preferences.Save();

            if (res != 0)
            {
                this.TempData["message"] =
                    $"Preference with category " +
                    $"{this.preferences.GetById(id).Category} " +
                    $"has been removed.";
            }
            else
            {
                this.TempData["error"] = 
                    $"The specified preference can not be removed " +
                    $"because it doesn't exist.";
            }

            return this.RedirectToAction(nameof(this.ViewPreferences));
        }
    }
}
