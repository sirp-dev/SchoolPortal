namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "IsPrimaryNursery", c => c.Boolean(nullable: false));
            AddColumn("dbo.TimeTables", "Time6_7", c => c.String());
            AddColumn("dbo.TimeTables", "Time7_8", c => c.String());
            AddColumn("dbo.TimeTables", "Time8_9", c => c.String());
            AddColumn("dbo.TimeTables", "Time9_10", c => c.String());
            AddColumn("dbo.TimeTables", "Time10_11", c => c.String());
            AddColumn("dbo.TimeTables", "Time11_12", c => c.String());
            AddColumn("dbo.TimeTables", "Time12_13", c => c.String());
            AddColumn("dbo.TimeTables", "Time13_14", c => c.String());
            AddColumn("dbo.TimeTables", "Time14_15", c => c.String());
            AddColumn("dbo.TimeTables", "Time15_16", c => c.String());
            AddColumn("dbo.TimeTables", "Time16_17", c => c.String());
            AddColumn("dbo.TimeTables", "Time17_18", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TimeTables", "Time17_18");
            DropColumn("dbo.TimeTables", "Time16_17");
            DropColumn("dbo.TimeTables", "Time15_16");
            DropColumn("dbo.TimeTables", "Time14_15");
            DropColumn("dbo.TimeTables", "Time13_14");
            DropColumn("dbo.TimeTables", "Time12_13");
            DropColumn("dbo.TimeTables", "Time11_12");
            DropColumn("dbo.TimeTables", "Time10_11");
            DropColumn("dbo.TimeTables", "Time9_10");
            DropColumn("dbo.TimeTables", "Time8_9");
            DropColumn("dbo.TimeTables", "Time7_8");
            DropColumn("dbo.TimeTables", "Time6_7");
            DropColumn("dbo.Settings", "IsPrimaryNursery");
        }
    }
}
