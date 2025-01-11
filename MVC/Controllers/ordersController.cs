using System.Linq;
using System.Web.Mvc;
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

            string userName = Session["UserName"].ToString();
            string userEmail = db.users
                .Where(u => u.name == userName)
                .Select(u => u.email)
                .FirstOrDefault();

            var cartItems = db.orders
                .Where(o => o.first_name + " " + o.last_name == userName || o.card_owner_name == userName)
                .Select(o => new
                {
                    o.product,
                    o.price,
                    o.buy_borrow // סוג המוצר: קנייה או השכרה
                })
                .ToList();

            ViewBag.UserName = userName;
            ViewBag.UserEmail = userEmail;
            ViewBag.CartItems = cartItems;

            return View();
        }

        [HttpPost]
        public ActionResult SubmitOrder(string cardOwner, string cardNumber, string expiryDate, string cvc)
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

            TempData["SuccessMessage"] = "ההזמנה בוצעה בהצלחה!";
            return RedirectToAction("Checkout");
        }
    }
}
