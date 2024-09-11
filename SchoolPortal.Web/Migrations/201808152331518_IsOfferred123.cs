namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsOfferred123 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EnrolledSubjects", "IsOffered", c => c.Boolean(nullable: false));
            DropColumn("dbo.Settings", "JssGradingOption");
            DropColumn("dbo.Settings", "SssGradingOption");
            DropColumn("dbo.Settings", "UseCumulativePositionForThirdTerm");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Settings", "UseCumulativePositionForThirdTerm", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "SssGradingOption", c => c.Int(nullable: false));
            AddColumn("dbo.Settings", "JssGradingOption", c => c.Int(nullable: false));
            DropColumn("dbo.EnrolledSubjects", "IsOffered");
        }
    }
}
