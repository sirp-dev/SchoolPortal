namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finance20495k3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Enrollments", "AmountRequiredToPay", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Enrollments", "AmountPaid", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Enrollments", "AmountPaid");
            DropColumn("dbo.Enrollments", "AmountRequiredToPay");
        }
    }
}
