using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MVC.Models;
using MyMvcProject.Data;

namespace MyMvcProject.Controllers
{
    public class PersonalAreaController : Controller
    {
        private readonly MvcProjectContext db;

        public PersonalAreaController()
        {
            db = new MvcProjectContext();
        }

        private bool IsUserLoggedIn()
        {
            return Session["UserName"] != null;
        }

        public ActionResult PersonalArea()
        {
            // 🔒 בדיקה אם המשתמש מחובר
            if (Session["UserName"] == null)
            {
                TempData["ErrorMessage"] = "עליך להתחבר כדי לצפות באזור האישי.";
                return RedirectToAction("Login", "Users");
            }

            // 📧 שליפת שם המשתמש והאימייל שלו
            string userName = Session["UserName"].ToString();
            string userEmail = db.users
                .Where(u => u.name == userName)
                .Select(u => u.email)
                .FirstOrDefault();

            // ❌ בדיקה אם לא נמצא אימייל
            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["ErrorMessage"] = "משתמש לא נמצא במערכת.";
                return RedirectToAction("Login", "Users");
            }

            try
            {
                // ✅ שליפת כל ההזמנות שבוצעו לפי המייל
                var allOrders = db.orders
                    .Where(o => o.email == userEmail)
                    .OrderByDescending(o => o.date)  // סידור מהחדש לישן
                    .ToList();

                // ✅ שליפת כל הספרים שנרכשו (Buy)
                var purchasedBooks = allOrders
                    .Where(o => o.buy_borrow == "Buy")
                    .ToList();

                // ✅ שליפת כל הספרים שהושכרו (Rent)
                var rentedBooks = allOrders
                    .Where(o => o.buy_borrow == "Rent")
                    .ToList();

                // ✅ שליפת כל הספרים המושכרים לפי המייל
                var borrowedBooks = db.borrowing_Books
                    .Where(b => b.email == userEmail)
                    .OrderBy(b => b.return_date)  // סידור לפי תאריך החזרה
                    .ToList();

                // ✅ שליפת כל הספרים ברשימת המתנה לפי המייל
                var waitingBooks = db.waiting_Lists
                    .Where(w => w.email == userEmail)
                    .OrderBy(w => w.date)  // סידור לפי תאריך הצטרפות
                    .ToList();

                // 📦 שליחת המידע ל-View
                ViewBag.AllOrders = allOrders;
                ViewBag.PurchasedBooks = purchasedBooks;
                ViewBag.RentedBooks = rentedBooks;
                ViewBag.BorrowedBooks = borrowedBooks;
                ViewBag.WaitingBooks = waitingBooks;

                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "אירעה שגיאה בעת שליפת המידע: " + ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult UpdateDetails(string name, string email, string password)
        {
            if (!IsUserLoggedIn())
            {
                TempData["ErrorMessage"] = "עליך להתחבר כדי לעדכן פרטים.";
                return RedirectToAction("Login", "Users");
            }

            string userName = Session["UserName"].ToString();
            var user = db.users.FirstOrDefault(u => u.name == userName);

            if (user == null)
            {
                TempData["ErrorMessage"] = "משתמש לא נמצא.";
                return RedirectToAction("PersonalArea");
            }

            // עדכון פרטי המשתמש
            if (!string.IsNullOrEmpty(name))
                user.name = name;

            if (!string.IsNullOrEmpty(email))
                user.email = email;

            if (!string.IsNullOrEmpty(password))
                user.password = HashPassword(password);

            db.SaveChanges();
            Session["UserName"] = user.name;
            TempData["SuccessMessage"] = "הפרטים האישיים עודכנו בהצלחה.";
            return RedirectToAction("PersonalArea");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return System.Convert.ToBase64String(hash);
            }
        }
    }
}
