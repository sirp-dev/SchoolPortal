namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class schoolfeesandschoolaccount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SchoolAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AcctPurpose = c.String(),
                        BankName = c.String(),
                        AcctName = c.String(),
                        AcctNumber = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SchoolFees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(precision: 18, scale: 2),
                        Discount = c.Decimal(precision: 18, scale: 2),
                        AmountDue = c.Decimal(precision: 18, scale: 2),
                        EnrolmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Enrollments", "EnrollmentRemark2", c => c.String());
            AddColumn("dbo.Enrollments", "NextResumption", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Enrollments", "NextResumption");
            DropColumn("dbo.Enrollments", "EnrollmentRemark2");
            DropTable("dbo.SchoolFees");
            DropTable("dbo.SchoolAccounts");
        }
    }
}
