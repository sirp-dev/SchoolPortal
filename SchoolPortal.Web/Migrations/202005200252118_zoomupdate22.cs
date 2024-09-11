namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class zoomupdate22 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OnlineZooms", "MeetingUUId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OnlineZooms", "MeetingUUId");
        }
    }
}
