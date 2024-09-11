namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sessionInPincardUpdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PinCodeModels", "SessionId", c => c.Int(nullable: true));
            CreateIndex("dbo.PinCodeModels", "SessionId");
            AddForeignKey("dbo.PinCodeModels", "SessionId", "dbo.Sessions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PinCodeModels", "SessionId", "dbo.Sessions");
            DropIndex("dbo.PinCodeModels", new[] { "SessionId" });
            DropColumn("dbo.PinCodeModels", "SessionId");
        }
    }
}
