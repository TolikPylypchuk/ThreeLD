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
        static AppDbContext()
        {
            Database.SetInitializer<AppDbContext>(new IdentityDbInit());
        }
        public static AppDbContext Create()
        {
            return new AppDbContext();
        }
        public DbSet<Event> Events { get; set; }
		public DbSet<Preference> Preferences { get; set; }
	}

    public class IdentityDbInit : DropCreateDatabaseIfModelChanges<AppDbContext>
    {
        protected override void Seed(AppDbContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }
        public void PerformInitialSetup(AppDbContext context)
        {
            AppUserManager userMgr = new AppUserManager(new UserStore<User>(context));
            AppRoleManager roleMgr = new AppRoleManager(new RoleStore<AppRole>(context));
            string roleName = "Administrators";
            string userName = "Admin";
            string password = "MySecret";
            string email = "admin@example.com";
            if (!roleMgr.RoleExists(roleName))
            {
                roleMgr.Create(new AppRole(roleName));
            }
            User user = userMgr.FindByName(userName);
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
    }
}
