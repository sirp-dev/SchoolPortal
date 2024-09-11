namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sortorder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClassLevels", "SortByOrder", c => c.Boolean(nullable: false));
            AddColumn("dbo.EnrolledSubjects", "SortOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EnrolledSubjects", "SortOrder");
            DropColumn("dbo.ClassLevels", "SortByOrder");
        }
    }
}
