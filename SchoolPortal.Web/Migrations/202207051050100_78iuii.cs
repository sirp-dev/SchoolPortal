namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _78iuii : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SitePages", "Title", c => c.String());
            AddColumn("dbo.SitePages", "TitleLink", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SitePages", "TitleLink");
            DropColumn("dbo.SitePages", "Title");
        }
    }
}
