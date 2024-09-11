namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class promoteall : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "PromoteAll", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "PromoteAll");
        }
    }
}
