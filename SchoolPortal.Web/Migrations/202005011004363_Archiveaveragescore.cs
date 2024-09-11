namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Archiveaveragescore : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Enrollments", "ArchiveAverageScore", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Enrollments", "ArchiveAverageScore");
        }
    }
}
