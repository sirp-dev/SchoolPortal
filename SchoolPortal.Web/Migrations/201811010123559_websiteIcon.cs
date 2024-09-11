namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class websiteIcon : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WebsiteSettings", "WebsiteIcon", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.WebsiteSettings", "WebsiteIcon");
        }
    }
}
