using SchoolPortal.Web.Areas.Data.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SchoolPortal.Web.Models.Dtos;
using SchoolPortal.Web.Models.Entities;
using System.Threading.Tasks;
using SchoolPortal.Web.Models;
using System.Data;
using System.Data.Entity;
using SchoolPortal.Web.Areas.Service;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace SchoolPortal.Web.Areas.Data.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IResultService _resultService = new ResultService();

        public EnrollmentService()
        {

        }

        public EnrollmentService(ApplicationUserManager userManager,
           ApplicationRoleManager roleManager,
             ResultService resultService)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            _resultService = resultService;
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

        public async Task Create(Enrollment model)
        {
            bool check = CheckNewEnrollment(model.StudentProfileId, model.SessionId);
            if (check == false)
            {
                db.Enrollments.Add(model);
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
                    tracker.Note = tracker.FullName + " " + "Added an enrollment";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
            }
        }


        public async Task UpdateEnrollmentStatus(int? classId)
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);

            var sub = db.EnrolledSubjects.FirstOrDefault();
            var setting = await db.Settings.FirstOrDefaultAsync();
            //var enrolment = await db.Enrollments.Include(x => x.ClassLevel).Where(c => c.ClassLevelId == classId).ToListAsync();

            IQueryable<Enrollment> enrolment = from s in db.Enrollments.Include(x => x.User).Include(x => x.StudentProfile.user)
                                          .Include(x => x.ClassLevel).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(c => c.ClassLevelId == classId && c.SessionId == currentSession.Id)
                                               select s;

            foreach (var student in enrolment)
            {
                try
                {


                    if (student.EnrollmentRemark != null || student.EnrollmentRemark != "")
                    {
                        var getuserenrolment = await db.Enrollments.FirstOrDefaultAsync(x => x.Id == student.Id);
                        getuserenrolment.EnrollmentRemark = setting.DefaultEnrollmentRemark;
                        db.Entry(getuserenrolment).State = EntityState.Modified;
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
                            tracker.Note = tracker.FullName + " " + "Updated Enrollment Status";
                            //db.Trackers.Add(tracker);
                            await db.SaveChangesAsync();
                        }

                    }


                }
                catch (Exception e)
                {

                }

            }
        }

        public async Task Delete(int? id)
        {
            var item = await db.Enrollments.FirstOrDefaultAsync(x => x.Id == id);
            if (item != null)
            {
                db.Enrollments.Remove(item);
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
                    tracker.Note = tracker.FullName + " " + "Deleted An Enrollment";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }

            }
        }

        public async Task Edit(Enrollment models)
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
                tracker.Note = tracker.FullName + " " + "Edited An Enrollment";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }

        }


        public async Task<List<EnrolledStudentsDto>> EnrolledStudents(string searchString, string currentFilter, int? page)
        {
            var session = db.Sessions.OrderByDescending(x => x.Id);
            if (session != null)
            {
                var sess = db.Sessions;
                var allStudents = db.StudentProfiles.Include(x => x.user);
                var currentSession = session.FirstOrDefault(x => x.Status == SessionStatus.Current);
                var enrolledStudents = db.Enrollments.Include(x => x.User).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(p => p.ClassLevel).Include(c => c.Session).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.Session.Id == currentSession.Id);


                var students = enrolledStudents.Select(x => new EnrolledStudentsDto
                {

                    UserName = x.StudentProfile.user.UserName,
                    ProfileId = x.StudentProfile.Id,
                    Surname = x.StudentProfile.user.Surname,
                    FirstName = x.StudentProfile.user.FirstName,
                    OtherName = x.StudentProfile.user.OtherName,
                    EnrollmentId = x.Id,
                    StudentRegNumberPin = x.StudentProfile.StudentRegNumber,
                    EnrolledClass = x.ClassLevel.ClassName,
                    UserId = x.StudentProfile.UserId
                }).ToList();

                if (!String.IsNullOrEmpty(searchString))
                {
                    if (CountString(searchString) > 1)
                    {
                        string[] searchStringCollection = searchString.Split(' ');
                        List<EnrolledStudentsDto> studentlist = new List<EnrolledStudentsDto>();

                        foreach (var item in searchStringCollection)
                        {
                            students = students.Where(s => (s.Surname != null) && s.Surname.ToUpper().Contains(item.ToUpper())
                            || (s.FirstName != null) && s.FirstName.ToUpper().Contains(item.ToUpper())
                            || (s.OtherName != null) && s.OtherName.ToUpper().Contains(item.ToUpper())
                            || (s.StudentRegNumberPin != null) && s.StudentRegNumberPin.ToUpper().Contains(item.ToUpper())
                            || (s.UserName != null) && s.UserName.ToUpper().Contains(item.ToUpper())).ToList();

                            studentlist.AddRange(students);
                        }
                        students = studentlist.ToList();
                    }
                    else
                    {
                        students = students.Where(s => (s.Surname != null) && s.Surname.ToUpper().Contains(searchString.ToUpper())
                        || (s.FirstName != null) && s.FirstName.ToUpper().Contains(searchString.ToUpper())
                        || (s.OtherName != null) && s.OtherName.ToUpper().Contains(searchString.ToUpper())
                        || (s.StudentRegNumberPin != null) && s.StudentRegNumberPin.ToUpper().Contains(searchString.ToUpper())
                        || (s.UserName != null) && s.UserName.ToUpper().Contains(searchString.ToUpper())).ToList();
                    }

                }

                return students.OrderByDescending(x => x.Surname).ToList();

            }
            return null;
        }

        public async Task<List<EnrollStudentsDto>> Enrollment(string searchString, string currentFilter, int? page)
        {

            var session = db.Sessions.OrderByDescending(x => x.Id);
            if (session != null)
            {

                var currentSession = await session.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);

                var allStudents = db.StudentProfiles.Include(x => x.user).Where(x => x.user.Status == EntityStatus.Active);
                var allStudentsd = db.StudentProfiles.Include(x => x.user).Where(x => x.user.Status == EntityStatus.Active && x.user.UserName.ToUpper() == "OKOROVAL").ToList();
                var enrolledStudents = db.Enrollments.Include(x => x.User).Include(c => c.Session).Include(x => x.StudentProfile.user).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.Session.Id == currentSession.Id).Select(u => u.StudentProfileId).ToList();
                var enrolledStudentskk = db.Enrollments.Include(x => x.User).Include(x => x.StudentProfile.user).Include(c => c.Session).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.Session.SessionYear == currentSession.SessionYear && x.StudentProfile.user.UserName.ToUpper() == "OKOROVAL").Select(u => u.StudentProfileId).ToList();
                var yetToEnroll = allStudents.Where(x => !enrolledStudents.Contains(x.Id));
                var df = yetToEnroll.Where(x => x.user.UserName.ToUpper() == "OKOROVAL").ToList();
                var studentsto = from s in yetToEnroll
                                 select s;
                var students = studentsto.Select(x => new EnrollStudentsDto
                {

                    UserName = x.user.UserName,
                    ProfileId = x.Id,
                    Surname = x.user.Surname,
                    FirstName = x.user.FirstName,
                    OtherName = x.user.OtherName,
                    StudentRegNumberPin = x.StudentRegNumber,
                    UserId = x.UserId

                });
                if (!String.IsNullOrEmpty(searchString))
                {
                    if (CountString(searchString) > 1)
                    {
                        string[] searchStringCollection = searchString.Split(' ');

                        foreach (var item in searchStringCollection)
                        {
                            students = students.Where(s => s.Surname.ToUpper().Contains(item.ToUpper())
                            || s.FirstName.ToUpper().Contains(item.ToUpper())
                            || s.OtherName.ToUpper().Contains(item.ToUpper())
                            || s.UserName.ToUpper().Contains(item.ToUpper())
                            || s.StudentRegNumberPin.ToUpper().Contains(item.ToUpper()));
                        }
                    }
                    else
                    {
                        students = students.Where(s => s.Surname.ToUpper().Contains(searchString.ToUpper())
                        || s.FirstName.ToUpper().Contains(searchString.ToUpper())
                        || s.OtherName.ToUpper().Contains(searchString.ToUpper())
                        || s.UserName.ToUpper().Contains(searchString.ToUpper())
                        || s.StudentRegNumberPin.ToUpper().Contains(searchString.ToUpper()));
                    }

                }
                return await students.ToListAsync();
            }
            return null;
        }



        public async Task<List<EnrollStudentsDto>> EnrollmentAutoSearch()
        {

            var session = db.Sessions.OrderByDescending(x => x.Id);
            if (session != null)
            {

                var currentSession = await session.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);

                var allStudents = db.StudentProfiles.Include(x => x.user).Where(x => x.user.Status == EntityStatus.Active);
                var enrolledStudents = db.Enrollments.Include(x => x.User).Include(x => x.StudentProfile.user).Include(c => c.Session).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.Session.SessionYear == currentSession.SessionYear).Select(u => u.StudentProfileId).ToList();
                var yetToEnroll = allStudents.Where(x => !enrolledStudents.Contains(x.Id));
                var studentsto = from s in yetToEnroll
                                 select s;
                var students = studentsto.Select(x => new EnrollStudentsDto
                {

                    UserName = x.user.UserName,
                    ProfileId = x.Id,
                    Surname = x.user.Surname,
                    FirstName = x.user.FirstName,
                    OtherName = x.user.OtherName,
                    StudentRegNumberPin = x.StudentRegNumber,
                    UserId = x.UserId

                });

                return await students.ToListAsync();
            }
            return null;
        }

        //enro with session class userid

        public async Task<string> PromotionEnrol(string sess, int classid = 0, int oldclassid = 0, int oldsessionid = 0)
        {
            var newclass = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == classid);
            string cname = newclass.ClassName;
            var oldclass = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == oldclassid);
            string oldname = oldclass.ClassName;
            string outcome = "empty";
            int count1 = 0;
            int count2 = 0;
            var enrolment = await db.Enrollments.Include(x => x.User).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.Session).Include(x => x.ClassLevel).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.ClassLevelId == oldclassid && x.SessionId == oldsessionid).ToListAsync();
            var passmark = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == oldclassid);
            var passedEnroled = enrolment.Where(x => x.CummulativeAverageScore >= passmark.Passmark);
            var failedEnroled = enrolment.Where(x => x.CummulativeAverageScore < passmark.Passmark);
            var setting = await db.Settings.FirstOrDefaultAsync();
            string resultPassmark = "";
            if (setting.PromotionByMathsAndEng == true)
            {
                //promote those that pass english
                foreach (var i in enrolment)
                {
                    var promoted = PrintService.PromotionSubject(i.StudentProfileId);
                    if (promoted == "(PROMOTED)")
                    {
                        //bool checkenro = CheckEnrollment(i.StudentProfileId, classid);
                        //if (checkenro == false)
                        //{



                        //Get current session
                        var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.SessionYear == sess);
                        //Sessions to enroll for
                        var sessionsToEnroll = db.Sessions.Where(x => x.SessionYear == currentSession.SessionYear);
                        var sssGradingOption = GradingOption.SSS;
                        var jssGradingOption = GradingOption.JSS;
                        var priGradingOption = GradingOption.PRI;
                        var nurGradingOption = GradingOption.NUR;
                        var preGradingOption = GradingOption.PRE;
                        var pgGradingOption = GradingOption.PG;

                        //for all terms in the session
                        foreach (var term in sessionsToEnroll.ToList())
                        {
                            bool checkenro1 = CheckNewEnrollment(i.StudentProfileId, term.Id);
                            if (checkenro1 == false)
                            {

                                try
                                {
                                    Enrollment enrollment = db.Enrollments.Create();

                                    //other data for enrollment table
                                    enrollment.StudentProfileId = i.StudentProfileId;
                                    enrollment.SessionId = term.Id;
                                    enrollment.ClassLevelId = classid;
                                    enrollment.EnrollmentRemark = setting.DefaultEnrollmentRemark;
                                    db.Enrollments.Add(enrollment);
                                    await db.SaveChangesAsync();
                                    count1++;
                                    //Get all subjects for the class level selected
                                    var subjects = db.Subjects.Where(s => s.ClassLevelId == enrollment.ClassLevelId);
                                    var currentlevel = db.ClassLevels.FirstOrDefault(x => x.Id == classid).ClassName;

                                    //Add Subjects to the student
                                    foreach (var item in subjects.ToList())
                                    {
                                        EnrolledSubject enrolledSubject = db.EnrolledSubjects.Create();
                                        enrolledSubject.SubjectId = item.Id;
                                        enrolledSubject.EnrollmentId = enrollment.Id;
                                        enrolledSubject.TotalScore = 0;
                                        enrolledSubject.ExamScore = 0;
                                        enrolledSubject.TestScore = 0;
                                        enrolledSubject.TestScore2 = 0;
                                        enrolledSubject.Project = 0;
                                        enrolledSubject.ClassExercise = 0;
                                        enrolledSubject.Assessment = 0;
                                        enrolledSubject.TotalCA = 0;

                                        enrolledSubject.IsOffered = false;

                                        if (currentlevel.Contains("SSS"))
                                        {
                                            enrolledSubject.GradingOption = sssGradingOption;
                                        }
                                        else if (currentlevel.Contains("JSS"))
                                        {
                                            enrolledSubject.GradingOption = jssGradingOption;
                                        }
                                        else if (currentlevel.Contains("PRE"))
                                        {
                                            enrolledSubject.GradingOption = preGradingOption;
                                        }
                                        else if (currentlevel.Contains("PRI"))
                                        {
                                            enrolledSubject.GradingOption = priGradingOption;
                                        }
                                        else if (currentlevel.Contains("NUR"))
                                        {
                                            enrolledSubject.GradingOption = nurGradingOption;
                                        }
                                        else if (currentlevel.Contains("PG"))
                                        {
                                            enrolledSubject.GradingOption = pgGradingOption;
                                        }
                                        db.EnrolledSubjects.Add(enrolledSubject);
                                        await db.SaveChangesAsync();
                                    }
                                }
                                catch (Exception e)
                                {

                                }
                            }

                        }

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
                            tracker.Note = tracker.FullName + " " + "Promoted Students";
                            //db.Trackers.Add(tracker);
                            await db.SaveChangesAsync();
                        }

                        //}
                    }

                    ////if student was not promoted
                    if (promoted == "(NOT PROMOTED)")
                    {
                        //bool checkenro = CheckEnrollment(i.StudentProfileId, oldclassid);
                        //if (checkenro == false)
                        //{



                        //Get current session
                        var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.SessionYear == sess);
                        //Sessions to enroll for
                        var sessionsToEnroll = db.Sessions.Where(x => x.SessionYear == currentSession.SessionYear);
                        var sssGradingOption = GradingOption.SSS;
                        var jssGradingOption = GradingOption.JSS;
                        var priGradingOption = GradingOption.PRI;
                        var nurGradingOption = GradingOption.NUR;
                        var preGradingOption = GradingOption.PRE;
                        var pgGradingOption = GradingOption.PG;

                        //for all terms in the session
                        foreach (var term in sessionsToEnroll.ToList())
                        {
                            bool checkenro1 = CheckNewEnrollment(i.StudentProfileId, term.Id);
                            if (checkenro1 == false)
                            {
                                try
                                {
                                    Enrollment enrollment = db.Enrollments.Create();

                                    //other data for enrollment table
                                    enrollment.StudentProfileId = i.StudentProfileId;
                                    enrollment.SessionId = term.Id;
                                    enrollment.ClassLevelId = oldclassid;
                                    enrollment.EnrollmentRemark = setting.DefaultEnrollmentRemark;
                                    db.Enrollments.Add(enrollment);
                                    await db.SaveChangesAsync();
                                    count2++;
                                    //Get all subjects for the class level selected
                                    var subjects = db.Subjects.Where(s => s.ClassLevelId == enrollment.ClassLevelId);
                                    var currentlevel = db.ClassLevels.FirstOrDefault(x => x.Id == classid).ClassName;

                                    //Add Subjects to the student
                                    foreach (var item in subjects.ToList())
                                    {
                                        EnrolledSubject enrolledSubject = db.EnrolledSubjects.Create();
                                        enrolledSubject.SubjectId = item.Id;
                                        enrolledSubject.EnrollmentId = enrollment.Id;
                                        enrolledSubject.TotalScore = 0;
                                        enrolledSubject.ExamScore = 0;
                                        enrolledSubject.TestScore = 0;
                                        enrolledSubject.TestScore2 = 0;
                                        enrolledSubject.Project = 0;
                                        enrolledSubject.ClassExercise = 0;
                                        enrolledSubject.Assessment = 0;
                                        enrolledSubject.TotalCA = 0;

                                        enrolledSubject.IsOffered = false;

                                        if (currentlevel.Contains("SSS"))
                                        {
                                            enrolledSubject.GradingOption = sssGradingOption;
                                        }
                                        else if (currentlevel.Contains("JSS"))
                                        {
                                            enrolledSubject.GradingOption = jssGradingOption;
                                        }
                                        else if (currentlevel.Contains("PRE"))
                                        {
                                            enrolledSubject.GradingOption = preGradingOption;
                                        }
                                        else if (currentlevel.Contains("PRI"))
                                        {
                                            enrolledSubject.GradingOption = priGradingOption;
                                        }
                                        else if (currentlevel.Contains("NUR"))
                                        {
                                            enrolledSubject.GradingOption = nurGradingOption;
                                        }
                                        else if (currentlevel.Contains("PG"))
                                        {
                                            enrolledSubject.GradingOption = pgGradingOption;
                                        }
                                        db.EnrolledSubjects.Add(enrolledSubject);
                                        await db.SaveChangesAsync();
                                    }
                                }
                                catch (Exception e)
                                {

                                }
                            }

                        }
                        //}

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
                            tracker.Note = tracker.FullName + " " + "Promoted others and push student that was not promoted to previous class";
                            //db.Trackers.Add(tracker);
                            await db.SaveChangesAsync();
                        }

                    }

                    ///if 
                    ///

                }
                //promot with pass mark

                count1 = count1 / 3;
                count2 = count2 / 3;
                resultPassmark = count1.ToString() + " were Promoted to " + cname + " and " + count2.ToString() + " not promoted but were moved to thesame class " + oldname;


            }
            else if (setting.PromoteAll != true)
            {
                //promot with pass mark
                foreach (var pro in passedEnroled)
                {
                    //bool checkenro = CheckEnrollment(pro.StudentProfileId, classid);
                    //if (checkenro == false)
                    //{



                    //Get current session
                    var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.SessionYear == sess);
                    //Sessions to enroll for
                    var sessionsToEnroll = db.Sessions.Where(x => x.SessionYear == currentSession.SessionYear);
                    var sssGradingOption = GradingOption.SSS;
                    var jssGradingOption = GradingOption.JSS;
                    var priGradingOption = GradingOption.PRI;
                    var nurGradingOption = GradingOption.NUR;
                    var preGradingOption = GradingOption.PRE;
                    var pgGradingOption = GradingOption.PG;

                    //for all terms in the session
                    foreach (var term in sessionsToEnroll.ToList())
                    {
                        bool checkenro1 = CheckNewEnrollment(pro.StudentProfileId, term.Id);
                        if (checkenro1 == false)
                        {
                            try
                            {
                                Enrollment enrollment = db.Enrollments.Create();

                                //other data for enrollment table
                                enrollment.StudentProfileId = pro.StudentProfileId;
                                enrollment.SessionId = term.Id;
                                enrollment.ClassLevelId = classid;
                                enrollment.EnrollmentRemark = setting.DefaultEnrollmentRemark;
                                db.Enrollments.Add(enrollment);
                                await db.SaveChangesAsync();
                                count1++;
                                //Get all subjects for the class level selected
                                var subjects = db.Subjects.Where(s => s.ClassLevelId == enrollment.ClassLevelId);
                                var currentlevel = db.ClassLevels.FirstOrDefault(x => x.Id == classid).ClassName;

                                //Add Subjects to the student
                                foreach (var item in subjects.ToList())
                                {
                                    EnrolledSubject enrolledSubject = db.EnrolledSubjects.Create();
                                    enrolledSubject.SubjectId = item.Id;
                                    enrolledSubject.EnrollmentId = enrollment.Id;
                                    enrolledSubject.TotalScore = 0;
                                    enrolledSubject.ExamScore = 0;
                                    enrolledSubject.TestScore = 0;
                                    enrolledSubject.TestScore2 = 0;
                                    enrolledSubject.Project = 0;
                                    enrolledSubject.ClassExercise = 0;
                                    enrolledSubject.Assessment = 0;
                                    enrolledSubject.TotalCA = 0;

                                    enrolledSubject.IsOffered = false;

                                    if (currentlevel.Contains("SSS"))
                                    {
                                        enrolledSubject.GradingOption = sssGradingOption;
                                    }
                                    else if (currentlevel.Contains("JSS"))
                                    {
                                        enrolledSubject.GradingOption = jssGradingOption;
                                    }
                                    else if (currentlevel.Contains("PRE"))
                                    {
                                        enrolledSubject.GradingOption = preGradingOption;
                                    }
                                    else if (currentlevel.Contains("PRI"))
                                    {
                                        enrolledSubject.GradingOption = priGradingOption;
                                    }
                                    else if (currentlevel.Contains("NUR"))
                                    {
                                        enrolledSubject.GradingOption = nurGradingOption;
                                    }
                                    else if (currentlevel.Contains("PG"))
                                    {
                                        enrolledSubject.GradingOption = pgGradingOption;
                                    }
                                    db.EnrolledSubjects.Add(enrolledSubject);
                                    await db.SaveChangesAsync();
                                }
                            }
                            catch (Exception e)
                            {

                            }
                        }

                    }
                    //}

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
                        tracker.Note = tracker.FullName + " " + "Promoted student with pass mark";
                        //db.Trackers.Add(tracker);
                        await db.SaveChangesAsync();
                    }

                }

                //promot with pass mark
                foreach (var pro in failedEnroled)
                {
                    //bool checkenro = CheckEnrollment(pro.StudentProfileId, classid);
                    //if (checkenro == false)
                    //{


                    //Get current session
                    var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.SessionYear == sess);
                    //Sessions to enroll for
                    var sessionsToEnroll = db.Sessions.Where(x => x.SessionYear == currentSession.SessionYear);
                    var sssGradingOption = GradingOption.SSS;
                    var jssGradingOption = GradingOption.JSS;
                    var priGradingOption = GradingOption.PRI;
                    var nurGradingOption = GradingOption.NUR;
                    var preGradingOption = GradingOption.PRE;
                    var pgGradingOption = GradingOption.PG;

                    //for all terms in the session
                    foreach (var term in sessionsToEnroll.ToList())
                    {
                        bool checkenro1 = CheckNewEnrollment(pro.StudentProfileId, term.Id);
                        if (checkenro1 == false)
                        {
                            try
                            {
                                Enrollment enrollment = db.Enrollments.Create();

                                //other data for enrollment table
                                enrollment.StudentProfileId = pro.StudentProfileId;
                                enrollment.SessionId = term.Id;
                                enrollment.ClassLevelId = oldclassid;
                                enrollment.EnrollmentRemark = setting.DefaultEnrollmentRemark;
                                db.Enrollments.Add(enrollment);
                                await db.SaveChangesAsync();
                                count2++;
                                //Get all subjects for the class level selected
                                var subjects = db.Subjects.Where(s => s.ClassLevelId == enrollment.ClassLevelId);
                                var currentlevel = db.ClassLevels.FirstOrDefault(x => x.Id == classid).ClassName;

                                //Add Subjects to the student
                                foreach (var item in subjects.ToList())
                                {
                                    EnrolledSubject enrolledSubject = db.EnrolledSubjects.Create();
                                    enrolledSubject.SubjectId = item.Id;
                                    enrolledSubject.EnrollmentId = enrollment.Id;
                                    enrolledSubject.TotalScore = 0;
                                    enrolledSubject.ExamScore = 0;
                                    enrolledSubject.TestScore = 0;
                                    enrolledSubject.TestScore2 = 0;
                                    enrolledSubject.Project = 0;
                                    enrolledSubject.ClassExercise = 0;
                                    enrolledSubject.Assessment = 0;
                                    enrolledSubject.TotalCA = 0;

                                    enrolledSubject.IsOffered = false;

                                    if (currentlevel.Contains("SSS"))
                                    {
                                        enrolledSubject.GradingOption = sssGradingOption;
                                    }
                                    else if (currentlevel.Contains("JSS"))
                                    {
                                        enrolledSubject.GradingOption = jssGradingOption;
                                    }
                                    else if (currentlevel.Contains("PRE"))
                                    {
                                        enrolledSubject.GradingOption = preGradingOption;
                                    }
                                    else if (currentlevel.Contains("PRI"))
                                    {
                                        enrolledSubject.GradingOption = priGradingOption;
                                    }
                                    else if (currentlevel.Contains("NUR"))
                                    {
                                        enrolledSubject.GradingOption = nurGradingOption;
                                    }
                                    else if (currentlevel.Contains("PG"))
                                    {
                                        enrolledSubject.GradingOption = pgGradingOption;
                                    }
                                    db.EnrolledSubjects.Add(enrolledSubject);
                                    await db.SaveChangesAsync();
                                }
                            }
                            catch (Exception e)
                            {

                            }
                        }

                        //}

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
                            tracker.Note = tracker.FullName + " " + "Demoted student";
                            //db.Trackers.Add(tracker);
                            await db.SaveChangesAsync();
                        }

                    }
                }
                count1 = count1 / 3;
                count2 = count2 / 3;
                resultPassmark = count1.ToString() + " were Promoted to " + cname + " and " + count2.ToString() + " not promoted but were moved to thesame class " + oldname;

            }
            else
            {
                //promote all
                foreach (var pro in enrolment)
                {

                    //bool checkenro = CheckEnrollment(pro.StudentProfileId, classid);
                    //if (checkenro == false)
                    //{



                    //Get current session
                    var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.SessionYear == sess);
                    //Sessions to enroll for
                    var sessionsToEnroll = db.Sessions.Where(x => x.SessionYear == currentSession.SessionYear);
                    var sssGradingOption = GradingOption.SSS;
                    var jssGradingOption = GradingOption.JSS;
                    var priGradingOption = GradingOption.PRI;
                    var nurGradingOption = GradingOption.NUR;
                    var preGradingOption = GradingOption.PRE;
                    var pgGradingOption = GradingOption.PG;

                    //for all terms in the session
                    foreach (var term in sessionsToEnroll.ToList())
                    {
                        bool checkenro1 = CheckNewEnrollment(pro.StudentProfileId, term.Id);
                        if (checkenro1 == false)
                        {
                            try
                            {
                                Enrollment enrollment = db.Enrollments.Create();

                                //other data for enrollment table
                                enrollment.StudentProfileId = pro.StudentProfileId;
                                enrollment.SessionId = term.Id;
                                enrollment.ClassLevelId = classid;
                                enrollment.EnrollmentRemark = setting.DefaultEnrollmentRemark;
                                db.Enrollments.Add(enrollment);
                                await db.SaveChangesAsync();
                                count1++;
                                //Get all subjects for the class level selected
                                var subjects = db.Subjects.Where(s => s.ClassLevelId == enrollment.ClassLevelId);
                                var currentlevel = db.ClassLevels.FirstOrDefault(x => x.Id == classid).ClassName;

                                //Add Subjects to the student
                                foreach (var item in subjects.ToList())
                                {
                                    EnrolledSubject enrolledSubject = db.EnrolledSubjects.Create();
                                    enrolledSubject.SubjectId = item.Id;
                                    enrolledSubject.EnrollmentId = enrollment.Id;
                                    enrolledSubject.TotalScore = 0;
                                    enrolledSubject.ExamScore = 0;
                                    enrolledSubject.TestScore = 0;
                                    enrolledSubject.TestScore2 = 0;
                                    enrolledSubject.Project = 0;
                                    enrolledSubject.ClassExercise = 0;
                                    enrolledSubject.Assessment = 0;
                                    enrolledSubject.TotalCA = 0;

                                    enrolledSubject.IsOffered = false;

                                    if (currentlevel.Contains("SSS"))
                                    {
                                        enrolledSubject.GradingOption = sssGradingOption;
                                    }
                                    else if (currentlevel.Contains("JSS"))
                                    {
                                        enrolledSubject.GradingOption = jssGradingOption;
                                    }
                                    else if (currentlevel.Contains("PRE"))
                                    {
                                        enrolledSubject.GradingOption = preGradingOption;
                                    }
                                    else if (currentlevel.Contains("PRI"))
                                    {
                                        enrolledSubject.GradingOption = priGradingOption;
                                    }
                                    else if (currentlevel.Contains("NUR"))
                                    {
                                        enrolledSubject.GradingOption = nurGradingOption;
                                    }
                                    else if (currentlevel.Contains("PG"))
                                    {
                                        enrolledSubject.GradingOption = pgGradingOption;
                                    }
                                    db.EnrolledSubjects.Add(enrolledSubject);
                                    await db.SaveChangesAsync();
                                }
                            }
                            catch (Exception e)
                            {

                            }
                        }

                    }
                    //}

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
                        tracker.Note = tracker.FullName + " " + "Promoted all students/pupils";
                        //db.Trackers.Add(tracker);
                        await db.SaveChangesAsync();
                    }

                }
                count1 = count1 / 3;
                count2 = count2 / 3;
                resultPassmark = count1.ToString() + " were promoted to " + cname;
            }


            return resultPassmark;
        }

        public bool CheckEnrollment(int stId, int cId)
        {
            var enrol = db.Enrollments.Include(x => x.StudentProfile).FirstOrDefault(x => x.StudentProfileId == stId && x.ClassLevelId == cId);
            if (enrol != null)
            {
                return true;
            }
            return false;
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

        public bool CheckNewEnrollment2(int stId, int sessId, int classId)
        {
            var enrol = db.Enrollments.Include(x => x.StudentProfile).FirstOrDefault(x => x.StudentProfileId == stId && x.SessionId == sessId && x.ClassLevelId == classId);
            if (enrol != null)
            {
                return true;
            }
            return false;
        }

        public async Task<string> ChangeToDropoutStudent(int id = 0)
        {
            //fetch old data
            var result = "";
            //Get current session
            var profile = await db.StudentProfiles.FirstOrDefaultAsync(x => x.Id == id);
            if (profile != null)
            {
                var user = await UserManager.FindByIdAsync(profile.UserId);
                user.Status = EntityStatus.Dropout;
                await UserManager.UpdateAsync(user);
                return "successfully";
            }
            return "error";

        }


        public async Task<string> MoveStudent(int Ocid = 0, int ClassLevelId = 0, int id = 0)
        {
            //fetch old data
            var result = "";
            //Get current session
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            //Sessions to enroll for
            var sessionsToEnroll = db.Sessions.Where(x => x.SessionYear == currentSession.SessionYear);
            var sssGradingOption = GradingOption.SSS;
            var jssGradingOption = GradingOption.JSS;
            var priGradingOption = GradingOption.PRI;
            var nurGradingOption = GradingOption.NUR;
            var preGradingOption = GradingOption.PRE;
            var pgGradingOption = GradingOption.PG;
            var setting = db.Settings.FirstOrDefault();
            //for all terms in the session
            //select session 
            if (currentSession.Term.ToLower().Contains("first"))
            {
                sessionsToEnroll = sessionsToEnroll.OrderBy(x => x.Id);
            }
            else if (currentSession.Term.ToLower().Contains("second"))
            {
                sessionsToEnroll = sessionsToEnroll.Where(x => x.Term.ToLower() != "first");
            }
            else if (currentSession.Term.ToLower().Contains("third"))
            {
                sessionsToEnroll = sessionsToEnroll.Where(x => x.Term.ToLower() == "third");
            }

            try
            {
                foreach (var term in sessionsToEnroll.ToList())
                {
                    bool checkenro1 = CheckNewEnrollment2(id, term.Id, ClassLevelId);
                    if (checkenro1 == false)
                    {
                        var enr = await db.Enrollments.Include(x => x.StudentProfile).Include(x => x.Session).FirstOrDefaultAsync(x => x.Id == id);

                        Enrollment enrollment = new Enrollment();

                        //other data for enrollment table
                        enrollment.StudentProfileId = enr.StudentProfileId;
                        enrollment.SessionId = term.Id;
                        enrollment.ClassLevelId = ClassLevelId;
                        enrollment.EnrollmentRemark = setting.DefaultEnrollmentRemark;
                        db.Enrollments.Add(enrollment);
                        await db.SaveChangesAsync();

                        //Get all subjects for the class level selected
                        var subjects = db.Subjects.Where(s => s.ClassLevelId == enrollment.ClassLevelId);
                        var currentlevel = db.ClassLevels.FirstOrDefault(x => x.Id == ClassLevelId).ClassName;

                        try
                        {
                            //Add Subjects to the student
                            foreach (var item in subjects.ToList())
                            {
                                var enrollSub = db.EnrolledSubjects.Include(x => x.Subject).FirstOrDefault(x => x.EnrollmentId == enr.Id && (x.Subject.SubjectName.Contains(item.SubjectName) || x.Subject.SubjectName == item.SubjectName));
                                //var enrollSub = db.EnrolledSubjects.Include(x => x.Enrollments).Include(x => x.Subject).FirstOrDefault(x => x.Enrollments.StudentProfileId == id && x.Enrollments.ClassLevelId == Ocid && x.Enrollments.SessionId == term.Id);
                                if (enrollSub != null)
                                {

                                    EnrolledSubject enrolledSubject = new EnrolledSubject();
                                    enrolledSubject.SubjectId = item.Id;
                                    enrolledSubject.EnrollmentId = enrollment.Id;
                                    enrolledSubject.TotalScore = enrollSub.TotalScore;
                                    enrolledSubject.ExamScore = enrollSub.ExamScore;
                                    enrolledSubject.TestScore = enrollSub.TestScore;
                                    enrolledSubject.TestScore2 = enrollSub.TestScore2;
                                    enrolledSubject.Project = enrollSub.Project;
                                    enrolledSubject.ClassExercise = enrollSub.ClassExercise;
                                    enrolledSubject.Assessment = enrollSub.Assessment;
                                    enrolledSubject.TotalCA = enrollSub.TotalCA;
                                    enrolledSubject.IsOffered = enrollSub.IsOffered;

                                    if (currentlevel.Contains("SSS"))
                                    {
                                        enrolledSubject.GradingOption = sssGradingOption;
                                    }
                                    else if (currentlevel.Contains("JSS"))
                                    {
                                        enrolledSubject.GradingOption = jssGradingOption;
                                    }
                                    else if (currentlevel.Contains("PRE"))
                                    {
                                        enrolledSubject.GradingOption = preGradingOption;
                                    }
                                    else if (currentlevel.Contains("PRI"))
                                    {
                                        enrolledSubject.GradingOption = priGradingOption;
                                    }
                                    else if (currentlevel.Contains("NUR"))
                                    {
                                        enrolledSubject.GradingOption = nurGradingOption;
                                    }
                                    else if (currentlevel.Contains("PG"))
                                    {
                                        enrolledSubject.GradingOption = pgGradingOption;
                                    }
                                    db.EnrolledSubjects.Add(enrolledSubject);
                                    await db.SaveChangesAsync();
                                }
                                else
                                {
                                    EnrolledSubject enrolledSubject = new EnrolledSubject();
                                    enrolledSubject.SubjectId = item.Id;
                                    enrolledSubject.EnrollmentId = enrollment.Id;
                                    enrolledSubject.TotalScore = 0;
                                    enrolledSubject.ExamScore = 0;
                                    enrolledSubject.TestScore = 0;
                                    enrolledSubject.TestScore2 = 0;
                                    enrolledSubject.Project = 0;
                                    enrolledSubject.ClassExercise = 0;
                                    enrolledSubject.Assessment = 0;
                                    enrolledSubject.TotalCA = 0;

                                    enrolledSubject.IsOffered = false;

                                    if (currentlevel.Contains("SSS"))
                                    {
                                        enrolledSubject.GradingOption = sssGradingOption;
                                    }
                                    else if (currentlevel.Contains("JSS"))
                                    {
                                        enrolledSubject.GradingOption = jssGradingOption;
                                    }
                                    else if (currentlevel.Contains("PRE"))
                                    {
                                        enrolledSubject.GradingOption = preGradingOption;
                                    }
                                    else if (currentlevel.Contains("PRI"))
                                    {
                                        enrolledSubject.GradingOption = priGradingOption;
                                    }
                                    else if (currentlevel.Contains("NUR"))
                                    {
                                        enrolledSubject.GradingOption = nurGradingOption;
                                    }
                                    else if (currentlevel.Contains("PG"))
                                    {
                                        enrolledSubject.GradingOption = pgGradingOption;
                                    }
                                    db.EnrolledSubjects.Add(enrolledSubject);
                                    await db.SaveChangesAsync();
                                }


                            }

                            var attend = db.AttendanceDetails.Where(x => x.StudentId == enr.StudentProfileId).ToList();
                            if (attend != null)
                            {
                                foreach (var att in attend)
                                {
                                    AttendanceDetail attends = new AttendanceDetail();
                                    attends.Attendance = att.Attendance;
                                    attends.AttendanceId = att.AttendanceId;
                                    attends.EnrollmentId = att.EnrollmentId;
                                    attends.IsPresent = att.IsPresent;
                                    attends.SessionId = att.SessionId;
                                    attends.StudentId = att.StudentId;
                                    attends.UserId = att.UserId;
                                    db.AttendanceDetails.Add(att);
                                }
                            }

                        }
                        catch (Exception e)
                        {

                        }

                        //remove
                        try
                        {
                            //var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == Models.Entities.SessionStatus.Current);

                            var enrolledStudent = await db.Enrollments.Include(x => x.ClassLevel).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.EnrolledSubjects).FirstOrDefaultAsync(x => x.Id == id);
                            string name = enrolledStudent.StudentProfile.user.Surname + " " + enrolledStudent.StudentProfile.user.FirstName + " " + enrolledStudent.StudentProfile.user.OtherName;
                            string classs = enrolledStudent.ClassLevel.ClassName;
                            try
                            {

                                var sub = db.EnrolledSubjects.Where(x => x.EnrollmentId == enrolledStudent.Id);
                                foreach (var a in sub)
                                {
                                    db.EnrolledSubjects.Remove(a);
                                }

                                var attend = db.AttendanceDetails.Where(x => x.StudentId == enrolledStudent.StudentProfileId).ToList();
                                foreach (var att in attend)
                                {
                                    db.AttendanceDetails.Remove(att);
                                }
                                db.Enrollments.Remove(enrolledStudent);
                                db.SaveChanges();


                            }
                            catch (Exception e)
                            {

                            }

                        }
                        catch (Exception c)
                        {

                        }
                        var cid = await db.Enrollments.FirstOrDefaultAsync(x => x.ClassLevelId == ClassLevelId && x.SessionId == term.Id && x.Id == enrollment.Id);
                        await _resultService.UpdateResult(cid.Id);
                        //await _resultService.UpdateResult(enrollment.Id);
                    }

                }

                result = "true";

            }
            catch (Exception xc)
            {
                result = "false";
            }

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
                tracker.Note = tracker.FullName + " " + "Moved student to another class";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }

            return result;
        }


        public async Task<string> MoveClassStudent(int Ocid = 0, int ClassLevelId = 0)
        {
            //fetch old data

            string nameswitheerror = "";
            int sn = 0;

            //Get current session
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            //Sessions to enroll for
            var sessionsToEnroll = db.Sessions.Where(x => x.SessionYear == currentSession.SessionYear);
            var sssGradingOption = GradingOption.SSS;
            var jssGradingOption = GradingOption.JSS;
            var priGradingOption = GradingOption.PRI;
            var nurGradingOption = GradingOption.NUR;
            var preGradingOption = GradingOption.PRE;
            var pgGradingOption = GradingOption.PG;
            var setting = db.Settings.FirstOrDefault();
            //for all terms in the session
            //select session 
            if (currentSession.Term.ToLower().Contains("first"))
            {
                sessionsToEnroll = sessionsToEnroll.OrderBy(x => x.Id);
            }
            else if (currentSession.Term.ToLower().Contains("second"))
            {
                sessionsToEnroll = sessionsToEnroll.Where(x => x.Term.ToLower() != "first");
            }
            else if (currentSession.Term.ToLower().Contains("third"))
            {
                sessionsToEnroll = sessionsToEnroll.Where(x => x.Term.ToLower() == "third");
            }

            foreach (var term in sessionsToEnroll.ToList())
            {
                var enrolledClassStudent = await db.Enrollments.Include(x => x.User).Include(x => x.ClassLevel).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.EnrolledSubjects).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.ClassLevelId == Ocid && x.SessionId == term.Id).ToListAsync();
                foreach (var enrolled in enrolledClassStudent)
                {
                    bool checkenro1 = CheckNewEnrollment2(enrolled.Id, term.Id, ClassLevelId);
                    if (checkenro1 == false)
                    {
                        Enrollment enrollment = new Enrollment();

                        //other data for enrollment table
                        enrollment.StudentProfileId = enrolled.StudentProfileId;
                        enrollment.SessionId = term.Id;
                        enrollment.ClassLevelId = ClassLevelId;
                        enrollment.EnrollmentRemark = setting.DefaultEnrollmentRemark;
                        db.Enrollments.Add(enrollment);
                        await db.SaveChangesAsync();

                        //Get all subjects for the class level selected
                        var subjects = db.Subjects.Where(s => s.ClassLevelId == enrollment.ClassLevelId);
                        var currentlevel = db.ClassLevels.FirstOrDefault(x => x.Id == ClassLevelId).ClassName;
                        try
                        {
                            //Add Subjects to the student
                            foreach (var item in subjects.ToList())
                            {
                                var enrollSub = db.EnrolledSubjects.FirstOrDefault(x => x.EnrollmentId == enrolled.Id && (x.Subject.SubjectName.Contains(item.SubjectName) || x.Subject.SubjectName == item.SubjectName));
                                if (enrollSub != null)
                                {

                                    EnrolledSubject enrolledSubject = new EnrolledSubject();
                                    enrolledSubject.SubjectId = item.Id;
                                    enrolledSubject.EnrollmentId = enrollment.Id;
                                    enrolledSubject.TotalScore = enrollSub.TotalScore;
                                    enrolledSubject.ExamScore = enrollSub.ExamScore;
                                    enrolledSubject.TestScore = enrollSub.TestScore;
                                    enrolledSubject.TestScore2 = enrollSub.TestScore2;
                                    enrolledSubject.Project = enrollSub.Project;
                                    enrolledSubject.ClassExercise = enrollSub.ClassExercise;
                                    enrolledSubject.Assessment = enrollSub.Assessment;
                                    enrolledSubject.TotalCA = enrollSub.TotalCA;
                                    enrolledSubject.IsOffered = enrollSub.IsOffered;

                                    if (currentlevel.Contains("SSS"))
                                    {
                                        enrolledSubject.GradingOption = sssGradingOption;
                                    }
                                    else if (currentlevel.Contains("JSS"))
                                    {
                                        enrolledSubject.GradingOption = jssGradingOption;
                                    }
                                    else if (currentlevel.Contains("PRE"))
                                    {
                                        enrolledSubject.GradingOption = preGradingOption;
                                    }
                                    else if (currentlevel.Contains("PRI"))
                                    {
                                        enrolledSubject.GradingOption = priGradingOption;
                                    }
                                    else if (currentlevel.Contains("NUR"))
                                    {
                                        enrolledSubject.GradingOption = nurGradingOption;
                                    }
                                    else if (currentlevel.Contains("PG"))
                                    {
                                        enrolledSubject.GradingOption = pgGradingOption;
                                    }
                                    db.EnrolledSubjects.Add(enrolledSubject);
                                    await db.SaveChangesAsync();
                                }
                                else
                                {
                                    EnrolledSubject enrolledSubject = new EnrolledSubject();
                                    enrolledSubject.SubjectId = item.Id;
                                    enrolledSubject.EnrollmentId = enrollment.Id;
                                    enrolledSubject.TotalScore = 0;
                                    enrolledSubject.ExamScore = 0;
                                    enrolledSubject.TestScore = 0;
                                    enrolledSubject.TestScore2 = 0;
                                    enrolledSubject.Project = 0;
                                    enrolledSubject.ClassExercise = 0;
                                    enrolledSubject.Assessment = 0;
                                    enrolledSubject.TotalCA = 0;

                                    enrolledSubject.IsOffered = false;

                                    if (currentlevel.Contains("SSS"))
                                    {
                                        enrolledSubject.GradingOption = sssGradingOption;
                                    }
                                    else if (currentlevel.Contains("JSS"))
                                    {
                                        enrolledSubject.GradingOption = jssGradingOption;
                                    }
                                    else if (currentlevel.Contains("PRE"))
                                    {
                                        enrolledSubject.GradingOption = preGradingOption;
                                    }
                                    else if (currentlevel.Contains("PRI"))
                                    {
                                        enrolledSubject.GradingOption = priGradingOption;
                                    }
                                    else if (currentlevel.Contains("NUR"))
                                    {
                                        enrolledSubject.GradingOption = nurGradingOption;
                                    }
                                    else if (currentlevel.Contains("PG"))
                                    {
                                        enrolledSubject.GradingOption = pgGradingOption;
                                    }
                                    db.EnrolledSubjects.Add(enrolledSubject);
                                    await db.SaveChangesAsync();
                                }

                            }

                            var attendi = db.AttendanceDetails.Where(x => x.StudentId == enrolled.StudentProfileId).ToList();
                            if (attendi != null)
                            {
                                foreach (var att in attendi)
                                {
                                    AttendanceDetail attends = new AttendanceDetail();
                                    attends.Attendance = att.Attendance;
                                    attends.AttendanceId = att.AttendanceId;
                                    attends.EnrollmentId = att.EnrollmentId;
                                    attends.IsPresent = att.IsPresent;
                                    attends.SessionId = att.SessionId;
                                    attends.StudentId = att.StudentId;
                                    attends.UserId = att.UserId;
                                    db.AttendanceDetails.Add(att);
                                }
                            }

                        }
                        catch (Exception e)
                        {

                        }

                        //remove
                        try
                        {
                            //var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == Models.Entities.SessionStatus.Current);

                            var enrolledStudent = await db.Enrollments.Include(x => x.ClassLevel).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.EnrolledSubjects).FirstOrDefaultAsync(x => x.Id == enrolled.Id);
                            string name = enrolledStudent.StudentProfile.user.Surname + " " + enrolledStudent.StudentProfile.user.FirstName + " " + enrolledStudent.StudentProfile.user.OtherName;
                            string classs = enrolledStudent.ClassLevel.ClassName;
                            try
                            {

                                var sub = db.EnrolledSubjects.Where(x => x.EnrollmentId == enrolledStudent.Id);
                                foreach (var a in sub)
                                {
                                    db.EnrolledSubjects.Remove(a);
                                }

                                var attend = db.AttendanceDetails.Where(x => x.StudentId == enrolledStudent.StudentProfileId).ToList();
                                foreach (var att in attend)
                                {
                                    db.AttendanceDetails.Remove(att);
                                }
                                db.Enrollments.Remove(enrolledStudent);
                                db.SaveChanges();


                            }
                            catch (Exception e)
                            {

                            }
                        }
                        catch (Exception c) { }
                        var cid = await db.Enrollments.FirstOrDefaultAsync(x => x.ClassLevelId == ClassLevelId && x.SessionId == term.Id && x.Id == enrollment.Id);
                        await _resultService.UpdateResult(cid.Id);
                        nameswitheerror = nameswitheerror + "(" + sn++ + ")" + " has been moved successfully /// <br/>";
                    }
                    else
                    {
                        nameswitheerror = nameswitheerror + "(" + sn++ + ")" + " was not moved /// <br/>";

                    }
                }


            }

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
                tracker.Note = tracker.FullName + " " + "Moved student to another class";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }

            return nameswitheerror;

        }
        public async Task EnrollStudentFromSession(int ClassLevelId = 0, int id = 0, int sid = 0)
        {

            //Get current session
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var enrolment = db.Enrollments.Include(x => x.User).Include(x => x.StudentProfile.user).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.SessionId == currentSession.Id).Select(x => x.StudentProfileId).ToList();
            if (!enrolment.Contains(id))
            {


                //Sessions to enroll for
                var sessionsToEnroll = db.Sessions.Where(x => x.SessionYear == currentSession.SessionYear);
                var sssGradingOption = GradingOption.SSS;
                var jssGradingOption = GradingOption.JSS;
                var priGradingOption = GradingOption.PRI;
                var nurGradingOption = GradingOption.NUR;
                var preGradingOption = GradingOption.PRE;
                var pgGradingOption = GradingOption.PG;
                var setting = db.Settings.FirstOrDefault();
                //for all terms in the session
                //select session 
                if (currentSession.Term.ToLower().Contains("first"))
                {
                    sessionsToEnroll = sessionsToEnroll.OrderBy(x => x.Id);
                }
                else if (currentSession.Term.ToLower().Contains("second"))
                {
                    sessionsToEnroll = sessionsToEnroll.Where(x => x.Term.ToLower() != "first");
                }
                else if (currentSession.Term.ToLower().Contains("third"))
                {
                    sessionsToEnroll = sessionsToEnroll.Where(x => x.Term.ToLower() == "third");
                }

                foreach (var term in sessionsToEnroll.ToList())
                {
                    bool checkenro1 = CheckNewEnrollment(id, term.Id);
                    if (checkenro1 == false)
                    {
                        Enrollment enrollment = db.Enrollments.Create();

                        //other data for enrollment table
                        enrollment.StudentProfileId = id;
                        enrollment.SessionId = term.Id;
                        enrollment.ClassLevelId = ClassLevelId;
                        enrollment.EnrollmentRemark = setting.DefaultEnrollmentRemark;
                        db.Enrollments.Add(enrollment);
                        await db.SaveChangesAsync();

                        //Get all subjects for the class level selected
                        var subjects = db.Subjects.Where(s => s.ClassLevelId == enrollment.ClassLevelId);
                        var currentlevel = db.ClassLevels.FirstOrDefault(x => x.Id == ClassLevelId).ClassName;

                        //Add Subjects to the student
                        foreach (var item in subjects.ToList())
                        {
                            EnrolledSubject enrolledSubject = db.EnrolledSubjects.Create();
                            enrolledSubject.SubjectId = item.Id;
                            enrolledSubject.EnrollmentId = enrollment.Id;
                            enrolledSubject.TotalScore = 0;
                            enrolledSubject.ExamScore = 0;
                            enrolledSubject.TestScore = 0;
                            enrolledSubject.TestScore2 = 0;
                            enrolledSubject.Project = 0;
                            enrolledSubject.ClassExercise = 0;
                            enrolledSubject.Assessment = 0;
                            enrolledSubject.TotalCA = 0;

                            enrolledSubject.IsOffered = false;

                            if (currentlevel.Contains("SSS"))
                            {
                                enrolledSubject.GradingOption = sssGradingOption;
                            }
                            else if (currentlevel.Contains("JSS"))
                            {
                                enrolledSubject.GradingOption = jssGradingOption;
                            }
                            else if (currentlevel.Contains("PRE"))
                            {
                                enrolledSubject.GradingOption = preGradingOption;
                            }
                            else if (currentlevel.Contains("PRI"))
                            {
                                enrolledSubject.GradingOption = priGradingOption;
                            }
                            else if (currentlevel.Contains("NUR"))
                            {
                                enrolledSubject.GradingOption = nurGradingOption;
                            }
                            else if (currentlevel.Contains("PG"))
                            {
                                enrolledSubject.GradingOption = pgGradingOption;
                            }
                            db.EnrolledSubjects.Add(enrolledSubject);
                            await db.SaveChangesAsync();
                        }
                    }
                }

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
                    tracker.Note = tracker.FullName + " " + "Enrolled a student";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
            }

        }

        public async Task EnrollStudent(int ClassLevelId = 0, int id = 0)
        {
            //Get current session
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            //Sessions to enroll for
            var sessionsToEnroll = db.Sessions.Where(x => x.SessionYear == currentSession.SessionYear);
            var sssGradingOption = GradingOption.SSS;
            var jssGradingOption = GradingOption.JSS;
            var priGradingOption = GradingOption.PRI;
            var nurGradingOption = GradingOption.NUR;
            var preGradingOption = GradingOption.PRE;
            var pgGradingOption = GradingOption.PG;
            var setting = db.Settings.FirstOrDefault();
            //for all terms in the session
            //select session 
            if (currentSession.Term.ToLower().Contains("first"))
            {
                sessionsToEnroll = sessionsToEnroll.OrderBy(x => x.Id);
            }
            else if (currentSession.Term.ToLower().Contains("second"))
            {
                sessionsToEnroll = sessionsToEnroll.Where(x => x.Term.ToLower() != "first");
            }
            else if (currentSession.Term.ToLower().Contains("third"))
            {
                sessionsToEnroll = sessionsToEnroll.Where(x => x.Term.ToLower() == "third");
            }

            foreach (var term in sessionsToEnroll.ToList())
            {
                bool checkenro1 = CheckNewEnrollment(id, term.Id);
                if (checkenro1 == false)
                {
                    Enrollment enrollment = db.Enrollments.Create();

                    //other data for enrollment table
                    enrollment.StudentProfileId = id;
                    enrollment.SessionId = term.Id;
                    enrollment.ClassLevelId = ClassLevelId;
                    enrollment.EnrollmentRemark = setting.DefaultEnrollmentRemark;
                    db.Enrollments.Add(enrollment);
                    await db.SaveChangesAsync();

                    //Get all subjects for the class level selected
                    var subjects = db.Subjects.Where(s => s.ClassLevelId == enrollment.ClassLevelId);
                    var currentlevel = db.ClassLevels.FirstOrDefault(x => x.Id == ClassLevelId).ClassName;

                    //Add Subjects to the student
                    foreach (var item in subjects.ToList())
                    {
                        EnrolledSubject enrolledSubject = db.EnrolledSubjects.Create();
                        enrolledSubject.SubjectId = item.Id;
                        enrolledSubject.EnrollmentId = enrollment.Id;
                        enrolledSubject.TotalScore = 0;
                        enrolledSubject.ExamScore = 0;
                        enrolledSubject.TestScore = 0;
                        enrolledSubject.TestScore2 = 0;
                        enrolledSubject.Project = 0;
                        enrolledSubject.ClassExercise = 0;
                        enrolledSubject.Assessment = 0;
                        enrolledSubject.TotalCA = 0;

                        enrolledSubject.IsOffered = false;

                        if (currentlevel.Contains("SSS"))
                        {
                            enrolledSubject.GradingOption = sssGradingOption;
                        }
                        else if (currentlevel.Contains("JSS"))
                        {
                            enrolledSubject.GradingOption = jssGradingOption;
                        }
                        else if (currentlevel.Contains("PRE"))
                        {
                            enrolledSubject.GradingOption = preGradingOption;
                        }
                        else if (currentlevel.Contains("PRI"))
                        {
                            enrolledSubject.GradingOption = priGradingOption;
                        }
                        else if (currentlevel.Contains("NUR"))
                        {
                            enrolledSubject.GradingOption = nurGradingOption;
                        }
                        else if (currentlevel.Contains("PG"))
                        {
                            enrolledSubject.GradingOption = pgGradingOption;
                        }
                        db.EnrolledSubjects.Add(enrolledSubject);
                        await db.SaveChangesAsync();
                    }
                }
            }

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
                tracker.Note = tracker.FullName + " " + "Enrolled a student";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }


        }
        public async Task<OutComeDto> EnrollStudentMain(int ClassLevelId = 0, int id = 0)
        {
            OutComeDto result = new OutComeDto();
            //Get current session
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            //Sessions to enroll for
            var sessionsToEnroll = db.Sessions.Where(x => x.SessionYear == currentSession.SessionYear);
            var sssGradingOption = GradingOption.SSS;
            var jssGradingOption = GradingOption.JSS;
            var priGradingOption = GradingOption.PRI;
            var nurGradingOption = GradingOption.NUR;
            var preGradingOption = GradingOption.PRE;
            var pgGradingOption = GradingOption.PG;
            var setting = db.Settings.FirstOrDefault();
            //for all terms in the session
            //select session 
            if (currentSession.Term.ToLower().Contains("first"))
            {
                sessionsToEnroll = sessionsToEnroll.OrderBy(x => x.Id);
            }
            else if (currentSession.Term.ToLower().Contains("second"))
            {
                sessionsToEnroll = sessionsToEnroll.Where(x => x.Term.ToLower() != "first");
            }
            else if (currentSession.Term.ToLower().Contains("third"))
            {
                sessionsToEnroll = sessionsToEnroll.Where(x => x.Term.ToLower() == "third");
            }

            foreach (var term in sessionsToEnroll.ToList())
            {
                bool checkenro1 = CheckNewEnrollment(id, term.Id);
                if (checkenro1 == false)
                {
                    Enrollment enrollment = db.Enrollments.Create();

                    //other data for enrollment table
                    enrollment.StudentProfileId = id;
                    enrollment.SessionId = term.Id;
                    enrollment.ClassLevelId = ClassLevelId;
                    enrollment.EnrollmentRemark = setting.DefaultEnrollmentRemark;
                    db.Enrollments.Add(enrollment);
                    await db.SaveChangesAsync();

                    //Get all subjects for the class level selected
                    var subjects = db.Subjects.Where(s => s.ClassLevelId == enrollment.ClassLevelId);
                    var currentlevel = db.ClassLevels.FirstOrDefault(x => x.Id == ClassLevelId).ClassName;

                    //Add Subjects to the student
                    foreach (var item in subjects.ToList())
                    {
                        EnrolledSubject enrolledSubject = db.EnrolledSubjects.Create();
                        enrolledSubject.SubjectId = item.Id;
                        enrolledSubject.EnrollmentId = enrollment.Id;
                        enrolledSubject.TotalScore = 0;
                        enrolledSubject.ExamScore = 0;
                        enrolledSubject.TestScore = 0;
                        enrolledSubject.TestScore2 = 0;
                        enrolledSubject.Project = 0;
                        enrolledSubject.ClassExercise = 0;
                        enrolledSubject.Assessment = 0;
                        enrolledSubject.TotalCA = 0;

                        enrolledSubject.IsOffered = false;

                        if (currentlevel.Contains("SSS"))
                        {
                            enrolledSubject.GradingOption = sssGradingOption;
                        }
                        else if (currentlevel.Contains("JSS"))
                        {
                            enrolledSubject.GradingOption = jssGradingOption;
                        }
                        else if (currentlevel.Contains("PRE"))
                        {
                            enrolledSubject.GradingOption = preGradingOption;
                        }
                        else if (currentlevel.Contains("PRI"))
                        {
                            enrolledSubject.GradingOption = priGradingOption;
                        }
                        else if (currentlevel.Contains("NUR"))
                        {
                            enrolledSubject.GradingOption = nurGradingOption;
                        }
                        else if (currentlevel.Contains("PG"))
                        {
                            enrolledSubject.GradingOption = pgGradingOption;
                        }
                        db.EnrolledSubjects.Add(enrolledSubject);
                        await db.SaveChangesAsync();
                    }
                
                     result.Success =false;
                    result.Result = "Already in another class";
                    }

            }

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
                tracker.Note = tracker.FullName + " " + "Enrolled a student";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }

            return result;
        }



        public async Task JustEnrolToClass(int? ClassLevelId = 0, int id = 0, int termid = 0)
        {
            var sssGradingOption = GradingOption.SSS;
            var jssGradingOption = GradingOption.JSS;
            var priGradingOption = GradingOption.PRI;
            var nurGradingOption = GradingOption.NUR;
            var preGradingOption = GradingOption.PRE;
            var pgGradingOption = GradingOption.PG;
            var setting = db.Settings.FirstOrDefault();
            bool checkenro1 = CheckNewEnrollment(id, termid);
            if (checkenro1 == false)
            {
                Enrollment enrollment = db.Enrollments.Create();

                //other data for enrollment table
                enrollment.StudentProfileId = id;
                enrollment.SessionId = termid;
                enrollment.ClassLevelId = ClassLevelId;
                enrollment.EnrollmentRemark = setting.DefaultEnrollmentRemark;
                db.Enrollments.Add(enrollment);
                await db.SaveChangesAsync();

                //Get all subjects for the class level selected
                var subjects = db.Subjects.Where(s => s.ClassLevelId == enrollment.ClassLevelId);
                var currentlevel = db.ClassLevels.FirstOrDefault(x => x.Id == ClassLevelId).ClassName;

                //Add Subjects to the student
                foreach (var item in subjects.ToList())
                {
                    EnrolledSubject enrolledSubject = db.EnrolledSubjects.Create();
                    enrolledSubject.SubjectId = item.Id;
                    enrolledSubject.EnrollmentId = enrollment.Id;
                    enrolledSubject.TotalScore = 0;
                    enrolledSubject.ExamScore = 0;
                    enrolledSubject.TestScore = 0;
                    enrolledSubject.TestScore2 = 0;
                    enrolledSubject.Project = 0;
                    enrolledSubject.ClassExercise = 0;
                    enrolledSubject.Assessment = 0;
                    enrolledSubject.TotalCA = 0;

                    enrolledSubject.IsOffered = false;

                    if (currentlevel.Contains("SSS"))
                    {
                        enrolledSubject.GradingOption = sssGradingOption;
                    }
                    else if (currentlevel.Contains("JSS"))
                    {
                        enrolledSubject.GradingOption = jssGradingOption;
                    }
                    else if (currentlevel.Contains("PRE"))
                    {
                        enrolledSubject.GradingOption = preGradingOption;
                    }
                    else if (currentlevel.Contains("PRI"))
                    {
                        enrolledSubject.GradingOption = priGradingOption;
                    }
                    else if (currentlevel.Contains("NUR"))
                    {
                        enrolledSubject.GradingOption = nurGradingOption;
                    }
                    else if (currentlevel.Contains("PG"))
                    {
                        enrolledSubject.GradingOption = pgGradingOption;
                    }
                    db.EnrolledSubjects.Add(enrolledSubject);
                    await db.SaveChangesAsync();
                }
            }


        }



        public async Task EnrollStudentList(int ClassLevelId = 0, int id = 0, int SessionId = 0)
        {
            //Get current session
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Id == SessionId);
            //Sessions to enroll for
            var sessionsToEnroll = db.Sessions.Where(x => x.SessionYear == currentSession.SessionYear);
            var sssGradingOption = GradingOption.SSS;
            var jssGradingOption = GradingOption.JSS;
            var priGradingOption = GradingOption.PRI;
            var nurGradingOption = GradingOption.NUR;
            var preGradingOption = GradingOption.PRE;
            var pgGradingOption = GradingOption.PG;
            var setting = db.Settings.FirstOrDefault();
            //for all terms in the session
            //select session 
            if (currentSession.Term.ToLower().Contains("first"))
            {
                sessionsToEnroll = sessionsToEnroll.OrderBy(x => x.Id);
            }
            else if (currentSession.Term.ToLower().Contains("second"))
            {
                sessionsToEnroll = sessionsToEnroll.Where(x => x.Term.ToLower() != "first");
            }
            else if (currentSession.Term.ToLower().Contains("third"))
            {
                sessionsToEnroll = sessionsToEnroll.Where(x => x.Term.ToLower() == "third");
            }

            foreach (var term in sessionsToEnroll.ToList())
            {
                bool checkenro1 = CheckNewEnrollment(id, term.Id);
                if (checkenro1 == false)
                {
                    Enrollment enrollment = db.Enrollments.Create();

                    //other data for enrollment table
                    enrollment.StudentProfileId = id;
                    enrollment.SessionId = term.Id;
                    enrollment.ClassLevelId = ClassLevelId;
                    enrollment.EnrollmentRemark = setting.DefaultEnrollmentRemark;
                    db.Enrollments.Add(enrollment);
                    await db.SaveChangesAsync();

                    //Get all subjects for the class level selected
                    var subjects = db.Subjects.Where(s => s.ClassLevelId == enrollment.ClassLevelId);
                    var currentlevel = db.ClassLevels.FirstOrDefault(x => x.Id == ClassLevelId).ClassName;

                    //Add Subjects to the student
                    foreach (var item in subjects.ToList())
                    {
                        EnrolledSubject enrolledSubject = db.EnrolledSubjects.Create();
                        enrolledSubject.SubjectId = item.Id;
                        enrolledSubject.EnrollmentId = enrollment.Id;
                        enrolledSubject.TotalScore = 0;
                        enrolledSubject.ExamScore = 0;
                        enrolledSubject.TestScore = 0;
                        enrolledSubject.TestScore2 = 0;
                        enrolledSubject.Project = 0;
                        enrolledSubject.ClassExercise = 0;
                        enrolledSubject.Assessment = 0;
                        enrolledSubject.TotalCA = 0;

                        enrolledSubject.IsOffered = false;

                        if (currentlevel.Contains("SSS"))
                        {
                            enrolledSubject.GradingOption = sssGradingOption;
                        }
                        else if (currentlevel.Contains("JSS"))
                        {
                            enrolledSubject.GradingOption = jssGradingOption;
                        }
                        else if (currentlevel.Contains("PRE"))
                        {
                            enrolledSubject.GradingOption = preGradingOption;
                        }
                        else if (currentlevel.Contains("PRI"))
                        {
                            enrolledSubject.GradingOption = priGradingOption;
                        }
                        else if (currentlevel.Contains("NUR"))
                        {
                            enrolledSubject.GradingOption = nurGradingOption;
                        }
                        else if (currentlevel.Contains("PG"))
                        {
                            enrolledSubject.GradingOption = pgGradingOption;
                        }
                        db.EnrolledSubjects.Add(enrolledSubject);
                        await db.SaveChangesAsync();
                    }
                }

            }

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
                tracker.Note = tracker.FullName + " " + "Enrolled students";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }


        }



        public async Task<Enrollment> Get(int? id)
        {
            var item = await db.Enrollments.Include(x => x.ClassLevel).Include(x => x.Session).Include(x => x.User).Include(x => x.StudentProfile).FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task<List<Enrollment>> List()
        {
            var items = await db.Enrollments.ToListAsync();
            return items;
        }

        public async Task<string> RemoveStudent(int id = 0)
        {
            Enrollment enrollment = db.Enrollments.Find(id);

            var user = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.Id == enrollment.StudentProfileId);

            var className = db.Enrollments.Include(x => x.ClassLevel).FirstOrDefault(x => x.Id == id).ClassLevel.ClassName;
            var session = db.Sessions.FirstOrDefault(x => x.Id == enrollment.SessionId);
            var sessionsToRemove = db.Sessions.Where(x => x.SessionYear == session.SessionYear);
            var attend = db.AttendanceDetails.Where(x => x.StudentId == user.Id).ToList();
            foreach (var att in attend)
            {
                db.AttendanceDetails.Remove(att);
            }
            db.SaveChanges();
            string fullname = "";
            sessionsToRemove = sessionsToRemove.Where(x => x.Status == SessionStatus.Current);
            foreach (var term in sessionsToRemove.ToList())
            {
                var enrollmentToRemove = db.Enrollments.FirstOrDefault(x => x.SessionId == term.Id && x.StudentProfileId == user.Id);
                if (enrollmentToRemove != null)
                {
                    if (enrollmentToRemove.AverageScore == null)
                    {
                        if (enrollmentToRemove.CummulativeAverageScore == null)
                        {


                            var enrolledSubjects = db.EnrolledSubjects.Where(x => x.EnrollmentId == enrollmentToRemove.Id);

                            if (enrollmentToRemove != null || enrolledSubjects.Count() != 0)
                            {
                                //Remove STudent's enrolled Subjects
                                foreach (var item in enrolledSubjects)
                                {
                                    db.EnrolledSubjects.Remove(item);
                                }

                                //Remove Student's Enrollment
                                db.Enrollments.Remove(enrollmentToRemove);
                                await db.SaveChangesAsync();
                            }
                        }
                    }
                    fullname = GeneralService.StudentorPupil() + " has Result in this Term. Unable to Remove.";
                }
            }
            fullname = user.user.Surname + " " + user.user.FirstName + " " + user.user.OtherName;

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var users = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = users.UserName;
                tracker.FullName = users.Surname + " " + users.FirstName + " " + users.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Unenrolled student";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }


            return fullname;
        }

        //

        public async Task<string> RemoveSingleEnrollmentDirect(int id)
        {
            string fullname = "";
            var enrollmentToRemove = db.Enrollments.FirstOrDefault(x => x.Id == id);
            if (enrollmentToRemove != null)
            {
                if (enrollmentToRemove.AverageScore == null)
                {
                    if (enrollmentToRemove.CummulativeAverageScore == null)
                    {

                        try
                        {
                            var enrolledSubjects = db.EnrolledSubjects.Where(x => x.EnrollmentId == enrollmentToRemove.Id);

                            if (enrollmentToRemove != null || enrolledSubjects.Count() != 0)
                            {
                                //Remove STudent's enrolled Subjects
                                foreach (var item in enrolledSubjects)
                                {
                                    db.EnrolledSubjects.Remove(item);
                                }

                                //Remove Student's Enrollment
                                db.Enrollments.Remove(enrollmentToRemove);
                                await db.SaveChangesAsync();
                                fullname = GeneralService.StudentorPupil() + " has Result in this Term. Unable to Remove.";
                            }
                        }
                        catch (Exception r)
                        {
                            fullname = "error";
                        }

                    }
                }

            }
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
                tracker.Note = tracker.FullName + " " + "Unenrolled a student";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }


            return fullname;
        }

        //remove student from term

        public async Task<string> RemoveStudentFromSelectedTerm(int id = 0, int sessionId = 0)
        {
            Enrollment enrollment = db.Enrollments.Find(id);

            var user = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.Id == enrollment.StudentProfileId);

            var className = db.Enrollments.Include(x => x.ClassLevel).FirstOrDefault(x => x.Id == id).ClassLevel.ClassName;
            //var session = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            //  var sessionsToRemove = db.Sessions.Where(x => x.SessionYear == session.SessionYear);
            var attend = db.AttendanceDetails.Where(x => x.StudentId == user.Id).ToList();
            foreach (var att in attend)
            {
                db.AttendanceDetails.Remove(att);
            }
            db.SaveChanges();

            var enrollmentToRemove = db.Enrollments.FirstOrDefault(x => x.SessionId == sessionId && x.StudentProfileId == user.Id);
            var enrolledSubjects = db.EnrolledSubjects.Where(x => x.EnrollmentId == enrollmentToRemove.Id);

            if (enrollmentToRemove != null || enrolledSubjects.Count() != 0)
            {
                //Remove STudent's enrolled Subjects
                foreach (var item in enrolledSubjects)
                {
                    db.EnrolledSubjects.Remove(item);
                }

                //Remove Student's Enrollment
                db.Enrollments.Remove(enrollmentToRemove);
                await db.SaveChangesAsync();
            }

            string fullname = user.user.Surname + " " + user.user.FirstName + " " + user.user.OtherName;
            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var users = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = users.UserName;
                tracker.FullName = users.Surname + " " + users.FirstName + " " + users.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Unenrolled a student";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }

            return fullname;
        }

        ///
        public int CountString(string searchString)
        {
            int result = 0;

            searchString = searchString.Trim();

            if (searchString == "")
                return 0;

            while (searchString.Contains("  "))
                searchString = searchString.Replace("  ", " ");

            foreach (string y in searchString.Split(' '))

                result++;


            return result;
        }

        public async Task<int> TotalEnrolledStudentByTerm()
        {
            var currentsession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var count = await db.Enrollments.Include(x => x.User).Include(x => x.StudentProfile.user).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.SessionId == currentsession.Id).CountAsync();
            return count;
        }

        public async Task<int> TotalUnEnrolledStudentByTerm()
        {
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);

            var allStudents = db.StudentProfiles.Include(x => x.user).Where(x => x.user.Status == EntityStatus.Active);
            var enrolledStudents = db.Enrollments.Include(c => c.Session).Where(x => x.Session.SessionYear == currentSession.SessionYear).Select(u => u.StudentProfileId).ToList();
            var yetToEnroll = allStudents.Where(x => !enrolledStudents.Contains(x.Id));
            return await yetToEnroll.CountAsync();
        }

        public async Task<StudentProfile> GetStudent(int? id)
        {
            //var item = await db.Enrollments.FirstOrDefaultAsync(x => x.Id == id);
            //var itemd = await db.StudentProfiles.Include(x=>x.user).FirstOrDefaultAsync(x => x.Id == item.StudentProfileId);
            Enrollment enrollment = db.Enrollments.Find(id);

            var user = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.Id == enrollment.StudentProfileId);

            return user;
        }

        public async Task<Enrollment> classLevelFromEnrollmentbyProfileId(int id)
        {
            var classLevel = await db.Enrollments.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.StudentProfileId == id && x.Session.Status == SessionStatus.Current);
            return classLevel;
        }

        public async Task<Enrollment> classLevelFromEnrollmentbyEnrolId(int id)
        {
            var classLevel = await db.Enrollments.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == id && x.Session.Status == SessionStatus.Current);
            return classLevel;
        }

        public async Task<List<Enrollment>> EnrolledStudentBySessionClassId(int sessId, int classId)
        {
            //var enrolledStudents = db.Enrollments.Include(x => x.StudentProfile).Include(c => c.EnrolledSubjects).Where(s => s.ClassLevelId == classId && s.SessionId == sessId);

            IQueryable<Enrollment> enrolledStudents = from s in db.Enrollments
                                                      .Include(x => x.StudentProfile).Include(c => c.EnrolledSubjects)
                                                      .Where(s => s.ClassLevelId == classId && s.SessionId == sessId)
                                                      select s;

            return await enrolledStudents.ToListAsync();
        }

        public async Task<List<EnrolledSubjectDto>> StudentsListBySubIdBySessionId(int subId, int sessionId)
        {
            var classId = await db.Subjects.FirstOrDefaultAsync(x => x.Id == subId);
            if (classId == null)
            {
                return null;
            }

            //Get all students in that class
            var enrolledStudents = db.Enrollments.Include(x => x.User).Include(s => s.EnrolledSubjects).Include(x => x.StudentProfile.user).Include(u => u.StudentProfile).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(s => s.ClassLevelId == classId.ClassLevelId && s.SessionId == sessionId);

            //Check if there is any student in that class
            if (enrolledStudents.Count() == 0)
            {
                //Page not found
                return null;
            }

            var studentsEnrolled = db.EnrolledSubjects.Include(x => x.Enrollments).Include(x => x.Enrollments.ClassLevel).Include(x => x.Enrollments.User).Include(x => x.Enrollments.StudentProfile).Include(x => x.Enrollments.StudentProfile.user).Where(x => x.SubjectId == subId && enrolledStudents.Select(a => a.Id).Contains(x.EnrollmentId)).OrderBy(x => x.Enrollments.User.Surname);
            var output = studentsEnrolled.Select(x => new EnrolledSubjectDto
            {

                Id = x.Id,
                FullName = x.Enrollments.StudentProfile.user.Surname + " " + x.Enrollments.StudentProfile.user.FirstName + " " + x.Enrollments.StudentProfile.user.OtherName,
                Enrollments = x.Enrollments,
                Regnumber = x.Enrollments.StudentProfile.StudentRegNumber,
                StudentId = x.Enrollments.StudentProfileId,
                Subject = x.Subject,
                SubjectId = x.SubjectId,
                EnrollmentId = x.EnrollmentId,
                TestScore = x.TestScore,
                ExamScore = x.ExamScore,
                TotalScore = x.TotalScore,
                GradingOption = x.GradingOption,
                IsOffered = x.IsOffered,
                SortOrder = x.SortOrder,
                Assessment = x.Assessment,
                ClassExercise = x.ClassExercise,
                Project = x.Project,
                TestScore2 = x.TestScore2,
                TotalCA = x.TotalCA


            });
            return await output.OrderBy(x => x.FullName).ToListAsync();

        }


        public async Task<List<EnrolledSubject>> StudentsListBySubIdBySessionIdPreview(int subId, int sessionId)
        {
            var classId = await db.Subjects.FirstOrDefaultAsync(x => x.Id == subId);
            if (classId == null)
            {
                return null;
            }

            //Get all students in that class
            var enrolledStudents = db.Enrollments.Include(x => x.User).Include(s => s.EnrolledSubjects).Include(u => u.StudentProfile).Include(x => x.StudentProfile.user).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(s => s.ClassLevelId == classId.ClassLevelId && s.SessionId == sessionId);

            //Check if there is any student in that class
            if (enrolledStudents.Count() == 0)
            {
                //Page not found
                return null;
            }

            var studentsEnrolled = db.EnrolledSubjects.Include(x => x.Enrollments).Include(x => x.Enrollments.User).Include(x => x.Enrollments.StudentProfile).Include(x => x.Enrollments.StudentProfile.user).Where(x => x.SubjectId == subId && enrolledStudents.Select(a => a.Id).Contains(x.EnrollmentId));
            //var output = studentsEnrolled.Select(x => new EnrolledSubjectDto
            //{

            //    Id = x.Id,
            //    FullName = x.Enrollments.StudentProfile.user.Surname + " " + x.Enrollments.StudentProfile.user.FirstName + " " + x.Enrollments.StudentProfile.user.OtherName,
            //    Enrollments = x.Enrollments,
            //    Subject = x.Subject,
            //    TestScore = x.TestScore,
            //    ExamScore = x.ExamScore,
            //    TotalScore = x.TotalScore,
            //    GradingOption = x.GradingOption


            //});
            return await studentsEnrolled.ToListAsync();

        }

        public string Fullname(int id)
        {
            var user = db.StudentProfiles.Include(x => x.user).FirstOrDefault(x => x.Id == id);
            string name = user.user.Surname + " " + user.user.FirstName + " " + user.user.OtherName;
            return name;
        }

        ///
    }
}