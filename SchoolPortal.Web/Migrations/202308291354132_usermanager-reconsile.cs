namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usermanagerreconsile : DbMigration
    {
        public override void Up()
        {
              AddColumn("dbo.AspNetUsers", "ConcurrencyStamp", c => c.String());
              AddColumn("dbo.AspNetUsers", "LockoutEnd", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "NormalizedEmail", c => c.String());
            AddColumn("dbo.AspNetUsers", "NormalizedUserName", c => c.String());
        }
        
        public override void Down()
        {
        }
    }
}
