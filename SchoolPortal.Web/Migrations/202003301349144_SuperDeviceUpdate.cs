namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SuperDeviceUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApprovedDevices", "UserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.ApprovedDevices", "Date", c => c.DateTime());
            CreateIndex("dbo.ApprovedDevices", "UserId");
            AddForeignKey("dbo.ApprovedDevices", "UserId", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.ApprovedDevices", "DeviceThatAddedThis");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ApprovedDevices", "DeviceThatAddedThis", c => c.String());
            DropForeignKey("dbo.ApprovedDevices", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.ApprovedDevices", new[] { "UserId" });
            AlterColumn("dbo.ApprovedDevices", "Date", c => c.String());
            DropColumn("dbo.ApprovedDevices", "UserId");
        }
    }
}
