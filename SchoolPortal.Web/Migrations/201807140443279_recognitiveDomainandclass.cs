namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recognitiveDomainandclass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RecognitiveDomains",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Rememberance = c.String(),
                    Understanding = c.String(),
                    Application = c.String(),
                    Analyzing = c.String(),
                    Evaluation = c.String(),
                    Creativity = c.String(),
                    StudentId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            AddColumn("dbo.ClassLevels", "Passmark", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.ClassLevels", "PromotionByTrial", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }

        public override void Down()
        {
            DropColumn("dbo.ClassLevels", "PromotionByTrial");
            DropColumn("dbo.ClassLevels", "Passmark");
            DropTable("dbo.RecognitiveDomains");
        }
    }
}
