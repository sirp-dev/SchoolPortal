namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class paymentcallbackurl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "PaymentCallBackUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "PaymentCallBackUrl");
        }
    }
}
