using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Mvc;

using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;

namespace ThreeLD.Web.Controllers
{
    [Authorize(Roles = "Editor")]
	public class EditorController : Controller
	{
		private IRepository<Event> events;

		public EditorController(IRepository<Event> events)
		{
			this.events = events;
		}

		[HttpGet]
		[ExcludeFromCodeCoverage]
		public ViewResult CreateEvent()
		{
			this.ViewBag.Action = "Create";
			return this.View(nameof(this.EditEvent), new Event());
		}
		
		[HttpPost]
		public ActionResult CreateEvent(Event e)
		{
			if (!this.ModelState.IsValid)
			{
				this.ViewBag.Action = "Create";
				return this.View(nameof(this.EditEvent), e);
			}

			e.IsApproved = true;
			this.events.Add(e);
			this.events.Save();
			
			this.TempData["message"] = $"{e.Name} has been created.";

			return this.RedirectToAction(
				nameof(GuestController.ViewEvents), "Guest");
		}

		[HttpGet]
		public ViewResult EditEvent(int id)
		{
			this.ViewBag.Action = "Edit";
			return this.View(this.events.GetById(id));
		}

		[HttpPost]
		public ActionResult EditEvent(Event e)
		{
			if (!this.ModelState.IsValid)
			{
				this.ViewBag.Action = "Edit";
				return this.View(nameof(this.EditEvent), e);
			}
			
			e.IsApproved = true;
			this.events.Update(e);
			this.events.Save();
			
			this.TempData["message"] = $"{e.Name} has been updated.";

			return this.RedirectToAction(
				nameof(GuestController.ViewEvents), "Guest");
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

				this.TempData["message"] = $"{e.Name} has been approved.";
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
				
				this.TempData["message"] = $"{e.Name} has been deleted.";
			}
			
			return e != null && !e.IsApproved
				? this.RedirectToAction(nameof(this.ViewProposedEvents))
				: this.RedirectToAction(
					nameof(GuestController.ViewEvents), "Guest");
		}
	}
}
