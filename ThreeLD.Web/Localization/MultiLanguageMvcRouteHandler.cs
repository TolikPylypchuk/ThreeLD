using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ThreeLD.Web.Localization
{
	[ExcludeFromCodeCoverage]
	public class MultiLanguageMvcRouteHandler : MvcRouteHandler
	{
		protected override IHttpHandler GetHttpHandler(
			RequestContext requestContext)
		{
			string lang = requestContext.RouteData
				.Values["lang"]?.ToString() ??
					"en";
			
			var ci = new CultureInfo(lang);

			Thread.CurrentThread.CurrentUICulture = ci;
			Thread.CurrentThread.CurrentCulture =
				CultureInfo.CreateSpecificCulture(ci.Name);

			return base.GetHttpHandler(requestContext);
		}
	}
}
