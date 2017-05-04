using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;

using Owin;

using ThreeLD.DB;
using ThreeLD.DB.Infrastructure;

namespace ThreeLD.Web
{
	[ExcludeFromCodeCoverage]
    public class IdentityConfig
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext(
				DependencyResolver.Current.GetService<AppDbContext>);
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
            app.CreatePerOwinContext<AppRoleManager>(AppRoleManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType =
					DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/account/login")
            });
        }
    }
}
