namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Deveiceupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApprovedDevices", "DeviceName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApprovedDevices", "DeviceName");
        }
    }
}
