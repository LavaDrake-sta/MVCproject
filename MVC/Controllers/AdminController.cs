using MVC.Models;
using MyMvcProject.Data;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web.Mvc;
using System;

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
            var books = db.books.Select(b => new
            {
                b.book_id,
                b.book_name,
                b.category,
                b.language,
                b.publication_date,
                b.publisher,
                b.link,
                b.price,
                b.ImageUrl
            }).ToList();
            return Json(books, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddBook(books book)
        {
            if (ModelState.IsValid)
            {
                db.books.Add(book);
                db.SaveChanges();

                return Json(new { success = true, message = "ספר נוסף בהצלחה." });
            }

            return Json(new { success = false, message = "שגיאה בהוספת הספר." });
        }

        [HttpPost]
        public JsonResult EditBook(books book)
        {
            var existingBook = db.books.Find(book.book_id);
            if (existingBook != null)
            {
                existingBook.book_name = book.book_name;
                existingBook.category = book.category;
                existingBook.language = book.language;
                existingBook.publication_date = book.publication_date;
                existingBook.publisher = book.publisher;
                existingBook.link = book.link;
                existingBook.price = book.price;
                existingBook.ImageUrl = book.ImageUrl;
                db.SaveChanges();
                return Json(new { success = true, message = "הספר עודכן בהצלחה." });
            }
            return Json(new { success = false, message = "הספר לא נמצא." });
        }

        [HttpPost]
        public JsonResult DeleteBook(int id)
        {
            var book = db.books.Find(id);
            if (book != null)
            {
                db.books.Remove(book);
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
                u.type // ודא ששדה זה קיים בטבלה
            }).ToList();

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddUser(users user)
        {
            if (ModelState.IsValid)
            {
                user.password = HashPassword(user.password);
                db.users.Add(user);
                db.SaveChanges();
                return Json(new { success = true, message = "משתמש נוסף בהצלחה." });
            }
            return Json(new { success = false, message = "שגיאה בהוספת המשתמש." });
        }

        [HttpPost]
        public JsonResult EditUser(users user)
        {
            var existingUser = db.users.FirstOrDefault(u => u.email == user.email);
            if (existingUser != null)
            {
                existingUser.name = user.name;
                existingUser.type = user.type;
                db.SaveChanges();
                return Json(new { success = true, message = "המשתמש עודכן בהצלחה." });
            }
            return Json(new { success = false, message = "המשתמש לא נמצא." });
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

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
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
        public JsonResult AddToWaitingList(waiting_list waitingEntry)
        {
            if (ModelState.IsValid)
            {
                db.waiting_Lists.Add(waitingEntry);
                db.SaveChanges();

                return Json(new { success = true, message = "נוסף בהצלחה לרשימת ההמתנה." });
            }

            return Json(new { success = false, message = "שגיאה בהוספת לרשימת ההמתנה." });
        }

        [HttpPost]
        public JsonResult EditWaitingList(waiting_list waitingEntry)
        {
            var existingEntry = db.waiting_Lists.Find(waitingEntry.email);
            if (existingEntry != null)
            {
                existingEntry.name = waitingEntry.name;
                existingEntry.book_name = waitingEntry.book_name;
                existingEntry.date = waitingEntry.date;
                db.SaveChanges();
                return Json(new { success = true, message = "רשימת ההמתנה עודכנה בהצלחה." });
            }
            return Json(new { success = false, message = "הפריט לא נמצא ברשימת ההמתנה." });
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
        [HttpGet]
        public JsonResult GetSiteReviews()
        {
            var siteReviews = db.reviews
                .Where(r => r.type == "Site") // שליפת ביקורות על האתר בלבד
                .Select(r => new
                {
                    r.ID_review,
                    r.Content,
                    r.email,
                })
                .ToList();

            return Json(siteReviews, JsonRequestBehavior.AllowGet);
        }
    }
}
