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
            routes.MapRoute(
                name: "PersonalArea",
                url: "Personal_Area/PersonalArea",
                defaults: new { controller = "PersonalArea", action = "PersonalArea", id = UrlParameter.Optional }
            );

            // נתיב התחברות/הרשמה
            routes.MapRoute(
                name: "LoginRegister",
                url: "LoginRegister",
                defaults: new { controller = "Users", action = "LoginRegister" }
            );

            // נתיב לדף BuyBorrowBook
            routes.MapRoute(
                name: "BuyBorrowBook",
                url: "books/BuyBorrowBook",
                defaults: new { controller = "books", action = "BuyBorrowBook" }
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