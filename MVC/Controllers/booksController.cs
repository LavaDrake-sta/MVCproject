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
<<<<<<< HEAD
        [HttpGet]
        [ActionName("buy_borrow_book")]
=======
>>>>>>> d36e936cd72618132e26bdf07ef8b74ac5ef0ff4
        public ActionResult BuyBorrowBook()
        {
            var books = db.books.ToList(); // שליפת כל הספרים מהמסד נתונים
            return View(books); // החזרת הנתונים ל-View
        }

        [HttpPost]
<<<<<<< HEAD
        public JsonResult RentBook(int book_id)
=======
        public JsonResult RentBook(float book_id)
>>>>>>> d36e936cd72618132e26bdf07ef8b74ac5ef0ff4
        {
            try
            {
                var book = db.books.Find(book_id);
                if (book == null)
                {
                    return Json(new { success = false, message = "Book not found" });
                }

<<<<<<< HEAD
                if (book.CurrentRentCount >= book.MaxRentCount)
                {
                    return Json(new { success = false, message = "Cannot rent this book. Maximum limit reached." });
                }

                book.CurrentRentCount++;
                db.SaveChanges();
=======
                // בדיקה אם ניתן להשאיל את הספר (עד 3 פעמים)
                if (book.CurrentRentCount >= book.MaxRentCount)
                {
                    return Json(new { success = false, message = "Cannot rent this book. Maximum limit of 3 rentals reached." });
                }

                // עדכון מונה ההשאלות
                book.CurrentRentCount++;
                db.SaveChanges();

>>>>>>> d36e936cd72618132e26bdf07ef8b74ac5ef0ff4
                return Json(new { success = true, message = $"You have rented the book: {book.book_name}. Current rentals: {book.CurrentRentCount}" });
            }
            catch (Exception ex)
            {
<<<<<<< HEAD
                System.Diagnostics.Debug.WriteLine($"Error in RentBook: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while renting the book." });
=======
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
>>>>>>> d36e936cd72618132e26bdf07ef8b74ac5ef0ff4
            }
        }

        [HttpPost]
<<<<<<< HEAD
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
=======
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
>>>>>>> d36e936cd72618132e26bdf07ef8b74ac5ef0ff4
        }
    }
}