using System;
using System.Diagnostics.CodeAnalysis;
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
		private IRepository<Notification> notifications;

		public UserController(
			IRepository<Preference> preferences,
			IRepository<Event> events,
			IRepository<Notification> notifications)
		{
			this.preferences = preferences;
			this.events = events;
			this.notifications = notifications;
		}

		[ExcludeFromCodeCoverage]
		public AppUserManager UserManager
		{
			get => this.userManager ?? HttpContext.GetOwinContext()
					.GetUserManager<AppUserManager>();
			set => this.userManager = value;
		}

		[HttpGet]
		[Authorize(Roles = "User")]
		[ExcludeFromCodeCoverage]
		public ActionResult Index()
		{
			return this.RedirectToAction(nameof(this.ViewEvents));
		}

		[HttpGet]
		[Authorize(Roles = "User")]
		public ViewResult ViewEvents()
		{
			var approvedEvents =
				this.events.GetAll()
							.Where(e => e.IsApproved)
							.OrderBy(e => e.DateTime);

            var categories =
                this.events.GetAll()
                            .Where(e => e.IsApproved)
                            .Select(e => e.Category)
                            .Distinct()
                            .ToList();

            Dictionary<string, bool> dict = new Dictionary<string, bool>();

            foreach (var i in categories)
            {
                dict.Add(i, true);
            }

            var currentUser =
				this.UserManager.FindById(User.Identity.GetUserId());

			var model = new ViewEventsUserModel()
			{
				Events = new Dictionary<Event, bool>(),
                Categories = dict
			};

			currentUser.BookmarkedEvents.AsQueryable().Load();

			foreach (var e in approvedEvents)
			{
				model.Events.Add(
					e, currentUser.BookmarkedEvents.Any(ev => ev.Id == e.Id));
			}

			return this.View(model);
		}

        private ViewResult ViewEvents(
           string categories, DateTime? start, DateTime? end)
        {
            var categoriesArray = categories.Split(',');

            var currentUser =
                this.UserManager.FindById(User.Identity.GetUserId());
            
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

            var model = new ViewEventsUserModel()
            {
                Events = this.events.GetAll()
                    .Where(e => e.IsApproved)
                    .Where(e => categoriesArray.Contains(e.Category))
                    .Where(e =>
                        (start == null ||
                         DateTime.Compare(e.DateTime, start.Value) >= 0) &&
                        (end == null ||
                         DateTime.Compare(e.DateTime, end.Value) <= 0))
                    .ToDictionary(e => e, e => currentUser.BookmarkedEvents.Contains(e)),
                Categories = dict
            };

            return this.View(nameof(this.ViewEvents), model);
        }
        [HttpPost]
        public ViewResult ViewEvents(ICollection<string> categories)
        {
            string result = string.Empty;
            if (categories != null)
            {
                foreach (string i in categories)
                {
                    result += i + ",";
                }
            }

            return ViewEvents(result, null, null);
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
			newEvent.ProposedBy = this.User.Identity.GetUserId();

			this.events.Add(newEvent);
			this.events.Save();

			return this.RedirectToAction(nameof(this.ViewEvents));
		}

		[HttpPost]
		public ActionResult AddBookmark(int eventId)
		{
			var e = this.events.GetById(eventId);

			if (e == null)
			{
				this.TempData["error"] =
							$"Event with id {eventId} doesn't exist.";
			} else
			{
				var currentUser =
					this.UserManager.FindById(this.User.Identity.GetUserId());
				
				currentUser.BookmarkedEvents.Add(e);
				e.BookmarkedBy.Add(currentUser);

				int res = this.events.Save();

				if (res != 0)
				{
					this.TempData["message"] =
						$"Event {e.Name} has been bookmarked.";
				}
				else
				{
					this.TempData["error"] =
						$"Event {e.Name} has already been bookmarked.";
				}
			}
			return this.RedirectToAction(nameof(this.ViewEvents));
		}

		[HttpPost]
		public ActionResult RemoveBookmark(int eventId, string returnURL)
		{
			var currentUser =
				this.UserManager.FindById(User.Identity.GetUserId());
			var chosenEvent = this.events.GetById(eventId);
			
			chosenEvent.BookmarkedBy.Remove(currentUser);
			currentUser.BookmarkedEvents.Remove(chosenEvent);

			int res = this.events.Save();

			if (res != 0)
			{
				this.TempData["message"] =
					$"Bookmark on event {chosenEvent.Name} " +
						"has been removed.";
			}
			else
			{
				this.TempData["error"] =
					$"Bookmark on event {chosenEvent.Name} " +
						"doesn't exist.";
			}

			if (String.IsNullOrEmpty(returnURL))
			{
				return this.RedirectToAction(nameof(this.ViewEvents));
			}

			return this.Redirect(returnURL);
		}

		[HttpGet]
		public new ViewResult Profile()
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
					.ToList(),
				SelectedCategory = null
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
			}
			else
			{
				this.TempData["error"] =
					"The specified preference doesn't exist.";
			}

			return this.RedirectToAction(nameof(this.Profile));
		}

		[HttpGet]
		[Authorize(Roles = "User")]
		public ViewResult ViewNotifications()
		{
			var model = new NotificationsViewModel
			{
				UnreadNotifications = new List<Notification>(),
				ReadNotifications = new List<Notification>()
			};

			string userId = this.User.Identity.GetUserId();

			var userNotifications = this.notifications.GetAll()
				.Where(n => n.To == userId).ToList();

			foreach (var n in userNotifications)
			{
				if (!n.IsRead)
				{
					model.UnreadNotifications.Add(n);
				}
				else
				{
					model.ReadNotifications.Add(n);
				}
			}

			return this.View(model);
		}

		[HttpPost]
		[Authorize(Roles = "User")]
		public ActionResult CheckNotificationAsRead(int notificationId)
		{
			var notification = this.notifications.GetById(notificationId);

			if (notification != null)
			{
				if (!notification.IsRead)
				{
					notification.IsRead = true;
					this.notifications.Save();

					this.TempData["message"] =
						"The notification has been checked as read.";
				}
				else
				{
					this.TempData["error"] =
						"The notification is already checked as read.";
				}
			}
			else
			{
				this.TempData["error"] = "The notification doesn't exist.";
			}

			return RedirectToAction(nameof(this.ViewNotifications));
		}

		[HttpGet]
		public ActionResult ViewPreferredEvents()
		{
			var approvedEvents = this.events.GetAll().Where(e => e.IsApproved);

			var currentUser =
				this.UserManager.FindById(User.Identity.GetUserId());

			var currentUserPreferences = currentUser.Preferences.ToList();

			var model = new ViewPreferencesModel
			{
				EventsByPreferences = new Dictionary<string, Dictionary<Event, bool>>()
			};

			foreach (var preference in currentUserPreferences)
			{
				var eventsByPreference = approvedEvents.Where(
					e => e.Category == preference.Category);

                var eventsDict = new Dictionary<Event, bool>();
                
                foreach (var e in eventsByPreference)
                {
                    eventsDict.Add(e, currentUser.BookmarkedEvents.Any(ev => ev.Id == e.Id));
                }

				model.EventsByPreferences.Add(
					preference.Category, eventsDict);
			}

			return this.View(model);
		}
	}
}
