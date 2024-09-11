namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class configs : DbMigration
    {
        public override void Up()
        {
            // Sql("SET IDENTITY_INSERT DataConfigs ON");
            //Sql("INSERT INTO DataConfigs (Id, LoginCSS, LayoutCSS, DashboardCSS, RedirectTohttpswww, RedirectTohttps, Configuration, LiveConfiguration) VALUES (1, '<>', '<>', '<>', 0, 0, '<>', '<>')");
            //Sql("SET IDENTITY_INSERT DataConfigs OFF");
        }
        
        public override void Down()
        {
        }
    }
}
