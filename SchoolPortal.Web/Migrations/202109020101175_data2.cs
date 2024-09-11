namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class data2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FinanceInitializers", "PaymentAmountId", "dbo.PaymentAmounts");
            DropIndex("dbo.FinanceInitializers", new[] { "PaymentAmountId" });
            AlterColumn("dbo.FinanceInitializers", "PaymentAmountId", c => c.Int(nullable: false));
            CreateIndex("dbo.FinanceInitializers", "PaymentAmountId");
            AddForeignKey("dbo.FinanceInitializers", "PaymentAmountId", "dbo.PaymentAmounts", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FinanceInitializers", "PaymentAmountId", "dbo.PaymentAmounts");
            DropIndex("dbo.FinanceInitializers", new[] { "PaymentAmountId" });
            AlterColumn("dbo.FinanceInitializers", "PaymentAmountId", c => c.Int());
            CreateIndex("dbo.FinanceInitializers", "PaymentAmountId");
            AddForeignKey("dbo.FinanceInitializers", "PaymentAmountId", "dbo.PaymentAmounts", "Id");
        }
    }
}
