namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class financeenrolupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Finances", "EnrollmentId", c => c.Int());
            AddColumn("dbo.Invoices", "EnrollmentId", c => c.Int());
            CreateIndex("dbo.Finances", "EnrollmentId");
            CreateIndex("dbo.Invoices", "EnrollmentId");
            AddForeignKey("dbo.Finances", "EnrollmentId", "dbo.Enrollments", "Id");
            AddForeignKey("dbo.Invoices", "EnrollmentId", "dbo.Enrollments", "Id");
            DropColumn("dbo.Finances", "EnrolmentId");
            DropColumn("dbo.Invoices", "EnrolmentId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Invoices", "EnrolmentId", c => c.Int());
            AddColumn("dbo.Finances", "EnrolmentId", c => c.Int());
            DropForeignKey("dbo.Invoices", "EnrollmentId", "dbo.Enrollments");
            DropForeignKey("dbo.Finances", "EnrollmentId", "dbo.Enrollments");
            DropIndex("dbo.Invoices", new[] { "EnrollmentId" });
            DropIndex("dbo.Finances", new[] { "EnrollmentId" });
            DropColumn("dbo.Invoices", "EnrollmentId");
            DropColumn("dbo.Finances", "EnrollmentId");
        }
    }
}
