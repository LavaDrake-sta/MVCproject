namespace MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatabaseSchema : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.books", "book_name", c => c.String());
            AlterColumn("dbo.books", "category", c => c.String());
            AlterColumn("dbo.books", "language", c => c.String());
            AlterColumn("dbo.books", "publisher", c => c.String());
            AlterColumn("dbo.books", "ImageUrl", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.books", "ImageUrl", c => c.String(maxLength: 255));
            AlterColumn("dbo.books", "publisher", c => c.String(maxLength: 50));
            AlterColumn("dbo.books", "language", c => c.String(maxLength: 50));
            AlterColumn("dbo.books", "category", c => c.String(maxLength: 50));
            AlterColumn("dbo.books", "book_name", c => c.String(maxLength: 50));
        }
    }
}
