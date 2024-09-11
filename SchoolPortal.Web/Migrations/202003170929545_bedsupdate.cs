namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bedsupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HostelBeds", "HostelId", c => c.Int());
            CreateIndex("dbo.HostelBeds", "HostelId");
            AddForeignKey("dbo.HostelBeds", "HostelId", "dbo.Hostels", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HostelBeds", "HostelId", "dbo.Hostels");
            DropIndex("dbo.HostelBeds", new[] { "HostelId" });
            DropColumn("dbo.HostelBeds", "HostelId");
        }
    }
}
