namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class websitecolor : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WebsiteColors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ColorName = c.String(),
                        WebsiteSettingId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.WebsiteSettings", "WebsiteColorId", c => c.Int(nullable: false));
            DropColumn("dbo.WebsiteSettings", "Slider");
            //DropColumn("dbo.WebsiteSettings", "AboutUs");
            DropColumn("dbo.WebsiteSettings", "EventAndNews");
            DropColumn("dbo.WebsiteSettings", "ContactUs");
            DropColumn("dbo.WebsiteSettings", "Footer");
            DropColumn("dbo.WebsiteSettings", "Menu");
            DropColumn("dbo.WebsiteSettings", "Blog");
            DropColumn("dbo.WebsiteSettings", "BlogDetails");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WebsiteSettings", "BlogDetails", c => c.Int(nullable: false));
            AddColumn("dbo.WebsiteSettings", "Blog", c => c.Int(nullable: false));
            AddColumn("dbo.WebsiteSettings", "Menu", c => c.Int(nullable: false));
            AddColumn("dbo.WebsiteSettings", "Footer", c => c.Int(nullable: false));
            AddColumn("dbo.WebsiteSettings", "ContactUs", c => c.Int(nullable: false));
            AddColumn("dbo.WebsiteSettings", "EventAndNews", c => c.Int(nullable: false));
            AddColumn("dbo.WebsiteSettings", "AboutUs", c => c.Int(nullable: false));
            AddColumn("dbo.WebsiteSettings", "Slider", c => c.Int(nullable: false));
            DropColumn("dbo.WebsiteSettings", "WebsiteColorId");
            DropTable("dbo.WebsiteColors");
        }
    }
}
