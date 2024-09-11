namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class websitefrontend : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WebsiteSettings", "Slider", c => c.Int(nullable: false));
            //AddColumn("dbo.WebsiteSettings", "AboutUs", c => c.Int(nullable: false));
            AddColumn("dbo.WebsiteSettings", "EventAndNews", c => c.Int(nullable: false));
            AddColumn("dbo.WebsiteSettings", "ContactUs", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.WebsiteSettings", "ContactUs");
            DropColumn("dbo.WebsiteSettings", "EventAndNews");
            //DropColumn("dbo.WebsiteSettings", "AboutUs");
            DropColumn("dbo.WebsiteSettings", "Slider");
        }
    }
}
