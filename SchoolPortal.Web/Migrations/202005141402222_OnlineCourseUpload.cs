namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OnlineCourseUpload : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OnlineCourseUploads",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Topic = c.String(),
                        ClassLevelId = c.Int(),
                        SessionId = c.Int(),
                        SubjectId = c.Int(),
                        UploadType = c.Int(nullable: false),
                        Upload = c.String(nullable: false),
                        Description = c.String(),
                        Date = c.DateTime(),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClassLevels", t => t.ClassLevelId)
                .ForeignKey("dbo.Sessions", t => t.SessionId)
                .ForeignKey("dbo.Subjects", t => t.SubjectId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.ClassLevelId)
                .Index(t => t.SessionId)
                .Index(t => t.SubjectId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OnlineCourseUploads", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OnlineCourseUploads", "SubjectId", "dbo.Subjects");
            DropForeignKey("dbo.OnlineCourseUploads", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.OnlineCourseUploads", "ClassLevelId", "dbo.ClassLevels");
            DropIndex("dbo.OnlineCourseUploads", new[] { "UserId" });
            DropIndex("dbo.OnlineCourseUploads", new[] { "SubjectId" });
            DropIndex("dbo.OnlineCourseUploads", new[] { "SessionId" });
            DropIndex("dbo.OnlineCourseUploads", new[] { "ClassLevelId" });
            DropTable("dbo.OnlineCourseUploads");
        }
    }
}
