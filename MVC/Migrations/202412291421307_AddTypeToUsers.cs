namespace MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTypeToUsers : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.books",
            //    c => new
            //        {
            //            book_id = c.Single(nullable: false),
            //            book_name = c.String(),
            //            category = c.String(),
            //            language = c.String(),
            //            Publication_date = c.DateTime(),
            //            publisher = c.String(),
            //            link = c.String(),
            //            price = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            ImageUrl = c.String(),
            //        })
            //    .PrimaryKey(t => t.book_id);
            
            //CreateTable(
            //    "dbo.Borrowed_books_list",
            //    c => new
            //        {
            //            book_id = c.Single(nullable: false),
            //            book_name = c.String(),
            //            category = c.String(),
            //            Date_taken = c.DateTime(nullable: false),
            //            return_date = c.DateTime(nullable: false),
            //        })
            //    .PrimaryKey(t => t.book_id);
            
            //CreateTable(
            //    "dbo.Borrowing_books",
            //    c => new
            //        {
            //            book_id = c.Single(nullable: false),
            //            book_name = c.String(),
            //            category = c.String(),
            //            language = c.String(),
            //            publisher = c.String(),
            //            Publication_date = c.DateTime(),
            //            available = c.Boolean(nullable: false),
            //            price = c.Decimal(nullable: false, precision: 18, scale: 2),
            //        })
            //    .PrimaryKey(t => t.book_id);
            
            //CreateTable(
            //    "dbo.orders",
            //    c => new
            //        {
            //            order_number = c.Single(nullable: false),
            //            First_name = c.String(),
            //            Last_name = c.String(),
            //            ID = c.Single(nullable: false),
            //            card_owner_name = c.String(),
            //            card_number = c.Single(nullable: false),
            //            expird_date = c.String(),
            //            CVC = c.Single(nullable: false),
            //            number_of_payment = c.Single(nullable: false),
            //            price = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            prodact = c.String(),
            //            Buy_Borrow = c.String(),
            //            date = c.DateTime(nullable: false),
            //        })
            //    .PrimaryKey(t => t.order_number);
            
            //CreateTable(
            //    "dbo.users",
            //    c => new
            //        {
            //            email = c.String(nullable: false, maxLength: 128),
            //            name = c.String(),
            //            password = c.String(),
            //            type = c.String(),
            //        })
            //    .PrimaryKey(t => t.email);
            
            //CreateTable(
            //    "dbo.waiting_list",
            //    c => new
            //        {
            //            name = c.String(nullable: false, maxLength: 128),
            //            book_name = c.String(),
            //            date = c.DateTime(nullable: false),
            //            email = c.String(),
            //        })
            //    .PrimaryKey(t => t.name);
            
        }
        
        public override void Down()
        {
            //DropTable("dbo.waiting_list");
            //DropTable("dbo.users");
            //DropTable("dbo.orders");
            //DropTable("dbo.Borrowing_books");
            //DropTable("dbo.Borrowed_books_list");
            //DropTable("dbo.books");
        }
    }
}
