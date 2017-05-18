using System.Linq;
using System.Web;
using System.Web.Routing;

namespace ThreeLD.Web.Localization
{
	public class LanguageConstraint : IRouteConstraint
	{
		private string[] validValues;

		public LanguageConstraint(params string[] validValues)
		{
			this.validValues = validValues;
		}

		public bool Match(
			HttpContextBase httpContext,
			Route route,
			string parameterName,
			RouteValueDictionary values,
			RouteDirection routeDirection)
		{
			return this.validValues.Contains(values[parameterName].ToString());
		}
	}
}
