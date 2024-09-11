namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pinusageoption : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "PinValidOption", c => c.Int(nullable: false, defaultValue: 1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "PinValidOption");
        }
    }
}
