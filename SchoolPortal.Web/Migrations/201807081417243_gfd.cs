namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gfd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BatchResults",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BatchId = c.String(),
                        StudentRegNumber = c.String(),
                        ProfileId = c.Int(nullable: false),
                        SessionId = c.Int(nullable: false),
                        ClassId = c.Int(nullable: false),
                        EnrollmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BatchResults");
        }
    }
}
