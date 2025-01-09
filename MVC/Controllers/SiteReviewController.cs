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
        public ActionResult Index()
        {
            // שליפת ביקורות מהמסד נתונים
            var reviews = db.reviews
                .Where(r => r.type == "Site")
                .Select(r => new
                {
                    r.email,
                    r.Content,
                    r.created_at
                })
                .ToList();

            ViewBag.Reviews = reviews;

            // הצגת הודעות שגיאה/הצלחה
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            ViewBag.SuccessMessage = TempData["SuccessMessage"];

            return View();
        }

        // הוספת ביקורת על האתר
        [HttpPost]
        public ActionResult AddReview(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                TempData["ErrorMessage"] = "לא ניתן להוסיף ביקורת ריקה.";
                return RedirectToAction("Index");
            }

            try
            {
                // קבלת שם המשתמש מתוך ה-Session אם אינו זמין ב-User.Identity
                string userEmail = Session["UserEmail"] != null ? Session["UserEmail"].ToString() : "Unknown User";

                // הוספת הביקורת למסד הנתונים
                db.reviews.Add(new review
                {
                    email = userEmail,
                    Content = content,
                    type = "Site",
                    book_ID = null, // לא רלוונטי לביקורות על האתר
                    created_at = DateTime.Now
                });

                db.SaveChanges();
                TempData["SuccessMessage"] = "הביקורת נוספה בהצלחה!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "אירעה שגיאה בעת הוספת הביקורת: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}