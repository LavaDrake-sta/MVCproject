namespace MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CheckReviewTable : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.review",
            //    c => new
            //        {
            //            ID_review = c.Int(nullable: false, identity: true),
            //            email = c.String(nullable: false, maxLength: 128),
            //            Content = c.String(),
            //            type = c.String(),
            //            book_ID = c.Int(),
            //            users_email = c.String(maxLength: 128),
            //            books_book_id = c.Int(),
            //        })
            //    .PrimaryKey(t => t.ID_review)
            //    .ForeignKey("dbo.books", t => t.book_ID)
            //    .ForeignKey("dbo.users", t => t.users_email)
            //    .ForeignKey("dbo.users", t => t.email, cascadeDelete: true)
            //    .ForeignKey("dbo.books", t => t.books_book_id)
            //    .Index(t => t.email)
            //    .Index(t => t.book_ID)
            //    .Index(t => t.users_email)
            //    .Index(t => t.books_book_id);
            
            //AddColumn("dbo.books", "author", c => c.String());
            //AddColumn("dbo.Borrowing_books", "user_id", c => c.Int());
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.review", "books_book_id", "dbo.books");
            //DropForeignKey("dbo.review", "email", "dbo.users");
            //DropForeignKey("dbo.review", "users_email", "dbo.users");
            //DropForeignKey("dbo.review", "book_ID", "dbo.books");
            //DropIndex("dbo.review", new[] { "books_book_id" });
            //DropIndex("dbo.review", new[] { "users_email" });
            //DropIndex("dbo.review", new[] { "book_ID" });
            //DropIndex("dbo.review", new[] { "email" });
            //DropColumn("dbo.Borrowing_books", "user_id");
            //DropColumn("dbo.books", "author");
            //DropTable("dbo.review");
        }
    }
}
