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
        public ActionResult BookReview(int bookId)
        {

            ViewBag.Book = db.books.FirstOrDefault(b => b.book_id == bookId);
            return View();
        }

        // הוספת ביקורת על ספר
        [HttpPost]
        public ActionResult AddReview(int bookId, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                TempData["ErrorMessage"] = "לא ניתן להוסיף ביקורת ריקה.";
                return RedirectToAction("BookReview", new { bookId });
            }

            db.reviews.Add(new review
            {
                email = User.Identity.Name, // שם משתמש מחובר
                Content = content,
                type = "Book",
                book_ID = bookId
            });

            db.SaveChanges();
            TempData["SuccessMessage"] = "הביקורת נוספה בהצלחה!";
            return RedirectToAction("BookReview", new { bookId });
        }
    }
}
