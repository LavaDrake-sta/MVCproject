using System;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using MVC.Models; // עדכן בהתאם למודל שלך
using MyMvcProject.Data; // עדכן בהתאם לשם של ה-namespace שלך

namespace MyMvcProject.Controllers
{
    public class EmailController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // פונקציה לשליחת תזכורות במייל
        public void SendReminderEmails()
        {
            try
            {
                var today = DateTime.Now;

                // מציאת ספרים שצריך להחזיר תוך 5 ימים
                var dueBooks = db.Borrowing_books
                    .Where(b => DbFunctions.DiffDays(today, b.return_date) == 5)
                    .ToList();

                foreach (var book in dueBooks)
                {
                    // חיפוש המשתמש המקושר לספר
                    var user = db.users.FirstOrDefault(u => u.Id == book.user_id);
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