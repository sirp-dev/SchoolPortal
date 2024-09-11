namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class affectiveandpsychomotordomain : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AffectiveDomains",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Attentiveness = c.String(),
                        Honesty = c.String(),
                        Neatness = c.String(),
                        Punctuality = c.String(),
                        Relationship = c.String(),
                        EnrolmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PsychomotorDomains",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Drawing = c.String(),
                        Club = c.String(),
                        Painting = c.String(),
                        Handwriting = c.String(),
                        Hobbies = c.String(),
                        Speech = c.String(),
                        Sports = c.String(),
                        EnrolmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PsychomotorDomains");
            DropTable("dbo.AffectiveDomains");
        }
    }
}
