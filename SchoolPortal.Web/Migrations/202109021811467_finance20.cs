namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finance20 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FinanceInitializers", "ReferenceId", c => c.String());
            AddColumn("dbo.FinanceInitializers", "FinanceSource", c => c.Int(nullable: false));
            AddColumn("dbo.FinanceInitializers", "FinanceType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FinanceInitializers", "FinanceType");
            DropColumn("dbo.FinanceInitializers", "FinanceSource");
            DropColumn("dbo.FinanceInitializers", "ReferenceId");
        }
    }
}
