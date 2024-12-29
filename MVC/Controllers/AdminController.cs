using MVC.Models;
using MyMvcProject.Data;
using System;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web.Mvc;

namespace MyMvcProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly MvcProjectContext db = new MvcProjectContext();

        public ActionResult AdminPage()
        {
            return View();
        }

        // ספרים
        [HttpGet]
        public JsonResult GetBooksList()
        {
            var books = db.borrowing_Books.Select(b => new
            {
                b.book_id,
                b.book_name,
                b.category,
                b.available,
                b.price
            }).ToList();
            return Json(books, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddBook(Borrowing_books book)
        {
            if (ModelState.IsValid)
            {
                db.borrowing_Books.Add(book);
                db.SaveChanges();
                return Json(new { success = true, message = "ספר נוסף בהצלחה." });
            }
            return Json(new { success = false, message = "שגיאה בהוספת הספר." });
        }

        [HttpPost]
        public JsonResult EditBook(Borrowing_books book)
        {
            var existingBook = db.borrowing_Books.Find(book.book_id);
            if (existingBook != null)
            {
                existingBook.book_name = book.book_name;
                existingBook.category = book.category;
                existingBook.available = book.available;
                existingBook.price = book.price;
                db.SaveChanges();
                return Json(new { success = true, message = "הספר עודכן בהצלחה." });
            }
            return Json(new { success = false, message = "הספר לא נמצא." });
        }

        [HttpPost]
        public JsonResult DeleteBook(int id)
        {
            var book = db.borrowing_Books.Find(id);
            if (book != null)
            {
                db.borrowing_Books.Remove(book);
                db.SaveChanges();
                return Json(new { success = true, message = "הספר נמחק בהצלחה." });
            }
            return Json(new { success = false, message = "הספר לא נמצא." });
        }

        // משתמשים
        [HttpGet]
        public JsonResult GetUsersList()
        {
            var users = db.users.Select(u => new
            {
                u.name,
                u.email,
                u.type
            }).ToList();
            return Json(users, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteUser(string email)
        {
            var user = db.users.FirstOrDefault(u => u.email == email);
            if (user != null)
            {
                db.users.Remove(user);
                db.SaveChanges();
                return Json(new { success = true, message = "המשתמש נמחק בהצלחה." });
            }
            return Json(new { success = false, message = "המשתמש לא נמצא." });
        }

        // רשימת המתנה
        [HttpGet]
        public JsonResult GetWaitingList()
        {
            var waitingList = db.waiting_Lists.Select(w => new
            {
                w.name,
                w.book_name,
                w.date,
                w.email
            }).ToList();
            return Json(waitingList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteFromWaitingList(string email)
        {
            var waitingUser = db.waiting_Lists.FirstOrDefault(w => w.email == email);
            if (waitingUser != null)
            {
                db.waiting_Lists.Remove(waitingUser);
                db.SaveChanges();
                return Json(new { success = true, message = "המשתמש הוסר מרשימת ההמתנה." });
            }
            return Json(new { success = false, message = "המשתמש לא נמצא ברשימת ההמתנה." });
        }

        // ספרים מושאלים
        [HttpGet]
        public JsonResult GetBorrowedBooks()
        {
            var borrowedBooks = db.borrowed_Books_Lists.Select(b => new
            {
                b.book_id,
                b.book_name,
                b.category,
                b.Date_taken,
                b.return_date
            }).ToList();
            return Json(borrowedBooks, JsonRequestBehavior.AllowGet);
        }

        // שליחת מייל
        [HttpPost]
        public JsonResult SendEmailToUser(string email, string subject, string body)
        {
            try
            {
                SendEmail(email, subject, body);
                return Json(new { success = true, message = "המייל נשלח בהצלחה." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "שגיאה בשליחת המייל: " + ex.Message });
            }
        }

        private void SendEmail(string toEmail, string subject, string body)
        {
            var username = System.Configuration.ConfigurationManager.AppSettings["EmailUsername"];
            var password = System.Configuration.ConfigurationManager.AppSettings["EmailPassword"];

            using (var smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.Credentials = new NetworkCredential(username, password);
                smtp.EnableSsl = true;

                var mail = new MailMessage(username, toEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                smtp.Send(mail);
            }
        }
    }
}
