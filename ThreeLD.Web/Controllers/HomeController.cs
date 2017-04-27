using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using ThreeLD.DB.Infrastructure;

namespace ThreeLD.Web.Controllers
{
	[ExcludeFromCodeCoverage]
    public class HomeController : Controller
	{
		private AppUserManager UserManager
			=> HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

		public ActionResult Index()
		{
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				return RedirectToAction(
					"Index",
					UserManager.GetRoles(
						HttpContext.User.Identity.GetUserId())[0]);
			}

			return RedirectToAction("ViewEvents", "Guest");
		}
	}
}
