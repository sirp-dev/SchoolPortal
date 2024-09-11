namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tracker : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Trackers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        FullName = c.String(),
                        Note = c.String(),
                        ActionDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Trackers", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Trackers", new[] { "UserId" });
            DropTable("dbo.Trackers");
        }
    }
}
