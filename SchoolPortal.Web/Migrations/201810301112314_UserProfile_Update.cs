namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserProfile_Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StudentProfiles", "IsUpdated", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StudentProfiles", "IsUpdated");
        }
    }
}
