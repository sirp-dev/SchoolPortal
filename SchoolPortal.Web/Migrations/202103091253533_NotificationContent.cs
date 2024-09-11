namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class NotificationContent : DbMigration
    {
        public override void Up()
        {
            Sql("SET IDENTITY_INSERT Notifications ON");
            Sql("INSERT INTO Notifications (Id, Title, Message,ShowModal,ShowMarque) VALUES (1, 'NOTICE! NOTICE!! NOTICE!!!', 'PLEASE ENSURE ALL STUDENTS/PUPILS PRINTS THEIR RESULTS FOR ALL SESSIONS AND TERMS ELSE THEY WILL NOT BE ENROLLED TO THE NEXT CLASS.','"+ true +"','"+ true +"')");
            Sql("SET IDENTITY_INSERT Notifications OFF");
        }

        public override void Down()
        {
        }
    }
}
