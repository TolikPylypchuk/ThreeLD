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
			return this.View(new Event());
		}

		[HttpPost]
		public ActionResult CreateEvent(Event e)
		{
			if (!this.ModelState.IsValid)
			{
				return this.View(e);
			}

			this.events.Add(e);
			this.events.Save();

			return this.RedirectToAction(nameof(this.CreateEvent));
		}
	}
}
