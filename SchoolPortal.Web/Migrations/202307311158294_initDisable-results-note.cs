namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initDisableresultsnote : DbMigration
    {
        public override void Up()
        { 
               AddColumn("dbo.Settings", "DisableAllResultPrintingNote", c => c.String(defaultValue: "UNABLE TO ACCESS RESULTS. KINDLY CONTACT THE AUTHORITY"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "DisableAllResultPrintingNote");
        }
    }
}
