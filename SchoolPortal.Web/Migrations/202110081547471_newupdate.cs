namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Finances", "TellerNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Finances", "TellerNumber");
        }
    }
}
