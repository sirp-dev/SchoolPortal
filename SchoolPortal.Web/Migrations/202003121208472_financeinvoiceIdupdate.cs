namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class financeinvoiceIdupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Finances", "InvoiceId", c => c.Int());
            CreateIndex("dbo.Finances", "InvoiceId");
            AddForeignKey("dbo.Finances", "InvoiceId", "dbo.Invoices", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Finances", "InvoiceId", "dbo.Invoices");
            DropIndex("dbo.Finances", new[] { "InvoiceId" });
            DropColumn("dbo.Finances", "InvoiceId");
        }
    }
}
