namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finance2049 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Finances", "IncomeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Finances", "IncomeId");
        }
    }
}
