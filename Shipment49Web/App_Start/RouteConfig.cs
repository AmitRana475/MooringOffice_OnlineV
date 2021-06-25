using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Shipment49Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
           // routes.MapMvcAttributeRoutes();
            routes.MapRoute(
               name: "Default",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "account", action = "login", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "Home",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
           );

            //routes.MapRoute(
            //name: "Analysis_Area",
            //url: "{controller}/{action}/{id}",
            //defaults: new { area = "Analysis", controller = "analysis", action = "Index", id = UrlParameter.Optional }
            //);


        }
    }
}
