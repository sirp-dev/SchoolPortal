namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatePaymenttwo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Finances", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Finances", "UniqueIdCheck", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Finances", "UniqueIdCheck");
            DropColumn("dbo.Finances", "Balance");
        }
    }
}
