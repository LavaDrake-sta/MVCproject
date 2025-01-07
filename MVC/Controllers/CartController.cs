using MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class CartController : Controller
    {
        public ActionResult Index()
        {
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            return View(cart);
        }

        [HttpPost]
        public JsonResult RemoveFromCart(int bookId, string type)
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart != null)
            {
                var item = cart.FirstOrDefault(c => c.BookId == bookId && c.Type == type);
                if (item != null)
                {
                    cart.Remove(item);
                    Session["Cart"] = cart;
                }
            }
            return Json(new { success = true });
        }
    }

}
