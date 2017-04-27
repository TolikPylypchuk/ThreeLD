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
				var user = await UserManager.FindAsync(
                    details.UserName, details.Password);

				if (user == null)
				{
					ModelState.AddModelError("", "Invalid name or password.");
				}
				else
				{
					var ident = await UserManager.CreateIdentityAsync(
                        user, DefaultAuthenticationTypes.ApplicationCookie);

					AuthManager.SignOut();
					AuthManager.SignIn(
                        new AuthenticationProperties
					    {
						    IsPersistent = false
					    },
                        ident);

					if (String.IsNullOrEmpty(returnUrl))
					{
						return RedirectToAction(
                            "Index", UserManager.GetRoles(user.Id)[0]);
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
        
        [HttpGet]
		public ActionResult SignUp()
		{
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction(
                    nameof(UserController.ViewEvents), "User");
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

				var result = await UserManager.CreateAsync(
                    user, model.Password);

				if (result.Succeeded)
				{
                    var ident = await UserManager.CreateIdentityAsync(
                        user, DefaultAuthenticationTypes.ApplicationCookie);

                    AuthManager.SignIn(
                        new AuthenticationProperties
                        {
                            IsPersistent = true
                        },
                        ident);

                    return RedirectToAction(
                        nameof(UserController.ViewEvents), "User");
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
	}
}
