using System;
using System.Collections.Generic;
using System.Data.Entity; // EF6 Namespace
using System.Linq;
using MVC.Models;

namespace MyMvcProject.Data
{
    public class MvcProjectContext : DbContext
    {
        // הגדרת הקונסטרוקטור עם שרשרת החיבור
        public MvcProjectContext() : base("name=MvcProjectContext") { }

        // טבלת המשתמשים
        public DbSet<users> users { get; set; }
        public DbSet<books> books { get; set; }
        public DbSet<Borrowed_books_list> borrowed_Books_Lists { get; set; }
        public DbSet<Borrowing_books> borrowing_Books { get; set; }
        public DbSet<waiting_list> waiting_Lists { get; set; }
        public DbSet<orders> orders { get; set; }


        /// <summary>
        /// הוספת משתמש חדש לטבלה
        /// </summary>
        /// <param name="newUser">אובייקט המשתמש החדש</param>
        /// <returns>הצלחה או כשלון</returns>
        public bool AddUser(users newUser)
        {
            try
            {
                // בדוק אם המשתמש כבר קיים
                if (users.Any(u => u.email == newUser.email))
                {
                    return false; // משתמש כבר קיים
                }

                // הוסף את המשתמש לטבלה
                users.Add(newUser);
                SaveChanges(); // שמור את השינויים
                return true; // המשתמש נוסף בהצלחה
            }
            catch (Exception ex)
            {
                // טיפול בשגיאות
                Console.WriteLine($"שגיאה בהוספת משתמש: {ex.Message}");
                return false;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // הגדרת מפתחות ראשיים
            modelBuilder.Entity<users>().HasKey(u => u.email);
            modelBuilder.Entity<books>().HasKey(b => b.book_id);
            modelBuilder.Entity<Borrowed_books_list>().HasKey(b => b.book_id);
            modelBuilder.Entity<Borrowing_books>().HasKey(b => b.book_id);
            modelBuilder.Entity<waiting_list>().HasKey(w => w.name); // שים לב למפתח זה
            modelBuilder.Entity<orders>().HasKey(o => o.order_number);

            base.OnModelCreating(modelBuilder);
        }

    }
}
