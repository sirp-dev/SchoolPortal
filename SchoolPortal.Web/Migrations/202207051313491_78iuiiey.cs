namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _78iuiiey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SitePages", "IsNews", c => c.Boolean(nullable: false));
            AddColumn("dbo.SitePages", "PreviewContent", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SitePages", "PreviewContent");
            DropColumn("dbo.SitePages", "IsNews");
        }
    }
}
