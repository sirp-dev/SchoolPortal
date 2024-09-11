namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResultUpdates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EnrolledSubjects", "TestScore2", c => c.Decimal(precision: 18, scale: 2, nullable: false,defaultValue:0));
            AddColumn("dbo.EnrolledSubjects", "Project", c => c.Decimal(precision: 18, scale: 2, nullable: false, defaultValue: 0));
            AddColumn("dbo.EnrolledSubjects", "ClassExercise", c => c.Decimal(precision: 18, scale: 2, nullable: false, defaultValue: 0));
            AddColumn("dbo.EnrolledSubjects", "Assessment", c => c.Decimal(precision: 18, scale: 2, nullable: false, defaultValue: 0));
            AddColumn("dbo.EnrolledSubjects", "TotalCA", c => c.Decimal(precision: 18, scale: 2, nullable: false, defaultValue: 0));
            AddColumn("dbo.Subjects", "TestScore2", c => c.Decimal(precision: 18, scale: 2, nullable: false, defaultValue: 0));
            AddColumn("dbo.Subjects", "Project", c => c.Decimal(precision: 18, scale: 2, nullable: false, defaultValue: 0));
            AddColumn("dbo.Subjects", "ClassExercise", c => c.Decimal(precision: 18, scale: 2, nullable: false, defaultValue: 0));
            AddColumn("dbo.Subjects", "Assessment", c => c.Decimal(precision: 18, scale: 2, nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Subjects", "Assessment");
            DropColumn("dbo.Subjects", "ClassExercise");
            DropColumn("dbo.Subjects", "Project");
            DropColumn("dbo.Subjects", "TestScore2");
            DropColumn("dbo.EnrolledSubjects", "TotalCA");
            DropColumn("dbo.EnrolledSubjects", "Assessment");
            DropColumn("dbo.EnrolledSubjects", "ClassExercise");
            DropColumn("dbo.EnrolledSubjects", "Project");
            DropColumn("dbo.EnrolledSubjects", "TestScore2");
        }
    }
}
