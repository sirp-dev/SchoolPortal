namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShowInWebsiteHomeSettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WebsiteSettings", "ShowBlogInHome", c => c.Boolean(nullable: false));
            AddColumn("dbo.WebsiteSettings", "ShowHallOfFameInHome", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WebsiteSettings", "ShowHallOfFameInHome");
            DropColumn("dbo.WebsiteSettings", "ShowBlogInHome");
        }
    }
}
