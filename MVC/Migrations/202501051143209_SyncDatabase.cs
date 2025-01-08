namespace MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class SyncDatabase : DbMigration
    {
        public override void Up()
        {
            // בדוק אם הטבלה 'review' כבר קיימת
            Sql(@"
                IF NOT EXISTS (
                    SELECT 1
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_NAME = 'review'
                )
                BEGIN
                    CREATE TABLE [dbo].[review] (
                        [ID_review] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        [email] NVARCHAR(128) NULL,
                        [Content] NVARCHAR(MAX) NULL,
                        [type] NVARCHAR(50) NULL,
                        [book_ID] INT NOT NULL
                    );
                END
            ");

            // קוד אחר שהיה רלוונטי למיגרציה (לא מחיקת הערות)
            // DropPrimaryKey("dbo.Borrowed_books_list");
            // DropPrimaryKey("dbo.Borrowing_books");
            // AddColumn("dbo.books", "CurrentRentCount", c => c.Int());
            // AddColumn("dbo.books", "MaxRentCount", c => c.Int());
            // AddColumn("dbo.books", "IsSold", c => c.Boolean());
            // AddColumn("dbo.books", "IsRent", c => c.Boolean());
            // AddColumn("dbo.orders", "email", c => c.String(maxLength: 128));
            // AddColumn("dbo.waiting_list", "users_email", c => c.String(maxLength: 128));
            // AlterColumn("dbo.Borrowed_books_list", "book_id", c => c.Int(nullable: false));
            // AlterColumn("dbo.Borrowing_books", "book_id", c => c.Int(nullable: false));
            // AlterColumn("dbo.waiting_list", "email", c => c.String(nullable: false, maxLength: 128));
            // AddPrimaryKey("dbo.Borrowed_books_list", "book_id");
            // AddPrimaryKey("dbo.Borrowing_books", "book_id");
            // CreateIndex("dbo.Borrowed_books_list", "book_id");
            // CreateIndex("dbo.Borrowing_books", "book_id");
            // CreateIndex("dbo.orders", "email");
            // CreateIndex("dbo.waiting_list", "email");
            // CreateIndex("dbo.waiting_list", "users_email");
            // AddForeignKey("dbo.Borrowed_books_list", "book_id", "dbo.books", "book_id");
            // AddForeignKey("dbo.Borrowing_books", "book_id", "dbo.books", "book_id");
            // AddForeignKey("dbo.orders", "email", "dbo.users", "email");
            // AddForeignKey("dbo.waiting_list", "email", "dbo.users", "email", cascadeDelete: true);
            // AddForeignKey("dbo.waiting_list", "users_email", "dbo.users", "email");
        }

        public override void Down()
        {
            // מחק את הטבלה 'review' אם היא קיימת
            Sql(@"
                IF EXISTS (
                    SELECT 1
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_NAME = 'review'
                )
                BEGIN
                    DROP TABLE [dbo].[review];
                END
            ");

            // קוד אחר שהיה רלוונטי למיגרציה (לא מחיקת הערות)
            // DropForeignKey("dbo.waiting_list", "users_email", "dbo.users");
            // DropForeignKey("dbo.waiting_list", "email", "dbo.users");
            // DropForeignKey("dbo.orders", "email", "dbo.users");
            // DropForeignKey("dbo.Borrowing_books", "book_id", "dbo.books");
            // DropForeignKey("dbo.Borrowed_books_list", "book_id", "dbo.books");
            // DropIndex("dbo.waiting_list", new[] { "users_email" });
            // DropIndex("dbo.waiting_list", new[] { "email" });
            // DropIndex("dbo.orders", new[] { "email" });
            // DropIndex("dbo.Borrowing_books", new[] { "book_id" });
            // DropIndex("dbo.Borrowed_books_list", new[] { "book_id" });
            // DropPrimaryKey("dbo.Borrowing_books");
            // DropPrimaryKey("dbo.Borrowed_books_list");
            // AlterColumn("dbo.waiting_list", "email", c => c.String());
            // AlterColumn("dbo.Borrowing_books", "book_id", c => c.Int(nullable: false, identity: true));
            // AlterColumn("dbo.Borrowed_books_list", "book_id", c => c.Int(nullable: false, identity: true));
            // DropColumn("dbo.waiting_list", "users_email");
            // DropColumn("dbo.orders", "email");
            // DropColumn("dbo.books", "IsRent");
            // DropColumn("dbo.books", "IsSold");
            // DropColumn("dbo.books", "MaxRentCount");
            // DropColumn("dbo.books", "CurrentRentCount");
            // AddPrimaryKey("dbo.Borrowing_books", "book_id");
            // AddPrimaryKey("dbo.Borrowed_books_list", "book_id");
        }
    }
}
