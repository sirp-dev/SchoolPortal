namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prinoutandfeeupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SchoolFees", "Category", c => c.String());
            AddColumn("dbo.SchoolFees", "SessionId", c => c.Int());
            AddColumn("dbo.Settings", "ShowFeesOnResult", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "ShowAccctOnResult", c => c.Boolean(nullable: false));
            CreateIndex("dbo.SchoolFees", "SessionId");
            AddForeignKey("dbo.SchoolFees", "SessionId", "dbo.Sessions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SchoolFees", "SessionId", "dbo.Sessions");
            DropIndex("dbo.SchoolFees", new[] { "SessionId" });
            DropColumn("dbo.Settings", "ShowAccctOnResult");
            DropColumn("dbo.Settings", "ShowFeesOnResult");
            DropColumn("dbo.SchoolFees", "SessionId");
            DropColumn("dbo.SchoolFees", "Category");
        }
    }
}
