using System;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using MVC.Models; // ה-namespace שמכיל את המודלים ואת ה-DB Context
using MyMvcProject.Data;

namespace MyMvcProject.Controllers
{
    public class EmailController : Controller
    {
        // שימוש ב-DB Context
        private MvcProjectContext db = new MvcProjectContext();

        // פונקציה לשליחת תזכורות במייל
        public void SendReminderEmails()
        {
            try
            {
                var today = DateTime.Now;

                // מציאת ספרים שצריך להחזיר תוך 5 ימים
                var dueBooks = db.borrowing_Books
                    .Include(b => b.users) // טעינה מוקדמת של המשתמשים
                    .Where(b => DbFunctions.DiffDays(today, b.return_date) == 5)
                    .ToList();

                foreach (var book in dueBooks)
                {
                    // משתמש מקושר דרך הקשר הניווט
                    var user = book.users;
                    if (user != null)
                    {
                        bool emailSent = SendEmail(user.email, user.name, book.book_name, book.return_date);

                        if (emailSent)
                        {
                            Console.WriteLine($"Reminder sent to {user.name} ({user.email}) for book: {book.book_name}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // טיפול בשגיאה כללית
                Console.WriteLine($"Error occurred while sending reminder emails: {ex.Message}");
            }
        }

        // פונקציה לשליפת ספרים לפי מייל
        public ActionResult GetBooksByEmail(string email)
        {
            try
            {
                // שליפה של המשתמש לפי מייל
                var user = db.users.Include(u => u.Borrowing_books)
                                   .FirstOrDefault(u => u.email == email);

                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." }, JsonRequestBehavior.AllowGet);
                }

                // שליפה של הספרים שהמשתמש שואל
                var books = user.Borrowing_books.Select(b => new
                {
                    b.book_name,
                    b.return_date
                }).ToList();

                return Json(new { success = true, books }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // טיפול בשגיאה כללית
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // פונקציה פנימית לשליחת מייל בודד
        private bool SendEmail(string userEmail, string userName, string bookName, DateTime returnDate)
        {
            try
            {
                // הגדרות מייל
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("your_email@example.com"),
                    Subject = "Reminder to return your book",
                    Body = $"Hello {userName},\n\nThis is a reminder to return the book \"{bookName}\" by {returnDate:dd/MM/yyyy}.",
                    IsBodyHtml = false // אם אתה שולח טקסט פשוט ולא HTML
                };

                mail.To.Add(userEmail);

                SmtpClient client = new SmtpClient("smtp.example.com")
                {
                    Credentials = new System.Net.NetworkCredential("your_email@example.com", "your_password"),
                    Port = 587, // בדוק אם השרת שלך דורש פורט אחר
                    EnableSsl = true
                };

                // שליחת המייל
                client.Send(mail);

                return true;
            }
            catch (Exception ex)
            {
                // טיפול בשגיאת שליחת מייל
                Console.WriteLine($"Failed to send email to {userEmail}: {ex.Message}");
                return false;
            }
        }
    }
}