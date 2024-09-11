namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finance204 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FinanceInitializers", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.FinanceInitializers", "PaymentTypeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FinanceInitializers", "PaymentTypeId");
            DropColumn("dbo.FinanceInitializers", "Balance");
        }
    }
}
