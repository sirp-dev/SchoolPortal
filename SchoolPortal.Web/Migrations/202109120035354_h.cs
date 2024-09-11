namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class h : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PaymentDatas", "StudentProfileId", "dbo.StudentProfiles");
            DropIndex("dbo.PaymentDatas", new[] { "StudentProfileId" });
            AlterColumn("dbo.PaymentDatas", "StudentProfileId", c => c.Int());
            CreateIndex("dbo.PaymentDatas", "StudentProfileId");
            AddForeignKey("dbo.PaymentDatas", "StudentProfileId", "dbo.StudentProfiles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PaymentDatas", "StudentProfileId", "dbo.StudentProfiles");
            DropIndex("dbo.PaymentDatas", new[] { "StudentProfileId" });
            AlterColumn("dbo.PaymentDatas", "StudentProfileId", c => c.Int(nullable: false));
            CreateIndex("dbo.PaymentDatas", "StudentProfileId");
            AddForeignKey("dbo.PaymentDatas", "StudentProfileId", "dbo.StudentProfiles", "Id", cascadeDelete: true);
        }
    }
}
