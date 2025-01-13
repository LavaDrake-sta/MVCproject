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
        public ActionResult AddReview(string bookName, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                TempData["ErrorMessage"] = "לא ניתן להוסיף ביקורת ריקה.";
                return RedirectToAction("BookReview", new { bookName });
            }

            var book = db.books.FirstOrDefault(b => b.book_name == bookName);
            if (book == null)
            {
                TempData["ErrorMessage"] = "הספר לא נמצא.";
                return RedirectToAction("PersonalArea");
            }

            db.reviews.Add(new review
            {
                email = User.Identity.Name,
                Content = content,
                type = "Book",
                book_ID = book.book_id
            });

            db.SaveChanges();
            TempData["SuccessMessage"] = "הביקורת נוספה בהצלחה!";
            return RedirectToAction("PersonalArea");
        }
    }
}
