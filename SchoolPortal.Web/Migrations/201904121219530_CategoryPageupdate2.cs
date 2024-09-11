namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CategoryPageupdate2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CategoryPages", "RedirectUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CategoryPages", "RedirectUrl");
        }
    }
}
