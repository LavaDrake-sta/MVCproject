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
            var reviews = db.reviews
                .Where(r => r.type == "Site")
                .OrderByDescending(r => r.ID_review)
                .ToList();

            return View(reviews);
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

            db.reviews.Add(new review
            {
                email = User.Identity.Name, // שם משתמש מחובר
                Content = content,
                type = "Site",
                book_ID = null // לא רלוונטי לביקורות על האתר
            });

            db.SaveChanges();
            TempData["SuccessMessage"] = "הביקורת נוספה בהצלחה!";
            return RedirectToAction("Index");
        }
    }
}
