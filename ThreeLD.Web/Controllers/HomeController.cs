using System.Web.Mvc;

using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;

namespace ThreeLD.Web.Controllers
{
	public class HomeController : Controller
	{
		private IRepository<Event> events;

		public HomeController(IRepository<Event> events)
		{
			this.events = events;
		}
        
        public ActionResult Index()
		{
			return this.View(this.events.GetAll());
		}
	}
}
