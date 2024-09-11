namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hdfh : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PaymentDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StudentProfileId = c.Int(nullable: false),
                        SessionId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FinanceSource = c.Int(nullable: false),
                        ApproveId = c.String(),
                        ApproveUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApproveUser_Id)
                .ForeignKey("dbo.Sessions", t => t.SessionId, cascadeDelete: true)
                .ForeignKey("dbo.StudentProfiles", t => t.StudentProfileId, cascadeDelete: true)
                .Index(t => t.StudentProfileId)
                .Index(t => t.SessionId)
                .Index(t => t.ApproveUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PaymentDatas", "StudentProfileId", "dbo.StudentProfiles");
            DropForeignKey("dbo.PaymentDatas", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.PaymentDatas", "ApproveUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.PaymentDatas", new[] { "ApproveUser_Id" });
            DropIndex("dbo.PaymentDatas", new[] { "SessionId" });
            DropIndex("dbo.PaymentDatas", new[] { "StudentProfileId" });
            DropTable("dbo.PaymentDatas");
        }
    }
}
