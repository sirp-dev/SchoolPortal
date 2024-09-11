namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class accomodation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HostelAllotments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        HostelBedId = c.Int(),
                        HostelRoomId = c.Int(),
                        HostelId = c.Int(),
                        SessionId = c.Int(),
                        AllotedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Hostels", t => t.HostelId)
                .ForeignKey("dbo.HostelBeds", t => t.HostelBedId)
                .ForeignKey("dbo.HostelRooms", t => t.HostelRoomId)
                .ForeignKey("dbo.Sessions", t => t.SessionId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.HostelBedId)
                .Index(t => t.HostelRoomId)
                .Index(t => t.HostelId)
                .Index(t => t.SessionId);
            
            CreateTable(
                "dbo.Hostels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        HostelType = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.HostelRooms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        RoomNo = c.String(),
                        NoOfStudent = c.Int(),
                        HostelId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Hostels", t => t.HostelId)
                .Index(t => t.HostelId);
            
            CreateTable(
                "dbo.HostelBeds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BedNo = c.String(),
                        HostelRoomId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.HostelRooms", t => t.HostelRoomId)
                .Index(t => t.HostelRoomId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HostelAllotments", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.HostelAllotments", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.HostelAllotments", "HostelRoomId", "dbo.HostelRooms");
            DropForeignKey("dbo.HostelAllotments", "HostelBedId", "dbo.HostelBeds");
            DropForeignKey("dbo.HostelAllotments", "HostelId", "dbo.Hostels");
            DropForeignKey("dbo.HostelBeds", "HostelRoomId", "dbo.HostelRooms");
            DropForeignKey("dbo.HostelRooms", "HostelId", "dbo.Hostels");
            DropIndex("dbo.HostelBeds", new[] { "HostelRoomId" });
            DropIndex("dbo.HostelRooms", new[] { "HostelId" });
            DropIndex("dbo.HostelAllotments", new[] { "SessionId" });
            DropIndex("dbo.HostelAllotments", new[] { "HostelId" });
            DropIndex("dbo.HostelAllotments", new[] { "HostelRoomId" });
            DropIndex("dbo.HostelAllotments", new[] { "HostelBedId" });
            DropIndex("dbo.HostelAllotments", new[] { "UserId" });
            DropTable("dbo.HostelBeds");
            DropTable("dbo.HostelRooms");
            DropTable("dbo.Hostels");
            DropTable("dbo.HostelAllotments");
        }
    }
}
