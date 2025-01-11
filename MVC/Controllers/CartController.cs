using MVC.Models;
using MyMvcProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class CartController : Controller
    {
        private MvcProjectContext db = new MvcProjectContext();
        // הצגת עמוד העגלה
        public ActionResult Cart()
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || !cart.Any())
            {
                ViewBag.Message = "העגלה ריקה";
            }
            return View(cart ?? new List<CartItem>());
        }

        // הוספת פריט לעגלה
        [HttpPost]
        public ActionResult AddToCart(int bookId, string type)
        {
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            var book = db.books.FirstOrDefault(b => b.book_id == bookId);

            if (book == null)
            {
                TempData["ErrorMessage"] = "הספר לא נמצא.";
                return RedirectToAction("BuyBorrowBook", "books");
            }

            // 📦 בדיקה אם כל העותקים להשכרה כבר הושכרו
            if (type == "Rent" && book.IsRent == true && book.CurrentRentCount >= book.MaxRentCount)
            {
                // התראה עם הצעה להיכנס לרשימת המתנה
                TempData["OfferWaitingList"] = bookId;
                TempData["ErrorMessage"] = $"כל העותקים של הספר \"{book.book_name}\" מושכרים כרגע. האם תרצה להצטרף לרשימת ההמתנה?";
                return RedirectToAction("Cart", "Cart");
            }

            // חישוב מחיר
            var price = type == "Buy" ? book.price : book.price / 4;

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

            TempData["SuccessMessage"] = "הספר נוסף לעגלה.";
            return RedirectToAction("Cart", "Cart");
        }


        // הסרת פריט מהעגלה
        [HttpPost]
        public ActionResult RemoveFromCart(int bookId, string type)
        {
            try
            {
                // שליפת העגלה מה-Session
                var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

                // חיפוש הפריט בעגלה
                var itemToRemove = cart.FirstOrDefault(c => c.BookId == bookId && c.Type == type);
                if (itemToRemove != null)
                {
                    cart.Remove(itemToRemove); // הסרת הפריט מהעגלה
                }

                // עדכון העגלה ב-Session
                Session["Cart"] = cart;

                TempData["SuccessMessage"] = "The item was removed from your cart.";
                return RedirectToAction("Cart"); // הפניה חזרה לעמוד העגלה
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in RemoveFromCart: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while removing the item.";
                return RedirectToAction("Cart"); // הפניה חזרה לעמוד העגלה במקרה של שגיאה
            }
        }
    }
}