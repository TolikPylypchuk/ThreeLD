using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
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
    [Authorize(Roles = "Editor")]
	public class EditorController : Controller
	{
		private IRepository<Event> events;
		private IRepository<Notification> notifications;
		private AppUserManager userManager;

		public EditorController(
			IRepository<Event> events,
			IRepository<Notification> notifications)
		{
			this.events = events;
			this.notifications = notifications;
		}

		[ExcludeFromCodeCoverage]
		public AppUserManager UserManager
		{
			get => this.userManager ?? this.HttpContext.GetOwinContext()
					.GetUserManager<AppUserManager>();
			set => this.userManager = value;
		}

		[ExcludeFromCodeCoverage]
		public ActionResult Index()
		{
			return this.RedirectToAction(nameof(this.ViewEvents));
		}

		[HttpGet]
		public async Task<ViewResult> ViewEvents()
		{
			var approvedEvents =
				this.events.GetAll()
					.Where(e => e.IsApproved)
					.OrderBy(e => e.DateTime);

			var currentUser =
				await this.UserManager.FindByIdAsync(
					this.User.Identity.GetUserId());

			var model = new ViewEventsUserModel
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

        [HttpGet]
		[ExcludeFromCodeCoverage]
		public ViewResult CreateEvent()
		{
			this.ViewBag.Action = "Create";
			this.ViewBag.Role = "Editor";
			return this.View(nameof(this.EditEvent), new Event());
		}
		
		[HttpPost]
		public ActionResult CreateEvent(Event e)
		{
			if (!this.ModelState.IsValid)
			{
				this.ViewBag.Action = "Create";
				this.ViewBag.Role = "Editor";
				return this.View(nameof(this.EditEvent), e);
			}

			e.IsApproved = true;
			e.CreatedBy = this.User.Identity.GetUserId();
			this.events.Add(e);
			this.events.Save();
			
			this.TempData["message"] = $"{e.Name} has been created.";

			return this.RedirectToAction(nameof(this.ViewEvents));
		}

		[HttpGet]
		[ExcludeFromCodeCoverage]
		public ViewResult EditEvent(int id)
		{
			this.ViewBag.Action = "Edit";
			this.ViewBag.Role = "Editor";
			return this.View(this.events.GetById(id));
		}

		[HttpPost]
		public ActionResult EditEvent(Event e)
		{
			if (!this.ModelState.IsValid)
			{
				this.ViewBag.Action = "Edit";
				this.ViewBag.Role = "Editor";
				return this.View(nameof(this.EditEvent), e);
			}
			
			e.IsApproved = true;
			e.CreatedBy = this.User.Identity.GetUserId();
			this.events.Update(e);
			this.events.Save();

			string message = $"{e.Name} has been updated.";

			this.TempData["message"] = message;

			if (!String.IsNullOrEmpty(e.ProposedBy))
			{
				this.notifications.Add(new Notification
				{
					From = this.User.Identity.GetUserId(),
					To = e.ProposedBy,
					IsRead = false,
					Message = message
				});

				this.notifications.Save();
			}

			return this.RedirectToAction(nameof(this.ViewEvents));
		}
		
		public ViewResult ViewProposedEvents()
		{
			return this.View(this.events.GetAll().Where(e => !e.IsApproved));
		}

		[HttpPost]
		public RedirectToRouteResult ApproveEvent(int id)
		{
			var e = this.events.GetById(id);

			if (e == null)
			{
				this.TempData["error"] = "The specified event doesn't exist.";
			} else
			{
				string currentUserId = this.User.Identity.GetUserId();
				
				e.IsApproved = true;
				e.CreatedBy = currentUserId;
				this.events.Update(e);
				this.events.Save();

				string message = $"{e.Name} has been approved.";

				this.TempData["message"] = message;

				if (!String.IsNullOrEmpty(e.ProposedBy))
				{
					this.notifications.Add(new Notification
					{
						From = currentUserId,
						To = e.ProposedBy,
						IsRead = false,
						Message = message
					});

					this.notifications.Save();
				}
			}

			return this.RedirectToAction(nameof(this.ViewProposedEvents));
		}
		
		[HttpPost]
		public RedirectToRouteResult DeleteEvent(int id)
		{
			var e = this.events.GetById(id);

			if (e == null)
			{
				this.TempData["error"] = "The specified event doesn't exist.";
			} else
			{
				this.events.Delete(e);
				this.events.Save();

				string message = $"{e.Name} has been deleted.";

				this.TempData["message"] = message;

				if (!String.IsNullOrEmpty(e.ProposedBy))
				{
					this.notifications.Add(new Notification
					{
						From = this.User.Identity.GetUserId(),
						To = e.ProposedBy,
						IsRead = false,
						Message = message
					});

					this.notifications.Save();
				}
			}
			
			return e != null && !e.IsApproved
				? this.RedirectToAction(nameof(this.ViewProposedEvents))
				: this.RedirectToAction(nameof(this.ViewEvents));
		}
	}
}
