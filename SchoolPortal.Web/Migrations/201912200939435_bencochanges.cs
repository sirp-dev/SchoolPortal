namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bencochanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "NewsletterContent", c => c.String());
            AddColumn("dbo.Settings", "ShowNewsletterPage", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Settings", "ShowNewsletterPage");
            DropColumn("dbo.Settings", "NewsletterContent");
        }
    }
}
