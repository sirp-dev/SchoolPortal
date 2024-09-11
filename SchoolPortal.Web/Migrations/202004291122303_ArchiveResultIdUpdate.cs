namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ArchiveResultIdUpdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ArchiveResults", "SessionId", c => c.Int());
            AlterColumn("dbo.ArchiveResults", "ClassId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ArchiveResults", "ClassId", c => c.Int(nullable: false));
            AlterColumn("dbo.ArchiveResults", "SessionId", c => c.Int(nullable: false));
        }
    }
}
