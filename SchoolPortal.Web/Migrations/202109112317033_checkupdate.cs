namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class checkupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PaymentDatas", "UniqueId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PaymentDatas", "UniqueId");
        }
    }
}
