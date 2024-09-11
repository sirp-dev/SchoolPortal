namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class trackerUpdates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Trackers", "UserName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Trackers", "UserName");
        }
    }
}
