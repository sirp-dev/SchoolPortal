namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResultArchiveUpdates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EnrolledSubjectArchives", "TestScore2", c => c.Decimal(precision: 18, scale: 2, nullable: false, defaultValue: 0));
            AddColumn("dbo.EnrolledSubjectArchives", "Project", c => c.Decimal(precision: 18, scale: 2, nullable: false, defaultValue: 0));
            AddColumn("dbo.EnrolledSubjectArchives", "ClassExercise", c => c.Decimal(precision: 18, scale: 2, nullable: false, defaultValue: 0));
            AddColumn("dbo.EnrolledSubjectArchives", "Assessment", c => c.Decimal(precision: 18, scale: 2, nullable: false, defaultValue: 0));
            AddColumn("dbo.EnrolledSubjectArchives", "TotalCA", c => c.Decimal(precision: 18, scale: 2, nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EnrolledSubjectArchives", "TotalCA");
            DropColumn("dbo.EnrolledSubjectArchives", "Assessment");
            DropColumn("dbo.EnrolledSubjectArchives", "ClassExercise");
            DropColumn("dbo.EnrolledSubjectArchives", "Project");
            DropColumn("dbo.EnrolledSubjectArchives", "TestScore2");
        }
    }
}
