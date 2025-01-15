using System;
using System.Linq;
using System.Web.Mvc;
using MVC.Models;
using MyMvcProject.Data;

namespace MyMvcProject.Controllers
{
    public class BookReviewController : Controller
    {
        private readonly MvcProjectContext db;

        public BookReviewController()
        {
            db = new MvcProjectContext();
        }

        // הצגת ביקורות על ספר מסוים
        public ActionResult BookReview(string book_name)
        {
            if (string.IsNullOrEmpty(book_name))
            {
                TempData["ErrorMessage"] = "ספר לא נבחר.";
                return RedirectToAction("PersonalArea", "PersonalArea");
            }

            var book = db.books.FirstOrDefault(b => b.book_name == book_name);
            if (book == null)
            {
                TempData["ErrorMessage"] = "הספר לא נמצא.";
                return RedirectToAction("PersonalArea", "PersonalArea");
            }

            ViewBag.Book = book;
            return View();
        }

        // הוספת ביקורת על ספר

        // הוספת ביקורת על ספר לפי שם הספר
        [HttpPost]
        public ActionResult AddReview(string book_name, string content)
        {
            if (string.IsNullOrEmpty(book_name))
            {
                TempData["ErrorMessage"] = "ספר לא נבחר.";
                return RedirectToAction("PersonalArea", "PersonalArea");
            }

            if (string.IsNullOrEmpty(content))
            {
                TempData["ErrorMessage"] = "לא ניתן להוסיף ביקורת ריקה.";
                return RedirectToAction("BookReview", new { book_name });
            }

            var userName = Session["UserName"]?.ToString();
            if (string.IsNullOrEmpty(userName))
            {
                TempData["ErrorMessage"] = "עליך להיות מחובר כדי להוסיף ביקורת.";
                return RedirectToAction("Login", "Account");
            }

            var user = db.users.FirstOrDefault(u => u.name == userName);
            var book = db.books.FirstOrDefault(b => b.book_name == book_name);

            if (book == null || user == null)
            {
                TempData["ErrorMessage"] = "הספר או המשתמש לא נמצא.";
                return RedirectToAction("PersonalArea", "PersonalArea");
            }

            // בדיקה אם כבר קיימת ביקורת לאותו ספר
            var existingReview = db.reviews.FirstOrDefault(r => r.book_ID == book.book_id && r.email == user.email);
            if (existingReview != null)
            {
                TempData["ErrorMessage"] = "כבר השארת ביקורת על הספר הזה.";
                return RedirectToAction("BookReview", new { book_name });
            }

            db.reviews.Add(new review
            {
                email = user.email,
                Content = content,
                type = "Book",
                book_ID = book.book_id,
                created_at = DateTime.Now
            });

            db.SaveChanges();

            TempData["SuccessMessage"] = "הביקורת נוספה בהצלחה!";
            return RedirectToAction("PersonalArea", "PersonalArea");
        }
    }
}
