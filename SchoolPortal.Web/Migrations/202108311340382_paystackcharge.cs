namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class paystackcharge : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "PaystackChargePercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Settings", "FlutterwaveSecretKey", c => c.String());
            AddColumn("dbo.Settings", "FlutterwaveChargePercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "FlutterwaveChargePercentage");
            DropColumn("dbo.Settings", "FlutterwaveSecretKey");
            DropColumn("dbo.Settings", "PaystackChargePercentage");
        }
    }
}
