namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class financeupdate2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Finances", "FinanceType", c => c.Int(nullable: false));
            AddColumn("dbo.Finances", "FinanceSource", c => c.Int(nullable: false));
            AddColumn("dbo.Finances", "TransactionStatus", c => c.Int(nullable: false));
            DropColumn("dbo.Finances", "FinanceStatus");
            DropColumn("dbo.Finances", "TransactionSource");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Finances", "TransactionSource", c => c.Int(nullable: false));
            AddColumn("dbo.Finances", "FinanceStatus", c => c.Int(nullable: false));
            DropColumn("dbo.Finances", "TransactionStatus");
            DropColumn("dbo.Finances", "FinanceSource");
            DropColumn("dbo.Finances", "FinanceType");
        }
    }
}
