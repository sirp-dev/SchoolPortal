namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class google : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "GoogleAnalytics", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "GoogleAnalytics");
        }
    }
}
