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

        [HttpGet]
        [ActionName("buy_borrow_book")]
        public ActionResult BuyBorrowBook()
        {
            var books = db.books.ToList(); // שליפת כל הספרים מהמסד נתונים
            return View(books); // החזרת הנתונים ל-View
        }

        [HttpPost]
        public JsonResult RentBook(int book_id)

        {
            try
            {
                var book = db.books.Find(book_id);
                if (book == null)
                {
                    return Json(new { success = false, message = "Book not found" });
                }


                if (book.CurrentRentCount >= book.MaxRentCount)
                {
                    return Json(new { success = false, message = "Cannot rent this book. Maximum limit reached." });
                }

                book.CurrentRentCount++;
                db.SaveChanges();
                return Json(new { success = true, message = $"You have rented the book: {book.book_name}. Current rentals: {book.CurrentRentCount}" });
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine($"Error in RentBook: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while renting the book." });

            }
        }

        [HttpPost]
        public JsonResult BuyBook(int book_id)
        {
            try
            {
                var book = db.books.Find(book_id);
                if (book == null)
                {
                    return Json(new { success = false, message = "Book not found" });
                }

                book.IsSold = true;
                db.SaveChanges();
                return Json(new { success = true, message = $"You have purchased the book: {book.book_name}" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BuyBook: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while purchasing the book." });
            }
        }

        [HttpPost]
        public JsonResult ReturnBook(int book_id)
        {
            try
            {
                var book = db.books.Find(book_id);
                if (book == null)
                {
                    return Json(new { success = false, message = "Book not found" });
                }

                if (book.CurrentRentCount > 0)
                {
                    book.CurrentRentCount--;
                    db.SaveChanges();
                    return Json(new { success = true, message = $"You have returned the book: {book.book_name}. Current rentals: {book.CurrentRentCount}" });
                }
                else
                {
                    return Json(new { success = false, message = "No active rentals to return for this book." });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ReturnBook: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while returning the book." });
            }
        }
    }
}