namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class admissionupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "AdmissionPinOption", c => c.Int(nullable: false, defaultValue: 1));
            AddColumn("dbo.StudentDatas", "EmailAddress", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StudentDatas", "EmailAddress");
            DropColumn("dbo.Settings", "AdmissionPinOption");
        }
    }
}
