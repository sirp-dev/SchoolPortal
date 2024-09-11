namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finianceInitialization1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FinanceInitializers", "SessionId", c => c.Int(nullable: false));
            CreateIndex("dbo.FinanceInitializers", "IncomeId");
            AddForeignKey("dbo.FinanceInitializers", "IncomeId", "dbo.Incomes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FinanceInitializers", "IncomeId", "dbo.Incomes");
            DropIndex("dbo.FinanceInitializers", new[] { "IncomeId" });
            DropColumn("dbo.FinanceInitializers", "SessionId");
        }
    }
}
