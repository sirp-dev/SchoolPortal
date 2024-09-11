namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uiupdate3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.SuperSettings", "ProductTitle");
            DropColumn("dbo.SuperSettings", "ProductTitleHome");
            DropColumn("dbo.SuperSettings", "LoginCSS");
            DropColumn("dbo.SuperSettings", "LayoutCSS");
            DropColumn("dbo.SuperSettings", "DashboardCSS");
            DropColumn("dbo.SuperSettings", "RedirectTohttpswww");
            DropColumn("dbo.SuperSettings", "RedirectTohttps");
            DropColumn("dbo.SuperSettings", "Configuration");
            DropColumn("dbo.SuperSettings", "LiveConfiguration");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SuperSettings", "LiveConfiguration", c => c.String());
            AddColumn("dbo.SuperSettings", "Configuration", c => c.String());
            AddColumn("dbo.SuperSettings", "RedirectTohttps", c => c.Boolean(nullable: false));
            AddColumn("dbo.SuperSettings", "RedirectTohttpswww", c => c.Boolean(nullable: false));
            AddColumn("dbo.SuperSettings", "DashboardCSS", c => c.String());
            AddColumn("dbo.SuperSettings", "LayoutCSS", c => c.String());
            AddColumn("dbo.SuperSettings", "LoginCSS", c => c.String());
            AddColumn("dbo.SuperSettings", "ProductTitleHome", c => c.String());
            AddColumn("dbo.SuperSettings", "ProductTitle", c => c.String());
        }
    }
}
