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

        public ActionResult Index()
        {
            // הפעולה לדף האזור האישי
            return View();
        }
    }
}
