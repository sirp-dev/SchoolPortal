namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatePayment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Finances", "PaymentTypeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Finances", "PaymentTypeId");
        }
    }
}
