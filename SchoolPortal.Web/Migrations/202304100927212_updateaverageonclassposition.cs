namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateaverageonclassposition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClassLevels", "ShowAverageOverPositionInClass", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClassLevels", "ShowAverageOverPositionInClass");
        }
    }
}
