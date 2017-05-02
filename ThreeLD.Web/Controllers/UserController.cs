using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using ThreeLD.DB.Infrastructure;
using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;
using ThreeLD.Web.Models.ViewModels;

namespace ThreeLD.Web.Controllers
{
    [Authorize(Roles = "User, Editor")]
    public class UserController : Controller
    {
        private AppUserManager userManager;
        private IRepository<Preference> preferences;
        private IRepository<Event> events;

        public UserController(
            IRepository<Preference> preferences,
            IRepository<Event> events,
            AppUserManager userManager = null)
        {
            this.preferences = preferences;
            this.events = events;

            if (userManager == null)
            {
                this.userManager = HttpContext.GetOwinContext()
                    .GetUserManager<AppUserManager>();
            }
            else
            {
                this.userManager = userManager;
            }
        }

        private AppUserManager UserManager
        {
            get
            {
                return this.userManager;
            }
            set
            {
                this.userManager = value;
            }
        }

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
				this.UserManager.FindById(this.User.Identity.GetUserId());

			var e = this.events.GetById(eventId);
			
			currentUser.BookmarkedEvents.Add(e);

			this.UserManager.Update(currentUser);
			
			this.TempData["message"] =
				$"Event {e.Name} has been bookmarked.";
            
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
				this.UserManager.FindById(this.User.Identity.GetUserId());
			
			currentUser.Preferences.AsQueryable().Load();
            
			var categories =
				this.events.GetAll()
						   .Where(e => e.IsApproved)
						   .Select(e => e.Category)
						   .Distinct()
						   .ToList();

			return this.View(new ProfileViewModel
			{
				User = currentUser,
				Categories = categories
					.Where(c => currentUser.Preferences.All(p => p.Category != c))
					.ToList()
			});
		}
        
		[HttpPost]
		public ActionResult AddPreference(ProfileViewModel model)
		{
			string category = model.SelectedCategory;

			if (String.IsNullOrEmpty(category))
			{
				this.TempData["error"] = "Choose the category.";
				return this.View(nameof(this.Profile));
			}

			string userId = this.User.Identity.GetUserId();

			if (this.preferences.GetAll().Count(
				p => p.Category == category && p.UserId == userId) != 0)
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

			this.TempData["message"] = "The preference has been added.";

			return this.RedirectToAction(nameof(this.Profile));
		}

		[HttpPost]
		public ActionResult RemovePreference(int id)
		{
			this.preferences.Delete(id);
			int res = this.preferences.Save();

			if (res != 0)
			{
				this.TempData["message"] = "The preference has been removed.";
			} else
			{
				this.TempData["error"] = 
					"The specified preference doesn't exist.";
			}

			return this.RedirectToAction(nameof(this.Profile));
		}
	}
}
