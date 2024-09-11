namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class settingsvalue : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SettingsValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ValueData = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SettingsValues");
        }
    }
}
