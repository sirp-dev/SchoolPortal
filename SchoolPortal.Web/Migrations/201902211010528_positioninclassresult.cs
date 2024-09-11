namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class positioninclassresult : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClassLevels", "ShowPositionOnClassResult", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClassLevels", "ShowPositionOnClassResult");
        }
    }
}
