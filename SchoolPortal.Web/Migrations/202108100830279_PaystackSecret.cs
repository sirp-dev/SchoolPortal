namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PaystackSecret : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "PaystackSecretKey", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "PaystackSecretKey");
        }
    }
}
