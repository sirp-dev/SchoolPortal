namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CategoryPageupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CategoryPages", "ShowInHome", c => c.Boolean(nullable: false));
            AddColumn("dbo.CategoryPages", "ContentHome", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CategoryPages", "ContentHome");
            DropColumn("dbo.CategoryPages", "ShowInHome");
        }
    }
}
