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
            if (string.IsNullOrEmpty(content))
            {
                TempData["ErrorMessage"] = "לא ניתן להוסיף ביקורת ריקה.";
                return RedirectToAction("SiteReview");
            }

            db.reviews.Add(new review
            {
                email = User.Identity.Name,
                Content = content,
                type = "Site",
                book_ID = null
            });

            db.SaveChanges();
            TempData["SuccessMessage"] = "הביקורת נוספה בהצלחה!";
            return RedirectToAction("PersonalArea");
        }
    }
}
