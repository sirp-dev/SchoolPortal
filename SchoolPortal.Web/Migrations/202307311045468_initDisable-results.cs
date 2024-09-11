namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initDisableresults : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "DisableAllResultPrinting", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "DisableAllResultPrinting");
        }
    }
}
