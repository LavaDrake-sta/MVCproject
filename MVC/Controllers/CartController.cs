using MVC.Models;
using MyMvcProject.Data;
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
        public JsonResult AddToCart(int bookId, string type)
        {
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            var book = db.books.FirstOrDefault(b => b.book_id == bookId);
            if (book == null)
            {
                return Json(new { success = false, message = "הספר לא נמצא במערכת" });
            }

            var price = type == "Buy" ? book.price : book.price / 4; // חישוב מחיר השכרה

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

            return Json(new { success = true, message = "הספר נוסף לעגלה" });
        }

        // הסרת פריט מהעגלה
        [HttpPost]
        public JsonResult RemoveFromCart(int bookId, string type)
        {
            var cart = Session["Cart"] as List<Dictionary<string, object>> ?? new List<Dictionary<string, object>>();

            var itemToRemove = cart.FirstOrDefault(item => (int)item["BookId"] == bookId && (string)item["Type"] == type);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
            }

            Session["Cart"] = cart;

            return Json(new { success = true });
        }
    }
}