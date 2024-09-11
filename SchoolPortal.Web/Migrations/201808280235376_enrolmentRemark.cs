namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class enrolmentRemark : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Enrollments", "EnrollmentRemark", c => c.String());
            AddColumn("dbo.Enrollments", "RecognitiveDomain_Id", c => c.Int());
            AddColumn("dbo.RecognitiveDomains", "EnrolmentId", c => c.Int(nullable: false));
            AddColumn("dbo.Settings", "DefaultEnrollmentRemark", c => c.String());
            CreateIndex("dbo.Enrollments", "RecognitiveDomain_Id");
            AddForeignKey("dbo.Enrollments", "RecognitiveDomain_Id", "dbo.RecognitiveDomains", "Id");
            DropColumn("dbo.RecognitiveDomains", "StudentId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RecognitiveDomains", "StudentId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Enrollments", "RecognitiveDomain_Id", "dbo.RecognitiveDomains");
            DropIndex("dbo.Enrollments", new[] { "RecognitiveDomain_Id" });
            DropColumn("dbo.Settings", "DefaultEnrollmentRemark");
            DropColumn("dbo.RecognitiveDomains", "EnrolmentId");
            DropColumn("dbo.Enrollments", "RecognitiveDomain_Id");
            DropColumn("dbo.Enrollments", "EnrollmentRemark");
        }
    }
}
