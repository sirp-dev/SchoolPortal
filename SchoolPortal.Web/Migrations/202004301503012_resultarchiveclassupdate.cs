namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class resultarchiveclassupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ArchiveResults", "ClassLevelId", c => c.Int());
            AddColumn("dbo.EnrolledSubjectArchives", "EnrollmentSubjectId", c => c.Int());
            CreateIndex("dbo.ArchiveResults", "SessionId");
            CreateIndex("dbo.ArchiveResults", "ClassLevelId");
            AddForeignKey("dbo.ArchiveResults", "ClassLevelId", "dbo.ClassLevels", "Id");
            AddForeignKey("dbo.ArchiveResults", "SessionId", "dbo.Sessions", "Id");
            DropColumn("dbo.ArchiveResults", "ClassId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ArchiveResults", "ClassId", c => c.Int());
            DropForeignKey("dbo.ArchiveResults", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.ArchiveResults", "ClassLevelId", "dbo.ClassLevels");
            DropIndex("dbo.ArchiveResults", new[] { "ClassLevelId" });
            DropIndex("dbo.ArchiveResults", new[] { "SessionId" });
            DropColumn("dbo.EnrolledSubjectArchives", "EnrollmentSubjectId");
            DropColumn("dbo.ArchiveResults", "ClassLevelId");
        }
    }
}
