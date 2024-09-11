namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class data : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FinanceInitializers", "PaymentAmountId", c => c.Int());
            CreateIndex("dbo.FinanceInitializers", "PaymentAmountId");
            AddForeignKey("dbo.FinanceInitializers", "PaymentAmountId", "dbo.PaymentAmounts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FinanceInitializers", "PaymentAmountId", "dbo.PaymentAmounts");
            DropIndex("dbo.FinanceInitializers", new[] { "PaymentAmountId" });
            DropColumn("dbo.FinanceInitializers", "PaymentAmountId");
        }
    }
}
