namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class supportticketupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "TicketNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tickets", "TicketNumber");
        }
    }
}
