namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class onlineschool : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LiveClassOnlines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClassLevelId = c.Int(nullable: false),
                        SubjectId = c.Int(),
                        SessionId = c.Int(nullable: false),
                        UserId = c.String(),
                        UrlLive = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        ClassDate = c.String(),
                        ClassTime = c.String(),
                        Duration = c.String(),
                        TeacherName = c.String(),
                        Description = c.String(),
                        LiveStatus = c.Int(nullable: false),
                        Users_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClassLevels", t => t.ClassLevelId, cascadeDelete: true)
                .ForeignKey("dbo.Sessions", t => t.SessionId, cascadeDelete: true)
                .ForeignKey("dbo.Subjects", t => t.SubjectId)
                .ForeignKey("dbo.AspNetUsers", t => t.Users_Id)
                .Index(t => t.ClassLevelId)
                .Index(t => t.SubjectId)
                .Index(t => t.SessionId)
                .Index(t => t.Users_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LiveClassOnlines", "Users_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.LiveClassOnlines", "SubjectId", "dbo.Subjects");
            DropForeignKey("dbo.LiveClassOnlines", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.LiveClassOnlines", "ClassLevelId", "dbo.ClassLevels");
            DropIndex("dbo.LiveClassOnlines", new[] { "Users_Id" });
            DropIndex("dbo.LiveClassOnlines", new[] { "SessionId" });
            DropIndex("dbo.LiveClassOnlines", new[] { "SubjectId" });
            DropIndex("dbo.LiveClassOnlines", new[] { "ClassLevelId" });
            DropTable("dbo.LiveClassOnlines");
        }
    }
}
