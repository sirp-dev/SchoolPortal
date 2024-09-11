namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class onlineschoolnew : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.LiveClassOnlines", new[] { "Users_Id" });
            DropColumn("dbo.LiveClassOnlines", "UserId");
            RenameColumn(table: "dbo.LiveClassOnlines", name: "Users_Id", newName: "UserId");
            AlterColumn("dbo.LiveClassOnlines", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.LiveClassOnlines", "UserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.LiveClassOnlines", new[] { "UserId" });
            AlterColumn("dbo.LiveClassOnlines", "UserId", c => c.String());
            RenameColumn(table: "dbo.LiveClassOnlines", name: "UserId", newName: "Users_Id");
            AddColumn("dbo.LiveClassOnlines", "UserId", c => c.String());
            CreateIndex("dbo.LiveClassOnlines", "Users_Id");
        }
    }
}
