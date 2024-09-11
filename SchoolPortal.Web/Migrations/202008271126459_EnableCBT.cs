namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EnableCBT : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "EnableCBT", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "EnableCBT");
        }
    }
}
