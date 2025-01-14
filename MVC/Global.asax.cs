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
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<MyMvcProject.Data.MvcProjectContext, MVC.Migrations.Configuration>());
            AutoReturnBooks();




        }
        private void AutoReturnBooks()
        {
            using (var db = new MvcProjectContext())
            {
                var today = DateTime.Today;
                var overdueBooks = db.borrowing_Books.Where(b => b.return_date < today).ToList();

                foreach (var book in overdueBooks)
                {
                    var dbBook = db.books.FirstOrDefault(bk => bk.book_id == book.book_id);
                    if (dbBook != null && dbBook.CurrentRentCount > 0)
                    {
                        dbBook.CurrentRentCount--;
                    }
                    db.borrowing_Books.Remove(book);
                }
                db.SaveChanges();
            }
        }
    }
}