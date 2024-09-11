using SchoolPortal.Web.Areas.Data.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SchoolPortal.Web.Models.Dtos;
using SchoolPortal.Web.Models.Entities;
using System.Threading.Tasks;
using SchoolPortal.Web.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace SchoolPortal.Web.Areas.Data.Services
{
    public class ClassLevelService : IClassLevelService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ClassLevelService()
        {

        }

        public ClassLevelService(ApplicationUserManager userManager,
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

        public async Task<ClassLevelDetailsDto> ClassLevelDetails(int? id)
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var item = db.ClassLevels.Include(x => x.Subjects).Include(x => x.User).Where(x => x.Id == id);
            // var studnets = db.
            int c = await StudentsCount(id);
            int s = await SubjectCount(id);
            //check user

            var output = item.Select(x => new ClassLevelDetailsDto
            {
                ClassName = x.ClassName,
                Id = x.Id,
                FormTeacher = x.User.Surname + " " + x.User.FirstName + " " + x.User.OtherName,
                //NumberOfSubjects = x.Subjects.Count(),
                NumberOfSubjects = s,
                NumberOfStudents = c,
                UserId = x.UserId,
                Passmark = x.Passmark,
                PromotionByTrial = x.PromotionByTrial,
                ExamScore = x.ExamScore,
                AccessmentScore = x.AccessmentScore,
                ShowClass = x.ShowClass
            });
            var outputmain = await output.FirstOrDefaultAsync();

            return outputmain;
        }

        public async Task<List<ClassLevelListDto>> ClassLevelList()
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var item = db.ClassLevels.Include(x => x.User).OrderBy(x => x.ClassName).Where(x=>x.ShowClass ==true);

            var output = item.Select(x => new ClassLevelListDto
            {
                ClassLevelName = x.ClassName,
                ShowClass = x.ShowClass,
                Id = x.Id,
                userId = x.UserId,
                FormTeacher = x.User.Surname + " " + x.User.FirstName + " " + x.User.OtherName
            });
            return await output.ToListAsync();
        }


        public async Task<List<ClassLevelListDto>> AllClassLevelList()
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var item = db.ClassLevels.Include(x => x.User).OrderBy(x => x.ClassName);

            var output = item.Select(x => new ClassLevelListDto
            {
                ClassLevelName = x.ClassName,
                Id = x.Id,
                userId = x.UserId,
                ShowClass =x.ShowClass,
                FormTeacher = x.User.Surname + " " + x.User.FirstName + " " + x.User.OtherName
            });
            return await output.ToListAsync();
        }


        public async Task Create(ClassLevel model)
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            model.SessionId = currentSession.Id;
            db.ClassLevels.Add(model);
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
                tracker.Note = tracker.FullName + " " + "Added a class level";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }

        }

        public async Task Delete(int? id)
        {
            var TIME = await db.TimeTables.FirstOrDefaultAsync(x => x.ClassLevelId == id);
            db.TimeTables.Remove(TIME);
            await db.SaveChangesAsync();


            var subjects = await db.Subjects.Include(x => x.ClassLevel).Where(x => x.ClassLevelId == id).ToListAsync();
            foreach (var sub in subjects)
            {
                var subitem = await db.Subjects.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == sub.Id);
                db.Subjects.Remove(subitem);
                await db.SaveChangesAsync();
            }

            var item = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == id);
            db.ClassLevels.Remove(item);
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
                tracker.Note = tracker.FullName + " " + "deleted a class level";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();

            }


        }

        public async Task Edit(ClassLevel models)
        {
            db.Entry(models).State = EntityState.Modified;
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
                tracker.Note = tracker.FullName + " " + "edited a class level";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }

        }

        public async Task<ClassLevel> Get(int? id)
        {
            var item = await db.ClassLevels.Include(x => x.Subjects).Include(x => x.StaffProfile).FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }



        public async Task GradingOption(int? id)
        {
            var enrolledSubjects = db.EnrolledSubjects.Include(x => x.Enrollments).Where(x => x.Enrollments.ClassLevelId == id && x.Enrollments.Session.Status == SessionStatus.Current);
            foreach (var item in await enrolledSubjects.Where(x => x.GradingOption == Models.Entities.GradingOption.NONE).ToListAsync())
            {

                var currentClass = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == id);
                if (currentClass.ClassName.ToUpper().Contains("JSS"))
                {
                    item.GradingOption = Models.Entities.GradingOption.JSS;

                }
                else if (currentClass.ClassName.Substring(0, 2).Contains("PG"))
                {
                    item.GradingOption = Models.Entities.GradingOption.PG;
                }
                else if (currentClass.ClassName.Substring(0, 3).Contains("SSS"))
                {
                    item.GradingOption = Models.Entities.GradingOption.SSS;
                }
                else if (currentClass.ClassName.Substring(0, 3).Contains("PRI"))
                {
                    item.GradingOption = Models.Entities.GradingOption.PRI;
                }
                else if (currentClass.ClassName.Substring(0, 3).Contains("NUR"))
                {
                    item.GradingOption = Models.Entities.GradingOption.NUR;
                }
                else if (currentClass.ClassName.Substring(0, 3).Contains("PRE"))
                {
                    item.GradingOption = Models.Entities.GradingOption.PRE;
                }

                db.Entry(item).State = EntityState.Modified;
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
                    tracker.Note = tracker.FullName + " " + "Updated Grading Options";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }


            }


        }

        public async Task<List<PromotionStudentsDto>> StudentsList()
        {
            IQueryable<StudentProfile> allStudents = from s in db.StudentProfiles
                                       .Include(x => x.user).Where(c => c.Graduate == false)
                                               select s;

            
            var output = allStudents.Select(x => new PromotionStudentsDto
            {
                UserName = x.user.UserName,
                ProfileId = x.Id,
                StudentRegNumber = x.StudentRegNumber,
                FullName = x.user.Surname + " " + x.user.FirstName + " " + x.user.OtherName,
                PhoneNumber = x.user.Phone,
                EmailAddress = x.user.Email,
                UserId = x.user.Id,
                ClassName = ClassEnrolled(x.Id)

            });
            return await output.ToListAsync();
        }
        public string ClassEnrolled(int studentId)
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            string result = "";
            var item = db.Enrollments.Include(x=>x.ClassLevel).FirstOrDefault(x => x.StudentProfileId == studentId && x.SessionId == currentSession.Id);
            if(item != null)
            {
                result = item.ClassLevel.ClassName;
            }
            return result;
        }

        public async Task<List<ClassStudentsDto>> Students(int? id)
        {

            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);

            var enrolledStudentsId = db.Enrollments.Include(x=>x.User).Include(x => x.StudentProfile.user).Include(c => c.Session).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.Session.SessionYear == currentSession.SessionYear).Select(u => u.StudentProfileId);
            var allStudents = db.StudentProfiles.Include(x => x.user).Where(x => x.user.Status == EntityStatus.Active).Where(x => enrolledStudentsId.Contains(x.Id)).Select(x => x.Id);
            var enrolledStudents = db.Enrollments.Include(e => e.StudentProfile).Include(e => e.StudentProfile.user).Include(x => x.Session).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(s => s.ClassLevelId == id && s.Session.Status == SessionStatus.Current);

            var output = enrolledStudents.Select(x => new ClassStudentsDto
            {
                UserName = x.StudentProfile.user.UserName,
                ProfileId = x.StudentProfileId,
                StudentRegNumber = x.StudentProfile.StudentRegNumber,
                FullName = x.StudentProfile.user.Surname + " " + x.StudentProfile.user.FirstName + " " + x.StudentProfile.user.OtherName,
                PhoneNumber = x.StudentProfile.user.Phone,
                EmailAddress = x.StudentProfile.user.Email,
                ClassId = x.ClassLevelId,
                EnrollmentId = x.Id,
                UserId = x.StudentProfile.user.Id
                
            });
            return await output.ToListAsync();
        }

        public async Task<string> MoveStudents(int? id, int? sessionid, int classid)
        {
            int t = 0;
            var currentSession = db.Sessions.FirstOrDefault(x => x.Id == sessionid);

            var enrolledStudentsId = db.Enrollments.Include(x=>x.User).Include(x => x.StudentProfile.user).Include(c => c.Session).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.Session.SessionYear == currentSession.SessionYear).Select(u => u.StudentProfileId);
            var allStudents = db.StudentProfiles.Include(x => x.user).Where(x => x.user.Status == EntityStatus.Active).Where(x => enrolledStudentsId.Contains(x.Id)).Select(x => x.Id);
            var enrolledStudents = db.Enrollments.Include(x=>x.User).Include(e => e.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.Session).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(s => s.ClassLevelId == id && s.Session.Status == SessionStatus.Current);

            foreach (var i in enrolledStudents.ToList())
            {
                //Sessions to enroll for
                var sessionsToEnroll = db.Sessions.Where(x => x.SessionYear == currentSession.SessionYear);

                var setting = db.Settings.FirstOrDefault();
                //for all terms in the session

                foreach (var term in sessionsToEnroll.ToList())
                {
                    try
                    {
                        bool check = CheckNewEnrollment(i.StudentProfileId, term.Id);
                        if (check == false)
                        {



                            Enrollment enrollment = db.Enrollments.Create();

                            //other data for enrollment table
                            enrollment.StudentProfileId = i.StudentProfileId;
                            enrollment.SessionId = term.Id;
                            enrollment.ClassLevelId = classid;
                            enrollment.EnrollmentRemark = setting.DefaultEnrollmentRemark;
                            db.Enrollments.Add(enrollment);
                            await db.SaveChangesAsync();
                            t++;
                            //Get all subjects for the class level selected
                            var subjects = db.Subjects.Where(s => s.ClassLevelId == enrollment.ClassLevelId);
                            var currentlevel = db.ClassLevels.FirstOrDefault(x => x.Id == id).ClassName;

                            //Add Subjects to the student
                            foreach (var item in subjects.ToList())
                            {
                                EnrolledSubject enrolledSubject = db.EnrolledSubjects.Create();
                                enrolledSubject.SubjectId = item.Id;
                                enrolledSubject.EnrollmentId = enrollment.Id;
                                enrolledSubject.TotalScore = 0;
                                enrolledSubject.ExamScore = 0;
                                enrolledSubject.TestScore = 0;
                                enrolledSubject.Assessment = 0;
                                enrolledSubject.ClassExercise = 0;
                                enrolledSubject.Project = 0;
                                enrolledSubject.TestScore2 = 0;
                                enrolledSubject.TotalCA = 0;

                                enrolledSubject.IsOffered = true;

                                db.EnrolledSubjects.Add(enrolledSubject);
                                await db.SaveChangesAsync();

                            }
                        }
                    }
                    catch (Exception r)
                    {

                    }

                }
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
                    tracker.Note = tracker.FullName + " " + "Moved a student to another class";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
               
            }
            return t + "done";
        }



        public async Task<int> StudentsCount(int? id)
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);

            var enrolledStudentsId = db.Enrollments.Include(x=>x.User).Include(x => x.StudentProfile.user).Include(c => c.Session).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.Session.SessionYear == currentSession.SessionYear).Select(u => u.StudentProfileId);
            var allStudents = db.StudentProfiles.Include(x => x.user).Where(x => x.user.Status == EntityStatus.Active).Where(x => enrolledStudentsId.Contains(x.Id)).Select(x => x.Id);
            var enrolledStudents = db.Enrollments.Include(x=>x.User).Include(e => e.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.Session).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(s => s.ClassLevelId == id && s.Session.Status == SessionStatus.Current);
            return await enrolledStudents.CountAsync();
        }

        public async Task<int> SubjectCount(int? id)
        {
            var subject = db.Subjects.Include(x => x.ClassLevelId).Where(x => x.ClassLevelId == id && x.ShowSubject == true);
            return await subject.CountAsync();
        }


        public bool CheckNewEnrollment(int stId, int sessId)
        {
            var enrol = db.Enrollments.Include(x => x.StudentProfile).FirstOrDefault(x => x.StudentProfileId == stId && x.SessionId == sessId);
            if (enrol != null)
            {
                return true;
            }
            return false;
        }
    }
}