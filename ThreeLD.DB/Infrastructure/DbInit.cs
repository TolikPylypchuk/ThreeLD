using System.Data.Entity;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using ThreeLD.DB.Models;

namespace ThreeLD.DB.Infrastructure
{
	public class DbInit : DropCreateDatabaseAlways<AppDbContext>
	{
		public void PerformInitialSetup(AppDbContext context)
		{
			var userMgr = new AppUserManager(new UserStore<User>(context));
			var roleMgr = new AppRoleManager(new RoleStore<AppRole>(context));

			foreach (string role in new[] { "User", "Editor", "Admin" })
			{
				roleMgr.Create(new AppRole(role));
			}

			const string password = "password";
			
			var tolik = new User
			{
				FirstName = "Tolik",
				LastName = "Pylypchuk",
				UserName = "tolik",
				Email = "tolik@example.com"
			};

			userMgr.Create(tolik, password);
			userMgr.AddToRole(tolik.Id, "Admin");

			var bozhena = new User
			{
				FirstName = "Bozhena",
				LastName = "Telepko",
				UserName = "bozhena",
				Email = "bozhena@example.com"
			};
			
			userMgr.Create(bozhena, password);
			userMgr.AddToRole(bozhena.Id, "Editor");

			var artur = new User
			{
				FirstName = "Artur",
				LastName = "Zayats",
				UserName = "artur",
				Email = "artur@example.com"
			};

			userMgr.Create(artur, password);
			userMgr.AddToRole(artur.Id, "User");

			var natalia = new User
			{
				FirstName = "Natalia",
				LastName = "Slobodianiuk",
				UserName = "natalia",
				Email = "natalia@example.com"
			};

			userMgr.Create(natalia, password);
			userMgr.AddToRole(natalia.Id, "User");

			var julia = new User
			{
				FirstName = "Julia",
				LastName = "Zvizlo",
				UserName = "julia",
				Email = "julia@example.com"
			};

			userMgr.Create(julia, password);
			userMgr.AddToRole(julia.Id, "User");
		}

		protected override void Seed(AppDbContext context)
		{
			this.PerformInitialSetup(context);
			base.Seed(context);
		}
	}
}
