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
        public JsonResult RentBook(int book_id)
        {
            try
            {
                // שליפת הספר מהמסד
                var book = db.books.Find(book_id);
                if (book == null)
                {
                    return Json(new { success = false, message = "Book not found" });
                }

                // בדיקה אם הספר ניתן להשכרה
                if (book.IsRent == false || book.IsRent == null)
                {
                    return Json(new { success = false, message = "This book cannot be rented." });
                }

                // בדיקה אם אפשר להשכיר מבחינת מגבלת השכרות
                if (book.CurrentRentCount >= book.MaxRentCount)
                {
                    return Json(new { success = false, message = "Cannot rent this book. Maximum limit reached." });
                }

                // עדכון מספר ההשכרות
                book.CurrentRentCount++;

                // הוספת הספר לעגלה
                var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
                var existingItem = cart.FirstOrDefault(c => c.BookId == book.book_id && c.Type == "Rent");

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
                        Price = book.price / 4, // מחיר מושכר
                        Type = "Rent",
                        Quantity = 1
                    });
                }

                Session["Cart"] = cart;
                db.SaveChanges();

                return Json(new { success = true, message = "Book successfully rented and added to your cart!" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in RentBook: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while processing your request." });
            }
        }

        [HttpPost]
        public JsonResult BuyBook(int bookId)
        {
            return AddToCart(bookId, "Buy");
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
    }
}