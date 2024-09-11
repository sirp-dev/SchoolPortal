namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finance20495k : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Finances", "Payall", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Finances", "Payall");
        }
    }
}
