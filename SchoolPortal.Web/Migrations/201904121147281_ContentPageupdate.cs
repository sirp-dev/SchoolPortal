namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContentPageupdate : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.CategoryContentPages", newName: "ContentPages");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.ContentPages", newName: "CategoryContentPages");
        }
    }
}
