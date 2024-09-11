using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SchoolPortal.Web.Models.Entities;
using System.Net;
using Microsoft.AspNet.Identity;
using SchoolPortal.Web.Models.Dtos;
using Microsoft.AspNet.Identity.Owin;
using DocumentFormat.OpenXml.EMMA;
using SchoolPortal.Web.Areas.Service;

namespace SchoolPortal.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin,FormTeacher,Staff")]
    public class ClassLevelsController : Controller
    {

        #region services
        private ApplicationDbContext db = new ApplicationDbContext();
        private IClassLevelService _classLevelService = new ClassLevelService();
        private IStaffService _staffService = new StaffService();
        private ISubjectService _subjectService = new SubjectService();
        private IDefaulterService _defaulterService = new DefaulterService();
        private IEnrollmentService _enrollmentService = new EnrollmentService();
        private IStudentProfileService _studentService = new StudentProfileService();
        private IAttendanceService _attendanceService = new AttendanceService();
        private IAssignmentService _assignmentService = new AssignmentService();
        private ISessionService _sessionService = new SessionService();
        private IUserManagerService _userService = new UserManagerService();
        private IImageService _imageService = new ImageService();


        public ClassLevelsController()
        {

        }
        public ClassLevelsController(
            ClassLevelService classLevelService,
            UserManagerService userService,
            StaffService staffService,
             SessionService sessionService,
            SubjectService subjectService,
            EnrollmentService enrollmentService,
            StudentProfileService studentService,
           DefaulterService defaulterService,
           AttendanceService attendanceService,
           ApplicationUserManager userManager,
           ImageService imageService,
           AssignmentService assignmentService)
        {
            _classLevelService = classLevelService;
            _staffService = staffService;
            _userService = userService;
            _subjectService = subjectService;
            _imageService = imageService;
            _sessionService = sessionService;
            _defaulterService = defaulterService;
            _enrollmentService = enrollmentService;
            _studentService = studentService;
            UserManager = userManager;
            _attendanceService = attendanceService;
            _assignmentService = assignmentService;
        }
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        #endregion
        // GET: Admin/ClassLevels
        public async Task<ActionResult> Index()
        {
            if (User.IsInRole("SuperAdmin"))
            {
                var items = await _classLevelService.AllClassLevelList();
                return View(items);
            }
            else
            {
                var items = await _classLevelService.ClassLevelList();
                return View(items);
            }

        }

        public async Task<ActionResult> AllIndex()
        {

            var items = await _classLevelService.AllClassLevelList();
            return View(items);
        }


        public async Task<ActionResult> StudentPromotion(int classId = 0, int sessId = 0, int newclassId = 0)
        {
            var uId = User.Identity.GetUserId();
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var ClassData = await db.ClassLevels.Include(x => x.User).Where(x => x.ShowClass == true).FirstOrDefaultAsync(x => x.Id == classId);
            var item = db.ClassLevels.Include(x => x.User).Where(x => x.ShowClass == true).Where(x => x.Id != classId);
            var formTeacher = db.ClassLevels.Include(x => x.User).Where(x => x.UserId == uId);
            item = item.OrderBy(x => x.ClassName);
            if (ClassData == null)
            {
                TempData["error"] = "Unable to Load Class";
                return RedirectToAction("Index");
            }
            ViewBag.classd = ClassData.ClassName;
            ViewBag.classdId = ClassData.Id;



            if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
            {
                item = item.OrderBy(x => x.ClassName);
            }
            else if (formTeacher != null)
            {
                item = item.OrderBy(x => x.ClassName).Where(x => x.UserId == uId || x.Subjects.Select(w => w.UserId).Contains(uId)).OrderBy(x => x.ClassName);
            }
            else
            {
                item = item.Include(x => x.Subjects).Where(x => x.UserId == uId || x.Subjects.Select(w => w.UserId).Contains(uId)).OrderBy(x => x.ClassName);
            }
            var classlevel = item.Select(x => new ClassLevelListDto
            {
                ClassLevelName = x.ClassName,
                Id = x.Id,
                userId = x.UserId,
                FormTeacher = x.User.Surname + " " + x.User.FirstName + " " + x.User.OtherName
            });
            ViewBag.ClassLevelId = new SelectList(classlevel, "Id", "ClassLevelName");

            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session.Where(x => x.Year != currentSession.SessionYear).OrderByDescending(x => x.FullSession), "Id", "FullSession");
            var XXClassData = await db.ClassLevels.Include(x => x.User).Where(x => x.ShowClass == true).FirstOrDefaultAsync(x => x.Id == newclassId);
            if (XXClassData != null)
            {
                ViewBag.classdNN = XXClassData.ClassName;
            }
            List<Enrollment> students = new List<Enrollment>();
            if (newclassId > 0)
            {
                students = db.Enrollments.Include(x => x.User).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.ClassLevel).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.ClassLevelId == newclassId && x.SessionId == sessId).OrderBy(x => x.User.Surname).ToList();

            }

            var output = students.Select(x => new ClassStudentsDto
            {

                EnrollmentId = x.Id,
                FullName = x.StudentProfile.user.Surname + " " + x.StudentProfile.user.FirstName + " " + x.StudentProfile.user.OtherName,
                ClassId = x.ClassLevelId,
                Regnumber = x.StudentProfile.StudentRegNumber,
                ProfileId = x.StudentProfileId,
                UserName = x.StudentProfile.user.UserName,
                UserId = x.StudentProfile.UserId,
                Status = GeneralService.EnrolmentStatus(newclassId, currentSession.Id, x.StudentProfileId)

            });


            return View(output);

        }

        [HttpPost]
        public async Task<ActionResult> StudentPromotionUpdate(List<int> selectedStudents, int classId = 0)
        {
            var uId = User.Identity.GetUserId();
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var ClassData = await db.ClassLevels.Include(x => x.User).Where(x => x.ShowClass == true).FirstOrDefaultAsync(x => x.Id == classId);
            var item = db.ClassLevels.Include(x => x.User).Where(x => x.ShowClass == true).Where(x => x.Id != classId);
            var formTeacher = db.ClassLevels.Include(x => x.User).Where(x => x.UserId == uId);
            item = item.OrderBy(x => x.ClassName);
            if (ClassData == null)
            {
                TempData["error"] = "Unable to Load Class";
                return RedirectToAction("Index");
            }
            //else
            //{
            //    TempData["error"] = "Unable to Load Class";
            //    return RedirectToAction("Students", new { id = classId });
            //}

            //////
            //////
            //////EnrollStudent
            ///
            int countxx = 0;
            foreach (var stn in selectedStudents.ToList())
            {
                var prostn = await db.StudentProfiles.FirstOrDefaultAsync(x => x.Id == stn);
                if (prostn != null)
                {
                    try
                    {
                        await _enrollmentService.EnrollStudent(classId, prostn.Id);
                        countxx++;
                    }
                    catch (Exception d)
                    {

                    }
                }
            }
            TempData["success"] = "Successfully Promoted " + countxx;
            return RedirectToAction("Students", new { id = classId });

        }


        #region Mobile Specification

        public async Task<ActionResult> ClassSubject(int id)
        {
            var clas = await _classLevelService.ClassLevelDetails(id);
            ViewBag.classinfo = clas;
            var sub = await _subjectService.List(id);
            return View(sub);
        }

        #endregion
        #region Student/New students/
        public async Task<ActionResult> Students(int? id)
        {
            var items = await _classLevelService.Students(id);
            var classId = await _classLevelService.Get(id);
            ViewBag.classid = id;
            ViewBag.count = items.Count();
            ViewBag.ClassName = classId.ClassName;
            return View(items);
        }


        public async Task<ActionResult> New(int classId)
        {
            var classlevel = await _classLevelService.ClassLevelList();
            var cinfo = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == classId);
            ViewBag.ClassLevelId = cinfo;
            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName");
            return View();
        }

        // POST: Admin/Sessions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]

        public async Task<ActionResult> New(RegisterViewModel model, HttpPostedFileBase upload, int classId, string LastSchoolAttended, string ParentName, string ParentAddress, string ParentPhone, string ParentOccupation)
        {
            var ee = "";
            if (ModelState.IsValid)
            {



                try
                {
                    string succed;

                    succed = await _userService.NewStudent(model, LastSchoolAttended, ParentName, ParentAddress, ParentPhone, ParentOccupation);
                    if (succed == "true")
                    {
                        var Imageid = await _imageService.Create(upload);
                        var user = await UserManager.FindByNameAsync(model.Username);
                        // var user = await _userService.GetUserByUserEmail(model.Email);
                        var student = await _studentService.GetStudentByUserId(user.Id);

                        //profile pic upload
                        await _studentService.UpdateImageId(student.Id, Imageid);

                        //enrolment
                        await _enrollmentService.EnrollStudent(classId, student.Id);
                        var classLevel = await _classLevelService.Get(classId);
                        TempData["success"] = "Student with username <i> " + model.Username + "</i> Added Successfully to " + classLevel.ClassName + " Class";
                        return RedirectToAction("Students", new { id = classId });
                    }
                    else
                    {
                        TempData["error1"] = succed;
                    }


                }
                catch (Exception e)
                {
                    ee = e.ToString();
                }

            }
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            TempData["error"] = "Creation of new student not successful" + ee;
            var classlevel = await _classLevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel, "Id", "ClassLevelName");
            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName", model.StateOfOrigin);
            return View(model);
        }


        #endregion

        #region Promotion service

        public async Task<ActionResult> StartPromote()
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);

            IQueryable<StudentProfile> allStudents = from s in db.StudentProfiles
                                       .Include(x => x.user).Where(x => x.user.Status == EntityStatus.Active)
                                                     select s;
            var graduate = await allStudents.Where(x => x.Graduate == true).CountAsync();
            var activeStudent = await allStudents.Where(x => x.Graduate == false).CountAsync();
            var Suspended = await allStudents.Where(x => x.user.Status == EntityStatus.Suspeneded).CountAsync();

            var output = allStudents.Where(x => x.Graduate == false).Select(x => new PromotionDto
            {
                UserName = x.user.UserName,
                ProfileId = x.Id,
                StudentRegNumber = x.StudentRegNumber,
                FullName = x.user.Surname + " " + x.user.FirstName + " " + x.user.OtherName,

                ClassName = ClassEnrolled(x.Id)

            });
            return View();
        }

        public string ClassEnrolled(int studentId)
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            string result = "";
            var item = db.Enrollments.Include(x => x.ClassLevel).FirstOrDefault(x => x.StudentProfileId == studentId && x.SessionId == currentSession.Id);
            if (item != null)
            {
                result = item.ClassLevel.ClassName;
            }
            return result;
        }

        public ActionResult EnablePromoteAll()
        {
            var setting = db.Settings.FirstOrDefault();
            return PartialView(setting);
        }
        [HttpPost]
        public async Task<ActionResult> EnablePromoteAll(Setting setting)
        {


            db.Entry(setting).State = EntityState.Modified;
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId && x.Status == EntityStatus.Active).FirstOrDefault();
                if (user != null)
                {
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId;
                    tracker.UserName = user.UserName;
                    tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Enabled Promote All";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }

            }


            return RedirectToAction("Promotion");

        }

        public async Task<ActionResult> PromotStudent()
        {
            var csession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            ViewBag.sessionc = csession.SessionYear;

            var classlevel = await _classLevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel.ToList(), "Id", "ClassLevelName");

            var studentlist = await _classLevelService.StudentsList();
            ViewBag.StudentId = new SelectList(studentlist.ToList(), "Id", "ClassLevelName");

            return View();
        }


        //sessIdclassId
        public async Task<ActionResult> Promotion()
        {
            var csession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            ViewBag.sessionc = csession;
            var classlevel = await _classLevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel.ToList(), "Id", "ClassLevelName");

            var session = await _sessionService.GetAllSession();
            var distintsession = db.Sessions.Where(x => x.SessionYear != csession.SessionYear).Select(x => new { x.SessionYear }).Distinct();

            ViewBag.sessionId = new SelectList(distintsession, "SessionYear", "SessionYear");
            var setting = await db.Settings.FirstOrDefaultAsync();
            ViewBag.setting = setting;
            var classes = await db.ClassLevels.Include(x => x.Session).OrderBy(x => x.ClassName).ToListAsync();
            return View(classes);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Promoting(string sess, int classid, int oldclassid, int oldsessionid)
        {
            string items = await _enrollmentService.PromotionEnrol(sess, classid, oldclassid, oldsessionid);

            TempData["message"] = items;
            return RedirectToAction("Promotion");
        }


        #endregion

        #region move students
        public async Task<ActionResult> MoveStudents(int? id)
        {
            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session.OrderByDescending(x => x.Id), "Id", "FullSession");

            ViewBag.cId = new SelectList(db.ClassLevels.OrderBy(x => x.ClassName), "Id", "ClassName");
            var d = db.ClassLevels.FirstOrDefault(x => x.Id == id);
            ViewBag.mss = d.ClassName;
            return View();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> MoveStudents(int? id, int sessionid, int classid)
        {
            var items = await _classLevelService.MoveStudents(id, sessionid, classid);

            ViewBag.mess = items;
            return RedirectToAction("Details", new { id = id });
        }

        #endregion

        public async Task<ActionResult> Details(int? id)
        {
            var classinfo = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == id);


            var setting = await db.Settings.FirstOrDefaultAsync();
            var totalCA = classinfo.Project + classinfo.ClassExercise + classinfo.AccessmentScore + classinfo.TestScore2 + classinfo.Assessment;

            if (classinfo.Passmark == 0 || classinfo.Passmark == null)
            {
                classinfo.Passmark = setting.Passmark;
                classinfo.PromotionByTrial = setting.PromotionByTrial;
            }
            if (classinfo.ExamScore + totalCA != 100)
            {
                classinfo.ExamScore = setting.ExamScore;
                classinfo.AccessmentScore = setting.AccessmentScore;
                classinfo.Project = setting.Project;
                classinfo.ClassExercise = setting.ClassExercise;
                classinfo.Assessment = setting.Assessment;
                classinfo.TestScore2 = setting.TestScore2;
            }
            db.Entry(classinfo).State = EntityState.Modified;
            await db.SaveChangesAsync();

            //Add Tracking
            //var userId = User.Identity.GetUserId();
            //if (userId != null)
            //{
            //    var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
            //    Tracker tracker = new Tracker();
            //    tracker.UserId = userId;
            //    tracker.UserName = user.UserName;
            //    tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
            //    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
            //    tracker.Note = tracker.FullName + " " + "E";
            //    //db.Trackers.Add(tracker);
            //    await db.SaveChangesAsync();
            //}

            await _enrollmentService.UpdateEnrollmentStatus(id);
            //await _classLevelService.GradingOption(id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _classLevelService.ClassLevelDetails(id);
            if (item == null)
            {
                return RedirectToAction("HttpNotFound", "Error", new { area = "Content" });
            }

            //check if form teacher is no longer active
            var checkteacher = db.Users.FirstOrDefault(x => x.Id == item.UserId);
            if(checkteacher == null)
            {
                 
                    TempData["error"] = "Please Assign a form Teacher to class";
                    return RedirectToAction("EditClass", new { id = id });
                
            }
            else
            {
                var u = checkteacher.Status;
                if (u != EntityStatus.Active)
                {
                    TempData["error"] = "Please Change the form Teacher, <b>" + item.FormTeacher + "</b> has been " + u;
                    return RedirectToAction("EditClass", new { id = id });
                }
            }
           
            ViewBag.Class = item;

            if (User.IsInRole("SuperAdmin"))
            {
                var subject = await _subjectService.AllList(id);
                foreach (var i in subject)
                {
                    var ua = db.Users.FirstOrDefault(x => x.Id == i.UserId).Status;
                    if (ua != EntityStatus.Active)
                    {
                        TempData["error"] = "Please Change the " + i.SubjectName + " Teacher, <b>" + i.FormTeacher + "</b> has been " + ua;
                        return RedirectToAction("EditSubject", new { id = i.SubjectId });
                    }
                }
                var time = await db.TimeTables.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.ClassLevelId == id);
                ViewBag.time = time;
                ViewBag.subs = TempData["subs"];
                return View(subject);
            }
            else
            {
                var subject = await _subjectService.List(id);
                foreach (var i in subject)
                {
                    var ua = db.Users.FirstOrDefault(x => x.Id == i.UserId).Status;
                    if (ua != EntityStatus.Active)
                    {
                        TempData["error"] = "Please Change the " + i.SubjectName + " Teacher, <b>" + i.FormTeacher + "</b> has been " + ua;
                        return RedirectToAction("EditSubject", new { id = i.SubjectId });
                    }
                }
                var time = await db.TimeTables.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.ClassLevelId == id);
                ViewBag.time = time;
                ViewBag.subs = TempData["subs"];
                return View(subject);
            }



        }

        //reload class subjects

        public async Task<ActionResult> ReloadEnterscoreByClass(int id = 0)
        {
            int curentsessionid = await _sessionService.GetCurrentSessionId();
            var classsub = await db.Subjects.Where(x => x.ClassLevelId == id).ToListAsync();
            string check = "";
            foreach (var sub in classsub)
            {

                try
                {
                    await _staffService.ReloadStudents(sub.Id, curentsessionid, id);
                    check = check + ", " + sub.SubjectName;
                }
                catch (Exception f)
                {

                }

            }
            string infomation = check;
            TempData["subs"] = infomation;

            return RedirectToAction("Details", "ClassLevels", new { id = id, area = "Admin" });
        }

        #region new class / edit timetable / edit class

        public async Task<ActionResult> NewClass()
        {
            var staff = await _staffService.StaffDropdownList();
            ViewBag.UserId = new SelectList(staff, "UserId", "FullName");

            var setting = await db.Settings.FirstOrDefaultAsync();
            ViewBag.setting = setting.IsPrimaryNursery;
            ViewBag.Passmark = setting.Passmark;
            ViewBag.PromotionByTrial = setting.PromotionByTrial;
            ViewBag.AccessmentScore = setting.AccessmentScore;
            ViewBag.ExamScore = setting.ExamScore;
            ViewBag.projectscore = setting.Project;
            ViewBag.ClassExercisescore = setting.ClassExercise;
            ViewBag.test2score = setting.TestScore2;
            ViewBag.assessmentscore = setting.Assessment;
            ViewBag.passmark = setting.Passmark;
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> NewClass(ClassLevel model)
        {
            var setting = await db.Settings.FirstOrDefaultAsync();

            if(String.IsNullOrEmpty(model.UserId))
            {
                var staffq = await _staffService.StaffDropdownList();
                ViewBag.UserId = new SelectList(staffq, "UserId", "FullName", model.UserId);
                TempData["error"] = "Form Teacher is Required";
                return View(model);
            }

            if (ModelState.IsValid)
            {
                if (model.ClassName.Substring(0, 2) == "PG")
                {
                    model.ClassName = "P" + model.ClassName;
                }
                if (model.ClassName.Substring(0, 3) == "JSS" || model.ClassName.Substring(0, 3) == "SSS" || model.ClassName.Substring(0, 3) == "NUR" || model.ClassName.Substring(0, 3) == "PRI" || model.ClassName.Substring(0, 3) == "PRE" || model.ClassName.Substring(0, 3) == "PPG")
                {
                    if (model.ClassName.Substring(0, 2) == "PP")
                    {
                        model.ClassName = model.ClassName.Remove(0, 1);
                    }
                    var cname = await db.ClassLevels.Select(x => x.ClassName).ToListAsync();
                    if (cname.Contains(model.ClassName))
                    {
                        var staffq = await _staffService.StaffDropdownList();
                        ViewBag.UserId = new SelectList(staffq, "UserId", "FullName", model.UserId);
                        TempData["error"] = "Class already exist";
                        return View(model);
                    }

                    //model.Passmark = setting.Passmark;
                    //model.PromotionByTrial = setting.PromotionByTrial;
                    //model.AccessmentScore = setting.AccessmentScore;
                    //model.ExamScore = setting.ExamScore;

                    ViewBag.Passmark = setting.Passmark;
                    ViewBag.PromotionByTrial = setting.PromotionByTrial;
                    ViewBag.AccessmentScore = setting.AccessmentScore;
                    ViewBag.ExamScore = setting.ExamScore;
                    ViewBag.projectscore = setting.Project;
                    ViewBag.ClassExercisescore = setting.ClassExercise;
                    ViewBag.test2score = setting.TestScore2;
                    ViewBag.assessmentscore = setting.Assessment;
                    ViewBag.passmark = setting.Passmark;

                    if (model.AccessmentScore == null)
                    {
                        model.AccessmentScore = 0;
                    };
                    if (model.ExamScore == null)
                    {
                        model.ExamScore = 0;
                    };
                    if (model.Project == null)
                    {
                        model.Project = 0;
                    };
                    if (model.ClassExercise == null)
                    {
                        model.ClassExercise = 0;
                    };
                    if (model.TestScore2 == null)
                    {
                        model.TestScore2 = 0;
                    };
                    if (model.Assessment == null)
                    {
                        model.Assessment = 0;
                    };

                    await _classLevelService.Create(model);
                    TimeTable timeTable = new TimeTable();
                    timeTable.ClassLevelId = model.Id;
                    timeTable.Monday = "Monday";
                    timeTable.Tuesday = "Tuesday";
                    timeTable.Wednessday = "Wednessday";
                    timeTable.Thursday = "Thursday";
                    timeTable.Friday = "Friday";
                    timeTable.Time10_11 = "10am - 11am";
                    timeTable.Time11_12 = "11am - 12pm";
                    timeTable.Time12_13 = "12pm - 1pm";
                    timeTable.Time13_14 = "1pm - 2pm";
                    timeTable.Time14_15 = "2pm - 3pm";
                    timeTable.Time15_16 = "3pm - 4pm";
                    timeTable.Time16_17 = "4pm - 5pm";
                    timeTable.Time17_18 = "5pm - 6pm";
                    timeTable.Time6_7 = "6am - 7am";
                    timeTable.Time7_8 = "7am - 8am";
                    timeTable.Time8_9 = "8am - 9am";
                    timeTable.Time9_10 = "9am - 10am";

                    timeTable.Name = model.ClassName;
                    db.TimeTables.Add(timeTable);
                    await db.SaveChangesAsync();

                    //Add Tracking
                    var userId = User.Identity.GetUserId();
                    if (userId != null)
                    {
                        var user = UserManager.Users.Where(x => x.Id == userId && x.Status == EntityStatus.Active).FirstOrDefault();
                        Tracker tracker = new Tracker();
                        tracker.UserId = userId;
                        tracker.UserName = user.UserName;
                        tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                        tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                        tracker.Note = tracker.FullName + " " + "Enabled Promote All";
                        //db.Trackers.Add(tracker);
                        await db.SaveChangesAsync();
                    }
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Wrong Class Input";
            }
            var staff = await _staffService.StaffDropdownList();
            ViewBag.UserId = new SelectList(staff, "UserId", "FullName", model.UserId);

            //  var setting = await db.Settings.FirstOrDefaultAsync();
            ViewBag.setting = setting.IsPrimaryNursery;
            ViewBag.Passmark = setting.Passmark;
            ViewBag.PromotionByTrial = setting.PromotionByTrial;
            ViewBag.AccessmentScore = setting.AccessmentScore;
            ViewBag.ExamScore = setting.ExamScore;
            ViewBag.projectscore = setting.Project;
            ViewBag.ClassExercisescore = setting.ClassExercise;
            ViewBag.test2score = setting.TestScore2;
            ViewBag.assessmentscore = setting.Assessment;
            ViewBag.passmark = setting.Passmark;
            return View(model);
        }


        // GET: Content/TimeTables/Edit/5
        public async Task<ActionResult> EditTimeTable(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeTable timeTable = await db.TimeTables.FindAsync(id);
            if (timeTable == null)
            {
                return HttpNotFound();
            }
            return View(timeTable);
        }

        // POST: Content/TimeTables/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditTimeTable(TimeTable timeTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(timeTable).State = EntityState.Modified;
                await db.SaveChangesAsync();

                //Add Tracking
                var userId = User.Identity.GetUserId();
                if (userId != null)
                {
                    var user = UserManager.Users.Where(x => x.Id == userId && x.Status == EntityStatus.Active).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId;
                    tracker.UserName = user.UserName;
                    tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Enabled Promote All";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
                return RedirectToAction("Details", new { id = timeTable.ClassLevelId });
            }
            return View(timeTable);
        }


        public async Task<ActionResult> EditClass(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _classLevelService.Get(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            var staff = await _staffService.StaffDropdownList();
            ViewBag.UserId = new SelectList(staff, "UserId", "FullName", item.UserId);
            return View(item);
        }


        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditClass(ClassLevel model)
        {

            if (ModelState.IsValid)
            {
                if (model.AccessmentScore == null)
                {
                    model.AccessmentScore = 0;
                };
                if (model.ExamScore == null)
                {
                    model.ExamScore = 0;
                };
                if (model.Project == null)
                {
                    model.Project = 0;
                };
                if (model.ClassExercise == null)
                {
                    model.ClassExercise = 0;
                };
                if (model.TestScore2 == null)
                {
                    model.TestScore2 = 0;
                };
                if (model.Assessment == null)
                {
                    model.Assessment = 0;
                };
                await _classLevelService.Edit(model);
                var getClass = await _classLevelService.Get(model.Id);
                if (getClass.SubjectSettings == true)
                {
                    var subjectList = db.Subjects.Include(x => x.ClassLevel).Where(x => x.ClassLevelId == getClass.Id).ToList();
                    foreach (var item in subjectList)
                    {
                        var subjectModel = db.Subjects.Include(x => x.ClassLevel).FirstOrDefault(x => x.Id == item.Id);
                        subjectModel.ExamScore = getClass.ExamScore;
                        subjectModel.TestScore = getClass.AccessmentScore;
                        subjectModel.Project = getClass.Project;
                        subjectModel.ClassExercise = getClass.ClassExercise;
                        subjectModel.TestScore2 = getClass.TestScore2;
                        subjectModel.Assessment = getClass.Assessment;
                        subjectModel.PassMark = getClass.Passmark;
                        db.Entry(subjectModel).State = EntityState.Modified;
                        await db.SaveChangesAsync();


                        //Add Tracking
                        var userId = User.Identity.GetUserId();
                        if (userId != null)
                        {
                            //var user = UserManager.Users.Where(x => x.Id == userId && x.Status == EntityStatus.Active).FirstOrDefault();
                            //Tracker tracker = new Tracker();
                            //tracker.UserId = userId;
                            //tracker.UserName = user.UserName;
                            //tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                            //tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                            //tracker.Note = tracker.FullName + " " + "Enabled Promote All";
                            ////db.Trackers.Add(tracker);
                            await db.SaveChangesAsync();
                        }

                    }
                }

                return RedirectToAction("Index");
            }
            var staff = await _staffService.StaffDropdownList();
            ViewBag.UserId = new SelectList(staff, "UserId", "FullName", model.UserId);
            return View(model);
        }

        #endregion

        #region Edit Supper Class

        public async Task<ActionResult> EditSupperClass(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _classLevelService.Get(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            return View(item);
        }


        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditSupperClass(ClassLevel model)
        {

            if (ModelState.IsValid)
            {
                await _classLevelService.Edit(model);
                return RedirectToAction("Index");
            }

            return View(model);
        }


        #endregion
        // GET: Admin/Settings/Delete/5
        public async Task<ActionResult> DeleteClass(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _classLevelService.Get(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Admin/Settings/Delete/5
        [HttpPost, ActionName("DeleteClass")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _classLevelService.Delete(id);

            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Index");
        }

        //add subject to class
        public async Task<ActionResult> AddSubject(int id)
        {
            ViewBag.Id = id;
            var accessmentMark = db.ClassLevels.Where(x => x.Id == id).FirstOrDefault();
            var staff = await _staffService.StaffDropdownList();
            ViewBag.UserId = new SelectList(staff, "UserId", "FullName");
            //var sett = await db.Settings.FirstOrDefaultAsync();
            //ViewBag.examscore = sett.ExamScore;
            //ViewBag.testscore = sett.AccessmentScore;
            //ViewBag.passmark = sett.Passmark;
            ViewBag.examscore = accessmentMark.ExamScore;
            ViewBag.testscore = accessmentMark.AccessmentScore;
            ViewBag.projectscore = accessmentMark.Project;
            ViewBag.ClassExercisescore = accessmentMark.ClassExercise;
            ViewBag.test2score = accessmentMark.TestScore2;
            ViewBag.assessmentscore = accessmentMark.Assessment;
            ViewBag.passmark = accessmentMark.Passmark;

            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> AddSubject(Subject model, int id)
        {
            var staff = await _staffService.StaffDropdownList();
            if (model.TestScore == null)
            {
                model.TestScore = 0;
            };
            if (model.ExamScore == null)
            {
                model.ExamScore = 0;
            };
            if (model.Project == null)
            {
                model.Project = 0;
            };
            if (model.ClassExercise == null)
            {
                model.ClassExercise = 0;
            };
            if (model.TestScore2 == null)
            {
                model.TestScore2 = 0;
            };
            if (model.Assessment == null)
            {
                model.Assessment = 0;
            };
            var totalCA = model.Project + model.ClassExercise + model.TestScore + model.TestScore2 + model.Assessment;
            decimal? tot = totalCA + model.ExamScore;
            //decimal? tot = model.TestScore + model.ExamScore;
            if (tot > 100)
            {
                TempData["error"] = "Assessment and Exam score is Greater than 100.";
                ViewBag.Id = id;
                ViewBag.UserId = new SelectList(staff, "UserId", "FullName", model.UserId);
                if (Request.Browser.IsMobileDevice == true)
                {
                    return RedirectToAction("ClassSubject", new { id = id });
                }
                else
                {
                    return View(model);
                }


            }

            var checkSubject = await db.Subjects.FirstOrDefaultAsync(x => x.SubjectName.ToUpper() == model.SubjectName.ToUpper() && x.ClassLevelId == id);
            if (checkSubject != null)
            {
                TempData["error1"] = "Subject has already be entered.";
                if (Request.Browser.IsMobileDevice == true)
                {
                    return RedirectToAction("ClassSubject", new { id = id });
                }
                else
                {
                    return RedirectToAction("Details", new { id = id });
                }


            }
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var enrollment = db.Enrollments.Include(x => x.User).Include(x => x.StudentProfile.user).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(e => e.ClassLevelId == id && e.SessionId == currentSession.Id);

            if (ModelState.IsValid)
            {
                await _subjectService.Create(model, id);
                if (enrollment != null)
                {
                    foreach (var item in enrollment)
                    {
                        EnrolledSubject enrolledSubject = db.EnrolledSubjects.Create();
                        enrolledSubject.SubjectId = model.Id;
                        enrolledSubject.EnrollmentId = item.Id;
                        enrolledSubject.ExamScore = 0;
                        enrolledSubject.TestScore = 0;
                        enrolledSubject.TotalScore = 0;
                        enrolledSubject.Assessment = 0;
                        enrolledSubject.ClassExercise = 0;
                        enrolledSubject.Project = 0;
                        enrolledSubject.TestScore2 = 0;
                        enrolledSubject.TotalCA = 0;
                        enrolledSubject.IsOffered = false;
                        enrolledSubject.GradingOption = GradingOption.NONE;
                        await _subjectService.EnrolledSubject(enrolledSubject);

                    }
                }
                TempData["msg"] = "Subject Successfully Added";

                if (Request.Browser.IsMobileDevice == true)
                {
                    return RedirectToAction("ClassSubject", new { id = id });
                }
                else
                {
                    return RedirectToAction("Details", new { id = id });
                }
            }

            ViewBag.UserId = new SelectList(staff, "UserId", "FullName", model.UserId);
            ViewBag.Id = id;
            if (Request.Browser.IsMobileDevice == true)
            {
                return RedirectToAction("ClassSubject", new { id = id });
            }
            else
            {
                return View(model);
            }

        }

        public async Task<ActionResult> EditSubject(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _subjectService.Get(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            var staff = await _staffService.StaffDropdownList();
            ViewBag.UserId = new SelectList(staff, "UserId", "FullName", item.UserId);
            return View(item);
        }


        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditSubject(Subject model)
        {
            var staff = await _staffService.StaffDropdownList();
            if (ModelState.IsValid)
            {
                if (model.TestScore == null)
                {
                    model.TestScore = 0;
                };
                if (model.ExamScore == null)
                {
                    model.ExamScore = 0;
                };
                if (model.Project == null)
                {
                    model.Project = 0;
                };
                if (model.ClassExercise == null)
                {
                    model.ClassExercise = 0;
                };
                if (model.TestScore2 == null)
                {
                    model.TestScore2 = 0;
                };
                if (model.Assessment == null)
                {
                    model.Assessment = 0;
                };
                var totalCA = model.Project + model.ClassExercise + model.TestScore + model.TestScore2 + model.Assessment;
                decimal? tot = totalCA + model.ExamScore;
                //decimal? tot = model.TestScore + model.ExamScore;
                if (tot > 100)
                {
                    TempData["error"] = "Assessment and Exam score is Greater than 100.";

                    ViewBag.UserId = new SelectList(staff, "UserId", "FullName", model.UserId);

                    return View(model);
                }

                await _subjectService.Edit(model);
                if (Request.Browser.IsMobileDevice == true)
                {
                    return RedirectToAction("ClassSubject", new { id = model.ClassLevelId });
                }
                else
                {
                    return RedirectToAction("Details", new { id = model.ClassLevelId });
                }

            }

            ViewBag.UserId = new SelectList(staff, "UserId", "FullName", model.UserId);
            return View(model);
        }
        public async Task<ActionResult> DeleteSubject(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _subjectService.Get(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Admin/Settings/Delete/5
        [HttpPost, ActionName("DeleteSubject")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteSubjectConfirmed(int id, int cId)
        {
            await _subjectService.Delete(id);
            TempData["msg"] = "Subject Successfully Deleted";
            if (Request.Browser.IsMobileDevice == true)
            {
                return RedirectToAction("ClassSubject", new { id = cId });
            }
            else
            {
                return RedirectToAction("Details", new { id = cId });
            }
        }


        public ActionResult BlockStudent(int? profleid = 0, int? classId = 0)
        {
            ViewBag.profleid = profleid;
            ViewBag.classId = classId;
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> BlockStudent(Defaulter model, int profleid = 0, int? classId = 0)
        {
            if (ModelState.IsValid)
            {
                var userFullname = "";
                if (profleid != 0 || classId != 0)
                {
                    userFullname = await _defaulterService.Create(model, profleid);
                }
                TempData["success"] = userFullname + " has successfully been blocked from  checking results";
                return RedirectToAction("Students", new { id = classId });
            }

            return View(model);
        }

        public async Task<ActionResult> UnBlockStudent(int id = 0)
        {
            var item = await _defaulterService.GetDefaulter(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            ViewBag.classId = await _enrollmentService.classLevelFromEnrollmentbyProfileId(item.ProfileId);
            return View(item);
        }

        [HttpPost, ActionName("UnBlockStudent")]
        public async Task<ActionResult> UnBlockStudentConfirmed(int id)
        {
            var user = await _defaulterService.RemoveDefaulter(id);
            var classlevel = await _enrollmentService.classLevelFromEnrollmentbyProfileId(user.Id);
            TempData["success"] = user.user.Surname + " " + user.user.FirstName + " " + user.user.OtherName + " has successfully been unblocked from  checking results";
            return RedirectToAction("Students", new { id = classlevel.ClassLevelId });
        }

        /// <summary>
        /// 
        /// attendance
        /// </summary>
        /// <param name="disposing"></param>
        /// 
        public async Task<ActionResult> AttendanceList(int? id)
        {
            var items = await _classLevelService.Students(id);
            var classId = await _classLevelService.Get(id);
            ViewBag.count = items.Count();
            ViewBag.ClassName = classId.ClassName;
            return View(items);
        }

        #region

        public async Task<ActionResult> Attendance(int id)
        {
            var items = await _attendanceService.ListAttendanceByClassBySession(id);
            ViewBag.id = id;
            return View(items);

        }

        public async Task<ActionResult> Create(int id)
        {

            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            Attendance attendance = new Attendance();
            attendance.Date = DateTime.UtcNow.AddHours(1);
            attendance.ClassLevelId = id;
            attendance.SessionId = currentSession.Id;
            attendance.UserId = User.Identity.GetUserId();
            await _attendanceService.Create(attendance);
            ////
            var enrolledStudents = db.Enrollments.Include(x => x.User).Include(e => e.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.Session).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(s => s.ClassLevelId == id && s.Session.Status == SessionStatus.Current);

            foreach (var it in enrolledStudents)
            {
                AttendanceDetail attendanceDetail = new AttendanceDetail();
                attendanceDetail.StudentId = it.StudentProfileId;
                attendanceDetail.UserId = it.StudentProfile.UserId;
                attendanceDetail.EnrollmentId = it.Id;
                attendanceDetail.SessionId = currentSession.Id;
                attendanceDetail.AttendanceId = attendance.Id;
                db.AttendanceDetails.Add(attendanceDetail);

            }
            await db.SaveChangesAsync();
            //await _attendanceService.AddAllStudentsInClass(id);
            return RedirectToAction("Attendance", new { id = id });
        }

        public async Task<ActionResult> AttendanceDetail(int id)
        {
            var items = await _attendanceService.ListAttendanceDetail(id);
            ViewBag.id = id;
            var attdetail = await db.Attendances.Include(x => x.ClassLevel).Include(x => x.AttendanceDetails).FirstOrDefaultAsync(x => x.Id == id);
            ViewBag.check = attdetail.Updated;
            ViewBag.adid = attdetail.ClassLevelId;
            ViewBag.cname = attdetail.ClassLevel.ClassName;
            ViewBag.date = attdetail.Date;
            ViewBag.present = attdetail.AttendanceDetails.Count(x => x.IsPresent == true);
            ViewBag.Absent = attdetail.AttendanceDetails.Count(x => x.IsPresent == false);
            return View(items);
        }

        //update all
        public async Task<ActionResult> ChooseAll(int id)
        {
            var items = await _attendanceService.ListAttendanceDetail(id);
            foreach (var aaa in items)
            {
                var stud = await db.AttendanceDetails.FirstOrDefaultAsync(x => x.Id == aaa.Id);
                if (stud != null)
                {
                    stud.IsPresent = true;
                    db.Entry(stud).State = EntityState.Modified;

                }
            }
            await db.SaveChangesAsync();
            return RedirectToAction("AttendanceDetail", new { id = id });
        }
        public async Task<ActionResult> UnChooseAll(int id)
        {
            var items = await _attendanceService.ListAttendanceDetail(id);
            foreach (var aaa in items)
            {
                var stud = await db.AttendanceDetails.FirstOrDefaultAsync(x => x.Id == aaa.Id);
                if (stud != null)
                {
                    stud.IsPresent = false;
                    db.Entry(stud).State = EntityState.Modified;

                }
            }
            await db.SaveChangesAsync();
            return RedirectToAction("AttendanceDetail", new { id = id });
        }

        public async Task<ActionResult> Update(int id)
        {
            var aaa = await db.Attendances.FirstOrDefaultAsync(x => x.Id == id);
            if (aaa != null)
            {
                aaa.Updated = true;
                await _attendanceService.Edit(aaa);
            }
            return RedirectToAction("AttendanceDetail", new { id = id });
        }

        [HttpPost]

        public async Task<ActionResult> TakeAttendance(int profileId, int enroId, string userId, bool? ischecked)
        {
            var attendance = await db.AttendanceDetails.FirstOrDefaultAsync(x => x.EnrollmentId == enroId);
            if (ischecked.HasValue && ischecked.Value)
            {
                attendance.IsPresent = true;
                await _attendanceService.UpdateAttendance(attendance);
            }
            else
            {
                attendance.IsPresent = false;
                await _attendanceService.UpdateAttendance(attendance);
            }

            return RedirectToAction("AttendanceDetail", new { id = attendance.AttendanceId });
        }

        #endregion

        #region
        public async Task<ActionResult> ClassAssignment(int classId)
        {
            var ourclass = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == classId);
            ViewBag.classInfo = ourclass.ClassName;
            ViewBag.classId = ourclass.Id;
            var items = await _assignmentService.List(classId);
            return View(items);
        }

        public async Task<ActionResult> ClassAnsweredAssignment(int classId, int assId)
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var c = db.Assignments.Include(x => x.ClassLevel).Include(x => x.Session).FirstOrDefault(x => x.Id == assId && x.SessionId == currentSession.Id);
            ViewBag.AssignmentInfo = c;
            var items = await _assignmentService.ListForStudent(classId, assId);
            return View(items);
        }

        public async Task<ActionResult> AssignmentDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _assignmentService.Get(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        public async Task<ActionResult> AnswerDetail(int? id, int studentId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _assignmentService.GetAnswer(id, studentId);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        #endregion

        //syllabus
        #region


        #endregion

        public async Task<ActionResult> DelSubjectsInClass(int? id)
        {
            var sub = await db.Subjects.Where(x => x.ClassLevelId == id).ToListAsync();
            foreach (var i in sub)
            {
                var aa = await db.EnrolledSubjects.AsNoTracking().Where(x => x.SubjectId == i.Id).ToListAsync();
                foreach (var s in aa)
                {
                    var sds = db.EnrolledSubjects.AsNoTracking().FirstOrDefault(x => x.Id == s.Id);
                    // db.EnrolledSubjects.Remove(sds);
                    db.Entry(sds).State = EntityState.Deleted;
                    await db.SaveChangesAsync();
                }

                db.Subjects.Remove(i);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = id });
        }
        [AllowAnonymous]
        public JsonResult LgaList(string Id)
        {
            var stateId = db.States.FirstOrDefault(x => x.StateName == Id).Id;
            var local = from s in db.LocalGovs
                        where s.StatesId == stateId
                        select s;

            return Json(new SelectList(local.ToArray(), "LGAName", "LGAName"), JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> EditUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await _studentService.Get(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName", user.StateOfOrigin);

            return View(user);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(StudentInfoDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _studentService.Edit(model);
                    TempData["success"] = "Update Successful.";
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    TempData["success"] = "Update Unsuccessful, (" + e.ToString() + ")";
                }

            }
            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName", model.StateOfOrigin);
            return View(model);
        }


        [HttpPost]
        public async Task<ActionResult> UpdateStudentRecord(UpdateEmailAndPhoneDto models)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == models.UserId);
            user.Phone = models.PhoneNumber;
            user.Email = models.EmailAddress;
            db.Entry(user).State = EntityState.Modified;
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = User.Identity.GetUserId();
            if (userId != null)
            {

                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Enabled Promote All";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
            return Content("<script language='javascript' type='text/javascript'>alert('Saved Successful');</script>");
        }


        public async Task<ActionResult> NotOffered(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _classLevelService.Get(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [HttpPost, ActionName("NotOffered")]
        public async Task<ActionResult> NotOfferedConfirmed(int id)
        {
            try
            {
                var csession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
                var enrollment = db.Enrollments.Include(x => x.ClassLevel).Where(x => x.ClassLevelId == id && x.SessionId == csession.Id).ToList();
                foreach (var enrol in enrollment)
                {
                    var items = db.EnrolledSubjects.Include(x => x.Enrollments).Where(x => x.EnrollmentId == enrol.Id && x.TotalScore == 0).ToList();
                    foreach (var sub in items)
                    {
                        //var total = await db.EnrolledSubjects.Include(x => x.Enrollments).FirstOrDefaultAsync(x => x.Id == sub.Id);
                        if (sub.TotalScore <= 0)
                        {
                            sub.IsOffered = false;
                            db.Entry(sub).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Index");
        }


        public async Task<ActionResult> NotClassSubjectEnrolled(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _classLevelService.Get(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [HttpPost, ActionName("NotClassSubjectEnrolled")]
        public async Task<ActionResult> NotClassSubjectEnrolledConfirmed(int id)
        {
            try
            {
                var csession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
                var enrollment = db.Enrollments.Include(x => x.ClassLevel).Where(x => x.ClassLevelId == id && x.SessionId == csession.Id).ToList();
                foreach (var enrol in enrollment)
                {
                    var items = db.EnrolledSubjects.Include(x => x.Enrollments).Include(x => x.Enrollments).Where(x => x.EnrollmentId == enrol.Id && x.Enrollments.SessionId == csession.Id && x.Enrollments.ClassLevelId == id).ToList();
                    foreach (var sub in items)
                    {
                        var subName = await db.Subjects.Where(x => x.Id == sub.SubjectId && x.ClassLevelId == id).FirstOrDefaultAsync();
                        if (subName == null)
                        {
                            var subName1 = await db.Subjects.Where(x => x.Id == sub.SubjectId).FirstOrDefaultAsync();
                            var subName2 = await db.Subjects.Where(x => x.SubjectName == subName1.SubjectName && x.ClassLevelId == id).FirstOrDefaultAsync();
                            //var checkDelete1 = await db.EnrolledSubjects.Where(x => (x.SubjectId != subName2.Id && x.Subject.SubjectName == subName2.SubjectName) && x.EnrollmentId == enrol.Id && x.Enrollments.SessionId == csession.Id && x.Enrollments.ClassLevelId == id).FirstOrDefaultAsync();
                            var checkDelete2 = db.EnrolledSubjects.Include(x => x.Enrollments).Include(x => x.Subject).Where(x => (x.SubjectId != subName2.Id && x.Subject.SubjectName == subName1.SubjectName) && x.EnrollmentId == enrol.Id && x.Enrollments.SessionId == csession.Id && x.Enrollments.ClassLevelId == id);
                            if (checkDelete2 != null)
                            {
                                foreach (var itemRev in checkDelete2.ToList())
                                {
                                    db.EnrolledSubjects.Remove(itemRev);
                                    await db.SaveChangesAsync();
                                }
                            }

                        }

                    }
                }

            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Index");
        }


        public async Task<ActionResult> RemoveDuplicateEnrolledSubject(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _classLevelService.Get(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [HttpPost, ActionName("RemoveDuplicateEnrolledSubject")]
        public async Task<ActionResult> RemoveDuplicateEnrolledSubjectConfirmed(int id)
        {
            try
            {
                var csession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
                var sessionToRemoveFrom = await db.Sessions.OrderBy(x => x.Id).Where(x => x.SessionYear == csession.SessionYear).ToListAsync();
                //foreach (var term in sessionToRemoveFrom)
                //{
                var enrollment = db.Enrollments.Include(x => x.ClassLevel).Where(x => x.ClassLevelId == id && x.SessionId == csession.Id).ToList();
                foreach (var enrol in enrollment)
                {
                    var items = db.EnrolledSubjects.Include(x => x.Enrollments).Where(x => x.EnrollmentId == enrol.Id && x.Enrollments.SessionId == csession.Id && x.Enrollments.ClassLevelId == id).ToList();
                    foreach (var sub in items)
                    {
                        var subCount = db.EnrolledSubjects.Include(x => x.Enrollments).Where(x => x.SubjectId == sub.SubjectId && x.EnrollmentId == enrol.Id && x.Enrollments.SessionId == csession.Id && x.Enrollments.ClassLevelId == id).Count();
                        if (subCount > 1)
                        {
                            var subRemove = await db.EnrolledSubjects.Include(x => x.Enrollments).Where(x => x.SubjectId == sub.SubjectId && x.EnrollmentId == enrol.Id && x.Enrollments.SessionId == csession.Id && x.Enrollments.ClassLevelId == id && (x.TotalScore == 0 || x.TotalScore != 0)).ToListAsync();
                            var subRemove4 = await db.EnrolledSubjects.Include(x => x.Enrollments).Where(x => x.SubjectId == sub.SubjectId && x.EnrollmentId == enrol.Id && x.Enrollments.SessionId == csession.Id && x.Enrollments.ClassLevelId == id && (x.TotalScore == 0 || x.TotalScore != 0)).ToListAsync();
                            if (subRemove.Count() > 1)
                            {
                                var subRemove2 = await db.EnrolledSubjects.Include(x => x.Enrollments).Where(x => x.SubjectId == sub.SubjectId && x.EnrollmentId == enrol.Id && x.Enrollments.SessionId == csession.Id && x.Enrollments.ClassLevelId == id && (x.TotalScore == 0 || x.TotalScore != 0)).Take(1).ToListAsync();
                                foreach (var subRev in subRemove2)
                                {
                                    //var subRemove3 = await db.EnrolledSubjects.Include(x => x.Enrollments).Where(x => x.SubjectId == subRev.SubjectId && x.EnrollmentId == enrol.Id && x.Enrollments.SessionId == csession.Id && x.Enrollments.ClassLevelId == id).ToListAsync();
                                    //foreach (var rev in subRemove3)
                                    //{
                                    if (subRev.TotalScore == 0 || subRev.TotalScore != 0)
                                    {
                                        db.EnrolledSubjects.Remove(subRev);
                                        await db.SaveChangesAsync();
                                    }

                                    //}
                                }
                            }


                        }

                    }
                }
                //}

            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}