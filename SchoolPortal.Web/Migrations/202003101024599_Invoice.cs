namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Invoice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Finances", "TellerPic", c => c.String());
            AddColumn("dbo.Invoices", "UserId", c => c.String());
            AddColumn("dbo.Invoices", "RegistrationNumber", c => c.String());
            AddColumn("dbo.Invoices", "EnrolmentId", c => c.Int());
            AddColumn("dbo.Invoices", "SessionId", c => c.Int());
            CreateIndex("dbo.Invoices", "SessionId");
            AddForeignKey("dbo.Invoices", "SessionId", "dbo.Sessions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Invoices", "SessionId", "dbo.Sessions");
            DropIndex("dbo.Invoices", new[] { "SessionId" });
            DropColumn("dbo.Invoices", "SessionId");
            DropColumn("dbo.Invoices", "EnrolmentId");
            DropColumn("dbo.Invoices", "RegistrationNumber");
            DropColumn("dbo.Invoices", "UserId");
            DropColumn("dbo.Finances", "TellerPic");
        }
    }
}
