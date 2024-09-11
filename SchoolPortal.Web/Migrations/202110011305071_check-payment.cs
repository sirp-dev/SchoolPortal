namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class checkpayment : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PaymentAmounts", "ClassLevelId", "dbo.ClassLevels");
            DropForeignKey("dbo.PaymentAmounts", "IncomeId", "dbo.Incomes");
            DropForeignKey("dbo.FinanceInitializers", "PaymentAmountId", "dbo.PaymentAmounts");
            DropIndex("dbo.PaymentAmounts", new[] { "ClassLevelId" });
            DropIndex("dbo.PaymentAmounts", new[] { "IncomeId" });
            DropIndex("dbo.FinanceInitializers", new[] { "PaymentAmountId" });
            AddColumn("dbo.Incomes", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropTable("dbo.PaymentAmounts");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PaymentAmounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClassLevelId = c.Int(),
                        IncomeId = c.Int(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.Incomes", "Amount");
            CreateIndex("dbo.FinanceInitializers", "PaymentAmountId");
            CreateIndex("dbo.PaymentAmounts", "IncomeId");
            CreateIndex("dbo.PaymentAmounts", "ClassLevelId");
            AddForeignKey("dbo.FinanceInitializers", "PaymentAmountId", "dbo.PaymentAmounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PaymentAmounts", "IncomeId", "dbo.Incomes", "Id");
            AddForeignKey("dbo.PaymentAmounts", "ClassLevelId", "dbo.ClassLevels", "Id");
        }
    }
}
