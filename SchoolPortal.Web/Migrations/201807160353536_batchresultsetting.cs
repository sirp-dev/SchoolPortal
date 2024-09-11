namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class batchresultsetting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "EnableBatchResultPrinting", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "EnableBatchResultPrinting");
        }
    }
}
