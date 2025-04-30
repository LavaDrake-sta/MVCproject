namespace MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldsToUsers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.users", "FirstName", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.users", "LastName", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.users", "IDNumber", c => c.String(nullable: false, maxLength: 9));
            AddColumn("dbo.users", "CreditCardNumber", c => c.String(nullable: false, maxLength: 19));
            AddColumn("dbo.users", "ValidDate", c => c.String(nullable: false, maxLength: 5));
            AddColumn("dbo.users", "CVC", c => c.String(nullable: false, maxLength: 3));
            AddColumn("dbo.users", "IsAdmin", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
        }
    }
}
