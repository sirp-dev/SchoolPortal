namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class archiveresultstatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sessions", "ArchiveStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sessions", "ArchiveStatus");
        }
    }
}
