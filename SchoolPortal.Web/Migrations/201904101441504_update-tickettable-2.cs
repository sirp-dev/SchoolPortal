namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetickettable2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "IpAddress", c => c.String());
            AddColumn("dbo.Tickets", "browser", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tickets", "browser");
            DropColumn("dbo.Tickets", "IpAddress");
        }
    }
}
