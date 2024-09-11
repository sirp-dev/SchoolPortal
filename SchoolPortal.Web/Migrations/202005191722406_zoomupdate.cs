namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class zoomupdate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OnlineZooms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HostEmail = c.String(),
                        UserId = c.String(maxLength: 128),
                        MeetingId = c.String(),
                        ClassDate = c.String(),
                        ClassTime = c.String(),
                        Duration = c.String(),
                        ClassLevelId = c.Int(nullable: false),
                        SubjectId = c.Int(),
                        SessionId = c.Int(nullable: false),
                        Description = c.String(),
                        ClassPassword = c.String(),
                        MeetingType = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClassLevels", t => t.ClassLevelId, cascadeDelete: true)
                .ForeignKey("dbo.Sessions", t => t.SessionId, cascadeDelete: true)
                .ForeignKey("dbo.Subjects", t => t.SubjectId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ClassLevelId)
                .Index(t => t.SubjectId)
                .Index(t => t.SessionId);
            
            AddColumn("dbo.Settings", "ZoomHostEmail", c => c.String());
            AddColumn("dbo.Settings", "EnableZoomHostEmail", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OnlineZooms", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OnlineZooms", "SubjectId", "dbo.Subjects");
            DropForeignKey("dbo.OnlineZooms", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.OnlineZooms", "ClassLevelId", "dbo.ClassLevels");
            DropIndex("dbo.OnlineZooms", new[] { "SessionId" });
            DropIndex("dbo.OnlineZooms", new[] { "SubjectId" });
            DropIndex("dbo.OnlineZooms", new[] { "ClassLevelId" });
            DropIndex("dbo.OnlineZooms", new[] { "UserId" });
            DropColumn("dbo.Settings", "EnableZoomHostEmail");
            DropColumn("dbo.Settings", "ZoomHostEmail");
            DropTable("dbo.OnlineZooms");
        }
    }
}
