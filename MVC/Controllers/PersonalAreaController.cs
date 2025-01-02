using System;
using System.Linq;
using System.Web.Mvc;
using MyMvcProject.Data;
using MVC.Models;

namespace MyMvcProject.Controllers
{
    public class PersonalAreaController : Controller
    {
        private readonly MvcProjectContext db;

        public PersonalAreaController()
        {
            db = new MvcProjectContext();
        }

        //public ActionResult Index()
        //{
        //    if (Session["UserName"] == null)
        //    {
        //        TempData["ErrorMessage"] = "עליך להתחבר כדי לצפות באזור האישי.";
        //        return RedirectToAction("Login", "users");
        //    }

        //    string userName = Session["UserName"].ToString();

        //    var user = db.users.FirstOrDefault(u => u.name == userName);
        //    if (user == null)
        //    {
        //        TempData["ErrorMessage"] = "משתמש לא נמצא.";
        //        return RedirectToAction("Login", "users");
        //    }

        //    var orders = db.orders
        //        .Where(o => o.first_name + " " + o.last_name == user.name)
        //        .Select(o => new
        //        {
        //            o.product,
        //            o.date,
        //            o.price
        //        }).ToList();

        //    //var borrowedBooks = db.borrowing_Books
        //    //    .Where(b => b.user_name == user.name)
        //    //    .Select(b => new
        //    //    {
        //    //        b.book_name,
        //    //        b.borrow_date,
        //    //        b.return_date,
        //    //        Highlight = b.return_date.HasValue && b.return_date.Value <= DateTime.Now.AddDays(2)
        //    //    }).ToList();

        //    //var model = new PersonalAreaViewModel
        //    //{
        //    //    UserDetails = user,
        //    //    Orders = orders,
        //    //    BorrowedBooks = borrowedBooks
        //    //};

        //    return View(model);
        //}

        [HttpPost]
        public ActionResult UpdateDetails(string name, string newPassword)
        {
            if (Session["UserName"] == null)
            {
                TempData["ErrorMessage"] = "עליך להתחבר כדי לעדכן פרטים.";
                return RedirectToAction("Login", "users");
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

            if (!string.IsNullOrEmpty(newPassword))
                user.password = HashPassword(newPassword);

            db.SaveChanges();
            TempData["SuccessMessage"] = "הפרטים האישיים עודכנו בהצלחה.";
            return RedirectToAction("Index");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
