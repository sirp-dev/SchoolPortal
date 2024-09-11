namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Newsletterupdate : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.NewsLetters", "PTAMeetingDate", c => c.String());
            //AddColumn("dbo.NewsLetters", "PTAFee", c => c.String());
        }
        
        public override void Down()
        {
            //DropColumn("dbo.NewsLetters", "PTAFee");
            //DropColumn("dbo.NewsLetters", "PTAMeetingDate");
        }
    }
}
