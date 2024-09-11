namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class paymentamount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PaymentAmounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClassLevelId = c.Int(),
                        IncomeId = c.Int(),
                        Amount = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClassLevels", t => t.ClassLevelId)
                .ForeignKey("dbo.Incomes", t => t.IncomeId)
                .Index(t => t.ClassLevelId)
                .Index(t => t.IncomeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PaymentAmounts", "IncomeId", "dbo.Incomes");
            DropForeignKey("dbo.PaymentAmounts", "ClassLevelId", "dbo.ClassLevels");
            DropIndex("dbo.PaymentAmounts", new[] { "IncomeId" });
            DropIndex("dbo.PaymentAmounts", new[] { "ClassLevelId" });
            DropTable("dbo.PaymentAmounts");
        }
    }
}
