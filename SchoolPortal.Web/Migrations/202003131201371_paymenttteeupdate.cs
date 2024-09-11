namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class paymenttteeupdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Invoices", "EnrollmentId", "dbo.Enrollments");
            DropForeignKey("dbo.Invoices", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.Invoices", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Finances", "InvoiceId", "dbo.Invoices");
            DropIndex("dbo.Finances", new[] { "InvoiceId" });
            DropIndex("dbo.Invoices", new[] { "UserId" });
            DropIndex("dbo.Invoices", new[] { "EnrollmentId" });
            DropIndex("dbo.Invoices", new[] { "SessionId" });
            AddColumn("dbo.Finances", "InvoiceNumber", c => c.String());
            AddColumn("dbo.Finances", "ApprovedBy", c => c.String());
            DropColumn("dbo.Finances", "InvoiceId");
            DropColumn("dbo.Finances", "TellerPic");
            DropTable("dbo.Invoices");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Invoices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClassName = c.String(),
                        Title = c.String(),
                        InvoiceNumber = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UserId = c.String(maxLength: 128),
                        Status = c.Int(nullable: false),
                        RegistrationNumber = c.String(),
                        TransactionReference = c.String(),
                        EnrollmentId = c.Int(),
                        SessionId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Finances", "TellerPic", c => c.String());
            AddColumn("dbo.Finances", "InvoiceId", c => c.Int());
            DropColumn("dbo.Finances", "ApprovedBy");
            DropColumn("dbo.Finances", "InvoiceNumber");
            CreateIndex("dbo.Invoices", "SessionId");
            CreateIndex("dbo.Invoices", "EnrollmentId");
            CreateIndex("dbo.Invoices", "UserId");
            CreateIndex("dbo.Finances", "InvoiceId");
            AddForeignKey("dbo.Finances", "InvoiceId", "dbo.Invoices", "Id");
            AddForeignKey("dbo.Invoices", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Invoices", "SessionId", "dbo.Sessions", "Id");
            AddForeignKey("dbo.Invoices", "EnrollmentId", "dbo.Enrollments", "Id");
        }
    }
}
