using System;
using System.Collections.Generic;
using System.Data.Entity; // EF6 Namespace
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using MVC.Models;

namespace MyMvcProject.Data
{
    public class MvcProjectContext : DbContext
    {
        // הגדרת הקונסטרוקטור עם שרשרת החיבור
        public MvcProjectContext() : base("name=MvcProjectContext") { }

        // טבלאות במסד הנתונים
        public DbSet<users> users { get; set; }
        public DbSet<books> books { get; set; }
        public DbSet<Borrowed_books_list> borrowed_Books_Lists { get; set; }
        public DbSet<Borrowing_books> borrowing_Books { get; set; }
        public DbSet<waiting_list> waiting_Lists { get; set; }
        public DbSet<orders> orders { get; set; }
        public DbSet<review> reviews { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // הגדרת מפתחות ראשיים
            modelBuilder.Entity<users>().HasKey(u => u.email);
            modelBuilder.Entity<books>().HasKey(b => b.book_id);
            modelBuilder.Entity<Borrowed_books_list>().HasKey(b => b.book_id);
            modelBuilder.Entity<Borrowing_books>().HasKey(b => b.book_id);
            modelBuilder.Entity<waiting_list>().HasKey(w => w.name);
            modelBuilder.Entity<orders>().HasKey(o => o.order_number);
            modelBuilder.Entity<review>().HasKey(o => o.ID_review);

            // הסרת ריבוי שמות בלשון רבים
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Borrowed_books_list משתמשת ב-book_id כמפתח ראשי וזר
            modelBuilder.Entity<Borrowed_books_list>()
                .HasRequired(b => b.books) // Borrowed_books_list תלוי ב-books
                .WithOptional(b => (Borrowed_books_list)b.Borrowed_books_list); // ספר אחד יכול להיות מקושר לרשומת Borrowed_books_list אחת

            // Borrowing_books משתמשת ב-book_id כמפתח ראשי וזר
            modelBuilder.Entity<Borrowing_books>()
                .HasRequired(b => b.books) // Borrowing_books תלוי ב-books
                .WithOptional(b => (Borrowing_books)b.Borrowing_books); // ספר אחד יכול להיות מקושר לרשומת Borrowing_books אחת


            // קשרים בין waiting_list ל-users
            modelBuilder.Entity<waiting_list>()
                .HasRequired(w => w.users) // waiting_list תלוי ב-users
                .WithMany() // אין צורך בשדה הפוך
                .HasForeignKey(w => w.email);

            base.OnModelCreating(modelBuilder);
        }
    }
}
