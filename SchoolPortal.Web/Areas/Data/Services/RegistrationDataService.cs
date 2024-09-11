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
    public class RegistrationDataService : IRegistrationDataService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public RegistrationDataService()
        {

        }

        public RegistrationDataService(ApplicationUserManager userManager,
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

        public async Task AdmitStudent(int id)
        {
            StudentData check = await db.StudentDatas.FirstOrDefaultAsync(x => x.Id == id);
            if (check.Status != AdmissionStatus.GivenAdmission)
            {
                check.Status = AdmissionStatus.GivenAdmission;

            }
            else
            {
                check.Status = AdmissionStatus.NotGivenAdmission;
            }

            db.Entry(check).State = EntityState.Modified;
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Admitted Student";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }

        }

        public async Task Create(StudentData model, string pinNumber)
        {
            //db.StudentDatas.Add(model);
          
            var sett = db.Settings.FirstOrDefault();
            var sess = db.Sessions.Where(x => x.Status == SessionStatus.Current).FirstOrDefault();

            db.StudentDatas.Add(model);
            test:
            var number = db.StudentDatas.Count() + 1;
            var studentNumber = number.ToString("D3");
            var registrationNumber = sett.SchoolInitials + "/" + sess.SessionYear + "/" + studentNumber;
            var checkNumber = db.StudentDatas.FirstOrDefault(x => x.RegistrationNumber == registrationNumber);
            if (checkNumber == null && sett.AdmissionPinOption == AdmissionPinOption.UsedPin)
            {
                var pincode = db.PinCodeModels.FirstOrDefault(x=>x.PinNumber == pinNumber);
                pincode.StudentPin = registrationNumber;
                db.Entry(pincode).State = EntityState.Modified;

                model.RegistrationNumber = registrationNumber;
                model.ApplicationDate = DateTime.Now;
                model.ExamScore = 0;
            }
            else if (checkNumber == null && sett.AdmissionPinOption == AdmissionPinOption.NoPin)
            {
                model.RegistrationNumber = registrationNumber;
                model.ApplicationDate = DateTime.Now;
                model.ExamScore = 0;
            }
            else
            {
                goto test;
            }
            await db.SaveChangesAsync();


            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "admission application";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }



        }


        public Task Delete(int? id)
        {
            throw new NotImplementedException();
        }

        public async Task Edit(StudentData model)
        {
            db.Entry(model).State = EntityState.Modified;
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
                tracker.Note = tracker.FullName + " " + "Edited admission application";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task<StudentDataDto> Get(int? id)
        {
            var item = await db.StudentDatas.FirstOrDefaultAsync(x => x.Id == id);
            byte[] image = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
            var img = await db.ImageModel.FirstOrDefaultAsync(x => x.Id == item.ImageId);
            if (img != null)
            {
                image = img.ImageContent;
            }

            var output = new StudentDataDto
            {

                Id = item.Id,
                RegistrationNumber = item.RegistrationNumber,
                Surname = item.Surname,
                FirstName = item.FirstName,
                OtherNames = item.OtherNames,
                DateOfBirth = item.DateOfBirth,
                LastPrimarySchoolAttended = item.LastPrimarySchoolAttended,
                Religion = item.Religion,
                NameOfParents = item.NameOfParents,
                ParentsAddress = item.ParentsAddress,
                PhoneNumberOfParents = item.PhoneNumberOfParents,
                OccupationOfParents = item.OccupationOfParents,
                PermanentHomeAddress = item.PermanentHomeAddress,
                LocalGov = item.LocalGov,
                StateOfOrigin = item.StateOfOrigin,
                Nationality = item.Nationality,
                ExamCenter = item.ExamCenter,
                Disability = item.Disability,
                EmergencyContact = item.EmergencyContact,
                ExamScore = item.ExamScore,
                Remarks = item.Remarks,
                AdmissionNumber = item.AdmissionNumber,
                Status = item.Status,
                DateOfAdmission = item.DateOfAdmission,
                AdmissionOfficer = item.AdmissionOfficer,
                ApplicationDate = item.ApplicationDate,
                ImageId = item.ImageId,

                Image = image

            };
            return output;
        }

        public async Task<StudentData> GetEdit(int? id)
        {
            var item = await db.StudentDatas.FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task<PinCodeModel> GetPin()
        {
            AdmissionPinDto model = new AdmissionPinDto();
            //var item = await db.StudentDatas.FirstOrDefaultAsync(x => x.Id == id);
            var checkPin = await db.PinCodeModels.FirstOrDefaultAsync(pin => pin.PinNumber == model.PinNumber && pin.SerialNumber == model.SerialNumber);
            return checkPin;
        }

        public async Task<StudentData> FormData()
        {
            PinCodeModel model = new PinCodeModel();
            var formData = await db.StudentDatas.FirstOrDefaultAsync(x => x.RegistrationNumber == model.StudentPin);
            return formData;
        }


        public async Task<StudentData> FormEmail()
        {
            AdmissionPinDto model = new AdmissionPinDto();
            var formData = db.StudentDatas.FirstOrDefault(x => x.EmailAddress == model.EmailAddress);
            return formData;
        }


        public async Task<List<StudentData>> List()
        {
            var items = await db.StudentDatas.ToListAsync();
            return items;
        }
    }
}