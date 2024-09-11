namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class scriptfull : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClassLevels", "AccessmentScore", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.ClassLevels", "ExamScore", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.BatchResults", "AverageScore", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Settings", "PromotionByMathsAndEng", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "PromotionByMathsAndEng");
            DropColumn("dbo.BatchResults", "AverageScore");
            DropColumn("dbo.ClassLevels", "ExamScore");
            DropColumn("dbo.ClassLevels", "AccessmentScore");
        }
    }
}
