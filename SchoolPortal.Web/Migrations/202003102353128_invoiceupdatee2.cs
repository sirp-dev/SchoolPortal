namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class invoiceupdatee2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PaymentAmounts", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PaymentAmounts", "Amount", c => c.String());
        }
    }
}
