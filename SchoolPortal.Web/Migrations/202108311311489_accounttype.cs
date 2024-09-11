namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class accounttype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BankDetails", "AccountType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BankDetails", "AccountType");
        }
    }
}
