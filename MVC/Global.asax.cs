using MVC.Models;
using MyMvcProject.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<MyMvcProject.Data.MvcProjectContext, MVC.Migrations.Configuration>());

            addadmin();

        }
        private void addadmin()
        {
            using(var context =new MyMvcProject.Data.MvcProjectContext())
            {
                if (!context.users.Any(u => u.email == "admin@admin.com"))
                {
                    string hashedPassword = HashPassword("admin123");
                    // הוסף משתמש מסוג אדמין
                    var adminUser = new users
                    {
                        name = "Admin",
                        email = "admin@admin.com",
                        password = hashedPassword,
                        type = "Admin" // הנחתי שיש לך שדה type לזיהוי משתמשים
                    };

                    context.users.Add(adminUser);
                    context.SaveChanges();
                }
            }
        }
        // פונקציה להצפנת סיסמאות
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

    }
}
