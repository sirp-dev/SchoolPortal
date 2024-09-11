namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class websitemanager : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CategoryContentPages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        SortOrder = c.Int(nullable: false),
                        ShowInHome = c.Boolean(nullable: false),
                        Content = c.String(),
                        CategoryPageId = c.Int(nullable: false),
                        MetaTage = c.String(),
                        MetaDescription = c.String(),
                        MetaKeyword = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CategoryPages", t => t.CategoryPageId, cascadeDelete: true)
                .Index(t => t.CategoryPageId);
            
            CreateTable(
                "dbo.CategoryPages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MenuDescription = c.Int(nullable: false),
                        Title = c.String(),
                        SortOrder = c.Int(nullable: false),
                        Content = c.String(),
                        MetaTage = c.String(),
                        MetaDescription = c.String(),
                        MetaKeyword = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.WebsiteSettings", "Footer", c => c.Int(nullable: false));
            AddColumn("dbo.WebsiteSettings", "Menu", c => c.Int(nullable: false));
            AddColumn("dbo.WebsiteSettings", "Blog", c => c.Int(nullable: false));
            AddColumn("dbo.WebsiteSettings", "BlogDetails", c => c.Int(nullable: false));
          
        }
        
        public override void Down()
        {
           
            DropForeignKey("dbo.CategoryContentPages", "CategoryPageId", "dbo.CategoryPages");
            DropIndex("dbo.CategoryContentPages", new[] { "CategoryPageId" });
            DropColumn("dbo.WebsiteSettings", "BlogDetails");
            DropColumn("dbo.WebsiteSettings", "Blog");
            DropColumn("dbo.WebsiteSettings", "Menu");
            DropColumn("dbo.WebsiteSettings", "Footer");
            DropTable("dbo.CategoryPages");
            DropTable("dbo.CategoryContentPages");
        }
    }
}
