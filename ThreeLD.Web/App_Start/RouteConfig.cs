using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using System.Web.Routing;

using ThreeLD.Web.Localization;

namespace ThreeLD.Web
{
	[ExcludeFromCodeCoverage]
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			
			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new
				{
					controller = "Home",
					action = "Index",
					id = UrlParameter.Optional
				});

			foreach (var routeBase in routes)
			{
				var route = (Route)routeBase;

				if (!(route.RouteHandler is SingleLanguageMvcRouteHandler))
				{
					route.RouteHandler = new MultiLanguageMvcRouteHandler();
					route.Url = "{lang}/" + route.Url;

					if (route.Defaults == null)
					{
						route.Defaults = new RouteValueDictionary();
					}

					route.Defaults.Add(
						"lang", Language.En.ToString().ToLower());

					if (route.Constraints == null)
					{
						route.Constraints = new RouteValueDictionary();
					}

					route.Constraints.Add(
						"lang",
						new LanguageConstraint(
							Language.En.ToString().ToLower(),
							Language.Uk.ToString().ToLower()));
				}
			}
		}
	}
}
