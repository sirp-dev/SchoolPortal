namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class majorzoomupdate1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "EnableMultipleZoom", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "EnableMultipleZoom");
        }
    }
}
