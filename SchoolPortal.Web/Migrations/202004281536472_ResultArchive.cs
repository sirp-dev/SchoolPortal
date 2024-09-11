namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResultArchive : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AffectiveDomainArchives",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Attentiveness = c.String(),
                        Honesty = c.String(),
                        Neatness = c.String(),
                        Punctuality = c.String(),
                        Relationship = c.String(),
                        EnrolmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EnrolledSubjectArchives",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SubjectName = c.String(),
                        TestScore = c.Decimal(precision: 18, scale: 2),
                        ExamScore = c.Decimal(precision: 18, scale: 2),
                        TotalScore = c.Decimal(precision: 18, scale: 2),
                        EnrollmentId = c.Int(nullable: false),
                        IsOffered = c.Boolean(nullable: false),
                        SortOrder = c.Int(nullable: false),
                        GradingOption = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Enrollments", t => t.EnrollmentId, cascadeDelete: true)
                .Index(t => t.EnrollmentId);
            
            CreateTable(
                "dbo.NewsLetterArchives",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NextTResumptionDate = c.String(),
                        GenRemark = c.String(),
                        PTAMeetingDate = c.String(),
                        PTAFee = c.String(),
                        SessionId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PrincipalArchives",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PrincipalName = c.String(),
                        SessionId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PsychomotorDomainArchives",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Drawing = c.String(),
                        Club = c.String(),
                        Painting = c.String(),
                        Handwriting = c.String(),
                        Hobbies = c.String(),
                        Speech = c.String(),
                        Sports = c.String(),
                        EnrolmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RecognitiveDomainArchives",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Rememberance = c.String(),
                        Understanding = c.String(),
                        Application = c.String(),
                        Analyzing = c.String(),
                        Evaluation = c.String(),
                        Creativity = c.String(),
                        EnrolmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SchoolFeesArchives",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Category = c.String(),
                        Amount = c.String(),
                        Discount = c.String(),
                        AmountDue = c.String(),
                        EnrolmentId = c.Int(nullable: false),
                        SessionId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EnrolledSubjectArchives", "EnrollmentId", "dbo.Enrollments");
            DropIndex("dbo.EnrolledSubjectArchives", new[] { "EnrollmentId" });
            DropTable("dbo.SchoolFeesArchives");
            DropTable("dbo.RecognitiveDomainArchives");
            DropTable("dbo.PsychomotorDomainArchives");
            DropTable("dbo.PrincipalArchives");
            DropTable("dbo.NewsLetterArchives");
            DropTable("dbo.EnrolledSubjectArchives");
            DropTable("dbo.AffectiveDomainArchives");
        }
    }
}
