using System;
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

		[HttpGet]
		public ActionResult Login(string returnUrl)
		{
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				return this.RedirectToAction(
					nameof(HomeController.Index), "Home");
			}

			this.ViewBag.returnUrl = returnUrl;

			return this.View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(
			LoginModel details, string returnUrl)
		{
			if (this.ModelState.IsValid)
			{
				var user = await UserManager.FindAsync(
					details.UserName, details.Password);

				if (user == null)
				{
					this.ModelState.AddModelError(
						"", "Invalid name or password.");
				} else
				{
					var ident = await this.UserManager.CreateIdentityAsync(
						user, DefaultAuthenticationTypes.ApplicationCookie);

					this.AuthManager.SignOut();
					this.AuthManager.SignIn(
						new AuthenticationProperties
						{
							IsPersistent = false
						},
						ident);

					if (String.IsNullOrEmpty(returnUrl))
					{
						return this.RedirectToAction(
							nameof(HomeController.Index), "Home");
					}

					return this.Redirect(returnUrl);
				}
			}

			this.ViewBag.returnUrl = returnUrl;

			return this.View(details);
		}

		[Authorize]
		public ActionResult Logout()
		{
			this.AuthManager.SignOut();
			return this.RedirectToAction(nameof(HomeController.Index), "Home");
		}
        
		[HttpGet]
		public ActionResult SignUp()
		{
			if (this.HttpContext.User.Identity.IsAuthenticated)
			{
				return RedirectToAction(
					nameof(HomeController.Index), "Home");
			}

			return this.View();
		}

		[HttpPost]
		public async Task<ActionResult> SignUp(SignUpModel model)
		{
			if (this.ModelState.IsValid)
			{
				var user = new User
				{
					UserName = model.UserName,
					FirstName = model.FirstName,
					LastName = model.LastName,
					Email = model.Email
				};

				var result = await this.UserManager.CreateAsync(
					user, model.Password);

				if (result.Succeeded)
				{
					var ident = await this.UserManager.CreateIdentityAsync(
						user, DefaultAuthenticationTypes.ApplicationCookie);

					this.AuthManager.SignIn(
						new AuthenticationProperties
						{
							IsPersistent = true
						},
						ident);

					return this.RedirectToAction(
						nameof(HomeController.Index), "Home");
				}

				this.AddErrorsFromResult(result);
			}

			return this.View(model);
		}

		private void AddErrorsFromResult(IdentityResult result)
		{
			foreach (string error in result.Errors)
			{
				this.ModelState.AddModelError("", error);
			}
		}
	}
}
