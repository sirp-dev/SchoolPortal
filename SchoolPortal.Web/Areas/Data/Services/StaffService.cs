using SchoolPortal.Web.Areas.Data.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SchoolPortal.Web.Models.Entities;
using System.Threading.Tasks;
using SchoolPortal.Web.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using SchoolPortal.Web.Models.Dtos;
using Microsoft.AspNet.Identity.Owin;

namespace SchoolPortal.Web.Areas.Data.Services
{
    public class StaffService : IStaffService
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public StaffService()
        {

        }

        public StaffService(ApplicationUserManager userManager,
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

        public async Task UpdateImageId(int id, int imgId)
        {
            StaffProfile model = await db.StaffProfiles.FindAsync(id);
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
                tracker.Note = tracker.FullName + " " + "Updated Staff Profile Image";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }
        public async Task<List<ClassLevel>> ClassesByStaff()
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var item = db.ClassLevels.Where(x => x.ClassName != "" && x.ShowClass ==true);
            if (HttpContext.Current.User.IsInRole("Admin") || HttpContext.Current.User.IsInRole("SuperAdmin"))
            {
                item = item.OrderBy(x => x.ClassName);

            }
            else
            {
                var currentUser = HttpContext.Current.User.Identity.GetUserId();
                item = item.Where(x => x.UserId == currentUser);

            }
            return await item.OrderBy(x => x.ClassName).ToListAsync();
        }

        public async Task<int> CreateQualification(Qualification model)
        {
            var userid = HttpContext.Current.User.Identity.GetUserId();
            var staffid = await db.StaffProfiles.FirstOrDefaultAsync(x => x.UserId == userid);
            model.StaffId = staffid.Id;
            model.UserId = userid;
            db.Qualifications.Add(model);
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
                tracker.Note = tracker.FullName + " " + "Added Staff Qualification";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           

            return model.StaffId;
        }

        public async Task<int> DeleteQualification(int? id)
        {
            var item = await db.Qualifications.FirstOrDefaultAsync(x => x.Id == id);
            var idi = item.StaffId;
            if (item != null)
            {

                db.Qualifications.Remove(item);

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
                    tracker.Note = tracker.FullName + " " + "Deleted Staff Qualification";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
               
            }
            return idi;
        }

        public async Task Edit(StaffInfoDto model)
        {
            StaffProfile staff = await db.StaffProfiles.FindAsync(model.Id);
            staff.Disability = model.Disability;
            staff.EmergencyContact = model.EmergencyContact;
            staff.AboutMe = model.AboutMe;
            staff.FavouriteFood = model.FavouriteFood;
            staff.BooksYouLike = model.BooksYouLike;
            staff.MoviesYouLike = model.MoviesYouLike;
            staff.TVShowsYouLike = model.TVShowsYouLike;
            staff.YourLikes = model.YourLikes;
            staff.YourDisLikes = model.YourDisLikes;
            staff.FavouriteColour = model.FavouriteColour;
            staff.PostHeld = model.PostHeld;
            staff.MaritalStatus = model.MaritalStatus;
            db.Entry(staff).State = EntityState.Modified;
            await db.SaveChangesAsync();

            //   ApplicationUser user = _userManager.FindById(model.userid);
            ApplicationUser user = await UserManager.FindByIdAsync(model.userid);
            if (user != null)
            {


                if (HttpContext.Current.User.IsInRole("Admin"))
                {
                    user.Email = model.Email;
                    user.FirstName = model.Firstname;
                    user.Surname = model.Surname;
                    user.OtherName = model.Othername;
                }
                if (HttpContext.Current.User.IsInRole("SuperAdmin"))
                {
                    user.Email = model.Email;
                    user.FirstName = model.Firstname;
                    user.Surname = model.Surname;
                    user.OtherName = model.Othername;
                }
                user.DateOfBirth = model.DateOfBirth;
                user.Religion = model.Religion;
                user.Gender = model.Gender;
                user.ContactAddress = model.ContactAddress;
                user.City = model.City;
                user.Phone = model.Phone;
                user.LocalGov = model.LocalGov;
                user.StateOfOrigin = model.StateOfOrigin;
                user.Nationality = model.Nationality;
                await UserManager.UpdateAsync(user);
            }


            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
             
           
        }

        public async Task<int> EditQualification(Qualification models)
        {
            db.Entry(models).State = EntityState.Modified;
            await db.SaveChangesAsync();

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
                tracker.Note = tracker.FullName + " " + "Edited Staff Qualification";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           

            return models.StaffId;
        }

        public async Task<StaffInfoDto> Get(int? id)
        {
            var staff = await db.StaffProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.Id == id);
            byte[] c;
            var img = await db.ImageModel.FirstOrDefaultAsync(x => x.Id == staff.ImageId);
            if (img == null)
            {
                c = new byte[0];
            }
            else
            {
                c = img.ImageContent;
            }
            var output = new StaffInfoDto
            {
                Id = staff.Id,
                Disability = staff.Disability,
                EmergencyContact = staff.EmergencyContact,
                MaritalStatus = staff.MaritalStatus,
                StaffRegistrationId = staff.StaffRegistrationId,
                DateOfAppointment = staff.DateOfAppointment,
                PostHeld = staff.PostHeld,
                AboutMe = staff.AboutMe,
                FavouriteFood = staff.FavouriteFood,
                BooksYouLike = staff.BooksYouLike,
                MoviesYouLike = staff.MoviesYouLike,
                TVShowsYouLike = staff.TVShowsYouLike,
                YourLikes = staff.YourLikes,
                YourDisLikes = staff.YourDisLikes,
                FavouriteColour = staff.FavouriteColour,
                Fullname = staff.user.Surname + " " + staff.user.FirstName + " " + staff.user.OtherName,
                Surname = staff.user.Surname,
                Firstname = staff.user.FirstName,
                Othername = staff.user.OtherName,
                Email = staff.user.Email,
                Username = staff.user.UserName,
                DateOfBirth = staff.user.DateOfBirth,
                DateRegistered = staff.user.DateRegistered,
                Status = staff.user.Status,
                DataStatusChanged = staff.user.DataStatusChanged,
                Religion = staff.user.Religion,
                Gender = staff.user.Gender,
                ContactAddress = staff.user.ContactAddress,
                City = staff.user.City,
                Phone = staff.user.Phone,
                LocalGov = staff.user.LocalGov,
                StateOfOrigin = staff.user.StateOfOrigin,
                Nationality = staff.user.Nationality,
                RegisteredBy = staff.user.RegisteredBy,
                ImageId = staff.ImageId,
                Image = c,
                userid = staff.user.Id

            };
            return output;

        }

        public async Task<Qualification> GetQualification(int? id)
        {
            var item = await db.Qualifications.FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task<StaffInfoDto> GetStaffWithoutId()
        {
            var user = HttpContext.Current.User.Identity.GetUserId();
            var staff = await db.StaffProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.UserId == user);
            byte[] c;
            var img = await db.ImageModel.FirstOrDefaultAsync(x => x.Id == staff.ImageId);
            if (img == null)
            {
                c = new byte[0];
            }
            else
            {
                c = img.ImageContent;
            }
            var output = new StaffInfoDto
            {
                Id = staff.Id,
                Disability = staff.Disability,
                EmergencyContact = staff.EmergencyContact,
                MaritalStatus = staff.MaritalStatus,
                StaffRegistrationId = staff.StaffRegistrationId,
                DateOfAppointment = staff.DateOfAppointment,
                PostHeld = staff.PostHeld,
                AboutMe = staff.AboutMe,
                FavouriteFood = staff.FavouriteFood,
                BooksYouLike = staff.BooksYouLike,
                MoviesYouLike = staff.MoviesYouLike,
                TVShowsYouLike = staff.TVShowsYouLike,
                YourLikes = staff.YourLikes,
                YourDisLikes = staff.YourDisLikes,
                FavouriteColour = staff.FavouriteColour,
                Fullname = staff.user.Surname + " " + staff.user.FirstName + " " + staff.user.OtherName,
                Surname = staff.user.Surname,
                Firstname = staff.user.FirstName,
                Othername = staff.user.OtherName,
                Email = staff.user.Email,
                Username = staff.user.UserName,
                DateOfBirth = staff.user.DateOfBirth,
                DateRegistered = staff.user.DateRegistered,
                Status = staff.user.Status,
                DataStatusChanged = staff.user.DataStatusChanged,
                Religion = staff.user.Religion,
                Gender = staff.user.Gender,
                ContactAddress = staff.user.ContactAddress,
                City = staff.user.City,
                Phone = staff.user.Phone,
                LocalGov = staff.user.LocalGov,
                StateOfOrigin = staff.user.StateOfOrigin,
                Nationality = staff.user.Nationality,
                RegisteredBy = staff.user.RegisteredBy,
                ImageId = staff.ImageId,
                Image = c,
                userid = staff.user.Id

            };
            return output;

        }

        public async Task<List<StaffProfile>> List()
        {
            var items = await db.StaffProfiles.Include(x => x.Qualifications).Include(x => x.user).ToListAsync();
            return items;
        }

        public async Task<List<Qualification>> ListQualification(int id)
        {
            var items = await db.Qualifications.Include(x => x.StaffProfile).ToListAsync();
            return items;
        }

        public async Task<List<Qualification>> ListQualificationForUser()
        {
            var user = HttpContext.Current.User.Identity.GetUserId();
            var userProfileId = await db.StaffProfiles.Include(x => x.user).FirstOrDefaultAsync(f => f.UserId == user);
            var items = await db.Qualifications.Include(x => x.StaffProfile).Where(x => x.StaffId == userProfileId.Id).ToListAsync();
            return items;
        }


        public async Task ReloadStudents(int id = 0, int sessionId = 0, int classId = 0)
        {
            var subject = db.EnrolledSubjects.Include(x => x.Enrollments).Where(x => x.SubjectId == id && x.Enrollments.SessionId == sessionId);

            var studentClass = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == classId);

            //get students enrolled in that class
            var enrolledStudents = db.Enrollments.Where(c => c.ClassLevelId == studentClass.Id && c.SessionId == sessionId);
            var currentlevel = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == studentClass.Id);
            foreach (var student in enrolledStudents.ToList())
            {
                var checkSubject = subject.FirstOrDefault(x => x.EnrollmentId == student.Id);
                if (checkSubject == null)
                {
                    EnrolledSubject enrolledSubject = db.EnrolledSubjects.Create();
                    enrolledSubject.SubjectId = id;
                    if (currentlevel.ClassName.Substring(0, 2).Contains("PG"))
                    {
                        enrolledSubject.GradingOption = GradingOption.PG;
                    }
                    else if (currentlevel.ClassName.Substring(0, 3).Contains("SSS"))
                    {
                        enrolledSubject.GradingOption = GradingOption.SSS;
                    }
                    else if (currentlevel.ClassName.Substring(0, 3).Contains("JSS"))
                    {
                        enrolledSubject.GradingOption = GradingOption.JSS;
                    }
                    else if (currentlevel.ClassName.Substring(0, 3).Contains("PRI"))
                    {
                        enrolledSubject.GradingOption = GradingOption.PRI;
                    }
                    else if (currentlevel.ClassName.Substring(0, 3).Contains("PRE"))
                    {
                        enrolledSubject.GradingOption = GradingOption.PRE;
                    }
                    else if (currentlevel.ClassName.Substring(0, 3).Contains("NUR"))
                    {
                        enrolledSubject.GradingOption = GradingOption.NUR;
                    }

                    enrolledSubject.EnrollmentId = student.Id;
                    enrolledSubject.ExamScore = 0;
                    enrolledSubject.TestScore = 0;
                    enrolledSubject.TotalScore = 0;
                    enrolledSubject.IsOffered = false;
                    enrolledSubject.Project = 0;
                    enrolledSubject.TestScore2 = 0;
                    enrolledSubject.Assessment = 0;
                    enrolledSubject.ClassExercise = 0;
                    enrolledSubject.TotalCA = 0;
                    db.EnrolledSubjects.Add(enrolledSubject);
                }
            }

            await db.SaveChangesAsync();

        }



        public async Task AdminReloadStudents(int id = 0, int sessionId = 0, int classId = 0)
        {
            var subject = db.EnrolledSubjects.Include(x => x.Enrollments).Where(x => x.SubjectId == id && x.Enrollments.SessionId == sessionId && x.Enrollments.ClassLevelId == classId);

            var studentClass = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == classId);

            //get students enrolled in that class
            var enrolledStudents = db.Enrollments.Where(c => c.ClassLevelId == studentClass.Id && c.SessionId == sessionId);
            var currentlevel = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == studentClass.Id);
            if (enrolledStudents != null)
            {
                foreach (var student in await enrolledStudents.ToListAsync())
                {
                    var subName = await db.Subjects.Where(x => x.Id == id).FirstOrDefaultAsync();
                    var checkSubject = await subject.FirstOrDefaultAsync(x => x.EnrollmentId == student.Id);
                    var checkSubject2 = await db.EnrolledSubjects.Include(x => x.Subject).Include(x => x.Enrollments).FirstOrDefaultAsync(x => (x.SubjectId == id || x.Subject.SubjectName == subName.SubjectName) && x.EnrollmentId == student.Id && x.Enrollments.SessionId == sessionId && x.Enrollments.ClassLevelId == classId);

                    //Remove subject in the term not needed due to duplicate
                    var checkSubject1 = await db.EnrolledSubjects.Include(x => x.Subject).Include(x => x.Enrollments).FirstOrDefaultAsync(x => (x.SubjectId == id && x.Subject.SubjectName == subName.SubjectName) && x.EnrollmentId == student.Id && x.Enrollments.SessionId == sessionId && x.Enrollments.ClassLevelId == classId);
                    if (checkSubject1 != null)
                    {
                        var checkDelete1 = await db.EnrolledSubjects.Where(x => (x.SubjectId == id && x.Subject.SubjectName == subName.SubjectName) && x.EnrollmentId == student.Id && x.Enrollments.SessionId == sessionId && x.Enrollments.ClassLevelId == classId).FirstOrDefaultAsync();
                        var checkDelete2 = db.EnrolledSubjects.Where(x => (x.SubjectId == id && x.Subject.SubjectName == subName.SubjectName) && x.EnrollmentId == student.Id && x.Enrollments.SessionId == sessionId && x.Enrollments.ClassLevelId == classId);

                        foreach (var itemRev in checkDelete2.ToList())
                        {
                            db.EnrolledSubjects.Remove(itemRev);
                            await db.SaveChangesAsync();
                        }
                    }

                    //Update subject in the term not needed due to duplicate

                    if (checkSubject2 != null)
                    {
                        var enrolledSubject = await db.EnrolledSubjects.Include(x => x.Subject).Include(x => x.Enrollments).FirstOrDefaultAsync(x => (x.SubjectId != id && x.Subject.SubjectName == subName.SubjectName) && x.EnrollmentId == student.Id && x.Enrollments.SessionId == sessionId && x.Enrollments.ClassLevelId == classId);
                        var enrolledSubjectCount = db.EnrolledSubjects.Where(x => (x.SubjectId != id && x.Subject.SubjectName == subName.SubjectName) && x.EnrollmentId == student.Id && x.Enrollments.SessionId == sessionId && x.Enrollments.ClassLevelId == classId).Count();

                        if (enrolledSubjectCount != 0)
                        {
                            if (enrolledSubjectCount >= 1)
                            {
                                if (currentlevel.ClassName.Substring(0, 2).Contains("PG"))
                                {
                                    enrolledSubject.GradingOption = GradingOption.PG;
                                }
                                else if (currentlevel.ClassName.Substring(0, 3).Contains("SSS"))
                                {
                                    enrolledSubject.GradingOption = GradingOption.SSS;
                                }
                                else if (currentlevel.ClassName.Substring(0, 3).Contains("JSS"))
                                {
                                    enrolledSubject.GradingOption = GradingOption.JSS;
                                }
                                else if (currentlevel.ClassName.Substring(0, 3).Contains("PRI"))
                                {
                                    enrolledSubject.GradingOption = GradingOption.PRI;
                                }
                                else if (currentlevel.ClassName.Substring(0, 3).Contains("PRE"))
                                {
                                    enrolledSubject.GradingOption = GradingOption.PRE;
                                }
                                else if (currentlevel.ClassName.Substring(0, 3).Contains("NUR"))
                                {
                                    enrolledSubject.GradingOption = GradingOption.NUR;
                                }
                                //var subName = await db.Subjects.Where(x => x.Id == id).FirstOrDefaultAsync();
                                var subList = await db.EnrolledSubjects.Include(x => x.Subject).Include(x => x.Enrollments).Where(x => (x.SubjectId != id && x.Subject.SubjectName == subName.SubjectName) && x.EnrollmentId == student.Id && x.Enrollments.SessionId == sessionId && x.Enrollments.ClassLevelId == classId).ToListAsync();
                                foreach (var item in subList)
                                {

                                    if (item.SubjectId != id)
                                    {
                                        var checkDelete = db.EnrolledSubjects.Where(x => (x.SubjectId != id && x.Subject.SubjectName == subName.SubjectName) && x.EnrollmentId == student.Id && x.Enrollments.SessionId == sessionId && x.Enrollments.ClassLevelId == classId).Count();

                                        //Delete enrolled subjects that is not same with subject Id
                                        if (checkDelete != 0)
                                        {
                                            if (checkDelete >= 1)
                                            {

                                                enrolledSubject.SubjectId = id;
                                                enrolledSubject.EnrollmentId = student.Id;
                                                enrolledSubject.ExamScore = enrolledSubject.ExamScore;
                                                enrolledSubject.TestScore = enrolledSubject.TestScore;
                                                enrolledSubject.TotalScore = enrolledSubject.TotalScore;
                                                enrolledSubject.IsOffered = enrolledSubject.IsOffered;
                                                enrolledSubject.Project = enrolledSubject.Project;
                                                enrolledSubject.TestScore2 = enrolledSubject.TestScore2;
                                                enrolledSubject.GradingOption = enrolledSubject.GradingOption;
                                                enrolledSubject.Assessment = enrolledSubject.Assessment;
                                                enrolledSubject.ClassExercise = enrolledSubject.ClassExercise;
                                                enrolledSubject.TotalCA = enrolledSubject.TotalCA;
                                                db.Entry(enrolledSubject).State = EntityState.Modified;
                                                await db.SaveChangesAsync();
                                            }
                                            else
                                            {
                                                var checkDelete1 = await db.EnrolledSubjects.Where(x => (x.SubjectId != id && x.Subject.SubjectName == subName.SubjectName) && x.EnrollmentId == student.Id && x.Enrollments.SessionId == sessionId && x.Enrollments.ClassLevelId == classId).FirstOrDefaultAsync();
                                                var checkDelete2 = db.EnrolledSubjects.Where(x => (x.SubjectId != id && x.Subject.SubjectName == subName.SubjectName) && x.EnrollmentId == student.Id && x.Enrollments.SessionId == sessionId && x.Enrollments.ClassLevelId == classId);

                                                foreach (var itemRev in checkDelete2.ToList())
                                                {
                                                    db.EnrolledSubjects.Remove(itemRev);
                                                    await db.SaveChangesAsync();
                                                }
                                                
                                            }

                                        }


                                        //Delete enrolled subjects that is not same with subject Id
                                        //if (checkDelete != 0)
                                        //{
                                        //    if (checkDelete >= 1)
                                        //    {
                                        //        var checkDelete1 = await db.EnrolledSubjects.Where(x => (x.SubjectId != id && x.Subject.SubjectName == subName.SubjectName) && x.EnrollmentId == student.Id && x.Enrollments.SessionId == sessionId && x.Enrollments.ClassLevelId == classId).FirstOrDefaultAsync();
                                        //        var checkDelete2 = db.EnrolledSubjects.Where(x => (x.SubjectId != id && x.Subject.SubjectName == subName.SubjectName) && x.EnrollmentId == student.Id && x.Enrollments.SessionId == sessionId && x.Enrollments.ClassLevelId == classId);

                                        //        foreach (var itemRev in checkDelete2.ToList())
                                        //        {
                                        //            db.EnrolledSubjects.Remove(itemRev);
                                        //            await db.SaveChangesAsync();
                                        //        }

                                        //    }
                                        //    else
                                        //    {
                                        //        enrolledSubject.SubjectId = id;
                                        //        enrolledSubject.EnrollmentId = student.Id;
                                        //        enrolledSubject.ExamScore = enrolledSubject.ExamScore;
                                        //        enrolledSubject.TestScore = enrolledSubject.TestScore;
                                        //        enrolledSubject.TotalScore = enrolledSubject.TotalScore;
                                        //        enrolledSubject.IsOffered = enrolledSubject.IsOffered;
                                        //        db.Entry(enrolledSubject).State = EntityState.Modified;
                                        //        await db.SaveChangesAsync();
                                        //    }

                                        //}

                                    }
                                    else
                                    {
                                        enrolledSubject.SubjectId = id;
                                        enrolledSubject.EnrollmentId = student.Id;
                                        enrolledSubject.ExamScore = enrolledSubject.ExamScore;
                                        enrolledSubject.TestScore = enrolledSubject.TestScore;
                                        enrolledSubject.TotalScore = enrolledSubject.TotalScore;
                                        enrolledSubject.IsOffered = enrolledSubject.IsOffered;
                                        enrolledSubject.Project = enrolledSubject.Project;
                                        enrolledSubject.TestScore2 = enrolledSubject.TestScore2;
                                        enrolledSubject.Assessment = enrolledSubject.Assessment;
                                        enrolledSubject.ClassExercise = enrolledSubject.ClassExercise;
                                        enrolledSubject.GradingOption = enrolledSubject.GradingOption;
                                        enrolledSubject.TotalCA = enrolledSubject.TotalCA;
                                        db.Entry(enrolledSubject).State = EntityState.Modified;
                                        await db.SaveChangesAsync();

                                    }

                                }
                            }
                            else
                            {
                                enrolledSubject.SubjectId = id;
                                enrolledSubject.EnrollmentId = student.Id;
                                enrolledSubject.ExamScore = enrolledSubject.ExamScore;
                                enrolledSubject.TestScore = enrolledSubject.TestScore;
                                enrolledSubject.TotalScore = enrolledSubject.TotalScore;
                                enrolledSubject.IsOffered = enrolledSubject.IsOffered;
                                enrolledSubject.GradingOption = enrolledSubject.GradingOption;
                                enrolledSubject.Project = enrolledSubject.Project;
                                enrolledSubject.TestScore2 = enrolledSubject.TestScore2;
                                enrolledSubject.Assessment = enrolledSubject.Assessment;
                                enrolledSubject.ClassExercise = enrolledSubject.ClassExercise;
                                enrolledSubject.TotalCA = enrolledSubject.TotalCA;
                                db.Entry(enrolledSubject).State = EntityState.Modified;
                                await db.SaveChangesAsync();
                            }

                        }

                    }


                }


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
                tracker.Note = tracker.FullName + " " + "Reloaded Students";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();

            }
           
        }


        public async Task AdminReloadStudents2(int id = 0, int sessionId = 0, int classId = 0)
        {
            var subject = db.EnrolledSubjects.Include(x => x.Enrollments).Where(x => x.SubjectId == id && x.Enrollments.SessionId == sessionId && x.Enrollments.ClassLevelId == classId);

            var studentClass = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == classId);

            //get students enrolled in that class
            var enrolledStudents = db.Enrollments.Where(c => c.ClassLevelId == studentClass.Id && c.SessionId == sessionId);
            var currentlevel = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == studentClass.Id);
            if (enrolledStudents != null)
            {
                foreach (var student in await enrolledStudents.ToListAsync())
                {
                    var subName = await db.Subjects.Where(x => x.Id == id).FirstOrDefaultAsync();
                    var checkSubject = await subject.FirstOrDefaultAsync(x => x.EnrollmentId == student.Id);
                    var checkSubject2 = await db.EnrolledSubjects.Include(x => x.Subject).Include(x => x.Enrollments).FirstOrDefaultAsync(x => (x.SubjectId != id || x.Subject.SubjectName == subName.SubjectName) && x.EnrollmentId == student.Id && x.Enrollments.SessionId == sessionId && x.Enrollments.ClassLevelId == classId);


                    //Update subject in the term not needed due to duplicate

                    if (checkSubject2 != null)
                    {
                        var enrolledSubject = await db.EnrolledSubjects.Include(x => x.Subject).Include(x => x.Enrollments).FirstOrDefaultAsync(x => (x.SubjectId != id && x.Subject.SubjectName == subName.SubjectName) && x.EnrollmentId == student.Id && x.Enrollments.SessionId == sessionId && x.Enrollments.ClassLevelId == classId);
                        var enrolledSubjectCount = db.EnrolledSubjects.Where(x => (x.SubjectId != id && x.Subject.SubjectName == subName.SubjectName) && x.EnrollmentId == student.Id && x.Enrollments.SessionId == sessionId && x.Enrollments.ClassLevelId == classId).Count();

                        if (enrolledSubjectCount != 0)
                        {
                            if (enrolledSubjectCount >= 1)
                            {
                                if (currentlevel.ClassName.Substring(0, 2).Contains("PG"))
                                {
                                    enrolledSubject.GradingOption = GradingOption.PG;
                                }
                                else if (currentlevel.ClassName.Substring(0, 3).Contains("SSS"))
                                {
                                    enrolledSubject.GradingOption = GradingOption.SSS;
                                }
                                else if (currentlevel.ClassName.Substring(0, 3).Contains("JSS"))
                                {
                                    enrolledSubject.GradingOption = GradingOption.JSS;
                                }
                                else if (currentlevel.ClassName.Substring(0, 3).Contains("PRI"))
                                {
                                    enrolledSubject.GradingOption = GradingOption.PRI;
                                }
                                else if (currentlevel.ClassName.Substring(0, 3).Contains("PRE"))
                                {
                                    enrolledSubject.GradingOption = GradingOption.PRE;
                                }
                                else if (currentlevel.ClassName.Substring(0, 3).Contains("NUR"))
                                {
                                    enrolledSubject.GradingOption = GradingOption.NUR;
                                }
                                //var subName = await db.Subjects.Where(x => x.Id == id).FirstOrDefaultAsync();
                                var subList = await db.EnrolledSubjects.Include(x => x.Subject).Include(x => x.Enrollments).Where(x => (x.SubjectId != id && x.Subject.SubjectName == subName.SubjectName) && x.EnrollmentId == student.Id && x.Enrollments.SessionId == sessionId && x.Enrollments.ClassLevelId == classId).ToListAsync();
                                foreach (var item in subList)
                                {

                                    if (item.SubjectId != id)
                                    {
                                        var checkDelete = db.EnrolledSubjects.Where(x => (x.SubjectId != id && x.Subject.SubjectName == subName.SubjectName) && x.EnrollmentId == student.Id && x.Enrollments.SessionId == sessionId && x.Enrollments.ClassLevelId == classId).Count();

                                        //Delete enrolled subjects that is not same with subject Id
                                        if (checkDelete != 0)
                                        {
                                            if (checkDelete >= 1)
                                            {

                                                var checkDelete1 = await db.EnrolledSubjects.Where(x => (x.SubjectId != id && x.Subject.SubjectName == subName.SubjectName) && x.EnrollmentId == student.Id && x.Enrollments.SessionId == sessionId && x.Enrollments.ClassLevelId == classId).FirstOrDefaultAsync();
                                                var checkDelete2 = db.EnrolledSubjects.Where(x => (x.SubjectId != id && x.Subject.SubjectName == subName.SubjectName) && x.EnrollmentId == student.Id && x.Enrollments.SessionId == sessionId && x.Enrollments.ClassLevelId == classId);

                                                foreach (var itemRev in checkDelete2.ToList())
                                                {
                                                    db.EnrolledSubjects.Remove(itemRev);
                                                    await db.SaveChangesAsync();
                                                }
                                            }

                                        }

                                    }
                                    else
                                    {
                                        enrolledSubject.SubjectId = id;
                                        enrolledSubject.EnrollmentId = student.Id;
                                        enrolledSubject.ExamScore = enrolledSubject.ExamScore;
                                        enrolledSubject.TestScore = enrolledSubject.TestScore;
                                        enrolledSubject.TotalScore = enrolledSubject.TotalScore;
                                        enrolledSubject.IsOffered = enrolledSubject.IsOffered;
                                        enrolledSubject.Project = enrolledSubject.Project;
                                        enrolledSubject.TestScore2 = enrolledSubject.TestScore2;
                                        enrolledSubject.Assessment = enrolledSubject.Assessment;
                                        enrolledSubject.GradingOption = enrolledSubject.GradingOption;
                                        enrolledSubject.ClassExercise = enrolledSubject.ClassExercise;
                                        enrolledSubject.TotalCA = enrolledSubject.TotalCA;
                                        enrolledSubject.GradingOption = enrolledSubject.GradingOption;
                                        db.Entry(enrolledSubject).State = EntityState.Modified;
                                        await db.SaveChangesAsync();

                                    }

                                }
                            }
                            else
                            {
                                enrolledSubject.SubjectId = id;
                                enrolledSubject.EnrollmentId = student.Id;
                                enrolledSubject.ExamScore = enrolledSubject.ExamScore;
                                enrolledSubject.TestScore = enrolledSubject.TestScore;
                                enrolledSubject.TotalScore = enrolledSubject.TotalScore;
                                enrolledSubject.IsOffered = enrolledSubject.IsOffered;
                                enrolledSubject.GradingOption = enrolledSubject.GradingOption;
                                enrolledSubject.Project = enrolledSubject.Project;
                                enrolledSubject.TestScore2 = enrolledSubject.TestScore2;
                                enrolledSubject.Assessment = enrolledSubject.Assessment;
                                enrolledSubject.ClassExercise = enrolledSubject.ClassExercise;
                                enrolledSubject.TotalCA = enrolledSubject.TotalCA;
                                db.Entry(enrolledSubject).State = EntityState.Modified;
                                await db.SaveChangesAsync();
                            }

                        }

                    }


                }


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
                tracker.Note = tracker.FullName + " " + "Reloaded Students";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }


        public async Task RemoveStudents(int id = 0, int sessionId = 0, int classId = 0)
        {
            var subject = db.EnrolledSubjects.Include(x => x.Enrollments).Where(x => x.SubjectId == id && x.Enrollments.SessionId == sessionId);

            var studentClass = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == classId);

            //get students enrolled in that class
            var enrolledStudents = db.Enrollments.Where(c => c.ClassLevelId == studentClass.Id && c.SessionId == sessionId);
            var currentlevel = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == studentClass.Id);
            foreach (var student in enrolledStudents.ToList())
            {
                var checkSubject = subject.FirstOrDefault(x => x.EnrollmentId == student.Id);
                if (checkSubject != null)
                {
                    db.EnrolledSubjects.Remove(checkSubject);
                }
            }

            await db.SaveChangesAsync();

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
                tracker.Note = tracker.FullName + " " + "Unenroll student from subject";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task<List<ClassLevel>> StaffClassName()
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var user = HttpContext.Current.User.Identity.GetUserId();
            var name = await db.ClassLevels.Where(x => x.UserId == user && x.ShowClass ==true).ToListAsync();

            return name;
        }

        public async Task<List<StaffDropdownListDto>> StaffDropdownList()
        {

            var item = db.StaffProfiles.Include(x => x.user).Where(x => x.user.Status == EntityStatus.Active);
            var output = item.Select(x => new StaffDropdownListDto
            {
                StaffId = x.Id,
                FullName = x.user.Surname + " " + x.user.FirstName + " " + x.user.OtherName,
                UserId = x.UserId
            });
            return await output.OrderBy(x => x.FullName).ToListAsync();

        }

        public async Task<int> StaffSubjects()
        {
            var user = HttpContext.Current.User.Identity.GetUserId();
            var sub = await db.Subjects.Include(x => x.ClassLevel).Where(x => x.UserId == user && x.ShowSubject == true && x.ClassLevel.ShowClass ==true).CountAsync();
            return sub;

        }

        public Task<StudentProfile> StudentsList(int subjectId, int sessionId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Subject>> SubjectsByStaff()
        {
            var currentUser = HttpContext.Current.User.Identity.GetUserId();
            var subjects = db.Subjects.Include(x => x.ClassLevel).Where(x => x.SubjectName != "" && x.ShowSubject == true && x.ClassLevel.ShowClass == true);
            if (HttpContext.Current.User.IsInRole("Admin") || HttpContext.Current.User.IsInRole("SuperAdmin"))
            {
                subjects = subjects.OrderBy(x => x.SubjectName);
            }
            else
            {
                subjects = subjects.Where(x => x.UserId == currentUser);
            }

            return await subjects.OrderBy(x => x.ClassLevel.ClassName).ThenBy(x => x.SubjectName).ToListAsync();
        }
    }
}