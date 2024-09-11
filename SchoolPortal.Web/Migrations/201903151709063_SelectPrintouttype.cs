namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SelectPrintouttype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "PrintOutOption", c => c.Int(nullable: false, defaultValue: 1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "PrintOutOption");
        }
    }
}
