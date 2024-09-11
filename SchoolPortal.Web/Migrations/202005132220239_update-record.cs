namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updaterecord : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DataUserRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        DateOfBirth = c.String(),
                        ParentName = c.String(),
                        StudentsPhoneNumber = c.String(),
                        ParentsPhoneNumber = c.String(),
                        ParentsOccupation = c.String(),
                        ClassName = c.String(),
                        FormTeacher = c.String(),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DataUserRequests");
        }
    }
}
