using System.Collections.Generic;
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

        [Authorize]
        public ActionResult IndexLogin()
        {
            return View(GetData("IndexLogin"));
        }
        [Authorize(Roles = "Users")]
        public ActionResult OtherAction()
        {
            return View("IndexLogin", GetData("OtherAction"));
        }
        private Dictionary<string, object> GetData(string actionName)
        {
            Dictionary<string, object> dict
            = new Dictionary<string, object>();
            dict.Add("Action", actionName);
            dict.Add("User", HttpContext.User.Identity.Name);
            dict.Add("Authenticated", HttpContext.User.Identity.IsAuthenticated);
            dict.Add("Auth Type", HttpContext.User.Identity.AuthenticationType);
            dict.Add("In Users Role", HttpContext.User.IsInRole("Users"));
            return dict;
        }
    }
}
