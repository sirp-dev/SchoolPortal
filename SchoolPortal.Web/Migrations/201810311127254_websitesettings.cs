namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class websitesettings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WebsiteSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PortalUrl = c.String(),
                        WebsiteUrl = c.String(),
                        WebsiteLogo = c.Binary(),
                        Layout = c.Int(nullable: false),
                        Slider = c.Int(nullable: false),
                        AboutUs = c.Int(nullable: false),
                        EventAndNews = c.Int(nullable: false),
                        ContactUs = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WebsiteSettings");
        }
    }
}
