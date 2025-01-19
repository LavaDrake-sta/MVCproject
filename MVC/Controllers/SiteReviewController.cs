using System;
using System.Linq;
using System.Web.Mvc;
using MVC.Models;
using MyMvcProject.Data;

namespace MyMvcProject.Controllers
{
    public class SiteReviewController : Controller
    {
        private readonly MvcProjectContext db;

        public SiteReviewController()
        {
            db = new MvcProjectContext();
        }

        // הצגת ביקורות על האתר
        public ActionResult SiteReview()
        {
            return View();
        }

        // הוספת ביקורת על האתר
        [HttpPost]
        public ActionResult AddReview(string content)
        {
            // שליפת המייל מה-Session
            var userName = Session["UserName"].ToString();
            var user = db.users.FirstOrDefault(u => u.name == userName);

            // בדיקה אם המייל קיים
            if (string.IsNullOrEmpty(user.email))
            {
                TempData["ErrorMessage"] = "עליך להיות מחובר כדי להוסיף ביקורת.";
                return RedirectToAction("SiteReview");
            }

            // בדיקה אם התוכן ריק
            if (string.IsNullOrEmpty(content))
            {
                TempData["ErrorMessage"] = "לא ניתן להוסיף ביקורת ריקה.";
                return RedirectToAction("SiteReview");
            }

            // הוספת הביקורת למסד הנתונים
            db.reviews.Add(new review
            {
                email = user.email,
                Content = content,
                type = "Site",
                book_ID = null,
                created_at = DateTime.Now
            });

            db.SaveChanges();

            TempData["SuccessMessage"] = "הביקורת נוספה בהצלחה!";
            return RedirectToAction("PersonalArea", "PersonalArea");
        }
    }
}