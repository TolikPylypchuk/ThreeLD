﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    [Authorize(Roles = "Editor")]
	public class EditorController : Controller
	{
		private IRepository<Event> events;
		private IRepository<Notification> notifications;

		public EditorController(
			IRepository<Event> events,
			IRepository<Notification> notifications)
		{
			this.events = events;
			this.notifications = notifications;
		}

		[ExcludeFromCodeCoverage]
		private AppUserManager UserManager
			=> HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
		
		[ExcludeFromCodeCoverage]
		public ActionResult Index()
		{
			return this.RedirectToAction(nameof(this.ViewEvents));
		}

		[ExcludeFromCodeCoverage]
		public ViewResult ViewEvents()
		{
			var approvedEvents =
				this.events.GetAll()
					.Where(e => e.IsApproved)
					.OrderBy(e => e.DateTime);

			var currentUser =
				this.UserManager.FindById(User.Identity.GetUserId());

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
			this.events.Add(e);
			this.events.Save();
			
			this.TempData["message"] = $"{e.Name} has been created.";

			return this.RedirectToAction(nameof(this.ViewEvents));
		}

		[HttpGet]
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
				e.IsApproved = true;
				this.events.Update(e);
				this.events.Save();

				string message = $"{e.Name} has been approved.";

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
