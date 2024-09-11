namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class majorzoomupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Enrollments", "EnableLiveClass", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "ZoomHostOne", c => c.String());
            AddColumn("dbo.Settings", "ZoomHostOnePass", c => c.String());
            AddColumn("dbo.Settings", "ZoomHostTwo", c => c.String());
            AddColumn("dbo.Settings", "ZoomHostTwoPass", c => c.String());
            AddColumn("dbo.Settings", "ZoomHostThree", c => c.String());
            AddColumn("dbo.Settings", "ZoomHostThreePass", c => c.String());
            AddColumn("dbo.Settings", "EnableZoom", c => c.Boolean(nullable: false));
            DropColumn("dbo.Settings", "ZoomHostEmail");
            DropColumn("dbo.Settings", "EnableZoomHostEmail");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Settings", "EnableZoomHostEmail", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "ZoomHostEmail", c => c.String());
            DropColumn("dbo.Settings", "EnableZoom");
            DropColumn("dbo.Settings", "ZoomHostThreePass");
            DropColumn("dbo.Settings", "ZoomHostThree");
            DropColumn("dbo.Settings", "ZoomHostTwoPass");
            DropColumn("dbo.Settings", "ZoomHostTwo");
            DropColumn("dbo.Settings", "ZoomHostOnePass");
            DropColumn("dbo.Settings", "ZoomHostOne");
            DropColumn("dbo.Enrollments", "EnableLiveClass");
        }
    }
}
