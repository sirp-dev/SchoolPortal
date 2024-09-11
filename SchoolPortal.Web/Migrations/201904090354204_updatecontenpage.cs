namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecontenpage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CategoryContentPages", "ContentHome", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CategoryContentPages", "ContentHome");
        }
    }
}
