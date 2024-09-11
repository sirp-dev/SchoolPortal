namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uiupdate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BlogCategories",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Title = c.String(),
                        Publish = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Blogs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Title = c.String(),
                        Date = c.DateTime(nullable: false),
                        VideoUrl = c.String(),
                        VideoKey = c.String(),
                        Publish = c.Boolean(nullable: false),
                        SortOrder = c.Int(nullable: false),
                        ImageUrl = c.String(),
                        ImageKey = c.String(),
                        BlogCategoryId = c.Long(),
                        Description = c.String(),
                        FullDescription = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BlogCategories", t => t.BlogCategoryId)
                .Index(t => t.BlogCategoryId);
            
            CreateTable(
                "dbo.FAQs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Message = c.String(),
                        SortOrder = c.Int(nullable: false),
                        Show = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PageSectionLists",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        VideoUrl = c.String(),
                        VideoKey = c.String(),
                        ImageUrl = c.String(),
                        ImageKey = c.String(),
                        IconText = c.String(),
                        SortOrder = c.Int(nullable: false),
                        Title = c.String(),
                        MiniTitle = c.String(),
                        Description = c.String(),
                        MoreDescription = c.String(),
                        ButtonText = c.String(),
                        ButtonLink = c.String(),
                        DirectUrl = c.String(),
                        Disable = c.Boolean(nullable: false),
                        PageSectionId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PageSections", t => t.PageSectionId)
                .Index(t => t.PageSectionId);
            
            CreateTable(
                "dbo.PageSections",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        VideoUrl = c.String(),
                        VideoKey = c.String(),
                        ImageUrl = c.String(),
                        ImageKey = c.String(),
                        SecondImageUrl = c.String(),
                        SecondImageKey = c.String(),
                        YoutubeUrlLink = c.String(),
                        Title = c.String(),
                        MiniTitle = c.String(),
                        Qoute = c.String(),
                        Description = c.String(),
                        FullDescription = c.String(),
                        ButtonText = c.String(),
                        ButtonLink = c.String(),
                        DirectUrl = c.String(),
                        ShowInHome = c.Boolean(nullable: false),
                        DisableButton = c.Boolean(nullable: false),
                        Disable = c.Boolean(nullable: false),
                        FixedAfterFooter = c.Boolean(nullable: false),
                        HomePageSortOrder = c.Int(nullable: false),
                        HomeSortFrom = c.Int(nullable: false),
                        PageSortOrder = c.Int(nullable: false),
                        WebPageId = c.Long(),
                        TemplateKey = c.String(),
                        CustomClass = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WebPages", t => t.WebPageId)
                .Index(t => t.WebPageId);
            
            CreateTable(
                "dbo.WebPages",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Title = c.String(),
                        SortOrder = c.Int(nullable: false),
                        Publish = c.Boolean(nullable: false),
                        SecurityPage = c.Boolean(nullable: false),
                        PageCategoryId = c.Long(),
                        ImageUrl = c.String(),
                        ImageKey = c.String(),
                        ShowInMainTop = c.Boolean(nullable: false),
                        ShowInMenuDropDown = c.Boolean(nullable: false),
                        ShowInMainMenu = c.Boolean(nullable: false),
                        ShowInFooter = c.Boolean(nullable: false),
                        EnableDirectUrl = c.Boolean(nullable: false),
                        DirectUrl = c.String(),
                        HomeSortFrom = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PageCategories", t => t.PageCategoryId)
                .Index(t => t.PageCategoryId);
            
            CreateTable(
                "dbo.PageCategories",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Title = c.String(),
                        Publish = c.Boolean(nullable: false),
                        MenuSortOrder = c.Int(nullable: false),
                        HomeSortFrom = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PostModals",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ImageUrl = c.String(),
                        ImageKey = c.String(),
                        Title = c.String(),
                        Description = c.String(),
                        ModalTime = c.Int(nullable: false),
                        ModalOccurance = c.Int(nullable: false),
                        StartTime = c.DateTime(),
                        StopTime = c.DateTime(),
                        ShowOnlyImage = c.Boolean(nullable: false),
                        ShowTitle = c.Boolean(nullable: false),
                        ShowDescription = c.Boolean(nullable: false),
                        ShowImage = c.Boolean(nullable: false),
                        Publish = c.Boolean(nullable: false),
                        ButtonLink = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sliders",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Url = c.String(),
                        Key = c.String(),
                        SecondUrl = c.String(),
                        SecondKey = c.String(),
                        YoutubeVideo = c.String(),
                        IsVideo = c.Boolean(nullable: false),
                        SortOrder = c.Int(nullable: false),
                        Show = c.Boolean(nullable: false),
                        Title = c.String(),
                        MiniTitle = c.String(),
                        Text = c.String(),
                        ButtonText = c.String(),
                        ButtonLink = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SuperSettings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ActivateDashboard = c.Boolean(nullable: false),
                        StatisticsDashboard = c.Boolean(nullable: false),
                        ChartsDashboard = c.Boolean(nullable: false),
                        ProjectsDashboard = c.Boolean(nullable: false),
                        CommunicationDashboard = c.Boolean(nullable: false),
                        UsersDashboard = c.Boolean(nullable: false),
                        FinanceDashboard = c.Boolean(nullable: false),
                        OrderDashboard = c.Boolean(nullable: false),
                        ActivateProject = c.Boolean(nullable: false),
                        MaximumUsers = c.Int(nullable: false),
                        ActivateUserManagement = c.Boolean(nullable: false),
                        ActivateUser = c.Boolean(nullable: false),
                        ActivateRoles = c.Boolean(nullable: false),
                        ActivateAttendance = c.Boolean(nullable: false),
                        ActivateReport = c.Boolean(nullable: false),
                        ActivateTask = c.Boolean(nullable: false),
                        ActivateTrainings = c.Boolean(nullable: false),
                        ActivateSalaries = c.Boolean(nullable: false),
                        ActivateJobReferrals = c.Boolean(nullable: false),
                        ActivateFinanceManagement = c.Boolean(nullable: false),
                        ActivatePaymentOnFinanceManagement = c.Boolean(nullable: false),
                        ActivateBudgetOnFinanceManagement = c.Boolean(nullable: false),
                        ActivateExpensesOnFinanceManagement = c.Boolean(nullable: false),
                        ActivateAccountsOnFinanceManagement = c.Boolean(nullable: false),
                        ActivateReportOnFinanceManagement = c.Boolean(nullable: false),
                        ActivateProposalManagement = c.Boolean(nullable: false),
                        ActivateReportAndAnalysis = c.Boolean(nullable: false),
                        ActivateUserReportAndAnalysis = c.Boolean(nullable: false),
                        ActivateVisitorsReportAndAnalysis = c.Boolean(nullable: false),
                        ActivateProjectReportAndAnalysis = c.Boolean(nullable: false),
                        ActivateCommunicationReportAndAnalysis = c.Boolean(nullable: false),
                        ActivateContentReportAndAnalysis = c.Boolean(nullable: false),
                        ActivateContentManagement = c.Boolean(nullable: false),
                        ActivatePagesContent = c.Boolean(nullable: false),
                        ActivateBlogContent = c.Boolean(nullable: false),
                        ActivateProductContent = c.Boolean(nullable: false),
                        ActivateFaqContent = c.Boolean(nullable: false),
                        ActivateTestimonyContent = c.Boolean(nullable: false),
                        ActivateSliderContent = c.Boolean(nullable: false),
                        ActivateContactInformationContent = c.Boolean(nullable: false),
                        ActivateSiteMap = c.Boolean(nullable: false),
                        ActivateCommnunicationTools = c.Boolean(nullable: false),
                        ActivateForumCommnunicationTools = c.Boolean(nullable: false),
                        ActivateChatCommnunicationTools = c.Boolean(nullable: false),
                        ActivateNoticeBoardCommnunicationTools = c.Boolean(nullable: false),
                        ActivateContactUsCommnunicationTools = c.Boolean(nullable: false),
                        ActivateManagementTools = c.Boolean(nullable: false),
                        ActivateSmsTools = c.Boolean(nullable: false),
                        ActivateEmailTools = c.Boolean(nullable: false),
                        ActivateNewsletterTools = c.Boolean(nullable: false),
                        ActivateBirthdayTools = c.Boolean(nullable: false),
                        ActivateSettings = c.Boolean(nullable: false),
                        ActivateOrder = c.Boolean(nullable: false),
                        ActivateLogin = c.Boolean(nullable: false),
                        ActivateOfficeActivity = c.Boolean(nullable: false),
                        TemplateLayoutKey = c.String(),
                        LoginTemplateKey = c.String(),
                        ColorTemplateKey = c.String(),
                        SliderTemplateKey = c.String(),
                        ProductTemplateKey = c.String(),
                        BlogTemplateKey = c.String(),
                        ReloaderTemplateKey = c.String(),
                        PageHeaderTemplateKey = c.String(),
                        FooterTemplateKey = c.String(),
                        WebsiteTitle = c.String(),
                        CompanyDescription = c.String(),
                        CompanyName = c.String(),
                        CompanyWebsiteLink = c.String(),
                        DashboardTitle = c.String(),
                        CompanyLogoUrl = c.String(),
                        CompanyLogoKey = c.String(),
                        CompanyWhiteLogoUrl = c.String(),
                        CompanyWhiteLogoKey = c.String(),
                        CompanyIconUrl = c.String(),
                        CompanyIconKey = c.String(),
                        JustWebsite = c.Boolean(nullable: false),
                        ActivateOnlyAuthorizedDevice = c.Boolean(nullable: false),
                        ActivateSMS = c.Boolean(nullable: false),
                        EmailTemplate = c.String(),
                        SMSTemplate = c.String(),
                        LoginBackgroundImageUrl = c.String(),
                        LoginBackgroundImageKey = c.String(),
                        UseNormalLogoInLogin = c.Boolean(nullable: false),
                        UseWhiteLogoInLogin = c.Boolean(nullable: false),
                        LoginTitle = c.String(),
                        LoginNoteTitle = c.String(),
                        LoginNote = c.String(),
                        LoginNoteFooter = c.String(),
                        ActivateComingSoon = c.Boolean(nullable: false),
                        ProductTitle = c.String(),
                        ProductTitleHome = c.String(),
                        LoginCSS = c.String(),
                        LayoutCSS = c.String(),
                        DashboardCSS = c.String(),
                        RedirectTohttpswww = c.Boolean(nullable: false),
                        RedirectTohttps = c.Boolean(nullable: false),
                        Configuration = c.String(),
                        LiveConfiguration = c.String(),
                        ShowMadeByJuray = c.Boolean(nullable: false),
                        ActivateProfilePortfolio = c.Boolean(nullable: false),
                        VerifyTokenFolio = c.String(),
                        PortfolioImageOneUrl = c.String(),
                        PortfolioImageOneKey = c.String(),
                        PortfolioImageTwoUrl = c.String(),
                        PortfolioImageTwoKey = c.String(),
                        PortfolioBreacrumImageUrl = c.String(),
                        PortfolioBreacrumImageKey = c.String(),
                        PortfolioTitle = c.String(),
                        PortfolioMiniTitle = c.String(),
                        PortfolioDescription = c.String(),
                        ShowInMenuPortfolio = c.Boolean(nullable: false),
                        ShowInFooterPortfolio = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Testimonies",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Title = c.String(),
                        SortOrder = c.String(),
                        Show = c.String(),
                        ImageUrl = c.String(),
                        ImageKey = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PageSections", "WebPageId", "dbo.WebPages");
            DropForeignKey("dbo.WebPages", "PageCategoryId", "dbo.PageCategories");
            DropForeignKey("dbo.PageSectionLists", "PageSectionId", "dbo.PageSections");
            DropForeignKey("dbo.Blogs", "BlogCategoryId", "dbo.BlogCategories");
            DropIndex("dbo.WebPages", new[] { "PageCategoryId" });
            DropIndex("dbo.PageSections", new[] { "WebPageId" });
            DropIndex("dbo.PageSectionLists", new[] { "PageSectionId" });
            DropIndex("dbo.Blogs", new[] { "BlogCategoryId" });
            DropTable("dbo.Testimonies");
            DropTable("dbo.SuperSettings");
            DropTable("dbo.Sliders");
            DropTable("dbo.PostModals");
            DropTable("dbo.PageCategories");
            DropTable("dbo.WebPages");
            DropTable("dbo.PageSections");
            DropTable("dbo.PageSectionLists");
            DropTable("dbo.FAQs");
            DropTable("dbo.Blogs");
            DropTable("dbo.BlogCategories");
        }
    }
}
