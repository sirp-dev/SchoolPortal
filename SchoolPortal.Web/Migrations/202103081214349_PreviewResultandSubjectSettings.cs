namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PreviewResultandSubjectSettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subjects", "ShowSubject", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.Settings", "EnablePreviewResult", c => c.Boolean(nullable: false,defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "EnablePreviewResult");
            DropColumn("dbo.Subjects", "ShowSubject");
        }
    }
}
