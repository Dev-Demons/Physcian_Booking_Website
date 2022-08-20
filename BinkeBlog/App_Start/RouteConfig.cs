using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BinkeBlog
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
                       routes.MapRoute(
                "Blog",                                           // Route name
                "Blog/BlogItem/{ArticleName}",                            // URL with parameters
                new { controller = "Blog", action = "Index", type = UrlParameter.Optional, ArticleName = UrlParameter.Optional },// Parameter defaults
                new string[] { "BinkeBlog.Controllers.Index" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Blog", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
