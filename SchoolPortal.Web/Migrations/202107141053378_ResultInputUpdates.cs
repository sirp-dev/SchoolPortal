namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResultInputUpdates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "EnableTestScore", c => c.Boolean(nullable: false,defaultValue:true));
            AddColumn("dbo.Settings", "EnableTestScore2", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "EnableExamScore", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.Settings", "EnableProject", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "EnableClassExercise", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "EnableAssessment", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "EnableAssessment");
            DropColumn("dbo.Settings", "EnableClassExercise");
            DropColumn("dbo.Settings", "EnableProject");
            DropColumn("dbo.Settings", "EnableExamScore");
            DropColumn("dbo.Settings", "EnableTestScore2");
            DropColumn("dbo.Settings", "EnableTestScore");
        }
    }
}
