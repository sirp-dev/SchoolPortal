namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uiupdate38 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "Qoute", c => c.String());
            AddColumn("dbo.Settings", "TopNote", c => c.String());
            AddColumn("dbo.Settings", "GoogleMap", c => c.String());
            AddColumn("dbo.Settings", "ShowAddressOneInTop", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "AddressOne", c => c.String());
            AddColumn("dbo.Settings", "AddressTwo", c => c.String());
            AddColumn("dbo.Settings", "FacebookPage", c => c.String());
            AddColumn("dbo.Settings", "InstagramPage", c => c.String());
            AddColumn("dbo.Settings", "TwitterPage", c => c.String());
            AddColumn("dbo.Settings", "TiktokPage", c => c.String());
            AddColumn("dbo.Settings", "YoutubeChannel", c => c.String());
            AddColumn("dbo.Settings", "EmailOne", c => c.String());
            AddColumn("dbo.Settings", "ShowEmailOneInTop", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "ShowEmailOneInFooter", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "EmailTwo", c => c.String());
            AddColumn("dbo.Settings", "ShowEmailTwoInTop", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "ShowEmailTwoInFooter", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "EmailThree", c => c.String());
            AddColumn("dbo.Settings", "ShowEmailThreeInTop", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "ShowEmailThreeInFooter", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "PhoneOne", c => c.String());
            AddColumn("dbo.Settings", "ShowPhoneOneInTop", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "ShowPhoneOneInFooter", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "PhoneTwo", c => c.String());
            AddColumn("dbo.Settings", "ShowPhoneTwoInTop", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "ShowPhoneTwoInFooter", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "PhoneThree", c => c.String());
            AddColumn("dbo.Settings", "ShowPhoneThreeInTop", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "ShowPhoneThreeInFooter", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "WorkingHour", c => c.String());
            AddColumn("dbo.Settings", "AddFaqToHome", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "AddFaqToFooter", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "AddTestimonyToHome", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "AddTestimonyToFooter", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "AddPartnerToHome", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "DisableMainTopMenu", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "ShowContactUsMenu", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "ShowContactUsFooter", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "DefaultTitleBackgroundUrl", c => c.String());
            AddColumn("dbo.Settings", "DefaultTitleBackgroundKey", c => c.String());
            AddColumn("dbo.Settings", "AddBlogToHome", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "AddBlogToMenu", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "AddBlogToFooter", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "BlogDisplayTitle", c => c.String());
            AddColumn("dbo.Settings", "AddCareerToMenu", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "AddCareerToFooter", c => c.Boolean(nullable: false));
            AddColumn("dbo.Settings", "CareerDisplayTitle", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "CareerDisplayTitle");
            DropColumn("dbo.Settings", "AddCareerToFooter");
            DropColumn("dbo.Settings", "AddCareerToMenu");
            DropColumn("dbo.Settings", "BlogDisplayTitle");
            DropColumn("dbo.Settings", "AddBlogToFooter");
            DropColumn("dbo.Settings", "AddBlogToMenu");
            DropColumn("dbo.Settings", "AddBlogToHome");
            DropColumn("dbo.Settings", "DefaultTitleBackgroundKey");
            DropColumn("dbo.Settings", "DefaultTitleBackgroundUrl");
            DropColumn("dbo.Settings", "ShowContactUsFooter");
            DropColumn("dbo.Settings", "ShowContactUsMenu");
            DropColumn("dbo.Settings", "DisableMainTopMenu");
            DropColumn("dbo.Settings", "AddPartnerToHome");
            DropColumn("dbo.Settings", "AddTestimonyToFooter");
            DropColumn("dbo.Settings", "AddTestimonyToHome");
            DropColumn("dbo.Settings", "AddFaqToFooter");
            DropColumn("dbo.Settings", "AddFaqToHome");
            DropColumn("dbo.Settings", "WorkingHour");
            DropColumn("dbo.Settings", "ShowPhoneThreeInFooter");
            DropColumn("dbo.Settings", "ShowPhoneThreeInTop");
            DropColumn("dbo.Settings", "PhoneThree");
            DropColumn("dbo.Settings", "ShowPhoneTwoInFooter");
            DropColumn("dbo.Settings", "ShowPhoneTwoInTop");
            DropColumn("dbo.Settings", "PhoneTwo");
            DropColumn("dbo.Settings", "ShowPhoneOneInFooter");
            DropColumn("dbo.Settings", "ShowPhoneOneInTop");
            DropColumn("dbo.Settings", "PhoneOne");
            DropColumn("dbo.Settings", "ShowEmailThreeInFooter");
            DropColumn("dbo.Settings", "ShowEmailThreeInTop");
            DropColumn("dbo.Settings", "EmailThree");
            DropColumn("dbo.Settings", "ShowEmailTwoInFooter");
            DropColumn("dbo.Settings", "ShowEmailTwoInTop");
            DropColumn("dbo.Settings", "EmailTwo");
            DropColumn("dbo.Settings", "ShowEmailOneInFooter");
            DropColumn("dbo.Settings", "ShowEmailOneInTop");
            DropColumn("dbo.Settings", "EmailOne");
            DropColumn("dbo.Settings", "YoutubeChannel");
            DropColumn("dbo.Settings", "TiktokPage");
            DropColumn("dbo.Settings", "TwitterPage");
            DropColumn("dbo.Settings", "InstagramPage");
            DropColumn("dbo.Settings", "FacebookPage");
            DropColumn("dbo.Settings", "AddressTwo");
            DropColumn("dbo.Settings", "AddressOne");
            DropColumn("dbo.Settings", "ShowAddressOneInTop");
            DropColumn("dbo.Settings", "GoogleMap");
            DropColumn("dbo.Settings", "TopNote");
            DropColumn("dbo.Settings", "Qoute");
        }
    }
}
