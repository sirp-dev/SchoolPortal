namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uiupdate30 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DataConfigs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LoginCSS = c.String(),
                        LayoutCSS = c.String(),
                        DashboardCSS = c.String(),
                        RedirectTohttpswww = c.Boolean(nullable: false),
                        RedirectTohttps = c.Boolean(nullable: false),
                        Configuration = c.String(),
                        LiveConfiguration = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DataConfigs");
        }
    }
}
