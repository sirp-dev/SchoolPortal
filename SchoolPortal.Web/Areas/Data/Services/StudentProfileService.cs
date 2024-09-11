using SchoolPortal.Web.Areas.Data.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SchoolPortal.Web.Models.Entities;
using System.Threading.Tasks;
using SchoolPortal.Web.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using SchoolPortal.Web.Models.Dtos;
using Microsoft.AspNet.Identity.Owin;

namespace SchoolPortal.Web.Areas.Data.Services
{
    public class StudentProfileService : IStudentProfileService
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public StudentProfileService()
        {

        }

        public StudentProfileService(ApplicationUserManager userManager,
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

        public async Task Edit(StudentInfoDto model)
        {
            StudentProfile student = await db.StudentProfiles.FindAsync(model.Id);
            student.Disability = model.Disability;
            student.EmergencyContact = model.EmergencyContact;
            student.AboutMe = model.AboutMe;
            student.FavouriteFood = model.FavouriteFood;
            student.BooksYouLike = model.BooksYouLike;
            student.MoviesYouLike = model.MoviesYouLike;
            student.TVShowsYouLike = model.TVShowsYouLike;
            student.YourLikes = model.YourLikes;
            student.YourDisLikes = model.YourDisLikes;
            student.FavouriteColour = model.FavouriteColour;
            student.LastPrimarySchoolAttended = model.LastPrimarySchoolAttended;
            student.ParentGuardianAddress = model.ParentGuardianAddress;
            student.ParentGuardianName = model.ParentGuardianName;
            student.ParentGuardianPhoneNumber = model.ParentGuardianPhoneNumber;
            student.ParentGuardianOccupation = model.ParentGuardianOccupation;
            student.StudentRegNumber = model.StudentRegNumber;


            db.Entry(student).State = EntityState.Modified;
            await db.SaveChangesAsync();

            ApplicationUser user = await UserManager.FindByIdAsync(model.userid);
            if (user != null)
            {

                user.Email = model.Email;
                user.FirstName = model.Firstname;
                user.Surname = model.Surname;
                user.OtherName = model.Othername;

                user.DateOfBirth = model.DateOfBirth;
                user.Religion = model.Religion;
                user.Gender = model.Gender;
                user.ContactAddress = model.ContactAddress;
                user.City = model.City;
                user.Phone = model.Phone;
                user.LocalGov = model.LocalGov;
                user.StateOfOrigin = model.StateOfOrigin;
                user.Nationality = model.Nationality;
                // UserManager.Update(user);
                await UserManager.UpdateAsync(user);
            }


            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if(userId != null)
            {
                var user2 = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Updated student profile";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
            

        }

        public async Task UpdateImageId(int id, int imgId)
        {
            StudentProfile model = await db.StudentProfiles.FindAsync(id);
            model.ImageId = imgId;
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
                tracker.Note = tracker.FullName + " " + "Updated Student Profile Image";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task<StudentInfoDto> Get(int? id)
        {
            var student = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.Id == id);

            byte[] c;
            var img = await db.ImageModel.FirstOrDefaultAsync(x => x.Id == student.ImageId);
            if (img == null)
            {
                c = new byte[0];
            }
            else
            {
                c = img.ImageContent;
            }

            var output = new StudentInfoDto
            {
                Id = student.Id,
                Disability = student.Disability,
                StudentRegNumber = student.StudentRegNumber,
                LastPrimarySchoolAttended = student.LastPrimarySchoolAttended,
                ParentGuardianAddress = student.ParentGuardianAddress,
                ParentGuardianName = student.ParentGuardianName,
                ParentGuardianOccupation = student.ParentGuardianOccupation,
                ParentGuardianPhoneNumber = student.ParentGuardianPhoneNumber,
                AboutMe = student.AboutMe,
                FavouriteFood = student.FavouriteFood,
                BooksYouLike = student.BooksYouLike,
                MoviesYouLike = student.MoviesYouLike,
                TVShowsYouLike = student.TVShowsYouLike,
                YourLikes = student.YourLikes,
                YourDisLikes = student.YourDisLikes,
                FavouriteColour = student.FavouriteColour,
                Fullname = student.user.Surname + " " + student.user.FirstName + " " + student.user.OtherName,
                Surname = student.user.Surname,
                Firstname = student.user.FirstName,
                Othername = student.user.OtherName,
                Email = student.user.Email,
                Username = student.user.UserName,
                DateOfBirth = student.user.DateOfBirth,
                DateRegistered = student.user.DateRegistered,
                Status = student.user.Status,
                DataStatusChanged = student.user.DataStatusChanged,
                Religion = student.user.Religion,
                Gender = student.user.Gender,
                ContactAddress = student.user.ContactAddress,
                City = student.user.City,
                Phone = student.user.Phone,
                LocalGov = student.user.LocalGov,
                StateOfOrigin = student.user.StateOfOrigin,
                Nationality = student.user.Nationality,
                RegisteredBy = student.user.RegisteredBy,
                ImageId = student.ImageId,
                Image = c,
                userid = student.user.Id

            };
            return output;
        }

        public async Task<StudentInfoDto> GetStudentWithoutId()
        {
            var user = HttpContext.Current.User.Identity.GetUserId();
            var student = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.UserId == user);
            byte[] c;
            var img = await db.ImageModel.FirstOrDefaultAsync(x => x.Id == student.ImageId);
            if (img == null)
            {
                c = new byte[0];
            }
            else
            {
                c = img.ImageContent;
            }

            var output = new StudentInfoDto
            {
                Id = student.Id,
                Disability = student.Disability,
                StudentRegNumber = student.StudentRegNumber,
                LastPrimarySchoolAttended = student.LastPrimarySchoolAttended,
                ParentGuardianAddress = student.ParentGuardianAddress,
                ParentGuardianName = student.ParentGuardianName,
                ParentGuardianOccupation = student.ParentGuardianOccupation,
                ParentGuardianPhoneNumber = student.ParentGuardianPhoneNumber,
                AboutMe = student.AboutMe,
                FavouriteFood = student.FavouriteFood,
                BooksYouLike = student.BooksYouLike,
                MoviesYouLike = student.MoviesYouLike,
                TVShowsYouLike = student.TVShowsYouLike,
                YourLikes = student.YourLikes,
                YourDisLikes = student.YourDisLikes,
                FavouriteColour = student.FavouriteColour,
                Fullname = student.user.Surname + " " + student.user.FirstName + " " + student.user.OtherName,
                Surname = student.user.Surname,
                Firstname = student.user.FirstName,
                Othername = student.user.OtherName,
                Email = student.user.Email,
                Username = student.user.UserName,
                DateOfBirth = student.user.DateOfBirth,
                DateRegistered = student.user.DateRegistered,
                Status = student.user.Status,
                DataStatusChanged = student.user.DataStatusChanged,
                Religion = student.user.Religion,
                Gender = student.user.Gender,
                ContactAddress = student.user.ContactAddress,
                City = student.user.City,
                Phone = student.user.Phone,
                LocalGov = student.user.LocalGov,
                StateOfOrigin = student.user.StateOfOrigin,
                Nationality = student.user.Nationality,
                RegisteredBy = student.user.RegisteredBy,
                ImageId = student.ImageId,
                Image = c,
                userid = student.user.Id

            };
            return output;
        }

        public async Task<List<StudentProfile>> List()
        {
            var items = await db.StudentProfiles.Include(x => x.user).ToListAsync();
            return items;
        }

        public async Task<string> StudentCurrentClass(int? profileId)
        {
            var session = db.Sessions.OrderByDescending(x => x.Id);
            var currentSession = session.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var current = await db.Enrollments.Include(c => c.ClassLevel).Where(x => x.SessionId == currentSession.Id && x.StudentProfileId == profileId).FirstOrDefaultAsync(x => x.StudentProfileId == profileId);
            if (current != null)
            {
                string currentclass = current.ClassLevel.ClassName;
                return currentclass;
            }
            return "No Class Yet";
        }


        public async Task<int> SubjectCountStudent()
        {
            int item = 0;
            var user = HttpContext.Current.User.Identity.GetUserId();
            var pId = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.UserId == user);
            var enrol = await db.Enrollments.Include(x => x.EnrolledSubjects).FirstOrDefaultAsync(x => x.StudentProfileId == pId.Id);
            if (enrol != null)
            {
                item = enrol.EnrolledSubjects.Count();
            }


            return item;
        }
        public async Task<string> ClassNameForStudent()
        {
            string item = "Empty";
            var user = HttpContext.Current.User.Identity.GetUserId();
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);

            var pId = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.UserId == user);
            var enrol = await db.Enrollments.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.StudentProfileId == pId.Id && x.SessionId == currentSession.Id);
            if (enrol != null)
            {
                item = enrol.ClassLevel.ClassName;
            }

            return item;
        }

        public async Task<StudentProfile> GetStudentByUserId(string id)
        {
            var student = await db.StudentProfiles.FirstOrDefaultAsync(x => x.UserId == id);
            return student;
        }
    }
}