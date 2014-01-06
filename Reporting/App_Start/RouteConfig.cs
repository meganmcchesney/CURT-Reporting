using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Reporting {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Login",
                url: "auth",
                defaults: new { controller = "Authenticate", action = "Index" });
            routes.MapRoute(
                name: "Logout",
                url: "logout",
                defaults: new { controller = "Authenticate", action = "Logout" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Authenticate", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}