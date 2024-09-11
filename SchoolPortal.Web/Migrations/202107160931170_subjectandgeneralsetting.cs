namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subjectandgeneralsetting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClassLevels", "TestScore2", c => c.Decimal(precision: 18, scale: 2, nullable: false, defaultValue: 0));
            AddColumn("dbo.ClassLevels", "Project", c => c.Decimal(precision: 18, scale: 2, nullable: false, defaultValue: 0));
            AddColumn("dbo.ClassLevels", "ClassExercise", c => c.Decimal(precision: 18, scale: 2, nullable: false, defaultValue: 0));
            AddColumn("dbo.ClassLevels", "Assessment", c => c.Decimal(precision: 18, scale: 2, nullable: false, defaultValue: 0));
            AddColumn("dbo.Settings", "TestScore2", c => c.Decimal(precision: 18, scale: 2, nullable: false, defaultValue: 0));
            AddColumn("dbo.Settings", "Project", c => c.Decimal(precision: 18, scale: 2, nullable: false, defaultValue: 0));
            AddColumn("dbo.Settings", "ClassExercise", c => c.Decimal(precision: 18, scale: 2, nullable: false, defaultValue: 0));
            AddColumn("dbo.Settings", "Assessment", c => c.Decimal(precision: 18, scale: 2, nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "Assessment");
            DropColumn("dbo.Settings", "ClassExercise");
            DropColumn("dbo.Settings", "Project");
            DropColumn("dbo.Settings", "TestScore2");
            DropColumn("dbo.ClassLevels", "Assessment");
            DropColumn("dbo.ClassLevels", "ClassExercise");
            DropColumn("dbo.ClassLevels", "Project");
            DropColumn("dbo.ClassLevels", "TestScore2");
        }
    }
}
