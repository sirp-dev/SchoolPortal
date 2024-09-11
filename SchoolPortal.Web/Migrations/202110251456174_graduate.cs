namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class graduate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StudentProfiles", "Graduate", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StudentProfiles", "Graduate");
        }
    }
}
