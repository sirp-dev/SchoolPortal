namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Newsletter : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NewsLetters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NextTResumptionDate = c.String(),
                        GenRemark = c.String(),
                        SessionId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sessions", t => t.SessionId)
                .Index(t => t.SessionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NewsLetters", "SessionId", "dbo.Sessions");
            DropIndex("dbo.NewsLetters", new[] { "SessionId" });
            DropTable("dbo.NewsLetters");
        }
    }
}
