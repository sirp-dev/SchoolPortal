namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _78iui : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SitePages", "PageLink", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SitePages", "PageLink");
        }
    }
}
