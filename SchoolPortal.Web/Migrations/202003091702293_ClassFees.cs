namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClassFees : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClassLevels", "SchoolFee", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClassLevels", "SchoolFee");
        }
    }
}
