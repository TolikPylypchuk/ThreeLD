using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ThreeLD.DB.Infrastructure;
using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;
using ThreeLD.Web.Models.ViewModels;

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
        public ActionResult Index()
        {
            return RedirectToAction(nameof(this.ViewEvents));
        }

        [HttpGet]
        [Authorize(Roles ="User")]
        public ViewResult ViewEvents()
        {
            var approvedEvents = this.events.GetAll().Where(e => e.IsApproved);

            var currentUser =
                this.UserManager.FindById(User.Identity.GetUserId());

            var model = new ViewEventsUserModel()
            {
                Events = new Dictionary<Event, bool>()
            };

            foreach (var e in approvedEvents)
            {
                model.Events.Add(
                    e, currentUser.BookmarkedEvents.Any(ev => ev.Id == e.Id));
            }

            return this.View(model);
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public ViewResult ProposeEvent()
        {
            this.ViewBag.Action = "Propose";
	        this.ViewBag.Role = "User";
            return this.View("EditEvent", new Event());
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult ProposeEvent(Event newEvent)
        {
            if (!this.ModelState.IsValid)
            {
                this.ViewBag.Action = "Propose";
	            this.ViewBag.Role = "User";
				return this.View(nameof(EditorController.EditEvent), newEvent);
            }

            newEvent.IsApproved = false;

            this.events.Add(newEvent);
            this.events.Save();

            return this.RedirectToAction(nameof(this.ViewEvents));
        }

        [HttpPost]
        public ActionResult AddBookmark(int eventId)
        {
            var currentUser =
                this.UserManager.FindById(User.Identity.GetUserId());
            var chosenEvent = this.events.GetById(eventId);

            chosenEvent.BookmarkedBy.Add(currentUser);
            this.events.Save();

            currentUser.BookmarkedEvents.Add(chosenEvent);

            this.TempData["message"] =
                $"Event {chosenEvent.Name} has been bookmarked.";
            
            return this.RedirectToAction(nameof(this.ViewEvents));
        }

        [HttpPost]
        public ActionResult RemoveBookmark(int eventId, string returnURL)
        {
            var currentUser =
                this.UserManager.FindById(User.Identity.GetUserId());
            var chosenEvent = this.events.GetById(eventId);

            chosenEvent.BookmarkedBy.Remove(currentUser);
            int res = this.events.Save();

            currentUser.BookmarkedEvents.Remove(chosenEvent);

            if (res != 0)
            {
                this.TempData["message"] =
                    $"Bookmark on event {chosenEvent.Name} " +
                     "has been removed.";
            }
            
            if (String.IsNullOrEmpty(returnURL))
            {
                return this.RedirectToAction(nameof(this.ViewEvents));
            }

            return this.Redirect(returnURL);
        }
        
        public new ActionResult Profile()
        {
            ViewBag.ReturnURL = "/User/Profile";

            var currentUser =
                this.UserManager.FindById(User.Identity.GetUserId());
            
            var categories = this.events.GetAll()
                .Where(e => e.IsApproved).Select(e => e.Category).Distinct();

            return View(new ProfileViewModel
            {
                User = currentUser, Categories = categories
            });
        }
        
        [HttpPost]
        public ActionResult AddPreference(ProfileViewModel model)
        {
            string category = model.SelectedCategory;

            if (String.IsNullOrEmpty(category))
            {
                this.TempData["error"] = "Choose category.";
                return this.View(nameof(this.Profile));
            }

            var userId = this.User.Identity.GetUserId();

            if (this.preferences.GetAll().Where(p =>
                    p.Category == category && p.UserId == userId).Count() != 0)
            {
                this.TempData["error"] = $"Category {category} is already " +
                    "assigned as preferred. Choose an unassigned one.";
                return this.View(nameof(this.Profile));
            }

            var newPreference = new Preference
            {
                UserId = userId,
		        Category = category
            };

            this.preferences.Add(newPreference);
            this.preferences.Save();

            this.TempData["message"] =
                $"Preference with category {newPreference.Category} " +
                 "has been created.";

            return this.RedirectToAction(nameof(this.Profile));
        }

        [HttpPost]
        public ActionResult RemovePreference(int id)
        {
            this.preferences.Delete(id);
            int res = this.preferences.Save();

            if (res != 0)
            {
                this.TempData["message"] =
                     "Preference with category " +
                    $"{this.preferences.GetById(id).Category} " +
                     "has been removed.";
            }
            else
            {
                this.TempData["error"] = 
                    "The specified preference can not be removed " +
                    "because it doesn't exist.";
            }

            return this.RedirectToAction(nameof(this.Profile));
        }
    }
}
