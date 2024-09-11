namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updaterecord2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DataUserRequests", "Email", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DataUserRequests", "Email");
        }
    }
}
