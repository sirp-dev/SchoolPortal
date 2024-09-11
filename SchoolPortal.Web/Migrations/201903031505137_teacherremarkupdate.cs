namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class teacherremarkupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Enrollments", "EnrollmentRemark1", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Enrollments", "EnrollmentRemark1");
        }
    }
}
