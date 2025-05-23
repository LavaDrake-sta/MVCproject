﻿using System;
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
            // נתיב לעמוד אזור אישי
            routes.MapRoute(
                name: "PersonalArea",
                url: "PersonalArea/PersonalArea",
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
            //נתיב לעמוד תשלומים
            routes.MapRoute(
                name: "SubmitOrder",
                url: "Order/SubmitOrder",
                defaults: new { controller = "Order", action = "SubmitOrder" }
            );
            // נתיב של עגלה
            routes.MapRoute(
                name: "Cart",
                url: "Cart/Index",
                defaults: new { controller = "Cart", action = "Index" }
            );
            // נתיב לעמוד ביקורת על האתר
            routes.MapRoute(
                name: "SiteReview",
                url: "SiteReview/SiteReview",
                defaults: new { controller = "SiteReview", action = "SiteReview" }
            );
            // נתיב לביקורת על ספר 
            routes.MapRoute(
                name: "BookReview",
                url: "BookReview/BookReview",
                defaults: new { controller = "BookReview", action = "BookReview" }
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