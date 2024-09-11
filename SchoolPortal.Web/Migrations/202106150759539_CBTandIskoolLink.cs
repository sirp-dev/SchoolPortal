namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CBTandIskoolLink : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "IskoollLink", c => c.String(nullable: false,defaultValue: "http://iskools.com"));
            AddColumn("dbo.Settings", "CBTLink", c => c.String(nullable: false,defaultValue: "http://cbt.iskools.com"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "CBTLink");
            DropColumn("dbo.Settings", "IskoollLink");
        }
    }
}
