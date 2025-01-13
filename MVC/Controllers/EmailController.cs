using System;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using MyMvcProject.Data;

namespace MyMvcProject.Controllers
{
    public class EmailController : Controller
    {
        private MvcProjectContext db = new MvcProjectContext();

        // שליחת מייל תודה לאחר רכישה/השכרה
        public void SendThankYouEmail(string email)
        {
            try
            {
                // שליפת המשתמש כולל הזמנות
                var user = db.users.Include(u => u.orders)
                                   .FirstOrDefault(u => u.email == email);

                if (user == null)
                {
                    Console.WriteLine($"User with email {email} not found.");
                    return;
                }

                // שליפת הספרים שנרכשו או נשכרו
                var books = user.orders
                                .Select(o => o.product)
                                .ToList();

                if (!books.Any())
                {
                    Console.WriteLine($"No books found for user {email}.");
                    return;
                }

                // בניית גוף המייל
                string bookList = string.Join("\n", books);
                string emailBody = $"Hello {user.name},\n\nThank you for purchasing or renting books from us! The following books have been added to your account:\n\n{bookList}\n\nBest regards,\nThe Bookstore Team";

                // שליחת המייל
                if (SendEmail(user.email, user.name, "Thank You for Your Purchase!", emailBody))
                {
                    Console.WriteLine($"Thank you email sent to {user.name} ({user.email}).");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while sending thank you email: {ex.Message}");
            }
        }

        // שליחת מייל תזכורות
        public void SendReminderEmails()
        {
            try
            {
                var today = DateTime.Now;

                // שליפת ספרים שצריכים לחזור תוך 5 ימים
                var dueBooks = db.borrowing_Books
                    .Include(b => b.users)
                    .Where(b => DbFunctions.DiffDays(today, b.return_date) == 5 && !string.IsNullOrEmpty(b.email))
                    .ToList();

                foreach (var book in dueBooks)
                {
                    var user = book.users;
                    if (user != null && !string.IsNullOrEmpty(user.email))
                    {
                        // בניית גוף המייל
                        string emailBody = $"Dear {user.name},\n\nThis is a friendly reminder that the book '{book.book_name}' is due for return on {book.return_date:dd/MM/yyyy}.\n\nPlease make sure to return it on time.\n\nBest regards,\nThe Library Team";

                        // שליחת המייל ועדכון סטטוס
                        if (SendEmail(user.email, user.name, "Reminder: Book Return Due", emailBody))
                        {
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

        // שליחת מייל בודד
        private bool SendEmail(string userEmail, string userName, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("adistamker88@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };

                mail.To.Add(userEmail);

                SmtpClient client = new SmtpClient("smtp.gmail.com")
                {
                    Credentials = new System.Net.NetworkCredential("adistamker88@gmail.com", "159753"),
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
