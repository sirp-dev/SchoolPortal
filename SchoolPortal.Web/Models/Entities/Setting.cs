using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SchoolPortal.Web.Models.Entities
{
    public class Setting
    {
        public Setting()
        {
            //Passmark = 50;
            PromotionByTrial = 0;
            ShowPositionOnResult = true;
            ShowCumulativeResultForThirdTerm = false;
            SslEnabled = false;
            ShowFeesOnResult = false;
            ShowAccctOnResult = false;
            AdmissionPinOption = AdmissionPinOption.UsedPin;
            PinValidOption = PinValidOption.Termly;
            PrintOutOption = PrintOutOption.PrintOutOne;
            EnablePreviewResult = true;

        }

        public int Id { get; set; }

        [Display(Name="School Name")]
        public string SchoolName { get; set; }

        [Display(Name = "Short Name")]
        public string SchoolInitials { get; set; }

        [Display(Name = "School Address")]
        public string SchoolAddress { get; set; }

        [Display(Name = "School Address 2")]
        public string SchoolAddress2 { get; set; }

        [Display(Name = "State Located")]
        public string State { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Contact Mail")]
        public string ContactEmail { get; set; }

        [Display(Name = "Contact Phone Number")]
        public string ContactPhoneNumber { get; set; }

        public Nullable<int> ImageId { get; set; }


        //Sms Settings
        [Display(Name = "SMS Username")]
        public string SmsUsername { get; set; }

        [Display(Name = "SMS Password")]
        public string SmsPassword { get; set; }

        //Academic and Result Settings

        public decimal Passmark { get; set; }

        [Display(Name = "Promote All")]
        public bool PromoteAll { get; set; }

        [Display(Name = "Mark for Promotion on Trial")]
        public decimal PromotionByTrial { get; set; }

        [Display(Name = "Show Position on Result")]
        public bool ShowPositionOnResult { get; set; }

        [Display(Name = "Show School Fees on Result")]
        public bool ShowFeesOnResult { get; set; }

        [Display(Name = "Show School Account on Result")]
        public bool ShowAccctOnResult { get; set; }

        [Display(Name = "Show Cumulative Result for Third Term")]
        public bool ShowCumulativeResultForThirdTerm { get; set; }
        

        //Mail Settings
        [Display(Name = "Email")]
        public string EmailFrom { get; set; }

        [Display(Name = "Host")]
        public string MailHost { get; set; }

        public string Port { get; set; }

        [Display(Name = "Username")]
        public string MailUsername { get; set; }

        [Display(Name = "Password")]
        public string MailPassword { get; set; }

        [Display(Name = "SSL Enabled")]
        public bool SslEnabled { get; set; }

        [Display(Name = "Test or Assessment Total Score")]
        public decimal AccessmentScore { get; set; }

        [Display(Name = "Exam Total Score")]
        public decimal ExamScore { get; set; }


        [Display(Name = "2nd Test Total Score")]
        public decimal TestScore2 { get; set; }

        [Display(Name = "Project Total Score")]
        public decimal Project { get; set; }

        [Display(Name = "Class Exercise Total Score")]
        public decimal ClassExercise { get; set; }

        [Display(Name = "Assessment Total Score")]
        public decimal Assessment { get; set; }

        //admission screening settings

        [Display(Name = "Screening Venue")]
        public string ScreeningVenue { get; set; }

        [Display(Name = "Screening Time")]
        public string ScreeningTime { get; set; }

        [Display(Name = "Screening Date")]
        public string ScreeningDate { get; set; }

        [Display(Name = "Website Link")]
        public string WebsiteLink { get; set; }
        [Display(Name = "Portal Link")]
        public string PortalLink { get; set; }

        [Display(Name = "Iskool Link")]
        public string IskoollLink { get; set; }


        [Display(Name = "CBT Link")]
        public string CBTLink { get; set; }

        [Display(Name = "Is Primary and Nursery")]
        public bool IsPrimaryNursery { get; set; }


        [Display(Name = "Enable Batch Result Printing")]
        public bool EnableBatchResultPrinting { get; set; }

        [Display(Name = "Promotion By Maths And Eng")]
        public bool PromotionByMathsAndEng { get; set; }

        [Display(Name = "Default Enrollment Remark")]
        public string DefaultEnrollmentRemark { get; set; }

        [AllowHtml]
        public string GoogleAnalytics { get; set; }

        [UIHint("Enum")]
        public PrintOutOption PrintOutOption { get; set; }

        [UIHint("Enum")]
        public AdmissionPinOption AdmissionPinOption { get; set; }
        [AllowHtml]
        public string NewsletterContent { get; set; }
        public bool ShowNewsletterPage { get; set; }

        [UIHint("Enum")]
        public PinValidOption PinValidOption { get; set; }

        [Display(Name = "Payment Call Back URL")]
        public string PaymentCallBackUrl { get; set; }

        [Display(Name = "Zoom Host One")]
        public string ZoomHostOne { get; set; }

        [Display(Name = "Zoom Host One Pass")]
        public string ZoomHostOnePass { get; set; }

        [Display(Name = "Zoom Host Two")]
        public string ZoomHostTwo { get; set; }

        [Display(Name = "Zoom Host Two Pass")]
        public string ZoomHostTwoPass { get; set; }

        [Display(Name = "Zoom Host Three")]
        public string ZoomHostThree { get; set; }

        [Display(Name = "Zoom Host Three Pass")]
        public string ZoomHostThreePass { get; set; }

        [Display(Name = "Enable Zoom")]
        public bool EnableZoom { get; set; }

        [Display(Name = "Enable Multiple Zoom")]
        public bool EnableMultipleZoom { get; set; }

        [Display(Name = "Enable CBT")]
        public bool EnableCBT { get; set; }

        [Display(Name = "Enable Preview Result")]
        public bool EnablePreviewResult { get; set; }

        [Display(Name = "Enable Test Score")]
        public bool EnableTestScore { get; set; }

        [Display(Name = "Enable 2nd Test Score")]
        public bool EnableTestScore2 { get; set; }

        [Display(Name = "Enable Exam Score")]
        public bool EnableExamScore { get; set; }

        [Display(Name = "Enable Project Score")]
        public bool EnableProject { get; set; }

        [Display(Name = "Enable Class Exercise Score")]
        public bool EnableClassExercise { get; set; }

        [Display(Name = "Enable Assessment Score")]
        public bool EnableAssessment { get; set; }


        [Display(Name = "Hostel Feature")]
        public bool EnableHostel { get; set; }

        [Display(Name = "Financial Feature")]
        public bool EnableFinance { get; set; }

        [Display(Name = "Paystack Secret Key")]
        public string PaystackSecretKey { get; set; }
        public decimal? PaystackChargePercentage { get; set; }
        [Display(Name = "FlutterWave Secret Key")]
        public string FlutterwaveSecretKey { get; set; }
        public decimal? FlutterwaveChargePercentage { get; set; }



         [Display(Name = "Disable All Result Printing")]
        public bool DisableAllResultPrinting { get; set; }

         [Display(Name = "Disable All Result PrintingNote")]
        public string DisableAllResultPrintingNote { get; set; }



         [Display(Name = "Qoute")]
        public string Qoute { get; set; }
        [Display(Name = "Top Note")]
        public string TopNote { get; set; }

        [Display(Name = "Google Map")]
        public string GoogleMap { get; set; }
         [Display(Name = "Show Address One In Top")]
        public bool ShowAddressOneInTop { get; set; }
        [Display(Name = "Address One")]
        public string AddressOne { get; set; }

        [Display(Name = "Address Two")]
        public string AddressTwo { get; set; }

        [Display(Name = "Facebook Page")]
        public string FacebookPage { get; set; }

        [Display(Name = "Instagram Page")]
        public string InstagramPage { get; set; }

        [Display(Name = "Twitter Page")]
        public string TwitterPage { get; set; }

        [Display(Name = "Tiktok Page")]
        public string TiktokPage { get; set; }

        [Display(Name = "Youtube Channel")]
        public string YoutubeChannel { get; set; }

        [Display(Name = "Email One")]
        public string EmailOne { get; set; }


        [Display(Name = "Show Email One In Top")]
        public bool ShowEmailOneInTop { get; set; }

        [Display(Name = "Show Email One In Footer")]
        public bool ShowEmailOneInFooter { get; set; }


        [Display(Name = "Email Two")]
        public string EmailTwo { get; set; }
        [Display(Name = "Show Email Two In Top")]
        public bool ShowEmailTwoInTop { get; set; }

        [Display(Name = "Show Email Two In Footer")]
        public bool ShowEmailTwoInFooter { get; set; }

        [Display(Name = "Email Three")]
        public string EmailThree { get; set; }
        [Display(Name = "Show Email Three In Top")]
        public bool ShowEmailThreeInTop { get; set; }

        [Display(Name = "Show Email Three In Footer")]
        public bool ShowEmailThreeInFooter { get; set; }

        [Display(Name = "Phone One")]
        public string PhoneOne { get; set; }

        [Display(Name = "Show Phone One In Top")]
        public bool ShowPhoneOneInTop { get; set; }

        [Display(Name = "Show Phone One In Footer")]
        public bool ShowPhoneOneInFooter { get; set; }

        [Display(Name = "PhoneTwo")]
        public string PhoneTwo { get; set; }
        [Display(Name = "Show Phone Two In Top")]
        public bool ShowPhoneTwoInTop { get; set; }

        [Display(Name = "Show Phone Two In Footer")]
        public bool ShowPhoneTwoInFooter { get; set; }

        [Display(Name = "Phone Three")]
        public string PhoneThree { get; set; }
        [Display(Name = "Show Phone Three In Top")]
        public bool ShowPhoneThreeInTop { get; set; }

        [Display(Name = "Show Phone Three In Footer")]
        public bool ShowPhoneThreeInFooter { get; set; }
        [Display(Name = "Working Hour")]
        public string WorkingHour { get; set; }


        [Display(Name = "Add Faq To Home")]
        public bool AddFaqToHome { get; set; }

        [Display(Name = "Add Faq To Footer")]
        public bool AddFaqToFooter { get; set; }

        [Display(Name = "Add Testimony To Home")]
        public bool AddTestimonyToHome { get; set; }

        [Display(Name = "Add Testimony To Footer")]
        public bool AddTestimonyToFooter { get; set; }

        [Display(Name = "Add Partner To Home")]
        public bool AddPartnerToHome { get; set; }


        [Display(Name = "Disable Main Top Menu")]
        public bool DisableMainTopMenu { get; set; }

         

          [Display(Name = "Show Contact Us Menu")]
        public bool ShowContactUsMenu { get; set; }
        
          [Display(Name = "Show Contact Us Footer")]
        public bool ShowContactUsFooter { get; set; }


        [Display(Name = "Default Title Background")]
        public string DefaultTitleBackgroundUrl { get; set; }
        public string DefaultTitleBackgroundKey { get; set; }

       

        [Display(Name = "Add Blog To Home")]
        public bool AddBlogToHome { get; set; }

        [Display(Name = "Add Blog To Menu")]
        public bool AddBlogToMenu { get; set; }

        [Display(Name = "Add Blog To Footer")]
        public bool AddBlogToFooter { get; set; }


        [Display(Name = "Blog Display Title")]
        public string BlogDisplayTitle { get; set; }

        
        [Display(Name = "Add Career To Menu")]
        public bool AddCareerToMenu { get; set; }

        [Display(Name = "Add Career To Footer")]
        public bool AddCareerToFooter { get; set; }


        [Display(Name = "Career Display Title")]
        public string CareerDisplayTitle { get; set; }



    }
}
