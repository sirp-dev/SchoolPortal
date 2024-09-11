namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _78iuiie : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SiteNewsPostLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        SortOrder = c.Int(nullable: false),
                        Show = c.Boolean(nullable: false),
                        link = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SiteNewsPosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UpperSection = c.String(),
                        LowerSection = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SiteNewsPosts");
            DropTable("dbo.SiteNewsPostLists");
        }
    }
}
