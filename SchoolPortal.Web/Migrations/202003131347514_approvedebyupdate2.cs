namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class approvedebyupdate2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Finances", "ApprovedById", "dbo.AspNetUsers");
            DropIndex("dbo.Finances", new[] { "ApprovedById" });
            DropColumn("dbo.Finances", "ApprovedById");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Finances", "ApprovedById", c => c.String(maxLength: 128));
            CreateIndex("dbo.Finances", "ApprovedById");
            AddForeignKey("dbo.Finances", "ApprovedById", "dbo.AspNetUsers", "Id");
        }
    }
}
