using MVC.Models;
using MyMvcProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class booksController : Controller
    {
        private MvcProjectContext db = new MvcProjectContext();

        // פעולה להצגת רשימת ספרים עם אפשרויות קנייה והשאלה
        [HttpGet]
        public ActionResult BuyBorrowBook()
        {
            var books = db.books.ToList(); // שליפת כל הספרים מהמסד נתונים
            return View(books); // החזרת הנתונים ל-View
        }

        // פונקציה להחזרת כל הספרים
        [HttpGet]
        public JsonResult GetAllBooks()
        {
            try
            {
                var books = db.books.Select(b => new
                {
                    b.book_id,
                    b.book_name,
                    b.category,
                    b.language,
                    b.price,
                    b.ImageUrl,
                    b.CurrentRentCount,
                    b.MaxRentCount,
                    b.IsRent,
                    b.author // הוספת השדה author
                }).ToList();

                return Json(books, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetAllBooks: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while retrieving the books." }, JsonRequestBehavior.AllowGet);
            }
        }

        // פונקציית חיפוש ספרים
        [HttpGet]
        public JsonResult SearchBooks(string query)
        {
            try
            {
                var books = db.books
                              .Where(b => b.book_name.Contains(query))
                              .Select(b => new
                              {
                                  b.book_id,
                                  b.book_name,
                                  b.category,
                                  b.language,
                                  b.price,
                                  b.ImageUrl,
                                  b.CurrentRentCount,
                                  b.MaxRentCount,
                                  b.IsRent,
                                  b.author // הוספת השדה author
                              })
                              .ToList();

                if (books.Count == 0)
                {
                    return Json(new { success = false, message = "Book not found. Please make sure the book exists in the system." }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = true, books }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SearchBooks: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while searching for books." }, JsonRequestBehavior.AllowGet);
            }
        }

        // פונקציות השכרה, קנייה והחזרה
        [HttpPost]
        public ActionResult RentBook(int bookId)
        {
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            var book = db.books.FirstOrDefault(b => b.book_id == bookId);

            if (book == null)
            {
                TempData["ErrorMessage"] = "הספר לא נמצא.";
                return RedirectToAction("BuyBorrowBook", "books");
            }

            if (book.IsRent == true && book.CurrentRentCount >= book.MaxRentCount)
            {
                TempData["OfferWaitingList"] = bookId;
                TempData["ErrorMessage"] = $"אין עותקים זמינים להשכרה עבור הספר \"{book.book_name}\". האם תרצה להצטרף לרשימת המתנה?";
                return RedirectToAction("BuyBorrowBook", "books");
            }

            var existingItem = cart.FirstOrDefault(c => c.BookId == bookId && c.Type == "Rent");
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    BookId = book.book_id,
                    BookName = book.book_name,
                    Price = book.price / 4,
                    Type = "Rent",
                    Quantity = 1
                });
            }

            Session["Cart"] = cart;

            TempData["SuccessMessage"] = "הספר נוסף לעגלה.";
            return RedirectToAction("BuyBorrowBook", "books");
        }

        [HttpPost]
        public JsonResult BuyBook(int bookId)
        {
            return AddToCart(bookId, "Buy");
        }


        [HttpPost]
        public JsonResult AddToCart(int bookId, string type)
        {
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            var book = db.books.FirstOrDefault(b => b.book_id == bookId);
            if (book == null)
            {
                return Json(new { success = false, message = "Book not found" });
            }

            // חישוב מחיר לפי סוג הפעולה
            var price = type == "Buy" ? book.price : book.price / 4;

            // בדיקה אם הפריט כבר בעגלה
            var existingItem = cart.FirstOrDefault(c => c.BookId == bookId && c.Type == type);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    BookId = book.book_id,
                    BookName = book.book_name,
                    Price = price,
                    Type = type,
                    Quantity = 1
                });
            }

            Session["Cart"] = cart;

            return Json(new { success = true, message = "The item was added to your cart!" });
        }

        [HttpGet]
        public JsonResult GetBookReviews(int bookId)
        {
            try
            {
                // שליפת הביקורות הקשורות לספר
                var review = db.review
                                .Where(r => r.book_ID == bookId) // סינון לפי מזהה הספר
                                .Select(r => new
                                {
                                    r.ID_review, // מזהה הביקורת
                                    r.email, // אימייל של הכותב
                                    r.Content, // תוכן הביקורת
                                    r.type, // סוג הביקורת
                                    created_at = r.created_at.HasValue ? r.created_at.Value.ToString("yyyy-MM-dd HH:mm:ss") : "N/A" // תאריך יצירה בפורמט
                                })
                                .OrderByDescending(r => r.created_at) // סידור לפי תאריך יצירה
                                .ToList();

                // בדיקה אם נמצאו ביקורות
                if (!review.Any())
                {
                    return Json(new { success = false, message = "No reviews found for this book." }, JsonRequestBehavior.AllowGet);
                }

                // החזרת הביקורות בפורמט JSON
                return Json(new { success = true, review }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // כתיבה לדיבאג במקרה של שגיאה
                System.Diagnostics.Debug.WriteLine($"Error in GetBookReviews: {ex.Message}");
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}