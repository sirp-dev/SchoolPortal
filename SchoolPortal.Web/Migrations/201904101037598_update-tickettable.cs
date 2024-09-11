namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetickettable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Responses", "RepliedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Responses", "RepliedBy");
        }
    }
}
