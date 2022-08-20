using System.Web;
using System.Web.Routing;

namespace Binke.App_Helpers
{
    public interface IRouteConstraint
    {
        bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection);
    }

    public class CustomRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return true;
        }
    }

    public class CustomRouteWithParameterConstraint : IRouteConstraint
    {

        private readonly string _parameterValue;
        public CustomRouteWithParameterConstraint(string parameteValue)
        {
            _parameterValue = parameteValue;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (!values.TryGetValue(parameterName, out var value) || value == null) return false;
            return value.ToString() == _parameterValue;
        }
    }
}