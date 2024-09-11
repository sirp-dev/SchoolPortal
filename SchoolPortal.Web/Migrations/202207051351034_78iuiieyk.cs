namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _78iuiieyk : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "Link", c => c.String());
            AddColumn("dbo.PostImages", "ImageByte", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PostImages", "ImageByte");
            DropColumn("dbo.Posts", "Link");
        }
    }
}
