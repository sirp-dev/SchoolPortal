using SchoolPortal.Web.Areas.Data.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SchoolPortal.Web.Models.Entities;
using System.Threading.Tasks;
using SchoolPortal.Web.Models;
using System.Data.Entity;
using SchoolPortal.Web.Models.Dtos;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace SchoolPortal.Web.Areas.Data.Services
{
    public class SettingService : ISettingService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public SettingService()
        {

        }

        public SettingService(ApplicationUserManager userManager,
           ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }


        public async Task Create(Setting model)
        {

            db.Settings.Add(model);
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if(userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Added setting";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           

        }

        public async Task CreateSettingValue(SettingsValue model)
        {
            db.SettingsValues.Add(model);
            await db.SaveChangesAsync();


            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if(userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Added setting value";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task Edit(Setting models)
        {
            db.Entry(models).State = EntityState.Modified;
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if(userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Edit setting";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task EditSettingValue(SettingsValue models)
        {
            db.Entry(models).State = EntityState.Modified;
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if(userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Edit setting value";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task<Setting> Get(int? id)
        {
            var item = await db.Settings.FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task<SettingsDto> GetSetting()
        {
            var item = await db.Settings.FirstOrDefaultAsync();
            var img = await db.ImageModel.FirstOrDefaultAsync(x => x.Id == item.ImageId);
            var output = new SettingsDto
            {
                Id = item.Id,
                SchoolName = item.SchoolName,
                SchoolInitials = item.SchoolInitials,
                SchoolAddress = item.SchoolAddress,
                SchoolAddress2 = item.SchoolAddress2,
                State = item.State,
                Country = item.Country,
                ContactEmail = item.ContactEmail,
                ContactPhoneNumber = item.ContactPhoneNumber,
                Image = img.ImageContent,
                SmsUsername = item.SmsUsername,
                SmsPassword = item.SmsPassword,
                Passmark = item.Passmark,
                PromotionByTrial = item.PromotionByTrial,
                ShowPositionOnResult = item.ShowPositionOnResult,
                ShowCumulativeResultForThirdTerm = item.ShowCumulativeResultForThirdTerm,
                EmailFrom = item.EmailFrom,
                MailHost = item.MailHost,
                Port = item.Port,
                MailUsername = item.MailUsername,
                MailPassword = item.MailPassword,
                SslEnabled = item.SslEnabled,
                AccessmentScore = item.AccessmentScore,
                ExamScore = item.ExamScore,
                DefaultEnrollmentRemark = item.DefaultEnrollmentRemark,
                AdmissionPinOption = item.AdmissionPinOption,
                EnableBatchResultPrinting = item.EnableBatchResultPrinting,
                GoogleAnalytics = item.GoogleAnalytics,
                ImageId = item.ImageId,
                IsPrimaryNursery = item.IsPrimaryNursery,
                NewsletterContent = item.NewsletterContent,
                PinValidOption = item.PinValidOption,
                PortalLink = item.PortalLink,
                PrintOutOption = item.PrintOutOption,
                PromoteAll = item.PromoteAll,
                PromotionByMathsAndEng = item.PromotionByMathsAndEng,
                ScreeningDate = item.ScreeningDate,
                ScreeningTime = item.ScreeningTime,
                ScreeningVenue = item.ScreeningVenue,
                ShowAccctOnResult = item.ShowAccctOnResult,
                ShowFeesOnResult = item.ShowFeesOnResult,
                ShowNewsletterPage = item.ShowNewsletterPage,
                WebsiteLink = item.WebsiteLink,
                EnableAssessment = item.EnableAssessment,
                CBTLink = item.CBTLink,
                EnableCBT = item.EnableCBT,
                EnableClassExercise = item.EnableClassExercise,
                EnableExamScore = item.EnableExamScore,
                EnableMultipleZoom = item.EnableMultipleZoom,
                EnablePreviewResult = item.EnablePreviewResult,
                EnableProject = item.EnableProject,
                EnableTestScore = item.EnableTestScore,
                EnableTestScore2 = item.EnableTestScore2,
                EnableZoom = item.EnableZoom,
                IskoollLink = item.IskoollLink,
                PaymentCallBackUrl = item.PaymentCallBackUrl,
                ZoomHostOne = item.ZoomHostOne,
                ZoomHostOnePass = item.ZoomHostOnePass,
                ZoomHostThree = item.ZoomHostThree,
                ZoomHostThreePass = item.ZoomHostThreePass,
                ZoomHostTwo = item.ZoomHostTwo,
                ZoomHostTwoPass = item.ZoomHostTwoPass

            };
            return output;

        }

        public async Task<SettingsValue> GetSettingValue(int? id)
        {
            var item = await db.SettingsValues.FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task DeleteSettingValue(int? id)
        {
            SettingsValue data = await db.SettingsValues.FindAsync(id);
            db.SettingsValues.Remove(data);
            await db.SaveChangesAsync();


            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if(userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Delete setting value";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task<List<SettingsValue>> SettingsValueList()
        {
            var settings = await db.SettingsValues.ToListAsync();
            return settings;
        }
    }
}