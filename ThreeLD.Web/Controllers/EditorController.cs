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
		public ViewResult CreateEvent()
		{
			return this.View(nameof(this.EditEvent), new Event());
		}

		[HttpPost]
		public ActionResult CreateEvent(Event e)
		{
			if (!this.ModelState.IsValid)
			{
				return this.View(nameof(this.EditEvent), e);
			}

			this.events.Add(e);
			this.events.Save();

			this.TempData["message"] = $"{e.Name} has been created.";

			// TODO Redirect to view all events
			return this.RedirectToAction("");
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

			this.events.Update(e);
			this.events.Save();

			this.TempData["message"] = $"{e.Name} has been updated.";

			// TODO Redirect to view all events
			return this.RedirectToAction("");
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

			// TODO Redirect to view all events
			return this.RedirectToAction("");
		}
	}
}
