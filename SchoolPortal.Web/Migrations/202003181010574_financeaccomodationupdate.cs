namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class financeaccomodationupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Finances", "AllocationStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Finances", "AllocationStatus");
        }
    }
}
