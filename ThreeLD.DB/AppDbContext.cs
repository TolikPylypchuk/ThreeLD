using System.Data.Entity;

using Microsoft.AspNet.Identity.EntityFramework;

using ThreeLD.DB.Models;

using Microsoft.AspNet.Identity;
using ThreeLD.DB.Infrastructure;

namespace ThreeLD.DB
{
	public class AppDbContext : IdentityDbContext<User>
	{
		public AppDbContext() : base("name=DefaultConnection")
		{
		}

		//static AppDbContext()
		//{
		//	Database.SetInitializer<AppDbContext>(new IdentityDbInit());
		//}

		public static AppDbContext Create()
		{
			return new AppDbContext();
		}

		public DbSet<Event> Events { get; set; }
		public DbSet<Preference> Preferences { get; set; }
	}

	public class IdentityDbInit : DropCreateDatabaseIfModelChanges<AppDbContext>
	{
		public void PerformInitialSetup(AppDbContext context)
		{
			var userMgr = new AppUserManager(new UserStore<User>(context));
			var roleMgr = new AppRoleManager(new RoleStore<AppRole>(context));

			const string roleName = "Administrators";
			const string userName = "Admin";
			const string password = "MySecret";
			const string email = "admin@example.com";

			if (!roleMgr.RoleExists(roleName))
			{
				roleMgr.Create(new AppRole(roleName));
			}

			var user = userMgr.FindByName(userName);
			if (user == null)
			{
				userMgr.Create(new User { UserName = userName, Email = email },
				password);
				user = userMgr.FindByName(userName);
			}

			if (!userMgr.IsInRole(user.Id, roleName))
			{
				userMgr.AddToRole(user.Id, roleName);
			}
		}

		protected override void Seed(AppDbContext context)
		{
			this.PerformInitialSetup(context);
			base.Seed(context);
		}
	}
}
