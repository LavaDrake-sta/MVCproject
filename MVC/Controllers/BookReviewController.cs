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

            ViewBag.Book = db.books.FirstOrDefault(b => b.book_name == book_name);
            return View();
        }

        // הוספת ביקורת על ספר

        // הוספת ביקורת על ספר לפי שם הספר
        [HttpPost]
        public ActionResult AddReview(int bookid, string content)
        {
            var userName = Session["UserName"].ToString();
            var user = db.users.FirstOrDefault(u => u.name == userName);
            // בדיקה אם המייל קיים
            if (string.IsNullOrEmpty(user.email))
            {
                TempData["ErrorMessage"] = "עליך להיות מחובר כדי להוסיף ביקורת.";
                return RedirectToAction("SiteReview");
            }

            if (string.IsNullOrEmpty(content))
            {
                TempData["ErrorMessage"] = "לא ניתן להוסיף ביקורת ריקה.";
                return RedirectToAction("BookReview", new { bookid });
            }

            var book = db.books.FirstOrDefault(b => b.book_id == bookid);
            if (book == null)
            {
                TempData["ErrorMessage"] = "הספר לא נמצא.";
                return RedirectToAction("PersonalArea");
            }

            int nextReviewId = db.reviews.Any() ? db.reviews.Max(r => r.ID_review) + 1 : 1;

            db.reviews.Add(new review
            {
                ID_review = nextReviewId,
                email = user.email,
                Content = content,
                type = "Book",
                book_ID = bookid,
                created_at = DateTime.Now
            });

            db.SaveChanges();
            TempData["SuccessMessage"] = "הביקורת נוספה בהצלחה!";
            return RedirectToAction("PersonalArea");
        }
    }
}
