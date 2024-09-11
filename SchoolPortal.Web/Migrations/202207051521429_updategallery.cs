namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updategallery : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ImageGalleries", "ImageByte", c => c.String());
            AddColumn("dbo.SiteGalleries", "UpperSection", c => c.String());
            AddColumn("dbo.SiteGalleries", "LowerSection", c => c.String());
            DropColumn("dbo.SiteGalleries", "Content");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SiteGalleries", "Content", c => c.String());
            DropColumn("dbo.SiteGalleries", "LowerSection");
            DropColumn("dbo.SiteGalleries", "UpperSection");
            DropColumn("dbo.ImageGalleries", "ImageByte");
        }
    }
}
