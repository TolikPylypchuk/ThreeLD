using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

using ThreeLD.DB.Infrastructure;
using ThreeLD.DB.Models;
using ThreeLD.Web.Models.ViewModels;

namespace ThreeLD.Web.Controllers
{
	public class AdminController : Controller
	{
        private AppUserManager UserManager
                => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

        [HttpGet]
        public ViewResult ViewUsers()
        {
            var model = new AdminUsersModel();

            foreach (var user in this.UserManager.Users)
            {
                model.Users.Add(user, this.UserManager.IsInRole(user.Id, "Editor"));
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult AssignEditor(string id)
        {
            UserManager.RemoveFromRole(id, "User");
            UserManager.AddToRole(id, "Editor");

            return RedirectToAction(nameof(ViewUsers));
        }

        [HttpPost]
        public ActionResult UnassignEditor(string id)
        {
            UserManager.RemoveFromRole(id, "Editor");
            UserManager.AddToRole(id, "User");

            return RedirectToAction(nameof(ViewUsers));
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

		private void AddErrorsFromResult(IdentityResult result)
		{
			foreach (string error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}
	}
}