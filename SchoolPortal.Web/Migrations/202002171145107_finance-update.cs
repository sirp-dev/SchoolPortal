namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class financeupdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Finances", "TransactionSource", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Finances", "TransactionSource", c => c.String());
        }
    }
}
