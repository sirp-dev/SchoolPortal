namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lessonnote : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LessonNotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SubjectId = c.Int(),
                        SessionId = c.Int(),
                        StaffProfileId = c.Int(),
                        Topic = c.String(),
                        Note = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        LastEdited = c.DateTime(nullable: false),
                        IsPublished = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sessions", t => t.SessionId)
                .ForeignKey("dbo.StaffProfiles", t => t.StaffProfileId)
                .ForeignKey("dbo.Subjects", t => t.SubjectId)
                .Index(t => t.SubjectId)
                .Index(t => t.SessionId)
                .Index(t => t.StaffProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LessonNotes", "SubjectId", "dbo.Subjects");
            DropForeignKey("dbo.LessonNotes", "StaffProfileId", "dbo.StaffProfiles");
            DropForeignKey("dbo.LessonNotes", "SessionId", "dbo.Sessions");
            DropIndex("dbo.LessonNotes", new[] { "StaffProfileId" });
            DropIndex("dbo.LessonNotes", new[] { "SessionId" });
            DropIndex("dbo.LessonNotes", new[] { "SubjectId" });
            DropTable("dbo.LessonNotes");
        }
    }
}
