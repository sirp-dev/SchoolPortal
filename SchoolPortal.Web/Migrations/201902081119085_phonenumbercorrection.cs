namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class phonenumbercorrection : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ContactUs", "PhoneNumber", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ContactUs", "PhoneNumber", c => c.Int(nullable: false));
        }
    }
}
