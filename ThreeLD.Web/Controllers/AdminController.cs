using System.Diagnostics.CodeAnalysis;
﻿using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using ThreeLD.DB.Infrastructure;
using ThreeLD.DB.Models;
using ThreeLD.Web.Models.ViewModels;

namespace ThreeLD.Web.Controllers
{
	public class AdminController : Controller
	{
        private AppUserManager UserManager
            => this.HttpContext.GetOwinContext()
				   .GetUserManager<AppUserManager>();

		private AppRoleManager RoleManager
			=> this.HttpContext.GetOwinContext()
				.GetUserManager<AppRoleManager>();

		public ActionResult Index()
        {
            return this.RedirectToAction(nameof(this.ViewUsers));
        }

        [HttpGet]
        public ViewResult ViewUsers()
        {
	        var role = this.RoleManager.FindByName("Admin");

            var model = new AdminUsersModel
            {
                Users = new Dictionary<User, bool>(),
				Admins = this.UserManager.Users
					.Where(user => user.Roles.Any(r => r.RoleId == role.Id))
					.Select(user => user.Id),
				CurrentUserId = this.HttpContext.User.Identity.GetUserId()
            };

            foreach (var user in this.UserManager.Users)
            {
                model.Users.Add(
					user, this.UserManager.IsInRole(user.Id, "Editor"));
            }

            return this.View(model);
        }

        [HttpPost]
        [ExcludeFromCodeCoverage]
        public ActionResult AssignEditor(string id)
        {
            this.UserManager.RemoveFromRole(id, "User");
            this.UserManager.AddToRole(id, "Editor");

            return this.RedirectToAction(nameof(ViewUsers));
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
			var user = await UserManager.FindByIdAsync(id);

			if (user != null)
			{
				var result = await UserManager.DeleteAsync(user);
				if (result.Succeeded)
				{
					return RedirectToAction(nameof(Index));
				}
				
				return View("Error", result.Errors);
			}

			return View("Error", new[] { "User Not Found" });
		}
	}
}