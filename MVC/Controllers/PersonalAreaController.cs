using System.Linq;
using System.Web.Mvc;
using MyMvcProject.Data;

namespace MyMvcProject.Controllers
{
    public class PersonalAreaController : Controller
    {
        private readonly MvcProjectContext db;

        public PersonalAreaController()
        {
            db = new MvcProjectContext();
        }

        private bool IsUserLoggedIn()
        {
            return Session["UserName"] != null;
        }

        public ActionResult Index()
        {
            if (!IsUserLoggedIn())
            {
                TempData["ErrorMessage"] = "עליך להתחבר כדי לצפות באזור האישי.";
                return RedirectToAction("Login", "Users");
            }

            string userName = Session["UserName"].ToString();
            string userEmail = db.users
                .Where(u => u.name == userName)
                .Select(u => u.email)
                .FirstOrDefault();

            if (userEmail == null)
            {
                TempData["ErrorMessage"] = "משתמש לא נמצא במסד הנתונים.";
                return RedirectToAction("Login", "Users");
            }

            // שליפת הזמנות הקשורות למשתמש
            var orders = db.orders
                .Where(o => o.first_name + " " + o.last_name == userName || o.card_owner_name == userName)
                .ToList();

            // שליפת ספרים מושכרים
            var borrowedBooks = db.borrowing_Books
                .Where(b => b.category == userEmail)
                .ToList();

            // שליפת ספרים ברשימת המתנה
            var waitingBooks = db.waiting_Lists
                .Where(w => w.email == userEmail)
                .ToList();

            ViewBag.UserName = userName;
            ViewBag.UserEmail = userEmail;
            ViewBag.Orders = orders;
            ViewBag.BorrowedBooks = borrowedBooks;
            ViewBag.WaitingBooks = waitingBooks;

            return View();
        }

        [HttpPost]
        public ActionResult UpdateDetails(string name, string email, string password)
        {
            if (!IsUserLoggedIn())
            {
                TempData["ErrorMessage"] = "עליך להתחבר כדי לעדכן פרטים.";
                return RedirectToAction("Login", "Users");
            }

            string userName = Session["UserName"].ToString();
            var user = db.users.FirstOrDefault(u => u.name == userName);

            if (user == null)
            {
                TempData["ErrorMessage"] = "משתמש לא נמצא.";
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrEmpty(name))
                user.name = name;

            if (!string.IsNullOrEmpty(email))
                user.email = email;

            if (!string.IsNullOrEmpty(password))
                user.password = HashPassword(password);

            db.SaveChanges();
            Session["UserName"] = user.name;
            TempData["SuccessMessage"] = "הפרטים האישיים עודכנו בהצלחה.";
            return RedirectToAction("Index");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return System.Convert.ToBase64String(hash);
            }
        }
    }
}
