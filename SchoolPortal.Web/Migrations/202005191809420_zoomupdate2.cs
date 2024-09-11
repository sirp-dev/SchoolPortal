namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class zoomupdate2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OnlineZooms", "MeetingId", c => c.Long());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OnlineZooms", "MeetingId", c => c.String());
        }
    }
}
