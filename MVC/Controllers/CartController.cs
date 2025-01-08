using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class CartController : Controller
    {
        // הצגת עמוד העגלה
        public ActionResult Index()
        {
            var cart = Session["Cart"] as List<Dictionary<string, object>> ?? new List<Dictionary<string, object>>();
            return View(cart);
        }

        // הוספת פריט לעגלה
        [HttpPost]
        public JsonResult AddToCart(int bookId, string bookName, decimal price, string type)
        {
            var cart = Session["Cart"] as List<Dictionary<string, object>> ?? new List<Dictionary<string, object>>();

            var existingItem = cart.FirstOrDefault(item => (int)item["BookId"] == bookId && (string)item["Type"] == type);
            if (existingItem != null)
            {
                existingItem["Quantity"] = (int)existingItem["Quantity"] + 1;
            }
            else
            {
                cart.Add(new Dictionary<string, object>
                {
                    { "BookId", bookId },
                    { "BookName", bookName },
                    { "Price", price },
                    { "Quantity", 1 },
                    { "Type", type }
                });
            }

            Session["Cart"] = cart;

            return Json(new { success = true, cartCount = cart.Count });
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