namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrintedStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Enrollments", "Printed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Enrollments", "Printed");
        }
    }
}
