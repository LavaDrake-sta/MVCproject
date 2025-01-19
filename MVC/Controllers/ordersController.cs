using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MVC;
using MVC.Models;
using MyMvcProject.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace MyMvcProject.Controllers
{
    public class OrderController : Controller
    {
        private readonly MvcProjectContext db;

        public OrderController()
        {
            db = new MvcProjectContext();
        }

        public ActionResult Checkout()
        {
            if (Session["UserName"] == null)
            {
                TempData["ErrorMessage"] = "עליך להתחבר כדי לבצע הזמנה.";
                return RedirectToAction("Login", "Users");
            }

            var cartItems = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            ViewBag.UserName = Session["UserName"];
            ViewBag.CartItems = cartItems;

            return View(cartItems);
        }
        // בדיקת תקינות מספר כרטיס אשראי
        private bool IsValidCardNumber(string cardNumber)
        {
            return !string.IsNullOrEmpty(cardNumber) && cardNumber.All(char.IsDigit) && cardNumber.Length == 16;
        }

        // בדיקת תקינות תוקף כרטיס אשראי בפורמט MM/YY
        private bool IsValidExpiryDate(string expiryDate)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(expiryDate, @"^(0[1-9]|1[0-2])\/\d{2}$");
        }

        // פירוק תאריך תוקף
        private DateTime? ParseExpiryDate(string expiryDate)
        {
            if (IsValidExpiryDate(expiryDate))
            {
                var parts = expiryDate.Split('/');
                int month = int.Parse(parts[0]);
                int year = int.Parse("20" + parts[1]);  // הוספת "20" לשנה

                return new DateTime(year, month, 1);
            }
            return null;
        }

        // בדיקת תקינות קוד CVC
        private bool IsValidCVC(string cvc)
        {
            return !string.IsNullOrEmpty(cvc) && cvc.All(char.IsDigit) && cvc.Length == 3;
        }
        private string FormatExpiryDate(string expiryDate)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(expiryDate, @"^(0[1-9]|1[0-2])\/\d{2}$"))
            {
                var parts = expiryDate.Split('/');
                return $"{parts[0]}/{parts[1]}";  // MM/YY
            }
            return "Invalid";
        }


        [HttpPost]
        public ActionResult SubmitOrder(string cardOwner, string cardNumber, string expiryDate, string cvc, int numberOfPayments)
        {
            //  בדיקת התחברות
            if (Session["UserName"] == null)
            {
                TempData["ErrorMessage"] = "עליך להתחבר כדי לבצע הזמנה.";
                return RedirectToAction("Login", "Users");
            }

            //  בדיקת תקינות פרטי האשראי
            if (string.IsNullOrEmpty(cardOwner) ||
                !IsValidCardNumber(cardNumber) ||
                !IsValidExpiryDate(expiryDate) ||
                !IsValidCVC(cvc))
            {
                TempData["ErrorMessage"] = "פרטי האשראי שהוזנו אינם תקינים.";
                return RedirectToAction("Checkout");
            }

            //  שליפת העגלה מה-Session
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            if (!cart.Any())
            {
                TempData["ErrorMessage"] = "העגלה שלך ריקה.";
                return RedirectToAction("Cart", "Cart");
            }

            try
            {
                //  שליפת פרטי המשתמש
                var userName = Session["UserName"].ToString();
                var user = db.users.FirstOrDefault(u => u.name == userName);

                if (user == null)
                {
                    TempData["ErrorMessage"] = "אירעה שגיאה במציאת המשתמש.";
                    return RedirectToAction("Checkout");
                }

                //  ביצוע הזמנה לכל פריט בעגלה
                foreach (var item in cart)
                {
                    var book = db.books.FirstOrDefault(b => b.book_id == item.BookId);

                    if (book == null)
                    {
                        TempData["ErrorMessage"] = "אירעה שגיאה במציאת הספר.";
                        return RedirectToAction("Checkout");
                    }

                    //  עדכון מלאי והשכרה
                    if (item.Type == "Rent" && book.IsRent == true)
                    {
                        // בדיקה אם יש מספיק מלאי להשכרה
                        if (book.CurrentRentCount + item.Quantity > book.MaxRentCount)
                        {
                            TempData["ErrorMessage"] = $"לא ניתן להשכיר את הספר {book.book_name}, אין מלאי זמין.";
                            return RedirectToAction("Checkout");
                        }

                        book.CurrentRentCount += item.Quantity;

                        //  הוספה לרשימת ההשכרות האישית
                        var borrowingBook = new Borrowing_books
                        {
                            book_id = book.book_id,
                            book_name = book.book_name,
                            category = book.category,
                            date_taken = DateTime.Now,
                            return_date = DateTime.Now.AddDays(7),  // השכרה ל-7 ימים
                            email = user.email,
                            EmailSent = false
                        };
                        db.borrowing_Books.Add(borrowingBook);

                        //  הוספה לרשימת כל ההשכרות
                        var borrowedBook = new Borrowed_books_list
                        {
                            book_id = book.book_id,
                            book_name = book.book_name,
                            category = book.category,
                            Date_taken = DateTime.Now,
                            return_date = DateTime.Now.AddDays(7)
                        };
                        db.borrowed_Books_Lists.Add(borrowedBook);
                    }

                    //  יצירת ההזמנה
                    var order = new orders
                    {
                        email = user.email,
                        first_name = userName.Split(' ')[0],
                        last_name = userName.Split(' ').Length > 1 ? userName.Split(' ')[1] : "",
                        card_owner_name = cardOwner,
                        card_number = cardNumber,
                        expiry_date = FormatExpiryDate(expiryDate),  // עיבוד תוקף אשראי
                        CVC = cvc,
                        number_of_payments = numberOfPayments,
                        price = item.Price * item.Quantity,
                        product = item.BookName,
                        buy_borrow = item.Type,
                        date = DateTime.Now
                    };

                    db.orders.Add(order);
                }

                //  שמירת כל השינויים במסד הנתונים
                db.SaveChanges();

                //  ניקוי העגלה לאחר ההזמנה
                Session["Cart"] = null;

                try
                {
                    // יצירת אובייקט EmailService
                    EmailService emailService = new EmailService();

                    // שליחת מייל תודה
                    string emailBody = $"<h1>Thank you for your order, {userName}!</h1><p>Your order has been processed successfully.</p>" +
                        $"<p>You can find and download the book from your personal area.</p>" +
                        $"<p>We hope you enjoy your purchase!</p>\r\n " +
                        $"   <p>Best regards,</p>\r\n" +
                        $"    <p><strong>The Digital Library Team</strong></p>";
                    emailService.SendEmail(user.email, "Thank You for Your Order", emailBody);

                    TempData["SuccessMessage"] = "ההזמנה בוצעה בהצלחה! מייל תודה נשלח לכתובת שלך.";
                }
                catch (Exception emailEx)
                {
                    Console.WriteLine($"Error sending thank you email: {emailEx.Message}");
                    TempData["WarningMessage"] = "ההזמנה בוצעה בהצלחה, אך לא ניתן היה לשלוח מייל תודה.";
                }

                //  הצגת הודעת הצלחה
                TempData["SuccessMessage"] = $"ההזמנה בוצעה בהצלחה! תוכל לראות את הספרים באזור האישי שלך.";

                //  הפניה לעמוד הבית
                return RedirectToAction("HomePage", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"אירעה שגיאה בעת ביצוע ההזמנה: {ex.Message}";
                return RedirectToAction("Checkout");
            }
        }
    }
}