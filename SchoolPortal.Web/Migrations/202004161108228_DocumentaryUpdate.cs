namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentaryUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documentaries", "Title", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Documentaries", "Title");
        }
    }
}
