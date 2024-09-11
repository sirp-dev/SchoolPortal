namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _78iuiieykr : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SiteHomeBodyAfterNews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        Show = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SiteHomeBodyAfterNews");
        }
    }
}
