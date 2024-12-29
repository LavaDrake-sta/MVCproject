using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // נתיב לעמוד הבית
            routes.MapRoute(
                name: "HomePage",
                url: "Home/HomePage",
                defaults: new { controller = "Home", action = "HomePage" }
            );

            // נתיב התחברות/הרשמה
            routes.MapRoute(
                name: "LoginRegister",
                url: "LoginRegister",
                defaults: new { controller = "Users", action = "LoginRegister" }
            );

            // נתיב ברירת מחדל
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "HomePage", id = UrlParameter.Optional }
            );
        }
    }
}
