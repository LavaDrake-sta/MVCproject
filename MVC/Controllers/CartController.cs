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
        public JsonResult AddToCart(int bookId, string type, bool? addToWaitingList)
        {
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            var book = db.books.FirstOrDefault(b => b.book_id == bookId);

            if (book == null)
            {
                return Json(new { success = false, message = "הספר לא נמצא במערכת." });
            }

            // 📦 טיפול בהשכרה (Rent)
            if (type == "Rent")
            {
                if (book.IsRent == true && book.CurrentRentCount >= book.MaxRentCount)
                {
                    if (addToWaitingList == null)
                    {
                        return Json(new
                        {
                            success = false,
                            offerWaitingList = true,
                            message = $"אין מלאי להשכרה עבור הספר \"{book.book_name}\". האם תרצה להצטרף לרשימת המתנה?"
                        });
                    }

                    if (addToWaitingList == true)
                    {
                        var userName = Session["UserName"]?.ToString();
                        var user = db.users.FirstOrDefault(u => u.name == userName);

                        if (user != null)
                        {
                            var waitingEntry = new waiting_list
                            {
                                name = user.name,
                                book_name = book.book_name,
                                email = user.email,
                                date = DateTime.Now
                            };

                            db.waiting_Lists.Add(waitingEntry);
                            db.SaveChanges();

                            return Json(new { success = true, message = $"נוספת לרשימת ההמתנה עבור הספר \"{book.book_name}\"." });
                        }
                    }

                    return Json(new { success = false, message = "לא הצלחנו להוסיף אותך לרשימת ההמתנה." });
                }

                var existingRentItem = cart.FirstOrDefault(c => c.BookId == bookId && c.Type == "Rent");
                if (existingRentItem != null)
                {
                    existingRentItem.Quantity++;
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

                return Json(new { success = true, message = $"הספר \"{book.book_name}\" נוסף לעגלה בהצלחה." });
            }

            // 🛒 טיפול ברכישה רגילה (Buy)
            if (type == "Buy")
            {
                var existingBuyItem = cart.FirstOrDefault(c => c.BookId == bookId && c.Type == "Buy");
                if (existingBuyItem != null)
                {
                    existingBuyItem.Quantity++;
                }
                else
                {
                    cart.Add(new CartItem
                    {
                        BookId = book.book_id,
                        BookName = book.book_name,
                        Price = book.price,
                        Type = "Buy",
                        Quantity = 1
                    });
                }

                Session["Cart"] = cart;

                return Json(new { success = true, message = $"הספר \"{book.book_name}\" נוסף לעגלה בהצלחה." });
            }

            return Json(new { success = false, message = "סוג הפעולה לא תקין." });
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
        [HttpPost]
        public ActionResult AddToWaitingList(int bookId)
        {
            // 1️⃣ בדיקה אם המשתמש מחובר
            if (Session["UserName"] == null)
            {
                TempData["ErrorMessage"] = "עליך להתחבר כדי להצטרף לרשימת ההמתנה.";
                return RedirectToAction("Login", "Users");
            }

            // 2️⃣ שליפת פרטי המשתמש והספר
            var userName = Session["UserName"].ToString();
            var user = db.users.FirstOrDefault(u => u.name == userName);
            var book = db.books.FirstOrDefault(b => b.book_id == bookId);

            if (user == null || book == null)
            {
                TempData["ErrorMessage"] = "אירעה שגיאה במציאת המשתמש או הספר.";
                return RedirectToAction("BuyBorrowBook", "books");
            }

            // 3️⃣ בדיקה אם המשתמש כבר ברשימת ההמתנה
            var alreadyInList = db.waiting_Lists.Any(w => w.email == user.email && w.book_name == book.book_name);

            if (alreadyInList)
            {
                TempData["ErrorMessage"] = "אתה כבר ברשימת ההמתנה עבור הספר הזה.";
                return RedirectToAction("BuyBorrowBook", "books");
            }

            // 4️⃣ הוספה לרשימת ההמתנה
            var waitingEntry = new waiting_list
            {
                name = user.name,
                book_name = book.book_name,
                email = user.email,
                date = DateTime.Now
            };

            db.waiting_Lists.Add(waitingEntry);
            db.SaveChanges();

            // 5️⃣ הודעת הצלחה
            TempData["SuccessMessage"] = $"נוספת לרשימת ההמתנה עבור הספר \"{book.book_name}\".";
            return RedirectToAction("BuyBorrowBook", "books");
        }
    }
}