using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data;
using SchoolPortal.Web.Models.Dtos;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace SchoolPortal.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class ResultManagementController : Controller
    {
        #region services


        private ApplicationDbContext db = new ApplicationDbContext();
        private ISessionService _sessionService = new SessionService();
        private IClassLevelService _classlevelService = new ClassLevelService();
        private IPublishResultService _publishService = new PublishResultService();
        private IEnrollmentService _enrolService = new EnrollmentService();
        private IResultService _resultService = new ResultService();
        private IStudentProfileService _studentService = new StudentProfileService();
        private ISettingService _settingService = new SettingService();
        private IEnrolledSubjectService _enrolledService = new EnrolledSubjectService();


        public ResultManagementController()
        {

        }
        public ResultManagementController(
            SessionService sessionService,
            ClassLevelService classLevelService,
            PublishResultService publishService,
            ResultService resultService,
            StudentProfileService studentService,
            SettingService settingService,
            EnrolledSubjectService enrolledService,
            EnrollmentService enrolService,
            ApplicationUserManager userManager
            )
        {
            _sessionService = sessionService;
            _classlevelService = classLevelService;
            _publishService = publishService;
            _enrolService = enrolService;
            _resultService = resultService;
            _studentService = studentService;
            _settingService = settingService;
            _enrolledService = enrolledService;
            UserManager = userManager;
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
        // GET: Admin/ResultManagement
        public async Task<ActionResult> Index()
        {
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel.OrderBy(x => x.ClassLevelName), "Id", "ClassLevelName");

            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            ViewBag.sessionId = session.Id;
            ViewBag.session = session.SessionYear + " - " + session.Term + " Term";
            return View();
        }

        public async Task<ActionResult> OtherTerm()
        {
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel.OrderBy(x => x.ClassLevelName), "Id", "ClassLevelName");

            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session.OrderBy(x => x.FullSession), "Id", "FullSession");
            return View();
        }

        //NEW ENROLMENT
        public async Task<ActionResult> NewEnrolment()
        {
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel.OrderBy(x => x.ClassLevelName), "Id", "ClassLevelName");

            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session.OrderByDescending(x => x.FullSession), "Id", "FullSession");
            return View();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AllStudents(int sessId, int classId)
        {
            if (sessId == null || classId == null)
            {
                TempData["error"] = "make a selection";
            }

            IQueryable<StudentProfile> user = from s in db.StudentProfiles
                                              .Include(x=>x.user)
                                               .Include(p => p.user).Where(x => x.user.Status == EntityStatus.Active)
                                               .OrderBy(x => x.user.UserName)
                                              select s;
            var classname = db.ClassLevels.FirstOrDefault(x => x.Id == classId);
            ViewBag.cname = classname.ClassName;
            var sessionname = db.Sessions.FirstOrDefault(x => x.Id == sessId);
            ViewBag.sname = sessionname.SessionYear;
            ViewBag.sessid = sessId;
            ViewBag.classid = classId;
            return View(user);
        }
        #region Batch result



        //batch print
        public async Task<ActionResult> BatchResult()
        {
             var sett = db.Settings.FirstOrDefault();
            if(sett.DisableAllResultPrinting == true)
            {
                return RedirectToAction("Verify", "Panel", new {area="Student"});
            }
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel.OrderBy(x => x.ClassLevelName), "Id", "ClassLevelName");

            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session.OrderByDescending(x => x.FullSession), "Id", "FullSession");
            return View();
        }

        public async Task<ActionResult> StudentsForBatchResult(int sessId, int classId)
        {
            var enrolledStudents = await _resultService.StudentsBySessIdAndByClassId(sessId, classId);
            if (enrolledStudents.Count() <= 0)
            {
                return HttpNotFound();
            }
            //UpdateComment recognitive domain
            foreach (var abcd in enrolledStudents)
            {
                var studentsss = await db.StudentProfiles.FirstOrDefaultAsync(x => x.StudentRegNumber == abcd.StudentRegNumber);
                var check = await db.RecognitiveDomains.FirstOrDefaultAsync(x => x.EnrolmentId == abcd.EnrollmentId);
                if (check == null)
                {
                    RecognitiveDomain recognitive = new RecognitiveDomain();
                    recognitive.EnrolmentId = abcd.EnrollmentId;
                    db.RecognitiveDomains.Add(recognitive);
                    await db.SaveChangesAsync();

                    //Add Tracking
                    var userId2 = User.Identity.GetUserId();
                    var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Updated Recognitive Domain";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }

                var check2 = await db.AffectiveDomains.FirstOrDefaultAsync(x => x.EnrolmentId == abcd.EnrollmentId);
                if (check2 == null)
                {
                    AffectiveDomain affective = new AffectiveDomain();
                    affective.EnrolmentId = abcd.EnrollmentId;
                    db.AffectiveDomains.Add(affective);
                    await db.SaveChangesAsync();

                    //Add Tracking
                    var userId2 = User.Identity.GetUserId();
                    var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Updated Affective Domain";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }

                var check3 = await db.PsychomotorDomains.FirstOrDefaultAsync(x => x.EnrolmentId == abcd.EnrollmentId);
                if (check3 == null)
                {
                    PsychomotorDomain psychomotor = new PsychomotorDomain();
                    psychomotor.EnrolmentId = abcd.EnrollmentId;
                    db.PsychomotorDomains.Add(psychomotor);
                    await db.SaveChangesAsync();

                    //Add Tracking
                    var userId2 = User.Identity.GetUserId();
                    var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Updated Psychomotor Domain";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }


            }
            ViewBag.sessId = sessId;
            ViewBag.classId = classId;
            var classlevel = await _classlevelService.Get(classId);

            var classResult = await _classlevelService.Get(classId);
            var showPosOnClassResult = classResult.ShowPositionOnClassResult;
            ViewBag.showPosOnClassResult = showPosOnClassResult;

            var session = await _sessionService.Get(sessId);
            ViewBag.Id = sessId;
            ViewBag.ClassId = classId;
            ViewBag.Term = session.Term.ToLower();
            ViewBag.Class = classlevel.ClassName;
            var stinfo = classlevel.UserId;
            var staff = await db.Users.FirstOrDefaultAsync(x => x.Id == stinfo);
            ViewBag.formteacher = staff.Surname + " " + staff.FirstName + " " + staff.OtherName;
            ViewBag.Session = session.SessionYear + " - " + session.Term + " Term";
            var batstudentenro = enrolledStudents.Where(x => x.AverageScore > 0);
            return View(batstudentenro);
        }

        //
        //
        // BatchResult printing


        [AllowAnonymous]
        public async Task<ActionResult> ViewBatch(int sessId, int classId)
        {
             var sett = db.Settings.FirstOrDefault();
            if(sett.DisableAllResultPrinting == true)
            {
                return RedirectToAction("Verify", "Panel", new {area="Student"});
            }
            
            var sessionlist = await _sessionService.GetAllSession();
            var publishcheck = await db.PublishResults.FirstOrDefaultAsync(x => x.ClassLevelId == classId);
            if (publishcheck == null)
            {
                TempData["error"] = "Result Not Yet Ready.";
                return RedirectToAction("BatchResult");
            }
            var batchIdInfo = "";
            var enrolledStudents = await _resultService.StudentsBySessIdAndByClassId(sessId, classId);
            var batcenro = enrolledStudents.Where(x => x.AverageScore > 0).ToList();
            System.Random randomInteger = new System.Random();
            int genNumber = randomInteger.Next(10000);
            foreach (var studentresult in batcenro)
            {
                var studentId = await db.StudentProfiles.FirstOrDefaultAsync(x => x.StudentRegNumber == studentresult.StudentRegNumber);

                var checkDebtProfile = await _resultService.GetDefaulterByProfileId(studentId.Id);

                if (checkDebtProfile == null)
                {
                    var enrollment = await _resultService.GetEnrollmentBySessIdStudentProfileId(sessId, studentId.Id);

                    BatchResult batch = new BatchResult();
                    batch.ProfileId = studentId.Id;
                    batch.ClassId = classId;
                    batch.SessionId = sessId;
                    batch.StudentRegNumber = studentId.StudentRegNumber;
                    batch.EnrollmentId = enrollment.Id;
                    batch.BatchId = genNumber + enrollment.ClassLevel.ClassName;
                    batch.AverageScore = studentresult.AverageScore;
                    db.BatchResults.Add(batch);
                    batchIdInfo = batch.BatchId;
                    await db.SaveChangesAsync();


                    //Add Tracking
                    //var userId2 = User.Identity.GetUserId();
                    //var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                    //Tracker tracker = new Tracker();
                    //tracker.UserId = userId2;
                    //tracker.UserName = user2.UserName;
                    //tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    //tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    //tracker.Note = tracker.FullName + " " + "Updated Batch Results";
                    ////db.Trackers.Add(tracker);
                    //await db.SaveChangesAsync();
                }
            }



            if (sett.PrintOutOption == PrintOutOption.PrintOutOne)
            {
                return RedirectToAction("Preview", new { batchid = batchIdInfo });

            }
            else if (sett.PrintOutOption == PrintOutOption.PrintOutTwo)
            {
                return RedirectToAction("Preview2", new { batchid = batchIdInfo });
            }
            else if (sett.PrintOutOption == PrintOutOption.PrintOutThree)
            {
                return RedirectToAction("Preview3", new { batchid = batchIdInfo });
            }
            else
            {
                TempData["failure"] = "No Print type selected";
                return RedirectToAction("BatchResult");

            }


        }


        [AllowAnonymous]
        public async Task<ActionResult> Preview(string batchid)
        { 
            var sett = db.Settings.FirstOrDefault();
            if(sett.DisableAllResultPrinting == true)
            {
                return RedirectToAction("Verify", "Panel", new {area="Student"});
            }

            var sessions = await db.Sessions.FirstOrDefaultAsync(x=>x.Status == SessionStatus.Current);
            ViewBag.session = sessions;

            var items = await db.BatchResults.Where(x => x.BatchId == batchid).ToListAsync();
            return View(items);
        }


      


        [AllowAnonymous]
        public async Task<ActionResult> Preview2(string batchid)
        {
             var sett = db.Settings.FirstOrDefault();
            if(sett.DisableAllResultPrinting == true)
            {
                return RedirectToAction("Verify", "Panel", new {area="Student"});
            }
            var items = await db.BatchResults.Where(x => x.BatchId == batchid).ToListAsync();
            return View(items);
        }

        [AllowAnonymous]
        public async Task<ActionResult> Preview3(string batchid)
        {
             var sett = db.Settings.FirstOrDefault();
            if(sett.DisableAllResultPrinting == true)
            {
                return RedirectToAction("Verify", "Panel", new {area="Student"});
            }
            var items = await db.BatchResults.Where(x => x.BatchId == batchid).ToListAsync();
            return View(items);
        }
        #endregion
        public async Task<ActionResult> Students(int sessId, int classId)
        {
            var sett = db.Settings.FirstOrDefault();
            ViewBag.sett = sett;
            ViewBag.enablePreviewResult = sett.EnablePreviewResult;
            ViewBag.Disableresults = sett.DisableAllResultPrinting;
            await _enrolService.UpdateEnrollmentStatus(classId);
            var enrolledStudents = await _resultService.StudentsBySessIdAndByClassId(sessId, classId);
            if (enrolledStudents.Count() <= 0)
            {
                TempData["msg"] = "HttpNotFound";
                return RedirectToAction("Index");
            }



            var setting = db.Settings.FirstOrDefault();
            ViewBag.setting = setting;

            var classResult = await _classlevelService.Get(classId);
            var showPosOnClassResult = classResult.ShowPositionOnClassResult;
            ViewBag.showPosOnClassResult = showPosOnClassResult;

            //UpdateComment recognitive domain
            foreach (var abcd in enrolledStudents)
            {
                var studentsss = await db.StudentProfiles.FirstOrDefaultAsync(x => x.StudentRegNumber == abcd.StudentRegNumber);
                var checki = await db.RecognitiveDomains.FirstOrDefaultAsync(x => x.EnrolmentId == abcd.EnrollmentId);
                if (checki == null)
                {
                    RecognitiveDomain recognitive = new RecognitiveDomain();
                    recognitive.EnrolmentId = abcd.EnrollmentId;
                    db.RecognitiveDomains.Add(recognitive);
                    await db.SaveChangesAsync();

                    //Add Tracking
                    //var userId2 = User.Identity.GetUserId();
                    //var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                    //Tracker tracker = new Tracker();
                    //tracker.UserId = userId2;
                    //tracker.UserName = user2.UserName;
                    //tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    //tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    //tracker.Note = tracker.FullName + " " + "Updated Recognitive Domain";
                    ////db.Trackers.Add(tracker);
                    //await db.SaveChangesAsync();
                }


            }

            //UpdateComment psychomotor domain
            foreach (var abcd in enrolledStudents)
            {
                var studentsss = await db.StudentProfiles.FirstOrDefaultAsync(x => x.StudentRegNumber == abcd.StudentRegNumber);
                var checki = await db.PsychomotorDomains.FirstOrDefaultAsync(x => x.EnrolmentId == abcd.EnrollmentId);
                if (checki == null)
                {
                    PsychomotorDomain psychomotor = new PsychomotorDomain();
                    psychomotor.EnrolmentId = abcd.EnrollmentId;
                    db.PsychomotorDomains.Add(psychomotor);
                    await db.SaveChangesAsync();


                    //Add Tracking
                    //var userId2 = User.Identity.GetUserId();
                    //var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                    //Tracker tracker = new Tracker();
                    //tracker.UserId = userId2;
                    //tracker.UserName = user2.UserName;
                    //tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    //tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    //tracker.Note = tracker.FullName + " " + "Updated Psychomotor Domain";
                    ////db.Trackers.Add(tracker);
                    //await db.SaveChangesAsync();
                }


            }


            //UpdateComment School Fees
            //foreach (var abcd in enrolledStudents)
            //{
            //    var studentsss = await db.StudentProfiles.FirstOrDefaultAsync(x => x.StudentRegNumber == abcd.StudentRegNumber);
            //    var checki = await db.SchoolFees.FirstOrDefaultAsync(x => x.EnrolmentId == abcd.EnrollmentId);
            //    if (checki == null)
            //    {
            //        SchoolFees fee = new SchoolFees();
            //        fee.EnrolmentId = abcd.EnrollmentId;
            //        db.SchoolFees.Add(fee);
            //        await db.SaveChangesAsync();
            //    }


            //}


            //UpdateComment affective domain
            foreach (var abcd in enrolledStudents)
            {
                var studentsss = await db.StudentProfiles.FirstOrDefaultAsync(x => x.StudentRegNumber == abcd.StudentRegNumber);
                var checki = await db.AffectiveDomains.FirstOrDefaultAsync(x => x.EnrolmentId == abcd.EnrollmentId);
                if (checki == null)
                {
                    AffectiveDomain affective = new AffectiveDomain();
                    affective.EnrolmentId = abcd.EnrollmentId;
                    db.AffectiveDomains.Add(affective);
                    await db.SaveChangesAsync();


                    //Add Tracking
                    //var userId2 = User.Identity.GetUserId();
                    //var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                    //Tracker tracker = new Tracker();
                    //tracker.UserId = userId2;
                    //tracker.UserName = user2.UserName;
                    //tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    //tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    //tracker.Note = tracker.FullName + " " + "Updated Affective Domain";
                    ////db.Trackers.Add(tracker);
                    //await db.SaveChangesAsync();
                }


            }
            var check = await _publishService.CheckPublishResult(sessId, classId);
            if (check == true)
            {
                ViewBag.Check = "Published";
            }
            else
            {
                ViewBag.Check = "Not Published";
            }

            var classlevel = await _classlevelService.Get(classId);
            var session = await _sessionService.Get(sessId);
            ViewBag.Id = sessId;
            ViewBag.ClassId = classId;
            ViewBag.sessionId = session.Id;
            ViewBag.Term = session.Term.ToLower();
            ViewBag.Class = classlevel.ClassName;
            ViewBag.ClassSubjectCount = classlevel.Subjects.Count();
            var stinfo = classlevel.UserId;
            var staff = await db.Users.FirstOrDefaultAsync(x => x.Id == stinfo);
            ViewBag.formteacher = staff.Surname + " " + staff.FirstName + " " + staff.OtherName;
            ViewBag.Session = session.SessionYear + " - " + session.Term + " Term";
            foreach (var name in enrolledStudents)
            {
                if (name.StudentName.Substring(0, 2).Contains("\t"))
                {
                    name.StudentName = name.StudentName.Remove(0, 2);
                }
                if (name.StudentName.Substring(0, 2).Contains(" "))
                {
                    name.StudentName = name.StudentName.TrimStart();
                }
            }
            return View(enrolledStudents);
        }

        public async Task<ActionResult> ResultPreview(int sessId, int classId)
        {
            var enrolledStudents = await _resultService.StudentsBySessIdAndByClassId(sessId, classId);
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Id == sessId);
            ViewBag.session = session;

            return View(enrolledStudents);
        }


        public async Task<ActionResult> ResultPreview2(int sessId, int classId)
        {
            var enrolledStudents = await _resultService.StudentsBySessIdAndByClassId(sessId, classId);
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Id == sessId);
            ViewBag.session = session;

            //var classResult = await _classlevelService.Get(classId);
            //var showPosOnClassResult = classResult.ShowPositionOnClassResult;
            //ViewBag.showPosOnClassResult = showPosOnClassResult;
            return View(enrolledStudents);
        }

        public async Task<ActionResult> ResultPreview3(int sessId, int classId)
        {
            var enrolledStudents = await _resultService.StudentsBySessIdAndByClassId(sessId, classId);
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Id == sessId);
            ViewBag.session = session;

            //var classResult = await _classlevelService.Get(classId);
            //var showPosOnClassResult = classResult.ShowPositionOnClassResult;
            //ViewBag.showPosOnClassResult = showPosOnClassResult;
            return View(enrolledStudents);
        }

        //update Regonitive domain

        public ActionResult UpdateInformation(int StudentId, int classId, int sessionId)
        {


            ViewBag.ClassId = classId;
            ViewBag.sessionId = sessionId;
            var recognitive = db.RecognitiveDomains.FirstOrDefault(x => x.EnrolmentId == StudentId);

            return View(recognitive);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult UpdateInformation(int StudentId, int classId, int sessionId, string Rememberance, string Understanding, string Application, string Analyzing, string Evaluation, string Creativity)
        {
            try
            {


                var recognitive = db.RecognitiveDomains.FirstOrDefault(x => x.EnrolmentId == StudentId);

                recognitive.Rememberance = Rememberance;
                recognitive.Understanding = Understanding;
                recognitive.Application = Application;
                recognitive.Evaluation = Evaluation;
                recognitive.Analyzing = Analyzing;
                recognitive.Creativity = Creativity;
                db.Entry(recognitive).State = EntityState.Modified;
                db.SaveChanges();


                ////Add Tracking
                //var userId2 = User.Identity.GetUserId();
                //var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                //Tracker tracker = new Tracker();
                //tracker.UserId = userId2;
                //tracker.UserName = user2.UserName;
                //tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                //tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                //tracker.Note = tracker.FullName + " " + "Updated Recognitive Domain";
                ////db.Trackers.Add(tracker);
                //db.SaveChangesAsync();
                TempData["success"] = "Successful.";
                return RedirectToAction("Students", new { classId = classId, sessId = sessionId });
            }
            catch (Exception e)
            {
                TempData["error"] = "Not Successful.";
                return RedirectToAction("Students", new { classId = classId, sessId = sessionId });
            }
        }


        //Update Psychomotor Domain

        //update Psychomotor domain

        public ActionResult UpdatePsychomotorDomain(int StudentId, int classId, int sessionId)
        {


            ViewBag.ClassId = classId;
            ViewBag.sessionId = sessionId;
            var psychomotor = db.PsychomotorDomains.FirstOrDefault(x => x.EnrolmentId == StudentId);

            return View(psychomotor);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult UpdatePsychomotorDomain(int StudentId, int classId, int sessionId, string Club, string Drawing, string Painting, string Handwriting, string Hobbies, string Speech, string Sports)
        {
            try
            {


                var psychomotor = db.PsychomotorDomains.FirstOrDefault(x => x.EnrolmentId == StudentId);

                psychomotor.Club = Club;
                psychomotor.Drawing = Drawing;
                psychomotor.Painting = Painting;
                psychomotor.Handwriting = Handwriting;
                psychomotor.Hobbies = Hobbies;
                psychomotor.Speech = Speech;
                psychomotor.Sports = Sports;
                db.Entry(psychomotor).State = EntityState.Modified;
                db.SaveChanges();

                ////Add Tracking
                //var userId2 = User.Identity.GetUserId();
                //var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                //Tracker tracker = new Tracker();
                //tracker.UserId = userId2;
                //tracker.UserName = user2.UserName;
                //tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                //tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                //tracker.Note = tracker.FullName + " " + "Updated Psychomotor Domain";
                ////db.Trackers.Add(tracker);
                //db.SaveChangesAsync();

                TempData["success"] = "Successful.";
                return RedirectToAction("Students", new { classId = classId, sessId = sessionId });
            }
            catch (Exception e)
            {
                TempData["error"] = "Not Successful.";
                return RedirectToAction("Students", new { classId = classId, sessId = sessionId });
            }
        }
        [Authorize(Roles = "SuperAdmin")]

        public async Task<ActionResult> RemoveEnrolSubjectByClass()
        {
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel.OrderBy(x => x.ClassLevelName), "Id", "ClassLevelName");


            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]

        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveEnrolSubjectByClass(int ClassLevelId, int SubjectId)
        {
            try
            {

                var enrolledsubject = await db.EnrolledSubjects.Where(x => x.SubjectId == SubjectId && x.Enrollments.ClassLevelId == ClassLevelId).ToListAsync();

                foreach (var i in enrolledsubject)
                {
                    db.EnrolledSubjects.Remove(i);

                }
                await db.SaveChangesAsync();

                TempData["success"] = "Successful.";
                return RedirectToAction("RemoveEnrolSubjectByClass");
            }
            catch (Exception e)
            {
                TempData["error"] = "Not Successful.";
                return RedirectToAction("RemoveEnrolSubjectByClass");
            }
            return View();
        }



        //update Affective domain

        public ActionResult UpdateAffectiveDomain(int StudentId, int classId, int sessionId)
        {


            ViewBag.ClassId = classId;
            ViewBag.sessionId = sessionId;
            var psychomotor = db.AffectiveDomains.FirstOrDefault(x => x.EnrolmentId == StudentId);

            return View(psychomotor);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult UpdateAffectiveDomain(int StudentId, int classId, int sessionId, string Attentiveness, string Honesty, string Neatness, string Punctuality, string Relationship)
        {
            try
            {


                var affective = db.AffectiveDomains.FirstOrDefault(x => x.EnrolmentId == StudentId);

                affective.Attentiveness = Attentiveness;
                affective.Honesty = Honesty;
                affective.Neatness = Neatness;
                affective.Punctuality = Punctuality;
                affective.Relationship = Relationship;
                db.Entry(affective).State = EntityState.Modified;
                db.SaveChanges();


                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Updated Affective Domain";
                //db.Trackers.Add(tracker);
                db.SaveChangesAsync();

                TempData["success"] = "Successful.";
                return RedirectToAction("Students", new { classId = classId, sessId = sessionId });
            }
            catch (Exception e)
            {
                TempData["error"] = "Not Successful.";
                return RedirectToAction("Students", new { classId = classId, sessId = sessionId });
            }
        }



        public ActionResult UpdateResultRemarkbyEnrolment(int EnrolId, int classId, int sessionId)
        {
            var remark = db.Enrollments.FirstOrDefault(x => x.Id == EnrolId);
            ViewBag.enr = remark;
            return View(remark);



        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult UpdateResultRemarkbyEnrolment(int EnrolId, int classId, int sessionId, string Enroll1, string Enroll2, string NextTerm2)
        {
            try
            {
                var remark = db.Enrollments.FirstOrDefault(x => x.Id == EnrolId);

                remark.EnrollmentRemark1 = Enroll1;
                remark.EnrollmentRemark2 = Enroll2;
                remark.NextResumption = NextTerm2;

                db.Entry(remark).State = EntityState.Modified;
                db.SaveChanges();


                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Updated Enrollment Remark";
                //db.Trackers.Add(tracker);
                db.SaveChangesAsync();

                TempData["success"] = "Successful.";
                return RedirectToAction("Students", new { classId = classId, sessId = sessionId });
            }
            catch (Exception e)
            {
                TempData["error"] = "Not Successful.";
                return RedirectToAction("Students", new { classId = classId, sessId = sessionId });
            }
        }


        //School Fees On Each Student Result
        //public ActionResult SchoolFees(int StudentId, int classId, int sessionId)
        //{
        //    var fee = db.SchoolFees.FirstOrDefault(x => x.EnrolmentId == StudentId);
        //    ViewBag.fee = fee;
        //    return View(fee);
        //}

        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public ActionResult SchoolFees(int StudentId, int classId, int sessionId, string SchFee, string Discount, string Duefee)
        //{
        //    try
        //    {
        //        var fee = db.SchoolFees.FirstOrDefault(x => x.EnrolmentId == StudentId);

        //        fee.Amount = SchFee;
        //        fee.Discount = Discount;
        //        fee.AmountDue = Duefee;
        //        db.Entry(fee).State = EntityState.Modified;
        //        db.SaveChanges();
        //        TempData["success"] = "Successful.";
        //        return RedirectToAction("Students", new { classId = classId, sessId = sessionId });
        //    }

        //    catch (Exception e)
        //    {
        //        TempData["error"] = "Not Successful.";
        //        return RedirectToAction("Students", new { classId = classId, sessId = sessionId });
        //    }
        //}





        /// <summary>
        /// /
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 

        //public JsonResult GetEmployees(int id)
        //{
        //   var list = db.EnrolledSubjects.
        //    return Json(new { mystudents = list });
        //}

        public async Task<ActionResult> EditStudentResult(int id)
        {
            decimal? total = await _resultService.SumEnrolledSubjectTotalScore(id);
            int StudentSubjectCount = await _resultService.TotalEnrolledSubjectCount(id);

            if (StudentSubjectCount == 0)
            {
                ViewBag.Sum = "No Subject has been entered";
            }
            else
            {
                decimal? dsum = total / StudentSubjectCount;
                var sum = decimal.Parse(dsum.ToString());
                ViewBag.Sum = Math.Round(sum, 2, MidpointRounding.AwayFromZero);
            }

            var enroledStudentData = await _enrolService.Get(id);
            var user = await _studentService.Get(enroledStudentData.StudentProfileId);
            string fullname = user.Fullname;
            ViewBag.Fullname = fullname;
            ViewBag.Class = enroledStudentData.ClassLevel.ClassName;
            ViewBag.Session = enroledStudentData.Session.SessionYear + "-" + enroledStudentData.Session.Term;
            ViewBag.SessionId = enroledStudentData.SessionId;


            var positions = await _resultService.Position(enroledStudentData.SessionId, enroledStudentData.ClassLevelId);
            int totalStudents = positions.Count();
            int position = 0;
            decimal? avg = 0;

            if (StudentSubjectCount != 0)
            {
                foreach (var p in positions)
                {
                    if (p.AverageScore != null)
                    {
                        position = position + 1;
                        if (p.Id == id)
                        {
                            avg = p.AverageScore.Value;
                            goto outloop;
                        }
                    }
                }

            outloop: ViewData["Position"] = position;
                ViewData["Avg"] = avg;
                ViewBag.TotalStudents = totalStudents;


            }
            ViewBag.SessionId = enroledStudentData.SessionId;
            ViewBag.ClassId = enroledStudentData.ClassLevelId;
            ViewBag.EnrollmentId = enroledStudentData.Id;

            var list = await _resultService.EnrolledSubjectForEnrollment(enroledStudentData.Id);
            return View(list);
        }

        //RECONSILE EDITRESULT

        public async Task<ActionResult> ReconcileEditStudentResult(int id)
        {
            decimal? total = await _resultService.SumEnrolledSubjectTotalScore(id);
            int StudentSubjectCount = await _resultService.TotalEnrolledSubjectCount(id);

            if (StudentSubjectCount == 0)
            {
                ViewBag.Sum = "No Subject has been entered";
            }
            else
            {
                decimal? dsum = total / StudentSubjectCount;
                var sum = decimal.Parse(dsum.ToString());
                ViewBag.Sum = Math.Round(sum, 2, MidpointRounding.AwayFromZero);
            }

            var enroledStudentData = await _enrolService.Get(id);
            var user = await _studentService.Get(enroledStudentData.StudentProfileId);
            string fullname = user.Fullname;
            ViewBag.Fullname = fullname;
            ViewBag.Class = enroledStudentData.ClassLevel.ClassName;
            ViewBag.Session = enroledStudentData.Session.SessionYear + "-" + enroledStudentData.Session.Term;
            ViewBag.SessionId = enroledStudentData.SessionId;


            var positions = await _resultService.Position(enroledStudentData.SessionId, enroledStudentData.ClassLevelId);
            int totalStudents = positions.Count();
            int position = 0;
            decimal? avg = 0;

            if (StudentSubjectCount != 0)
            {
                foreach (var p in positions)
                {
                    if (p.AverageScore != null)
                    {
                        position = position + 1;
                        if (p.Id == id)
                        {
                            avg = p.AverageScore.Value;
                            goto outloop;
                        }
                    }
                }

            outloop: ViewData["Position"] = position;
                ViewData["Avg"] = avg;
                ViewBag.TotalStudents = totalStudents;


            }
            ViewBag.SessionId = enroledStudentData.SessionId;
            ViewBag.ClassId = enroledStudentData.ClassLevelId;
            ViewBag.EnrollmentId = enroledStudentData.Id;

            var list = await _resultService.EnrolledSubjectForEnrollment(enroledStudentData.Id);

            return View(list);
        }

        /// <summary>
        /// //
        /// 
        public async Task<ActionResult> EditStudentResultClass(int id)
        {


            var listing = new List<EnrolledSubject>();
            var merge = new List<EnrolledSubject>();
            var sess = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var enrollments = db.Enrollments.Where(x => x.ClassLevelId == id && x.SessionId == sess.Id).ToList();
            foreach (var i in enrollments)
            {
                var list = await _resultService.EnrolledSubjectForEnrollment(i.Id);
                merge.AddRange(listing.Concat(list.ToList()));
            }

            var lis = merge.Select(x => x.Subject.SubjectName).Distinct();
            ViewBag.list = lis;
            return View(lis);
        }

        public async Task<ActionResult> RemoveEnrolsub(int id)
        {
            EnrolledSubject assignment = db.EnrolledSubjects.FirstOrDefault(x => x.Id == id);
            db.EnrolledSubjects.Remove(assignment);
            db.SaveChangesAsync();

            //Add Tracking
            var userId2 = User.Identity.GetUserId();
            var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
            Tracker tracker = new Tracker();
            tracker.UserId = userId2;
            tracker.UserName = user2.UserName;
            tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
            tracker.ActionDate = DateTime.UtcNow.AddHours(1);
            tracker.Note = tracker.FullName + " " + "Removed Enrolled Subject";
            //db.Trackers.Add(tracker);
            await db.SaveChangesAsync();

            return Content("success");

        }
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>


        public async Task<ActionResult> PreviewAndPublish(int id)
        {
            decimal? total = await _resultService.SumEnrolledSubjectTotalScore(id);
            int StudentSubjectCount = await _resultService.TotalEnrolledSubjectCount(id);

            if (StudentSubjectCount == 0)
            {
                ViewBag.Sum = "No Subject has been entered";
            }
            else
            {
                decimal? dsum = total / StudentSubjectCount;
                var sum = decimal.Parse(dsum.ToString());
                ViewBag.Sum = Math.Round(sum, 2, MidpointRounding.AwayFromZero);

            }

            var enroledStudentData = await _enrolService.Get(id);
            var user = await _studentService.Get(enroledStudentData.StudentProfileId);
            string fullname = user.Fullname;
            ViewBag.Fullname = fullname;
            ViewBag.Class = enroledStudentData.ClassLevel.ClassName;


            var positions = await _resultService.Position(enroledStudentData.SessionId, enroledStudentData.ClassLevelId);
            int totalStudents = positions.Count();
            int position = 0;
            decimal? avg = 0;

            if (StudentSubjectCount != 0)
            {
                foreach (var p in positions)
                {
                    if (p.AverageScore != null)
                    {
                        position = position + 1;
                        if (p.Id == id)
                        {
                            avg = p.AverageScore.Value;
                            goto outloop;
                        }
                    }
                }

            outloop: ViewData["Position"] = position;
                ViewData["Avg"] = avg;
                ViewBag.TotalStudents = totalStudents;


            }
            ViewBag.SessionId = enroledStudentData.SessionId;
            ViewBag.ClassId = enroledStudentData.ClassLevelId;
            ViewBag.EnrollmentId = enroledStudentData.Id;

            var classResult = await _classlevelService.Get(id);
            var showPosOnClassResult = classResult.ShowPositionOnClassResult;
            ViewBag.showPosOnClassResult = showPosOnClassResult;

            var list = await _resultService.EnrolledSubjectForEnrollment(enroledStudentData.Id);
            return View(list);
        }

        public async Task<ActionResult> IsOffered(int id, int enroId)
        {
            var enr = await db.EnrolledSubjects.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            try
            {

                if (enr.IsOffered == true)
                {
                    enr.IsOffered = false;
                }
                else if (enr.IsOffered == false)
                {
                    enr.IsOffered = true;
                }
                db.Entry(enr).State = EntityState.Modified;
                await db.SaveChangesAsync();

                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Updated Subject Offered";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();


            }
            catch (Exception e)
            {
                try
                {
                    if (enr.IsOffered == true)
                    {
                        enr.IsOffered = false;
                    }
                    else if (enr.IsOffered == false)
                    {
                        enr.IsOffered = true;
                    }
                    db.Entry(enr).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    //Add Tracking
                    var userId2 = User.Identity.GetUserId();
                    var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Updated Subject Offered";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
                catch (Exception c)
                {

                }

            }

            return RedirectToAction("EditStudentResult", new { id = enroId });
        }

        public async Task<ActionResult> Edit(int id)
        {


            var item = await _enrolledService.Get(id);
            var user = await _enrolService.Get(item.EnrollmentId);
            var name = await _studentService.Get(user.StudentProfileId);
            string fullname = name.Fullname;
            ViewBag.exam = user.ClassLevel.ExamScore; 
            ViewBag.acc = user.ClassLevel.AccessmentScore;
            ViewBag.StudentsName = fullname;
            ViewBag.sessionname = user.Session.SessionYear + "-" + user.Session.Term;
            ViewBag.c = user.ClassLevel.ClassName;

            var subjectsetting = await db.Subjects.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == item.Subject.Id);
            ViewBag.subsettingdata = subjectsetting;
            return View(item);
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EnrolledSubject models)
        {
            if (ModelState.IsValid)
            {
                var cid = await db.Enrollments.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == models.EnrollmentId);
                var setting = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == cid.ClassLevelId);

                if (models.TestScore == null)
                {
                    models.TestScore = 0;
                };
                if (models.ExamScore == null)
                {
                    models.ExamScore = 0;
                };
                if (models.Project == null)
                {
                    models.Project = 0;
                };
                if (models.ClassExercise == null)
                {
                    models.ClassExercise = 0;
                };
                if (models.TestScore2 == null)
                {
                    models.TestScore2 = 0;
                };
                if (models.Assessment == null)
                {
                    models.Assessment = 0;
                };
                if (models.TotalCA == null)
                {
                    models.TotalCA = 0;
                };
                if (models.TotalScore == null)
                {
                    models.TotalScore = 0;
                };
                var mainsett = await db.Settings.FirstOrDefaultAsync();
                ViewBag.sxsetting = mainsett;
                var subjectsetting = await db.Subjects.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == models.SubjectId);
                var totalCA = models.Project + models.ClassExercise + models.TestScore + models.TestScore2 + models.Assessment;
                //var totCA = setting.Project + setting.ClassExercise + setting.AccessmentScore + setting.TestScore2 + setting.Assessment;
                var totalSubCA = subjectsetting.Project + subjectsetting.ClassExercise + subjectsetting.TestScore + subjectsetting.TestScore2 + subjectsetting.Assessment;
                if (totalCA > totalSubCA)
                {
                    var settings = db.Settings.FirstOrDefault();
                    TempData["error"] = "Assessment Score or Exam Score not in Range";
                    if (settings.EnableTestScore == true)
                    {
                        TempData["error1"] = "Test Score  not in Range";
                    }
                    if (settings.EnableTestScore2 == true)
                    {
                        TempData["error2"] = "2nd Test Score  not in Range";
                    }

                    if (settings.EnableProject == true)
                    {
                        TempData["error3"] = "Project Score  not in Range";
                    }
                    if (settings.EnableAssessment == true)
                    {
                        TempData["error4"] = "Assessment Score not in Range";
                    }
                    if (settings.EnableClassExercise == true)
                    {
                        TempData["error5"] = "Class Exercise Score not in Range";
                    }
                    if (settings.EnableExamScore == true)
                    {

                        TempData["error6"] = "Exam Score not in Range";
                    }
                    //TempData["error"] = "Assessment Score or Exam Score not in Range";
                    return View();
                }
                var subject = await _resultService.GetSubjectByEnrolledSubId(models.SubjectId);
                var classlevel = await _resultService.GetClassByClassId(subject.ClassLevelId);
                string userLevel = classlevel.ClassName;

                try
                {
                    //decimal? totalScore = models.TestScore + models.ExamScore;
                    models.TotalCA = totalCA;
                    decimal? totalScore = totalCA + models.ExamScore;
                    models.TotalScore = totalScore;
                    models.IsOffered = true;

                    if (models.GradingOption == GradingOption.NONE)
                    {
                        if (userLevel.Substring(0, 3).Contains("SSS"))
                        {
                            models.GradingOption = GradingOption.SSS;
                        }
                        else if (userLevel.Substring(0, 3).Contains("JSS"))
                        {
                            models.GradingOption = GradingOption.JSS;
                        }
                        else if (userLevel.Substring(0, 3).Contains("PRE"))
                        {
                            models.GradingOption = GradingOption.PRE;
                        }
                        else if (userLevel.Substring(0, 3).Contains("PRI"))
                        {
                            models.GradingOption = GradingOption.PRI;
                        }
                        else if (userLevel.Substring(0, 3).Contains("NUR"))
                        {
                            models.GradingOption = GradingOption.NUR;
                        }
                        else if (userLevel.Substring(0, 2).Contains("PG"))
                        {
                            models.GradingOption = GradingOption.PG;
                        }
                    }

                    await _enrolledService.Edit(models);
                    await _resultService.UpdateResult(models.EnrollmentId);
                    TempData["msg"] = "success";
                    return RedirectToAction("EditStudentResult", new { id = models.EnrollmentId });


                }
                catch (Exception e)
                {

                }
            }
            string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
            return View(models);

        }

        [HttpPost]
        public async Task<ActionResult> UpdateScore(EnrolledSubject models)
        {
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var sessId = session.Id;
            var cid = await db.Enrollments.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == models.EnrollmentId);
            var setting = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == cid.ClassLevelId);
            if (models.TestScore == null)
            {
                models.TestScore = 0;
            };
            if (models.ExamScore == null)
            {
                models.ExamScore = 0;
            };
            if (models.Project == null)
            {
                models.Project = 0;
            };
            if (models.ClassExercise == null)
            {
                models.ClassExercise = 0;
            };
            if (models.TestScore2 == null)
            {
                models.TestScore2 = 0;
            };
            if (models.Assessment == null)
            {
                models.Assessment = 0;
            };
            if (models.TotalCA == null)
            {
                models.TotalCA = 0;
            };
            if (models.TotalScore == null)
            {
                models.TotalScore = 0;
            };
            var totalCA = models.Project + models.ClassExercise + models.TestScore + models.TestScore2 + models.Assessment;
            var totCA = setting.Project + setting.ClassExercise + setting.AccessmentScore + setting.TestScore2 + setting.Assessment;
            if (models.ExamScore > setting.ExamScore || totalCA > totCA)
            {
                var settings = db.Settings.FirstOrDefault();
                if (settings.EnableTestScore2 == true)
                {
                    TempData["error"] = "<span> Note: 2nd Test Score Total = " + setting.TestScore2 + "</span>";
                }
                if (settings.EnableTestScore == true)
                {
                    TempData["error"] = "<span> Note: Test Score Total = " + setting.AccessmentScore + "</span>";
                }
                if (settings.EnableProject == true)
                {
                    TempData["error"] = "<span> Note: Project Score Total = " + setting.Project + "</span>";
                }
                if (settings.EnableAssessment == true)
                {
                    TempData["error"] = "<span> Note: Class Exercise Score Total = " + setting.ClassExercise + "</span>";
                }
                if (settings.EnableClassExercise == true)
                {
                    TempData["error"] = "<span> Note: Assessment Score Total = " + setting.Assessment + "</span>";
                }
                TempData["error"] = "Assessment Score or Exam Score not in Range";
                ViewBag.sessId = sessId;
                return View(models);
            }
            var subject = await _resultService.GetSubjectByEnrolledSubId(models.SubjectId);
            var classlevel = await _resultService.GetClassByClassId(subject.ClassLevelId);
            string userLevel = classlevel.ClassName;

            try
            {

                models.TotalCA = totalCA;
                decimal? totalScore = totalCA + models.ExamScore;
                //decimal? totalScore = models.TestScore + models.ExamScore;
                models.TotalScore = totalScore;

                if (models.GradingOption == GradingOption.NONE)
                {
                    if (userLevel.Substring(0, 3).Contains("SSS"))
                    {
                        models.GradingOption = GradingOption.SSS;
                    }
                    else if (userLevel.Substring(0, 3).Contains("JSS"))
                    {
                        models.GradingOption = GradingOption.JSS;
                    }
                    else if (userLevel.Substring(0, 3).Contains("NUR"))
                    {
                        models.GradingOption = GradingOption.NUR;
                    }
                    else if (userLevel.Substring(0, 3).Contains("PRI"))
                    {
                        models.GradingOption = GradingOption.PRI;
                    }
                    else if (userLevel.Substring(0, 3).Contains("PRE"))
                    {
                        models.GradingOption = GradingOption.PRE;
                    }
                    else if (userLevel.Substring(0, 2).Contains("PG"))
                    {
                        models.GradingOption = GradingOption.PG;
                    }
                }

                await _enrolledService.Edit(models);
                await _resultService.UpdateResult(models.EnrollmentId);
                TempData["msg"] = "success";
                return RedirectToAction("EditStudentResult", new { id = models.EnrollmentId });


            }
            catch (Exception e)
            {

            }

            return new EmptyResult();
        }
        //update a class result

        public async Task<ActionResult> UpdateClassResult(int id = 0, int session = 0)
        {
            if (id != 0)
            {
                try
                {
                    int countSus = 0;
                    var cid = db.Enrollments.Where(x => x.ClassLevelId == id && x.SessionId == session);
                    foreach (var item in cid)
                    {
                        try
                        {
                            await _resultService.UpdateResult(item.Id);
                            countSus++;
                        }
                        catch (Exception e)
                        {

                        }

                    }

                    TempData["success"] = countSus + " Update was successful Out Of " + cid.Count();
                }
                catch (Exception e)
                {
                    TempData["error"] = "Update failed.";
                }

            }
            else
            {
                TempData["error"] = "Update failed.";
            }
            return RedirectToAction("Students", new { sessId = session, classId = id });
        }

        public async Task<ActionResult> UpdateStudentResult(int id = 0)
        {
            if (id != 0)
            {
                try
                {
                    await _resultService.UpdateResult(id);
                    TempData["success"] = "Update was successful.";
                }
                catch (Exception e)
                {
                    TempData["error"] = "Update failed.";
                }

            }
            else
            {
                TempData["error"] = "Update failed.";
            }
            return RedirectToAction("EditStudentResult", new { id = id });
        }

        public async Task<ActionResult> PublishResult(int id, int classId)
        {
            try
            {

                var enrolleds = await db.Enrollments.Where(x => x.ClassLevelId == classId).ToListAsync();
                try
                {
                    foreach (var enrolled in enrolleds)
                    {
                        var asd = db.Enrollments.FirstOrDefault(x => x.Id == enrolled.Id);
                        asd.CummulativeAverageScore = null;
                        db.Entry(asd).State = EntityState.Modified;
                        db.SaveChanges();

                        //Add Tracking
                        //var userId2 = User.Identity.GetUserId();
                        //var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                        //Tracker tracker = new Tracker();
                        //tracker.UserId = userId2;
                        //tracker.UserName = user2.UserName;
                        //tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                        //tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                        //tracker.Note = tracker.FullName + " " + "Updated Enrollment";
                        ////db.Trackers.Add(tracker);
                        await db.SaveChangesAsync();

                    }
                    foreach (var enrolled in enrolleds)
                    {
                        await _resultService.UpdateResult(enrolled.Id);

                    }
                }
                catch (Exception e)
                {

                }

                if (id != 0 && classId != 0)
                {
                    await _resultService.PublishResult(id, classId);
                    TempData["Report"] = "The Result has been published. Students in this class can now check their results online.";
                    return RedirectToAction("Students", new { classId = classId, sessId = id });
                }


            }
            catch (Exception e)
            {
                TempData["Error"] = "The Result has not been published. Please a try again or contact the Administrator.";
            }

            return RedirectToAction("Students", new { classId = classId, sessId = id });
        }

        public async Task<ActionResult> UnpublishResult(int id, int classId)
        {
            try
            {
                if (id != 0 && classId != 0)
                {
                    await _resultService.UnpublishResult(id, classId);
                    TempData["Report"] = "The Result has been unpublished. Students in this class can't check their results online until the result is published.";
                    return RedirectToAction("Students", new { classId = classId, sessId = id });

                }


            }
            catch (Exception e)
            {
                TempData["Error"] = "The Result has not been unpublished. Please a try again or contact the Administrator.";
            }
            return RedirectToAction("Students", new { classId = classId, sessId = id });
        }



        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="sessId"></param>
        /// <param name="classId"></param>
        /// <returns></returns>
        /// 
        public async Task<ActionResult> MasterListIndex()
        {
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel.OrderBy(x => x.ClassLevelName), "Id", "ClassLevelName");
            //ViewBag.ClassLevelId = new SelectList(classlevel.OrderByDescending(x => x.ClassLevelName), "Id", "ClassLevelName");

            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            ViewBag.sessionId = session.Id;
            ViewBag.session = session.SessionYear + " - " + session.Term + " Term";

            return View();
        }

        public async Task<ActionResult> MasterListAllSessions()
        {
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel.OrderBy(x => x.ClassLevelName), "Id", "ClassLevelName");
            //ViewBag.ClassLevelId = new SelectList(classlevel.OrderByDescending(x => x.ClassLevelName), "Id", "ClassLevelName");


            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session, "Id", "FullSession");
            //ViewBag.sessionId = new SelectList(session.OrderByDescending(x => x.FullSession), "Id", "FullSession");

            return View();
        }
        public async Task<ActionResult> MasterList(int sessId, int classId)
        {
            var enrolledStudents = await _resultService.StudentsBySessIdAndByClassId(sessId, classId);
            if (enrolledStudents.Count() <= 0)
            {
                var classlevel1 = await _classlevelService.ClassLevelList();
                ViewBag.ClassLevelId = new SelectList(classlevel1.OrderBy(x => x.ClassLevelName), "Id", "ClassLevelName", classId);
                //ViewBag.ClassLevelId = new SelectList(classlevel1.OrderByDescending(x => x.ClassLevelName), "Id", "ClassLevelName", classId);

                var session1 = await _sessionService.GetAllSession();
                ViewBag.sessionId = new SelectList(session1, "Id", "FullSession", sessId);
                //ViewBag.sessionId = new SelectList(session1.OrderByDescending(x => x.FullSession), "Id", "FullSession", sessId);
                TempData["msg"] = "No Record Found.";
                return RedirectToAction("MasterListIndex");
            }

            var check = await _publishService.CheckPublishResult(sessId, classId);
            if (check == true)
            {
                ViewBag.Check = "Published";
            }
            else
            {
                ViewBag.Check = "Not Published";
            }

            var classlevel = await _classlevelService.Get(classId);
            var session = await _sessionService.Get(sessId);
            var setting = db.Settings.FirstOrDefault();
            ViewBag.setting = setting;
            ViewBag.Id = sessId;
            ViewBag.ClassId = classId;
            ViewBag.Term = session.Term.ToLower();
            ViewBag.Class = classlevel.ClassName;
            ViewBag.ClassSubjectCount = classlevel.Subjects.Count();
            var stinfo = classlevel.UserId;
            var staff = await db.Users.FirstOrDefaultAsync(x => x.Id == stinfo);
            ViewBag.formteacher = staff.Surname + " " + staff.FirstName + " " + staff.OtherName;
            ViewBag.Session = session.SessionYear + " - " + session.Term + " Term";

            var classResult = await _classlevelService.Get(classId);
            var showPosOnClassResult = classResult.ShowPositionOnClassResult;
            ViewBag.showPosOnClassResult = showPosOnClassResult;


            foreach (var name in enrolledStudents)
            {
                if (name.StudentName.Substring(0, 2).Contains("\t"))
                {
                    name.StudentName = name.StudentName.Remove(0, 2);
                }
                if (name.StudentName.Substring(0, 2).Contains(" "))
                {
                    name.StudentName = name.StudentName.TrimStart();
                }
            }
            return View(enrolledStudents);
        }


        public async Task<ActionResult> MasterListPrint(int sessId, int classId)
        {
            var enrolledStudents = await _resultService.StudentsBySessIdAndByClassId(sessId, classId);
            if (enrolledStudents.Count() <= 0)
            {
                var classlevel1 = await _classlevelService.ClassLevelList();
                ViewBag.ClassLevelId = new SelectList(classlevel1.OrderBy(x => x.ClassLevelName), "Id", "ClassLevelName", classId);
                //ViewBag.ClassLevelId = new SelectList(classlevel1.OrderByDescending(x => x.ClassLevelName), "Id", "ClassLevelName", classId);

                var session1 = await _sessionService.GetAllSession();
                ViewBag.sessionId = new SelectList(session1.OrderByDescending(x => x.FullSession), "Id", "FullSession", sessId);
                TempData["msg"] = "No Record Found.";
                return RedirectToAction("MasterListIndex");
            }

            var check = await _publishService.CheckPublishResult(sessId, classId);
            if (check == true)
            {
                ViewBag.Check = "Published";
            }
            else
            {
                ViewBag.Check = "Not Published";
            }

            var classlevel = await _classlevelService.Get(classId);
            var session = await _sessionService.Get(sessId);
            var setting = db.Settings.FirstOrDefault();
            ViewBag.setting = setting;
            ViewBag.Id = sessId;
            ViewBag.ClassId = classId;
            ViewBag.Term = session.Term.ToLower();
            ViewBag.Class = classlevel.ClassName;
            ViewBag.ClassSubjectCount = classlevel.Subjects.Count();
            var stinfo = classlevel.UserId;
            var staff = await db.Users.FirstOrDefaultAsync(x => x.Id == stinfo);
            ViewBag.formteacher = staff.Surname + " " + staff.FirstName + " " + staff.OtherName;
            ViewBag.Session = session.SessionYear + " - " + session.Term + " Term";

            var classResult = await _classlevelService.Get(classId);
            var showPosOnClassResult = classResult.ShowPositionOnClassResult;
            ViewBag.showPosOnClassResult = showPosOnClassResult;


            foreach (var name in enrolledStudents)
            {
                if (name.StudentName.Substring(0, 2).Contains("\t"))
                {
                    name.StudentName = name.StudentName.Remove(0, 2);
                }
                if (name.StudentName.Substring(0, 2).Contains(" "))
                {
                    name.StudentName = name.StudentName.TrimStart();
                }
            }
            return View(enrolledStudents);
        }

        public async Task<ActionResult> MasterListPreview(int sessId, int classId)
        {
            var enrolledStudents = await _resultService.StudentsBySessIdAndByClassId(sessId, classId);
            if (enrolledStudents.Count() <= 0)
            {
                return HttpNotFound();
            }

            var check = await _publishService.CheckPublishResult(sessId, classId);
            if (check == true)
            {
                ViewBag.Check = "Published";
            }
            else
            {
                ViewBag.Check = "Not Published";
            }

            var classlevel = await _classlevelService.Get(classId);
            var session = await _sessionService.Get(sessId);
            ViewBag.Id = sessId;
            ViewBag.ClassId = classId;
            ViewBag.Term = session.Term.ToLower();
            ViewBag.Class = classlevel.ClassName;
            var stinfo = classlevel.UserId;
            var staff = await db.Users.FirstOrDefaultAsync(x => x.Id == stinfo);
            ViewBag.formteacher = staff.Surname + " " + staff.FirstName + " " + staff.OtherName;
            ViewBag.Session = session.SessionYear + " - " + session.Term + " Term";


            var classResult = await _classlevelService.Get(classId);
            var showPosOnClassResult = classResult.ShowPositionOnClassResult;
            ViewBag.showPosOnClassResult = showPosOnClassResult;


            foreach (var name in enrolledStudents)
            {
                if (name.StudentName.Substring(0, 2).Contains("\t"))
                {
                    name.StudentName = name.StudentName.Remove(0, 2);
                }
                if (name.StudentName.Substring(0, 2).Contains(" "))
                {
                    name.StudentName = name.StudentName.TrimStart();
                }
            }
            return View(enrolledStudents);
        }


        public async Task<ActionResult> CheckClassPublish()
        {
            var sess = await db.Sessions.FirstOrDefaultAsync(h => h.Status == SessionStatus.Current);
            ViewBag.ssId = sess.Id;
            var cname = await db.ClassLevels.OrderBy(x => x.ClassName).ToListAsync();
            return View(cname);
        }

        public ActionResult Publish(int id, int sesid)
        {
            var it = db.PublishResults.FirstOrDefault(x => x.SessionId == sesid && x.ClassLevelId == id);
            if (it == null)
            {
                PublishResult pub = new PublishResult();
                pub.ClassLevelId = id;
                pub.SessionId = sesid;
                pub.PublishedDate = DateTime.UtcNow;

                db.PublishResults.Add(pub);
                db.SaveChanges();


                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Published Result";
                //db.Trackers.Add(tracker);
                db.SaveChangesAsync();

            }
            else
            {
                db.PublishResults.Remove(it);
                db.SaveChanges();

                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Unpublished Student Result";
                //db.Trackers.Add(tracker);
                db.SaveChangesAsync();
            }

            return RedirectToAction("CheckClassPublish");


        }

        #region admin edit enrolled subject

        public async Task<ActionResult> EditStudentResultAdmin(int id)
        {
            decimal? total = await _resultService.SumEnrolledSubjectTotalScore(id);
            int StudentSubjectCount = await _resultService.TotalEnrolledSubjectCount(id);

            if (StudentSubjectCount == 0)
            {
                ViewBag.Sum = "No Subject has been entered";
            }
            else
            {
                decimal? dsum = total / StudentSubjectCount;
                var sum = decimal.Parse(dsum.ToString());
                ViewBag.Sum = Math.Round(sum, 2, MidpointRounding.AwayFromZero);
            }

            var enroledStudentData = await _enrolService.Get(id);
            var user = await _studentService.Get(enroledStudentData.StudentProfileId);
            string fullname = user.Fullname;
            ViewBag.Fullname = fullname;
            ViewBag.Class = enroledStudentData.ClassLevel.ClassName;
            ViewBag.Session = enroledStudentData.Session.SessionYear + "-" + enroledStudentData.Session.Term;
            ViewBag.SessionId = enroledStudentData.SessionId;


            var positions = await _resultService.Position(enroledStudentData.SessionId, enroledStudentData.ClassLevelId);
            int totalStudents = positions.Count();
            int position = 0;
            decimal? avg = 0;

            if (StudentSubjectCount != 0)
            {
                foreach (var p in positions)
                {
                    if (p.AverageScore != null)
                    {
                        position = position + 1;
                        if (p.Id == id)
                        {
                            avg = p.AverageScore.Value;
                            goto outloop;
                        }
                    }
                }

            outloop: ViewData["Position"] = position;
                ViewData["Avg"] = avg;
                ViewBag.TotalStudents = totalStudents;


            }
            ViewBag.SessionId = enroledStudentData.SessionId;
            ViewBag.ClassId = enroledStudentData.ClassLevelId;
            ViewBag.EnrollmentId = enroledStudentData.Id;

            var list = await _resultService.EnrolledSubjectForEnrollment(enroledStudentData.Id);
            return View(list);
        }


        public async Task<ActionResult> DeleteEnrolledSubject(int id, int enrolledsubid)
        {
            try
            {
                EnrolledSubject sub = await db.EnrolledSubjects.FindAsync(enrolledsubid);
                db.EnrolledSubjects.Remove(sub);
                await db.SaveChangesAsync();

                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Delete Enrolled Subject";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
                TempData["remove"] = "removed Successfull";
            }
            catch (Exception d)
            {


            }
            return RedirectToAction("EditStudentResultAdmin", new { id = id });
        }
        #endregion


        #region Result via batch printing

        [HttpPost]
        public async Task<ActionResult> ResultSearch(int? id = 0, int? Id = 0)
        {
            return RedirectToAction("Results", new { sid = id, cid = Id });
        }
        //[HttpGet]
        public async Task<ActionResult> Results(int? sid, int? cid)
        {
            //session dropdown
            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session, "Id", "FullSession", sid);

            //class dropdown
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassId = new SelectList(classlevel, "Id", "ClassLevelName", cid);

            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var result = db.Enrollments.Include(x => x.ClassLevel).Include(x => x.EnrolledSubjects).Include(x => x.Session).Include(x => x.StudentProfile.user).Include(x => x.StudentProfile).Include(x => x.User);
            if (sid == null)
            {
                if (cid == null)
                {
                    result = result.Where(x => x.SessionId == currentSession.Id);
                }
                else
                {
                    var classe = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == cid);
                    ViewBag.cname = classe;
                    result = result.Where(x => x.SessionId == currentSession.Id && x.ClassLevelId == cid);

                }

            }
            else
            {
                if (cid == null)
                {
                    result = result.Where(x => x.SessionId == currentSession.Id);
                }
                else
                {
                    var classe = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == cid);
                    ViewBag.cname = classe;
                    result = result.Where(x => x.SessionId == currentSession.Id && x.ClassLevelId == cid);

                }
            }
            return View(await result.OrderBy(x => x.ClassLevel.ClassName).ThenBy(x => x.AverageScore).ToListAsync());
        }

        #endregion

        public async Task<ActionResult> EnrolledSubjectLoop(int id)
        {


            var t = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var listenrolledd = await db.Enrollments.AsNoTracking().Include(x=>x.User).Include(x => x.StudentProfile.user).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.ClassLevelId == id && x.SessionId == t.Id).ToListAsync();


            foreach (var iss in listenrolledd)
            {


                var assignment = await db.EnrolledSubjects.AsNoTracking().Where(x => x.EnrollmentId == iss.Id).ToListAsync();
                foreach (var io in assignment.ToList())
                {
                    try
                    {
                        EnrolledSubject enrd = await db.EnrolledSubjects.FirstOrDefaultAsync(x => x.Id == io.Id);
                        if (enrd != null)
                        {

                                db.EnrolledSubjects.Remove(enrd);
                               await db.SaveChangesAsync();
                            

                            //Add Tracking
                            var userId2 = User.Identity.GetUserId();
                            var user2 = await UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefaultAsync();
                            Tracker tracker = new Tracker();
                            tracker.UserId = userId2;
                            tracker.UserName = user2.UserName;
                            tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                            tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                            tracker.Note = tracker.FullName + " " + "Removed Enrolled Subject";
                            //db.Trackers.Add(tracker);
                           await db.SaveChangesAsync();
                        }
                    }
                    catch (Exception d) { }

                }

            }


            return RedirectToAction("Index", "Dashboard");

        }

        public async Task<ActionResult> EnrollmentEnrolledSubjectLoop(int id)
        {


            var t = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var listenrolledd = await db.Enrollments.AsNoTracking().Include(x=>x.User).Include(x => x.StudentProfile.user).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.ClassLevelId == id && x.SessionId == t.Id).ToListAsync();


            foreach (var iss in listenrolledd)
            {


                var assignment = await db.EnrolledSubjects.AsNoTracking().Where(x => x.EnrollmentId == iss.Id).ToListAsync();
                foreach (var io in assignment.ToList())
                {
                    try
                    {
                        EnrolledSubject enrd = await db.EnrolledSubjects.FirstOrDefaultAsync(x => x.Id == io.Id);
                        if (enrd != null)
                        {

                            db.EnrolledSubjects.Remove(enrd);
                            await db.SaveChangesAsync();


                            //Add Tracking
                            var userId2 = User.Identity.GetUserId();
                            var user2 = await UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefaultAsync();
                            Tracker tracker = new Tracker();
                            tracker.UserId = userId2;
                            tracker.UserName = user2.UserName;
                            tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                            tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                            tracker.Note = tracker.FullName + " " + "Removed Enrolled Subject";
                            //db.Trackers.Add(tracker);
                            await db.SaveChangesAsync();
                        }
                    }
                    catch (Exception d) { }

                }

                try
                {
                    Enrollment enrd = await db.Enrollments.FirstOrDefaultAsync(x => x.Id == iss.Id);
                    if (enrd != null)
                    {

                        db.Enrollments.Remove(enrd);
                        await db.SaveChangesAsync();

                        //Add Tracking
                        var userId2 = User.Identity.GetUserId();
                        var user2 = await UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefaultAsync();
                        Tracker tracker = new Tracker();
                        tracker.UserId = userId2;
                        tracker.UserName = user2.UserName;
                        tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                        tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                        tracker.Note = tracker.FullName + " " + "Removed Enrollment and Enrolled Subject";
                        //db.Trackers.Add(tracker);
                        await db.SaveChangesAsync();
                    }
                }
                catch (Exception d) { }


            }


            return RedirectToAction("Index", "Dashboard");

        }


        public async Task<ActionResult> ListBatch()
        {
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);

            var batch = await db.BatchResults.Where(x => x.SessionId == session.Id).ToListAsync();
            return View(batch);
        }

        #region cummulative masterlist

        public async Task<ActionResult> CummulativeMasterListIndex()
        {
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel.OrderByDescending(x => x.ClassLevelName), "Id", "ClassLevelName");

            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            ViewBag.sessionId = session.Id;
            ViewBag.session = session.SessionYear + " - " + session.Term + " Term";

            return View();
        }

        public async Task<ActionResult> Reconsile()
        {

            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel.OrderByDescending(x => x.ClassLevelName), "Id", "ClassLevelName");

            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            ViewBag.sessionId = session.Id;
            ViewBag.session = session.SessionYear + " - " + session.Term + " Term";

            return View();
        }
        public async Task<ActionResult> ResultReconsile(int sessId, int classId)
        {
            var outp = await _resultService.CumulativeResultReconciliationByClassId(sessId, classId);
            return Content(outp);
        }

        public async Task<ActionResult> CummulativeMasterListAllSessions()
        {
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel.OrderBy(x => x.ClassLevelName), "Id", "ClassLevelName");
            //ViewBag.ClassLevelId = new SelectList(classlevel.OrderByDescending(x => x.ClassLevelName), "Id", "ClassLevelName");


            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session, "Id", "FullSession");
            //ViewBag.sessionId = new SelectList(session.OrderByDescending(x => x.FullSession), "Id", "FullSession");

            return View();
        }
        public async Task<ActionResult> CummulativeMasterList(int sessId, int classId)
        {
            var enrolledStudents = await _resultService.CumulativeStudentsBySessIdAndByClassId(sessId, classId);
            if (enrolledStudents.Count() <= 0)
            {
                var classlevel1 = await _classlevelService.ClassLevelList();
                ViewBag.ClassLevelId = new SelectList(classlevel1.OrderBy(x => x.ClassLevelName), "Id", "ClassLevelName", classId);
                //ViewBag.ClassLevelId = new SelectList(classlevel1.OrderByDescending(x => x.ClassLevelName), "Id", "ClassLevelName", classId);

                var session1 = await _sessionService.GetAllSession();
                ViewBag.sessionId = new SelectList(session1, "Id", "FullSession", sessId);
                //ViewBag.sessionId = new SelectList(session1.OrderByDescending(x => x.FullSession), "Id", "FullSession", sessId);
                TempData["msg"] = "No Record Found.";
                return RedirectToAction("MasterListIndex");
            }

            var check = await _publishService.CheckPublishResult(sessId, classId);
            if (check == true)
            {
                ViewBag.Check = "Published";
            }
            else
            {
                ViewBag.Check = "Not Published";
            }

            var classlevel = await _classlevelService.Get(classId);
            var session = await _sessionService.Get(sessId);
            var setting = db.Settings.FirstOrDefault();
            ViewBag.setting = setting;
            ViewBag.Id = sessId;
            ViewBag.ClassId = classId;
            ViewBag.Term = session.Term.ToLower();
            ViewBag.Class = classlevel.ClassName;
            ViewBag.ClassSubjectCount = classlevel.Subjects.Count();
            var stinfo = classlevel.UserId;
            var staff = await db.Users.FirstOrDefaultAsync(x => x.Id == stinfo);
            ViewBag.formteacher = staff.Surname + " " + staff.FirstName + " " + staff.OtherName;
            ViewBag.Session = session.SessionYear + " - " + session.Term + " Term";

            var classResult = await _classlevelService.Get(classId);
            var showPosOnClassResult = classResult.ShowPositionOnClassResult;
            ViewBag.showPosOnClassResult = showPosOnClassResult;


            foreach (var name in enrolledStudents)
            {
                if (name.StudentName.Substring(0, 2).Contains("\t"))
                {
                    name.StudentName = name.StudentName.Remove(0, 2);
                }
                if (name.StudentName.Substring(0, 2).Contains(" "))
                {
                    name.StudentName = name.StudentName.TrimStart();
                }
            }
            return View(enrolledStudents.OrderByDescending(x => x.CummulativeAverageScore));
        }


        #endregion

        public async Task<PartialViewResult> MasterSubjects(int enrId)
        {
            IEnumerable<EnrolledSubject> subname = new List<EnrolledSubject>();
            
                subname = db.EnrolledSubjects.Include(x => x.Subject).Include(x => x.Enrollments).AsNoTracking().Where(x => x.EnrollmentId == enrId && x.Subject.ShowSubject == true).OrderBy(x => x.Subject.SubjectName).AsEnumerable();
            ViewBag.listx = subname;
            ViewBag.idc = enrId;
            return PartialView();
        }

        #region Reconcile Enrollsubjects Main ICS where by enrol subject contain subject from two classes
        public async Task<ActionResult> ReconcileEnrollSubjects(int? classid)
        {
            string output = "";
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var classinfo = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == classid);
            var enrollments = await db.Enrollments.Where(x => x.SessionId == session.Id && x.ClassLevelId == classinfo.Id).ToListAsync();
            foreach (var student in enrollments)
            {
                var items = await db.EnrolledSubjects.Include(x => x.Subject.ClassLevel).Include(x => x.Enrollments.StudentProfile.user).Include(x => x.Enrollments).Include(x => x.Enrollments.ClassLevel).Include(x => x.Subject).Where(x => x.EnrollmentId == student.Id && x.Subject.SubjectName != null).OrderBy(x => x.Subject.SubjectName).ThenBy(x => x.Id).ToListAsync();
                foreach (var esub in items)
                {
                    if (esub.Subject.ClassLevel.Id != classid)
                    {
                        output = output + "enrol" + esub.Subject.SubjectName + "<br/>";
                        var enrolsub = await db.EnrolledSubjects.FirstOrDefaultAsync(x => x.Id == esub.Id);
                        db.EnrolledSubjects.Remove(enrolsub);

                    }
                }
                await db.SaveChangesAsync();

                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Reconcilled Enrolled Subject";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
            ViewBag.out1 = output;
            return View();
        }
        #endregion
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