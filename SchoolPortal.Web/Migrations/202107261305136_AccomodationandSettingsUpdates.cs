namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccomodationandSettingsUpdates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EnrolledHostelBeds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HostelBedId = c.Int(),
                        BedNo = c.String(),
                        EnrolledHostelRoomId = c.Int(),
                        EnrolledHostelId = c.Int(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EnrolledHostelRooms", t => t.EnrolledHostelRoomId)
                .ForeignKey("dbo.EnrolledHostels", t => t.EnrolledHostelId)
                .ForeignKey("dbo.HostelBeds", t => t.HostelBedId)
                .Index(t => t.HostelBedId)
                .Index(t => t.EnrolledHostelRoomId)
                .Index(t => t.EnrolledHostelId);
            
            CreateTable(
                "dbo.EnrolledHostels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        HostelId = c.Int(),
                        HostelType = c.String(),
                        Status = c.Int(nullable: false),
                        SessionId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Hostels", t => t.HostelId)
                .ForeignKey("dbo.Sessions", t => t.SessionId)
                .Index(t => t.HostelId)
                .Index(t => t.SessionId);
            
            CreateTable(
                "dbo.EnrolledHostelRooms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HostelRoomId = c.Int(),
                        Name = c.String(),
                        RoomNo = c.String(),
                        NoOfStudent = c.Int(),
                        EnrolledHostelId = c.Int(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EnrolledHostels", t => t.EnrolledHostelId)
                .ForeignKey("dbo.HostelRooms", t => t.HostelRoomId)
                .Index(t => t.HostelRoomId)
                .Index(t => t.EnrolledHostelId);
            
            AddColumn("dbo.Settings", "EnableHostel", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.Settings", "EnableFinance", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EnrolledHostelBeds", "HostelBedId", "dbo.HostelBeds");
            DropForeignKey("dbo.EnrolledHostelBeds", "EnrolledHostelId", "dbo.EnrolledHostels");
            DropForeignKey("dbo.EnrolledHostels", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.EnrolledHostels", "HostelId", "dbo.Hostels");
            DropForeignKey("dbo.EnrolledHostelRooms", "HostelRoomId", "dbo.HostelRooms");
            DropForeignKey("dbo.EnrolledHostelBeds", "EnrolledHostelRoomId", "dbo.EnrolledHostelRooms");
            DropForeignKey("dbo.EnrolledHostelRooms", "EnrolledHostelId", "dbo.EnrolledHostels");
            DropIndex("dbo.EnrolledHostelRooms", new[] { "EnrolledHostelId" });
            DropIndex("dbo.EnrolledHostelRooms", new[] { "HostelRoomId" });
            DropIndex("dbo.EnrolledHostels", new[] { "SessionId" });
            DropIndex("dbo.EnrolledHostels", new[] { "HostelId" });
            DropIndex("dbo.EnrolledHostelBeds", new[] { "EnrolledHostelId" });
            DropIndex("dbo.EnrolledHostelBeds", new[] { "EnrolledHostelRoomId" });
            DropIndex("dbo.EnrolledHostelBeds", new[] { "HostelBedId" });
            DropColumn("dbo.Settings", "EnableFinance");
            DropColumn("dbo.Settings", "EnableHostel");
            DropTable("dbo.EnrolledHostelRooms");
            DropTable("dbo.EnrolledHostels");
            DropTable("dbo.EnrolledHostelBeds");
        }
    }
}
