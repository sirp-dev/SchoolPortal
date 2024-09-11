namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class resultarchivesupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NewsLetterArchives", "NewsLetterId", c => c.Int());
            AddColumn("dbo.PrincipalArchives", "PrincipalId", c => c.Int());
            AddColumn("dbo.SchoolFeesArchives", "SchoolFeesId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SchoolFeesArchives", "SchoolFeesId");
            DropColumn("dbo.PrincipalArchives", "PrincipalId");
            DropColumn("dbo.NewsLetterArchives", "NewsLetterId");
        }
    }
}
