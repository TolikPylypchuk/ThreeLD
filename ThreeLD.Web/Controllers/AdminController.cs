using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using ThreeLD.DB.Infrastructure;
using ThreeLD.DB.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using ThreeLD.Web.Models.ViewModels;

namespace ThreeLD.Web.Controllers
{
	public class AdminController : Controller
	{
        private AppUserManager UserManager
                => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

        public ActionResult Index()
		{
			return View(UserManager.Users);
		}

		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> Create(CreateModel model)
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

				IdentityResult result = await UserManager.CreateAsync(user,
				    model.Password);

				if (result.Succeeded)
				{
					return RedirectToAction("Index");
				}
				else
				{
					AddErrorsFromResult(result);
				}
			}

			return View(model);
		}

		[HttpPost]
		public async Task<ActionResult> Delete(string id)
		{
			User user = await UserManager.FindByIdAsync(id);

			if (user != null)
			{
				IdentityResult result = await UserManager.DeleteAsync(user);
				if (result.Succeeded)
				{
					return RedirectToAction("Index");
				}
				else
				{
					return View("Error", result.Errors);
				}
			}
			else
			{
				return View("Error", new string[] { "User Not Found" });
			}
		}

		public async Task<ActionResult> Edit(string id)
		{
			User user = await UserManager.FindByIdAsync(id);

			if (user != null)
			{
				return View(user);
			}
			else
			{
				return RedirectToAction("Index");
			}
		}

		[HttpPost]
		public async Task<ActionResult> Edit(string id, string email, string password)
		{
			User user = await UserManager.FindByIdAsync(id);
			if (user != null)
			{
				user.Email = email;
				IdentityResult validEmail
				= await UserManager.UserValidator.ValidateAsync(user);

				if (!validEmail.Succeeded)
				{
					AddErrorsFromResult(validEmail);
				}

				IdentityResult validPass = null;

				if (password != string.Empty)
				{
					validPass
					    = await UserManager.PasswordValidator.ValidateAsync(password);
					if (validPass.Succeeded)
					{
						user.PasswordHash =
						UserManager.PasswordHasher.HashPassword(password);
					}
					else
					{
						AddErrorsFromResult(validPass);
					}
				}

				if ((validEmail.Succeeded && validPass == null) || (validEmail.Succeeded
				    && password != string.Empty && validPass.Succeeded))
				{
					IdentityResult result = await UserManager.UpdateAsync(user);

					if (result.Succeeded)
					{
						return RedirectToAction("Index");
					}
					else
					{
						AddErrorsFromResult(result);
					}
				}
			}
			else
			{
				ModelState.AddModelError("", "User Not Found");
			}

			return View(user);
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