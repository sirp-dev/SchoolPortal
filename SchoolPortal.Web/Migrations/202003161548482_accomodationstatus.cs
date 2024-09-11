namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class accomodationstatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Hostels", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.HostelRooms", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.HostelBeds", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.HostelBeds", "Status");
            DropColumn("dbo.HostelRooms", "Status");
            DropColumn("dbo.Hostels", "Status");
        }
    }
}
