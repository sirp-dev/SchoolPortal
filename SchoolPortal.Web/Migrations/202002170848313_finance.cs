namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finance : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApprovedDevices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MacAddress = c.String(),
                        ImelNumber = c.String(),
                        IpAddress = c.String(),
                        Date = c.String(),
                        DeviceThatAddedThis = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BankDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BankName = c.String(),
                        BankAccountNumber = c.String(),
                        AccountName = c.String(),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Expenditures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Date = c.DateTime(nullable: false),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Finances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Title = c.String(),
                        AdminNote = c.String(),
                        Description = c.String(),
                        FinanceStatus = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReferenceId = c.String(),
                        TransactionSource = c.String(),
                        UserId = c.String(),
                        RegistrationNumber = c.String(),
                        EnrolmentId = c.Int(),
                        SessionId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sessions", t => t.SessionId)
                .Index(t => t.SessionId);
            
            CreateTable(
                "dbo.Incomes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Date = c.DateTime(nullable: false),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Finances", "SessionId", "dbo.Sessions");
            DropIndex("dbo.Finances", new[] { "SessionId" });
            DropTable("dbo.Incomes");
            DropTable("dbo.Finances");
            DropTable("dbo.Expenditures");
            DropTable("dbo.BankDetails");
            DropTable("dbo.ApprovedDevices");
        }
    }
}
