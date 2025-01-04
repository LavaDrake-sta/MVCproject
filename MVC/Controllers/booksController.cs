using MyMvcProject.Data;
using System;
using System.Linq;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class booksController : Controller
    {
        private MvcProjectContext db = new MvcProjectContext();

        // פעולה להצגת רשימת ספרים עם אפשרויות קנייה והשאלה
        public ActionResult BuyBorrowBook()
        {
            var books = db.books.ToList(); // שליפת כל הספרים מהמסד נתונים
            return View(books); // החזרת הנתונים ל-View
        }

        [HttpPost]
        public JsonResult RentBook(float book_id)
        {
            try
            {
                var book = db.books.Find(book_id);
                if (book == null)
                {
                    return Json(new { success = false, message = "Book not found" });
                }

                // בדיקה אם ניתן להשאיל את הספר (עד 3 פעמים)
                if (book.CurrentRentCount >= book.MaxRentCount)
                {
                    return Json(new { success = false, message = "Cannot rent this book. Maximum limit of 3 rentals reached." });
                }

                // עדכון מונה ההשאלות
                book.CurrentRentCount++;
                db.SaveChanges();

                return Json(new { success = true, message = $"You have rented the book: {book.book_name}. Current rentals: {book.CurrentRentCount}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult BuyBook(float book_id)
        {
            var book = db.books.Find(book_id);
            if (book == null)
            {
                return Json(new { success = false, message = "Book not found" });
            }

            // עדכון הספר כ"נמכר"
            book.IsSold = true; // ודא ששדה זה קיים במודל שלך
            db.SaveChanges();

            return Json(new { success = true, message = $"You have purchased the book: {book.book_name}" });
        }
    }
}