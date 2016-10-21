using HttpModuleAndHandlerDemo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HttpModuleAndHandlerDemo
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            // Tell ASP.NET Routing Pipeline to process the physical files by activating RouteExistingFiles to true
            RouteTable.Routes.RouteExistingFiles = true;
            // IMPORTANT: we configure the image route first of all others routes
            RouteTable.Routes.Add("ImageRoute", new Route("images/{name}", new WatermarkImageHandler()));

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
