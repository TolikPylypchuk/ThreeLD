using System.Diagnostics.CodeAnalysis;
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
			return this.View(nameof(this.EditEvent), new Event());
		}
		
		[HttpGet]
		public ViewResult EditEvent(int id)
		{
			return this.View(this.events.GetById(id));
		}

		[HttpPost]
		public ActionResult EditEvent(Event e)
		{
			if (!this.ModelState.IsValid)
			{
				return this.View(nameof(this.EditEvent), e);
			}

			if (e.Id == 0)
			{
				this.events.Add(e);
			} else
			{
				this.events.Update(e);
			}
			
			this.events.Save();

			string action = e.Id == 0 ? "created" : "updated";

			this.TempData["message"] = $"{e.Name} has been {action}.";

			return this.RedirectToAction(
				nameof(GuestController.ViewEvents), "Guest");
		}

		[HttpPost]
		public RedirectToRouteResult DeleteEvent(int id)
		{
			this.events.Delete(id);

			int result = this.events.Save();

			if (result != 0)
			{
				this.TempData["message"] = "The event was deleted.";
			}

			return this.RedirectToAction(
				nameof(GuestController.ViewEvents), "Guest");
		}
	}
}
