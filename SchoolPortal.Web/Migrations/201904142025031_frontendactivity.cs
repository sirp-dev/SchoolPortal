namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class frontendactivity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "PreviewContent", c => c.String(nullable: false));
            AddColumn("dbo.NewsLetters", "PTAMeetingDate", c => c.String());
            AddColumn("dbo.NewsLetters", "PTAFee", c => c.String());
            AddColumn("dbo.WebsiteSettings", "Color", c => c.String());
            DropColumn("dbo.WebsiteSettings", "WebsiteColorId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WebsiteSettings", "WebsiteColorId", c => c.Int(nullable: false));
            DropColumn("dbo.WebsiteSettings", "Color");
            DropColumn("dbo.NewsLetters", "PTAFee");
            DropColumn("dbo.NewsLetters", "PTAMeetingDate");
            DropColumn("dbo.Posts", "PreviewContent");
        }
    }
}
