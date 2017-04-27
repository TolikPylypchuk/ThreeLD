using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

using ThreeLD.DB.Infrastructure;
using ThreeLD.DB.Models;
using ThreeLD.Web.Models.ViewModels;

namespace ThreeLD.Web.Controllers
{
	public class AccountController : Controller
	{
		private IAuthenticationManager AuthManager
			=> HttpContext.GetOwinContext().Authentication;

		private AppUserManager UserManager
			=> HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

		public ActionResult Login(string returnUrl)
		{
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				return RedirectToAction(
                    "Index",
                    UserManager.GetRoles(
                        HttpContext.User.Identity.GetUserId())[0]);
			}

			ViewBag.returnUrl = returnUrl;

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginModel details, string returnUrl)
		{
			if (ModelState.IsValid)
			{
				var user = await UserManager.FindAsync(details.UserName,
					details.Password);

				if (user == null)
				{
					ModelState.AddModelError("", "Invalid name or password.");
				}
				else
				{
					var ident = await UserManager.CreateIdentityAsync(user,
					DefaultAuthenticationTypes.ApplicationCookie);
					AuthManager.SignOut();
					AuthManager.SignIn(new AuthenticationProperties
					{
						IsPersistent = false
					}, ident);

					if (String.IsNullOrEmpty(returnUrl))
					{
						return RedirectToAction("ViewEvents", "Guest");
					}

					return Redirect(returnUrl);
				}
			}

			ViewBag.returnUrl = returnUrl;

			return View(details);
		}

		[Authorize]
		public ActionResult Logout()
		{
			AuthManager.SignOut();
			return RedirectToAction("Index", "Home");
		}

		[Authorize(Roles = "User")]
		public ActionResult OtherAction()
		{
			return View("AccountSettings", GetData("OtherAction"));
		}

		public ActionResult SignUp()
		{
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("ViewEvents", "Guest");
            }

            return View();
		}

		[HttpPost]
		public async Task<ActionResult> SignUp(SignUpModel model)
		{
			if (ModelState.IsValid)
			{
				User user = new User
				{
					UserName = model.UserName,
					FirstName = model.FirstName,
					LastName = model.LastName,
					Email = model.Email
				};

				var result = await UserManager.CreateAsync(user,
				    model.Password);

				if (result.Succeeded)
				{
					return RedirectToAction("ViewEvents", "Guest");
				}

				AddErrorsFromResult(result);
			}

			return View(model);
		}

		private void AddErrorsFromResult(IdentityResult result)
		{
			foreach (string error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private Dictionary<string, object> GetData(string actionName)
		{
			var dict = new Dictionary<string, object>
			{
				{ "Action", actionName },
				{ "User", this.HttpContext.User.Identity.Name },
				{ "Authenticated", this.HttpContext.User.Identity.IsAuthenticated },
				{ "Auth Type", this.HttpContext.User.Identity.AuthenticationType },
				{ "In Users Role", this.HttpContext.User.IsInRole("Users") }
			};

			return dict;
		}
	}
}
