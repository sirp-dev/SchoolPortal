namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class approvedeby : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Finances", "ApprovedById", c => c.String(maxLength: 128));
            CreateIndex("dbo.Finances", "ApprovedById");
            AddForeignKey("dbo.Finances", "ApprovedById", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.Finances", "ApprovedBy");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Finances", "ApprovedBy", c => c.String());
            DropForeignKey("dbo.Finances", "ApprovedById", "dbo.AspNetUsers");
            DropIndex("dbo.Finances", new[] { "ApprovedById" });
            DropColumn("dbo.Finances", "ApprovedById");
        }
    }
}
