namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class invoicefinanceupdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Finances", "UserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Invoices", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Finances", "UserId");
            CreateIndex("dbo.Invoices", "UserId");
            AddForeignKey("dbo.Finances", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Invoices", "UserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Invoices", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Finances", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Invoices", new[] { "UserId" });
            DropIndex("dbo.Finances", new[] { "UserId" });
            AlterColumn("dbo.Invoices", "UserId", c => c.String());
            AlterColumn("dbo.Finances", "UserId", c => c.String());
        }
    }
}
