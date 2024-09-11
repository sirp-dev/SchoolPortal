namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uiupdate389 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Settings", "ImageId", c => c.Int());
            AlterColumn("dbo.Settings", "TestScore2", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Settings", "Project", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Settings", "ClassExercise", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Settings", "Assessment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Settings", "PaystackChargePercentage", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Settings", "FlutterwaveChargePercentage", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Settings", "FlutterwaveChargePercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Settings", "PaystackChargePercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Settings", "Assessment", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Settings", "ClassExercise", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Settings", "Project", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Settings", "TestScore2", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Settings", "ImageId", c => c.Int(nullable: false));
        }
    }
}
