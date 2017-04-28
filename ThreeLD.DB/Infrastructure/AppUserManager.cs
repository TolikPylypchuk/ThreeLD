using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

using ThreeLD.DB.Models;

namespace ThreeLD.DB.Infrastructure
{
	public class AppUserManager : UserManager<User>
	{
		public AppUserManager(IUserStore<User> store) : base(store)
		{
		}
		
		public static AppUserManager Create(
			IdentityFactoryOptions<AppUserManager> options,
			IOwinContext context)
		{
			var db = context.Get<AppDbContext>();
			var manager = new AppUserManager(new UserStore<User>(db))
			{
				PasswordValidator = new PasswordValidator
				{
					RequiredLength = 6,
					RequireNonLetterOrDigit = false,
					RequireDigit = false,
					RequireLowercase = false,
					RequireUppercase = false
				}
			};

			manager.UserValidator = new UserValidator<User>(manager)
			{
				AllowOnlyAlphanumericUserNames = true,
				RequireUniqueEmail = true
			};

			return manager;
		}
	}
}
