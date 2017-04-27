using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity.Owin;

using ThreeLD.DB.Infrastructure;

namespace ThreeLD.Web.Infrastructure
{
	[ExcludeFromCodeCoverage]
	public static class IdentityHelpers
	{
		public static MvcHtmlString GetUserName(
			this HtmlHelper html,
			string id)
		{
			var mgr = HttpContext.Current.GetOwinContext()
				.GetUserManager<AppUserManager>();
			return new MvcHtmlString(mgr.FindByIdAsync(id).Result.UserName);
		}
	}
}