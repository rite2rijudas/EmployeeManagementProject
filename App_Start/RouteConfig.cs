using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EMPLOYEE_MANAGEMENT
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.IgnoreRoute("Content/{*pathInfo}");

            routes.IgnoreRoute("Scripts/{*pathInfo}");

            routes.IgnoreRoute("{WebPage}.aspx/{*pathInfo}");

            routes.IgnoreRoute("{resource}.ashx/{*pathInfo}");

            
            routes.MapRoute(
               name: "Default",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Login", action = "Login", id = UrlParameter.Optional }
           );
        }
    }
}
