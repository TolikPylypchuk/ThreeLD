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
			if (this.HttpContext.User.Identity.IsAuthenticated)
			{
				return this.RedirectToAction(
					"Index",
					this.UserManager.GetRoles(
						this.HttpContext.User.Identity.GetUserId())[0]);
			}

			return this.RedirectToAction("ViewEvents", "Guest");
		}
	}
}
