using System;
using System.Collections.Generic;
using System.IO;
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
        // הורדת ספר לדוגמה
        public FileResult DownloadBook(int bookId, string format)
        {
            // יצירת נתיב לתיקיית קבצים זמניים
            string tempFolderPath = Server.MapPath("~/TempFiles");

            // בדיקה אם התיקיה קיימת, אם לא - יצירתה
            if (!Directory.Exists(tempFolderPath))
            {
                Directory.CreateDirectory(tempFolderPath);
            }
            // יצירת קובץ טקסט זמני
            string fileName = "SampleBook.txt";
            string filePath = Path.Combine(Server.MapPath("~/TempFiles"), fileName);

            // כתיבת תוכן לדוגמה לקובץ
            if (!System.IO.File.Exists(filePath))
            {
                System.IO.File.WriteAllText(filePath, "This is a sample book content for download.");
            }

            // קביעת סוג הקובץ להורדה
            string contentType = "application/octet-stream";
            if (format == "pdf")
                contentType = "application/pdf";
            else if (format == "epub")
                contentType = "application/epub+zip";
            else if (format == "f2b")
                contentType = "application/octet-stream";
            else if (format == "mobi")
                contentType = "application/x-mobipocket-ebook";

            return File(filePath, contentType, fileName);
        }
        // מחיקת ספר מהיסטוריית ההזמנות
        [HttpPost]
        public ActionResult DeleteOrder(int orderId)
        {
            var order = db.orders.FirstOrDefault(o => o.id == orderId);
            if (order != null)
            {
                db.orders.Remove(order);
                db.SaveChanges();
                TempData["SuccessMessage"] = "ההזמנה נמחקה בהצלחה.";
            }
            return RedirectToAction("PersonalArea");
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

        [HttpPost]
        public ActionResult ReturnBookNow(int bookId)
        {
            if (Session["UserName"] == null)
            {
                TempData["ErrorMessage"] = "עליך להתחבר כדי להחזיר ספר.";
                return RedirectToAction("Login", "Users");
            }

            string userName = Session["UserName"].ToString();

            // שליפת האימייל מה-DB לפי שם המשתמש
            string userEmail = db.users.FirstOrDefault(u => u.name == userName)?.email;

            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["ErrorMessage"] = "משתמש לא נמצא במערכת.";
                return RedirectToAction("Login", "Users");
            }

            // מחיקת הספר מטבלת Borrowing_books
            var borrowedBook = db.borrowing_Books.FirstOrDefault(b => b.book_id == bookId && b.email == userEmail);
            if (borrowedBook != null)
            {
                db.borrowing_Books.Remove(borrowedBook);
            }

            // מחיקת הספר מטבלת Borrowed_books_list
            var borrowedBookList = db.borrowed_Books_Lists.FirstOrDefault(b => b.book_id == bookId);
            if (borrowedBookList != null)
            {
                db.borrowed_Books_Lists.Remove(borrowedBookList);
            }

            // עדכון מונה ההשכרות
            var dbBook = db.books.FirstOrDefault(b => b.book_id == bookId);
            if (dbBook != null)
            {
                if (dbBook.CurrentRentCount.HasValue && dbBook.CurrentRentCount > 0)
                {
                    dbBook.CurrentRentCount--;
                }
                else
                {
                    dbBook.CurrentRentCount = 0;
                }
            }

            db.SaveChanges();

            TempData["SuccessMessage"] = "הספר הוחזר בהצלחה.";
            return RedirectToAction("PersonalArea");
        }
       
        [HttpPost]
        public ActionResult RemoveFromWaitingList(string bookName)
        {
            var waitingBook = db.waiting_Lists.FirstOrDefault(w => w.book_name == bookName);
            if (waitingBook != null)
            {
                db.waiting_Lists.Remove(waitingBook);
                db.SaveChanges();
                TempData["SuccessMessage"] = "הספר הוסר מרשימת ההמתנה בהצלחה.";
            }
            else
            {
                TempData["ErrorMessage"] = "הספר לא נמצא ברשימת ההמתנה.";
            }
            return RedirectToAction("PersonalArea");
        }
    }
}
