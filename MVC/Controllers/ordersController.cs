using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MVC.Models;
using MyMvcProject.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

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
        // בדיקת תקינות מספר כרטיס אשראי
        private bool IsValidCardNumber(string cardNumber)
        {
            return !string.IsNullOrEmpty(cardNumber) && cardNumber.All(char.IsDigit) && cardNumber.Length == 16;
        }

        // בדיקת תקינות תוקף כרטיס אשראי בפורמט MM/YY
        private bool IsValidExpiryDate(string expiryDate)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(expiryDate, @"^(0[1-9]|1[0-2])\/\d{2}$");
        }

        // בדיקת תקינות קוד CVC
        private bool IsValidCVC(string cvc)
        {
            return !string.IsNullOrEmpty(cvc) && cvc.All(char.IsDigit) && cvc.Length == 3;
        }

        [HttpPost]
        public ActionResult SubmitOrder(string cardOwner, string cardNumber, string expiryDate, string cvc, int numberOfPayments)
        {
            if (Session["UserName"] == null)
            {
                TempData["ErrorMessage"] = "עליך להתחבר כדי לבצע הזמנה.";
                return RedirectToAction("Login", "Users");
            }

            if (string.IsNullOrEmpty(cardOwner) ||
                !IsValidCardNumber(cardNumber) ||
                !IsValidExpiryDate(expiryDate) ||
                !IsValidCVC(cvc))
            {
                TempData["ErrorMessage"] = "פרטי האשראי שהוזנו אינם תקינים.";
                return RedirectToAction("Checkout");
            }

            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            if (!cart.Any())
            {
                TempData["ErrorMessage"] = "העגלה שלך ריקה.";
                return RedirectToAction("Cart", "Cart");
            }

            try
            {
                // 🔢 שליפת מספר ההזמנה האחרון
                int lastOrderNumber = db.orders.Any() ? db.orders.Max(o => o.order_number) : 0;
                int newOrderNumber = lastOrderNumber + 1;

                foreach (var item in cart)
                {
                    var userName = Session["UserName"].ToString();
                    var user = db.users.FirstOrDefault(u => u.name == userName);
                    var book = db.books.FirstOrDefault(b => b.book_id == item.BookId);

                    if (user == null || book == null)
                    {
                        TempData["ErrorMessage"] = "אירעה שגיאה במציאת משתמש או ספר.";
                        return RedirectToAction("Checkout");
                    }

                    // 📦 עדכון מלאי לאחר תשלום
                    if (item.Type == "Rent" && book.IsRent == true)
                    {
                        book.CurrentRentCount += item.Quantity;
                    }

                    // 📝 יצירת ההזמנה
                    var order = new orders
                    {
                        order_number = newOrderNumber,
                        email = user.email,
                        first_name = userName.Split(' ')[0],
                        last_name = userName.Split(' ').Length > 1 ? userName.Split(' ')[1] : "",
                        card_owner_name = cardOwner,
                        card_number = cardNumber,
                        expiry_date = expiryDate,
                        CVC = cvc,
                        number_of_payments = numberOfPayments,
                        price = item.Price * item.Quantity,
                        product = item.BookName,
                        buy_borrow = item.Type,
                        date = DateTime.Now
                    };

                    db.orders.Add(order);
                }

                db.SaveChanges();

                // 🛒 ניקוי העגלה לאחר ההזמנה
                Session["Cart"] = null;

                // ✅ הודעת הצלחה עם הפניה לעמוד הבית
                TempData["SuccessMessage"] = $"ההזמנה בוצעה בהצלחה! מספר הזמנה: {newOrderNumber}. תוכל לראות את הספר באזור האישי שלך.";
                return RedirectToAction("Index", "Home");  // הפניה לעמוד הבית
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "אירעה שגיאה בעת ביצוע ההזמנה. אנא נסה שוב.";
                return RedirectToAction("Checkout");
            }
        }

    }
}
