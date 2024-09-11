namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finance2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FinanceInitializers", "InvoiceNumber", c => c.String());
            AddColumn("dbo.FinanceInitializers", "TransactionDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FinanceInitializers", "TransactionDate");
            DropColumn("dbo.FinanceInitializers", "InvoiceNumber");
        }
    }
}
