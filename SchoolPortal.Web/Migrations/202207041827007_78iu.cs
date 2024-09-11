namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _78iu : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SiteBreadCrumbs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        Show = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SiteContacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        Show = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SiteFooterJS",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        Show = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SiteFooters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        Show = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SiteGalleries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        Show = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SiteGalleryLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        Show = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SiteHeaders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        Show = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SiteHomeBodies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        Show = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SiteNavHeaders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        Show = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SiteOverrideCSSes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        Show = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SitePageCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Show = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SitePages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        Show = c.Boolean(nullable: false),
                        MainPage = c.Boolean(nullable: false),
                        SubPage = c.Boolean(nullable: false),
                        FooterPage = c.Boolean(nullable: false),
                        SortOrder = c.Int(nullable: false),
                        SitePageCategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SitePageCategories", t => t.SitePageCategoryId, cascadeDelete: true)
                .Index(t => t.SitePageCategoryId);
            
            CreateTable(
                "dbo.SiteSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Show = c.Boolean(nullable: false),
                        EmailOne = c.String(),
                        EmailTwo = c.String(),
                        EmailThree = c.String(),
                        PhoneOne = c.String(),
                        PhoneTwo = c.String(),
                        PhoneThree = c.String(),
                        AddressOne = c.String(),
                        AddressTwo = c.String(),
                        AddressThree = c.String(),
                        Host = c.String(),
                        PX = c.String(),
                        Sender = c.String(),
                        Receiver = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SiteSliders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        Show = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ImageGalleries", "Show", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SitePages", "SitePageCategoryId", "dbo.SitePageCategories");
            DropIndex("dbo.SitePages", new[] { "SitePageCategoryId" });
            DropColumn("dbo.ImageGalleries", "Show");
            DropTable("dbo.SiteSliders");
            DropTable("dbo.SiteSettings");
            DropTable("dbo.SitePages");
            DropTable("dbo.SitePageCategories");
            DropTable("dbo.SiteOverrideCSSes");
            DropTable("dbo.SiteNavHeaders");
            DropTable("dbo.SiteHomeBodies");
            DropTable("dbo.SiteHeaders");
            DropTable("dbo.SiteGalleryLists");
            DropTable("dbo.SiteGalleries");
            DropTable("dbo.SiteFooters");
            DropTable("dbo.SiteFooterJS");
            DropTable("dbo.SiteContacts");
            DropTable("dbo.SiteBreadCrumbs");
        }
    }
}
