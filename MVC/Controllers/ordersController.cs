using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MVC.Models;
using MyMvcProject.Data;

namespace MyMvcProject.Controllers
{
    public class OrderController : Controller
    {
        private readonly MvcProjectContext db;

        public OrderController()
        {
            db = new MvcProjectContext();
        }

        public ActionResult Checkout()
        {
            if (Session["UserName"] == null)
            {
                TempData["ErrorMessage"] = "עליך להתחבר כדי לבצע הזמנה.";
                return RedirectToAction("Login", "Users");
            }

            var cartItems = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            ViewBag.UserName = Session["UserName"];
            ViewBag.CartItems = cartItems;

            return View(cartItems);
        }

        [HttpPost]
        public ActionResult SubmitOrder(string cardOwner, string cardNumber, string expiryDate, string cvc, int numberOfPayments)
        {
            if (Session["UserName"] == null)
            {
                TempData["ErrorMessage"] = "עליך להתחבר כדי לבצע הזמנה.";
                return RedirectToAction("Login", "Users");
            }

            // בדיקת תקינות פרטי האשראי
            if (string.IsNullOrEmpty(cardOwner) || cardNumber.Length != 16 || !expiryDate.Contains("/") || cvc.Length != 3)
            {
                TempData["ErrorMessage"] = "פרטי האשראי שהוזנו אינם תקינים.";
                return RedirectToAction("Checkout");
            }

            // שליפת עגלה מה-Session
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            if (!cart.Any())
            {
                TempData["ErrorMessage"] = "העגלה שלך ריקה.";
                return RedirectToAction("Cart", "Cart");
            }

            // שמירת ההזמנה ב-DB
            foreach (var item in cart)
            {
                var order = new orders
                {
                    email = db.users
                              .Where(u => u.name == Session["UserName"].ToString())
                              .Select(u => u.email)
                              .FirstOrDefault(),
                    first_name = Session["UserName"].ToString().Split(' ')[0],
                    last_name = Session["UserName"].ToString().Split(' ')[1],
                    card_owner_name = cardOwner,
                    card_number = double.Parse(cardNumber),
                    expiry_date = expiryDate,
                    CVC = double.Parse(cvc),
                    number_of_payments = numberOfPayments,
                    price = item.Price * item.Quantity,
                    product = item.BookName,
                    buy_borrow = item.Type, // "Buy" or "Rent"
                    date = DateTime.Now
                };

                db.orders.Add(order);
            }

            db.SaveChanges();

            // ניקוי העגלה
            Session["Cart"] = null;

            TempData["SuccessMessage"] = "ההזמנה בוצעה בהצלחה!";
            return RedirectToAction("PersonalArea", "User");
        }
    }
}
