using System;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using MVC.Models;
using MyMvcProject.Data;

namespace MyMvcProject.Controllers
{
    public class EmailController : Controller
    {
        private MvcProjectContext db = new MvcProjectContext();

        // פונקציה לשליחת מייל תודה לאחר רכישה/השכרה
        public void SendThankYouEmail(string email)
        {
            try
            {
                // שליפה של המשתמש
                var user = db.users.Include(u => u.orders)
                                   .FirstOrDefault(u => u.email == email);

                if (user == null)
                {
                    Console.WriteLine($"User with email {email} not found.");
                    return;
                }

                // שליפה של הספרים שנרכשו או נשכרו (למשל מתוך טבלת orders)
                var books = db.orders
                              .Where(o => o.email == email)
                              .Select(o => o.product)
                              .ToList();

                if (!books.Any())
                {
                    Console.WriteLine($"No books found for user {email}.");
                    return;
                }

                // בניית גוף המייל
                string bookList = string.Join("\n", books);
                string emailBody = $"Hello {user.name},\n\nThank you for purchasing or renting books from us! The following books have been added to your account:\n\n{bookList}\n\nYou can view your books in your personal area.\n\nBest regards,\nThe Bookstore Team";

                // שליחת המייל
                bool emailSent = SendEmail(user.email, user.name, "Thank You for Your Purchase!", emailBody);

                if (emailSent)
                {
                    Console.WriteLine($"Thank you email sent to {user.name} ({user.email}).");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while sending thank you email: {ex.Message}");
            }
        }

        // פונקציה לשליחת מייל תזכורות
        public void SendReminderEmails()
        {
            try
            {
                var today = DateTime.Now;

                // מציאת ספרים שצריך להחזיר תוך 5 ימים והמייל קיים ואינו ריק
                var dueBooks = db.borrowing_Books
                    .Include(b => b.users)
                    .Where(b => DbFunctions.DiffDays(today, b.return_date) == 5 && !string.IsNullOrEmpty(b.email))
                    .ToList();

                foreach (var book in dueBooks)
                {
                    var user = book.users;
                    if (user != null && !string.IsNullOrEmpty(user.email)) // בדיקת קיום מייל של המשתמש
                    {
                        // שליחת המייל עם המרת תאריך למחרוזת
                        bool emailSent = SendEmail(user.email, user.name, book.book_name, book.return_date.ToString("dd/MM/yyyy"));

                        if (emailSent)
                        {
                            // עדכון שדה EmailSent ושמירת השינויים
                            book.EmailSent = true;
                            db.SaveChanges();
                            Console.WriteLine($"Reminder sent to {user.name} ({user.email}) for book: {book.book_name}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while sending reminder emails: {ex.Message}");
            }
        }





        // פונקציה לשליחת מייל בודד
        private bool SendEmail(string userEmail, string userName, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("your_email@example.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };

                mail.To.Add(userEmail);

                SmtpClient client = new SmtpClient("smtp.example.com")
                {
                    Credentials = new System.Net.NetworkCredential("your_email@example.com", "your_password"),
                    Port = 587,
                    EnableSsl = true
                };

                client.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email to {userEmail}: {ex.Message}");
                return false;
            }
        }
    }
}