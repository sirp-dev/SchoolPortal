namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Security : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StudentProfiles", "SecurityQuestion", c => c.String());
            AddColumn("dbo.StudentProfiles", "SecurityAnswer", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StudentProfiles", "SecurityAnswer");
            DropColumn("dbo.StudentProfiles", "SecurityQuestion");
        }
    }
}
