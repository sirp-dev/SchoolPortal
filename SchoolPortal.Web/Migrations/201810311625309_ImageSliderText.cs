namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImageSliderText : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ImageSliders", "TextOne", c => c.String());
            AddColumn("dbo.ImageSliders", "TextTwo", c => c.String());
            AddColumn("dbo.ImageSliders", "TextThree", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ImageSliders", "TextThree");
            DropColumn("dbo.ImageSliders", "TextTwo");
            DropColumn("dbo.ImageSliders", "TextOne");
        }
    }
}
