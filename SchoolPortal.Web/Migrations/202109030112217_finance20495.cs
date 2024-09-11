namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finance20495 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FinanceInitializers", "Payall", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FinanceInitializers", "Payall");
        }
    }
}
