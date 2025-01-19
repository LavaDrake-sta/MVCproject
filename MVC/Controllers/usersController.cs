using System;
using System.Linq;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using MyMvcProject.Data; 
using MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace MyMvcProject.Controllers
{
    public class usersController : Controller
    {
        private readonly MvcProjectContext db;
        public ActionResult LoginRegister()
        {
            return View();
        }
        public usersController()
        {
            db = new MvcProjectContext();
        }
        public ActionResult Login(string mode = "login")
        {
            ViewBag.Mode = mode; // מצב login או register
            return View("LoginRegister");
        }

        [HttpPost]
        public ActionResult Register(string name, string email, string password)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // בדיקה אם המשתמש כבר קיים
                    var existingUser = db.users.FirstOrDefault(u => u.email == email);
                    if (existingUser != null)
                    {
                        ViewBag.ErrorMessage = "האימייל כבר רשום במערכת.";
                        ViewBag.Mode = "register"; // מוודא שמצב הטופס נשמר
                        return View("LoginRegister");
                    }

                    // הצפנת סיסמה
                    string hashedPassword = HashPassword(password);

                    // יצירת משתמש חדש
                    var user = new users
                    {
                        name = name,
                        email = email,
                        password = hashedPassword,
                        type = "רגיל"
                    };

                    // הוספת המשתמש ל-DB
                    db.users.Add(user);
                    db.SaveChanges();

                    // הודעת הצלחה
                    ViewBag.SuccessMessage = "הרשמה בוצעה בהצלחה.";
                    ViewBag.Mode = "login"; // העברת המצב לטופס התחברות
                    return View("LoginRegister");
                }

                // אם ה-ModelState אינו תקין
                ViewBag.ErrorMessage = "שגיאה באימות הנתונים. נסה שוב.";
                ViewBag.Mode = "register"; // שמירת מצב הרשמה
                return View("LoginRegister");
            }
            catch (Exception ex)
            {
                // טיפול בחריגות בלתי צפויות
                ViewBag.ErrorMessage = "אירעה שגיאה בלתי צפויה: " + ex.Message;
                ViewBag.Mode = "register"; // שמירת מצב הרשמה
                return View("LoginRegister");
            }
        }

        // פונקציה להצפנת סיסמאות
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string hashedPassword = HashPassword(password);
                    var user = db.users.FirstOrDefault(u => u.email == email && u.password == hashedPassword);

                    if (user != null)
                    {
                        Session["UserName"] = user.name;
                        Session["UserType"] = user.type; // שמירת סוג המשתמש

                        System.Diagnostics.Debug.WriteLine("UserName: " + Session["UserName"]);
                        System.Diagnostics.Debug.WriteLine("UserType: " + Session["UserType"]);

                        return RedirectToAction("HomePage", "Home");
                    }

                    ViewBag.ErrorMessage = "אימייל או סיסמה שגויים.";
                    return View("LoginRegister");
                }

                ViewBag.ErrorMessage = "שגיאה באימות הנתונים. נסה שוב.";
                return View("LoginRegister");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "אירעה שגיאה בלתי צפויה: " + ex.Message;
                return View("LoginRegister");
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            TempData["Message"] = "התנתקת בהצלחה.";
            return RedirectToAction("HomePage", "Home");
        }
     

    }
    
    
}