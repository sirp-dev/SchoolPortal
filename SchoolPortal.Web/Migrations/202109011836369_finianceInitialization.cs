namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finianceInitialization : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FinanceInitializers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EnrollmentId = c.Int(nullable: false),
                        IncomeId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Percent = c.Int(nullable: false),
                        UniqueId = c.String(),
                        TransactionStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Enrollments", t => t.EnrollmentId, cascadeDelete: true)
                .Index(t => t.EnrollmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FinanceInitializers", "EnrollmentId", "dbo.Enrollments");
            DropIndex("dbo.FinanceInitializers", new[] { "EnrollmentId" });
            DropTable("dbo.FinanceInitializers");
        }
    }
}
