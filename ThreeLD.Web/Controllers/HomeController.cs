using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using ThreeLD.DB.Infrastructure;
using ThreeLD.Web.Localization;

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
		
		public ActionResult ChangeLanguage(Language newLang, string returnUrl)
		{
			if (!String.IsNullOrEmpty(returnUrl) && returnUrl.Length >= 3)
			{
				returnUrl = returnUrl.Substring(3);
			}

			return this.Redirect(
				$"/{newLang.ToString().ToLower()}/{returnUrl}");
		}
	}
}
