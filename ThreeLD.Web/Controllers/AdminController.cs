﻿using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using ThreeLD.DB.Infrastructure;
using ThreeLD.DB.Models;
using ThreeLD.Web.Models.ViewModels;
using System.Collections.Generic;

namespace ThreeLD.Web.Controllers
{
	public class AdminController : Controller
	{
        private AppUserManager UserManager
                => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

        public ActionResult Index()
        {
            return RedirectToAction(nameof(this.ViewUsers));
        }

        [HttpGet]
        public ViewResult ViewUsers()
        {
            var model = new AdminUsersModel
            {
                Users = new Dictionary<User, bool>()
            };

            foreach (var user in this.UserManager.Users)
            {
                model.Users.Add(user, this.UserManager.IsInRole(user.Id, "Editor"));
            }

            return View(model);
        }

        [HttpPost]
        [ExcludeFromCodeCoverage]
        public ActionResult AssignEditor(string id)
        {
            this.UserManager.RemoveFromRole(id, "User");
            this.UserManager.AddToRole(id, "Editor");

            return RedirectToAction(nameof(ViewUsers));
        }

        [HttpPost]
        [ExcludeFromCodeCoverage]
        public ActionResult UnassignEditor(string id)
        {
            this.UserManager.RemoveFromRole(id, "Editor");
            this.UserManager.AddToRole(id, "User");

            return RedirectToAction(nameof(ViewUsers));
        }

		[HttpPost]
		public async Task<ActionResult> DeleteUser(string id)
		{
			User user = await UserManager.FindByIdAsync(id);

			if (user != null)
			{
				IdentityResult result = await UserManager.DeleteAsync(user);
				if (result.Succeeded)
				{
					return RedirectToAction(nameof(Index));
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
	}
}