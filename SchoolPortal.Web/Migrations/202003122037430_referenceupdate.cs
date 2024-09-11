namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class referenceupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "TransactionReference", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invoices", "TransactionReference");
        }
    }
}
