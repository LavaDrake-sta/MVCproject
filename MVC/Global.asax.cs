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
            NotifyUsersWithFiveDaysLeft();
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

        private void NotifyUsersWithFiveDaysLeft()
        {
            using (var db = new MvcProjectContext())
            {
                var today = DateTime.Today;
                var fiveDaysFromNow = today.AddDays(5);

                var booksWithFiveDaysLeft = db.borrowing_Books
                    .Where(b => DbFunctions.TruncateTime(b.return_date) == fiveDaysFromNow)
                    .ToList();

                foreach (var book in booksWithFiveDaysLeft)
                {
                    var userEmail = book.email;
                    var bookName = book.book_name;

                    // שליחת מייל למשתמש
                    try
                    {
                        EmailService emailService = new EmailService();
                        string subject = "Reminder: 5 days left to return your book";
                        string body = $@"
                            <h1>Reminder</h1>
                            <p>Dear User,</p>
                            <p>This is a friendly reminder that you have 5 days left to return the book:</p>
                            <p><strong>{bookName}</strong></p>
                            <p>Please make sure to return it on time to avoid penalties.</p>
                            <p>Thank you,</p>
                            <p><strong>Your Digital Library Team</strong></p>";

                        emailService.SendEmail(userEmail, subject, body);
                        Console.WriteLine($"Reminder email sent to {userEmail} for book '{bookName}'.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to send reminder email to {userEmail}: {ex.Message}");
                    }
                }
            }
        }
    }
}