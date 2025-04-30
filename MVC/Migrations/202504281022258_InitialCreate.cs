namespace MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.books",
                c => new
                    {
                        book_id = c.Int(nullable: false, identity: true),
                        book_name = c.String(),
                        category = c.String(),
                        language = c.String(),
                        publication_date = c.DateTime(),
                        publisher = c.String(),
                        link = c.String(),
                        price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ImageUrl = c.String(),
                        CurrentRentCount = c.Int(),
                        MaxRentCount = c.Int(),
                        IsSold = c.Boolean(),
                        IsRent = c.Boolean(),
                        author = c.String(),
                    })
                .PrimaryKey(t => t.book_id);
            
            CreateTable(
                "dbo.Borrowed_books_list",
                c => new
                    {
                        book_id = c.Int(nullable: false),
                        book_name = c.String(),
                        category = c.String(),
                        Date_taken = c.DateTime(nullable: false),
                        return_date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.book_id)
                .ForeignKey("dbo.books", t => t.book_id)
                .Index(t => t.book_id);
            
            CreateTable(
                "dbo.Borrowing_books",
                c => new
                    {
                        book_id = c.Int(nullable: false),
                        book_name = c.String(),
                        category = c.String(),
                        date_taken = c.DateTime(nullable: false),
                        return_date = c.DateTime(nullable: false),
                        email = c.String(nullable: false, maxLength: 128),
                        EmailSent = c.Boolean(),
                    })
                .PrimaryKey(t => t.book_id)
                .ForeignKey("dbo.books", t => t.book_id)
                .ForeignKey("dbo.users", t => t.email, cascadeDelete: true)
                .Index(t => t.book_id)
                .Index(t => t.email);
            
            CreateTable(
                "dbo.users",
                c => new
                    {
                        email = c.String(nullable: false, maxLength: 128),
                        name = c.String(),
                        password = c.String(),
                        type = c.String(),
                    })
                .PrimaryKey(t => t.email);
            
            CreateTable(
                "dbo.orders",
                c => new
                    {
                        order_number = c.Int(nullable: false, identity: true),
                        email = c.String(maxLength: 128),
                        first_name = c.String(),
                        last_name = c.String(),
                        id = c.Int(nullable: false),
                        card_owner_name = c.String(),
                        card_number = c.String(),
                        expiry_date = c.String(),
                        CVC = c.String(),
                        number_of_payments = c.Int(nullable: false),
                        price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        product = c.String(),
                        buy_borrow = c.String(),
                        date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.order_number)
                .ForeignKey("dbo.users", t => t.email)
                .Index(t => t.email);
            
            CreateTable(
                "dbo.review",
                c => new
                    {
                        ID_review = c.Int(nullable: false, identity: true),
                        email = c.String(nullable: false, maxLength: 128),
                        Content = c.String(),
                        type = c.String(),
                        book_ID = c.Int(),
                        created_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID_review)
                .ForeignKey("dbo.books", t => t.book_ID)
                .ForeignKey("dbo.users", t => t.email, cascadeDelete: true)
                .Index(t => t.email)
                .Index(t => t.book_ID);
            
            CreateTable(
                "dbo.waiting_list",
                c => new
                    {
                        name = c.String(nullable: false, maxLength: 128),
                        book_name = c.String(),
                        date = c.DateTime(nullable: false),
                        email = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.name)
                .ForeignKey("dbo.users", t => t.email, cascadeDelete: true)
                .Index(t => t.email);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Borrowing_books", "email", "dbo.users");
            DropForeignKey("dbo.waiting_list", "email", "dbo.users");
            DropForeignKey("dbo.review", "email", "dbo.users");
            DropForeignKey("dbo.review", "book_ID", "dbo.books");
            DropForeignKey("dbo.orders", "email", "dbo.users");
            DropForeignKey("dbo.Borrowing_books", "book_id", "dbo.books");
            DropForeignKey("dbo.Borrowed_books_list", "book_id", "dbo.books");
            DropIndex("dbo.waiting_list", new[] { "email" });
            DropIndex("dbo.review", new[] { "book_ID" });
            DropIndex("dbo.review", new[] { "email" });
            DropIndex("dbo.orders", new[] { "email" });
            DropIndex("dbo.Borrowing_books", new[] { "email" });
            DropIndex("dbo.Borrowing_books", new[] { "book_id" });
            DropIndex("dbo.Borrowed_books_list", new[] { "book_id" });
            DropTable("dbo.waiting_list");
            DropTable("dbo.review");
            DropTable("dbo.orders");
            DropTable("dbo.users");
            DropTable("dbo.Borrowing_books");
            DropTable("dbo.Borrowed_books_list");
            DropTable("dbo.books");
        }
    }
}
