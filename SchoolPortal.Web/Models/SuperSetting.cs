using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace SchoolPortal.Web.Models
{
    public class SuperSetting
    {
        public long Id { get; set; }

        [Display(Name = "Activate Dashboard")]
        public bool ActivateDashboard { get; set; }

        [Display(Name = "Statistics Dashboard")]
        public bool StatisticsDashboard { get; set; }
        [Display(Name = "Charts Dashboard")]
        public bool ChartsDashboard { get; set; }
        [Display(Name = "Projects Dashboard")]
        public bool ProjectsDashboard { get; set; }
        [Display(Name = "Communication Dashboard")]
        public bool CommunicationDashboard { get; set; }
        [Display(Name = "Users Dashboard")]
        public bool UsersDashboard { get; set; }
        [Display(Name = "Finance Dashboard")]
        public bool FinanceDashboard { get; set; }
        [Display(Name = "Order Dashboard")]
        public bool OrderDashboard { get; set; }

        [Display(Name = "Activate Project on Dashboard")]
        public bool ActivateProject { get; set; }

        [Display(Name = "Maximum Users")]
        public int MaximumUsers { get; set; }

        [Display(Name = "Activate User Management and Role on Dashboard")]
        public bool ActivateUserManagement { get; set; }

        [Display(Name = "Activate User on Dashboard")]
        public bool ActivateUser { get; set; }
        [Display(Name = "Activate Roles on Dashboard")]
        public bool ActivateRoles { get; set; }
        [Display(Name = "Activate Attendance on Dashboard")]
        public bool ActivateAttendance { get; set; }
        [Display(Name = "Activate Report on Dashboard")]
        public bool ActivateReport { get; set; }
        [Display(Name = "Activate Task on Dashboard")]
        public bool ActivateTask { get; set; }
        [Display(Name = "Activate Trainings on Dashboard")]
        public bool ActivateTrainings { get; set; }
        [Display(Name = "Activate Salaries on Dashboard")]
        public bool ActivateSalaries { get; set; }

        [Display(Name = "Activate Job Referrals on Dashboard")]
        public bool ActivateJobReferrals { get; set; }
        [Display(Name = "Activate Finance Management on Dashboard")]
        public bool ActivateFinanceManagement { get; set; }
        [Display(Name = "Activate Payment on Finance Management on Dashboard")]
        public bool ActivatePaymentOnFinanceManagement { get; set; }

        [Display(Name = "Activate Budget on Finance Management on Dashboard")]
        public bool ActivateBudgetOnFinanceManagement { get; set; }
        [Display(Name = "Activate Expenses on Finance Management on Dashboard")]
        public bool ActivateExpensesOnFinanceManagement { get; set; }
        [Display(Name = "Activate Accounts on Finance Management on Dashboard")]
        public bool ActivateAccountsOnFinanceManagement { get; set; }
        [Display(Name = "Activate Report on Finance Management on Dashboard")]
        public bool ActivateReportOnFinanceManagement { get; set; }
        [Display(Name = "Activate Proposal Management on Dashboard")]
        public bool ActivateProposalManagement { get; set; }


        [Display(Name = "Activate Report And Analysis on Dashboard")]
        public bool ActivateReportAndAnalysis { get; set; }
        [Display(Name = "Activate user Report And Analysis on Dashboard")]
        public bool ActivateUserReportAndAnalysis { get; set; }

        [Display(Name = "Activate Visitors Report And Analysis on Dashboard")]
        public bool ActivateVisitorsReportAndAnalysis { get; set; }

        [Display(Name = "Activate Project Report And Analysis on Dashboard")]
        public bool ActivateProjectReportAndAnalysis { get; set; }

        [Display(Name = "Activate Communication Report And Analysis on Dashboard")]
        public bool ActivateCommunicationReportAndAnalysis { get; set; }

        [Display(Name = "Activate Content Report And Analysis on Dashboard")]
        public bool ActivateContentReportAndAnalysis { get; set; }

        [Display(Name = "Activate Content Management on Dashboard")]
        public bool ActivateContentManagement { get; set; }

        [Display(Name = "Activate Pages Content on Dashboard")]
        public bool ActivatePagesContent { get; set; }

        [Display(Name = "Activate Blog Content on Dashboard")]
        public bool ActivateBlogContent { get; set; }
        [Display(Name = "Activate Product Content on Dashboard")]
        public bool ActivateProductContent { get; set; }
        [Display(Name = "Activate FAQ Content on Dashboard")]
        public bool ActivateFaqContent { get; set; }
        [Display(Name = "Activate Testimony Content on Dashboard")]
        public bool ActivateTestimonyContent { get; set; }
        [Display(Name = "Activate Slider Content on Dashboard")]
        public bool ActivateSliderContent { get; set; }
        [Display(Name = "Activate Contact Information Content on Dashboard")]
        public bool ActivateContactInformationContent { get; set; }
        [Display(Name = "Activate Site Map on Dashboard")]
        public bool ActivateSiteMap { get; set; }

        [Display(Name = "Activate Commnunication Tools on Dashboard")]
        public bool ActivateCommnunicationTools { get; set; }

        [Display(Name = "Activate Forum Commnunication Tools on Dashboard")]
        public bool ActivateForumCommnunicationTools { get; set; }

        [Display(Name = "Activate Chat Commnunication Tools on Dashboard")]
        public bool ActivateChatCommnunicationTools { get; set; }

        [Display(Name = "Activate NoticeBoard Commnunication Tools on Dashboard")]
        public bool ActivateNoticeBoardCommnunicationTools { get; set; }
        [Display(Name = "Activate Contact Us Commnunication Tools on Dashboard")]
        public bool ActivateContactUsCommnunicationTools { get; set; }

        [Display(Name = "Activate Management Tools on Dashboard")]
        public bool ActivateManagementTools { get; set; }

        [Display(Name = "Activate SMS Tools on Dashboard")]
        public bool ActivateSmsTools { get; set; }

        [Display(Name = "Activate Email Tools on Dashboard")]
        public bool ActivateEmailTools { get; set; }

        [Display(Name = "Activate Email Tools on Dashboard")]
        public bool ActivateNewsletterTools { get; set; }


        [Display(Name = "Activate Birthday Tools on Dashboard")]
        public bool ActivateBirthdayTools { get; set; }

        [Display(Name = "Activate Settings on Dashboard")]
        public bool ActivateSettings { get; set; }


        [Display(Name = "Activate Order on Dashboard")]
        public bool ActivateOrder { get; set; }
        [Display(Name = "Activate Login Botton on Website")]
        public bool ActivateLogin { get; set; }

         [Display(Name = "Activate Office Activity")]
        public bool ActivateOfficeActivity { get; set; }

        public string TemplateLayoutKey { get; set; }
        public string LoginTemplateKey { get; set; }
        public string ColorTemplateKey { get; set; }
        public string SliderTemplateKey { get; set; }
        public string ProductTemplateKey { get; set; }
        public string BlogTemplateKey { get; set; }
        public string ReloaderTemplateKey { get; set; }
        public string PageHeaderTemplateKey { get; set; }
        public string FooterTemplateKey { get; set; }

        [Display(Name = "Website Title")]
        public string WebsiteTitle { get; set; }

        [Display(Name = "Company Description")]
        public string CompanyDescription { get; set; }

        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Company Website Link")]
        public string CompanyWebsiteLink { get; set; }


        [Display(Name = "Dashboard Title")]
        public string DashboardTitle { get; set; }


        [Display(Name = "Company Logo")]
        public string CompanyLogoUrl { get; set; }
        public string CompanyLogoKey { get; set; }

        [Display(Name = "Company White Logo")]
        public string CompanyWhiteLogoUrl { get; set; }
        public string CompanyWhiteLogoKey { get; set; }

        [Display(Name = "Company Icon")]
        public string CompanyIconUrl { get; set; }
        public string CompanyIconKey { get; set; }


        [Display(Name = "Just Website")]
        public bool JustWebsite { get; set; }


        [Display(Name = "Activate Only Authorized Device")]
        public bool ActivateOnlyAuthorizedDevice { get; set; }


        [Display(Name = "Activate SMS")]
        public bool ActivateSMS { get; set; }


        [Display(Name = "Email Template")]
        public string EmailTemplate { get; set; }


        [Display(Name = "SMS Template")]
        public string SMSTemplate { get; set; }

        [Display(Name = "Login Background Image")]
        public string LoginBackgroundImageUrl { get; set; }
        public string LoginBackgroundImageKey { get; set; }

        [Display(Name = "Use Normal Logo In Login")]
        public bool UseNormalLogoInLogin { get; set; }

        [Display(Name = "Use White Logo In Login")]
        public bool UseWhiteLogoInLogin { get; set; }

        [Display(Name = "Login Title")]
        public string LoginTitle { get; set; }

        [Display(Name = "Login Note Title")]
        public string LoginNoteTitle { get; set; }

        [Display(Name = "Login Note")]
        public string LoginNote { get; set; }



        [Display(Name = "Login Note Footer")]
        public string LoginNoteFooter { get; set; }


        [Display(Name = "Activate Coming Soon")]
        public bool ActivateComingSoon { get; set; }

         




        [Display(Name = "Show Made By Juray")]
        public bool ShowMadeByJuray { get; set; }



        [Display(Name = "Activate Profile Portfolio")]
        public bool ActivateProfilePortfolio { get; set; }



        [Display(Name = "Verify Token Folio")]
        public string VerifyTokenFolio { get; set; }


        [Display(Name = "Portfolio Image One")]
        public string PortfolioImageOneUrl { get; set; }
        public string PortfolioImageOneKey { get; set; }

        [Display(Name = "Portfolio Image Two")]
        public string PortfolioImageTwoUrl { get; set; }
        public string PortfolioImageTwoKey { get; set; }

        [Display(Name = "Portfolio Breacrum Image")]
        public string PortfolioBreacrumImageUrl { get; set; }
        public string PortfolioBreacrumImageKey { get; set; }

         [Display(Name = "Portfolio Title")]
        public string PortfolioTitle { get; set; }

        [Display(Name = "Portfolio Mini Title")]
        public string PortfolioMiniTitle { get; set; }

        [Display(Name = "Portfolio Description")]
        public string PortfolioDescription { get; set; }


         [Display(Name = "Show In Menu Portfolio")]
        public bool ShowInMenuPortfolio { get; set; }

        
         [Display(Name = "Show In Footer Portfolio")]
        public bool ShowInFooterPortfolio { get; set; }
    }
}
