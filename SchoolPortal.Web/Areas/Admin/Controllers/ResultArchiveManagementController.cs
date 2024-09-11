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
using SchoolPortal.Web.Models.ResultArchive;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace SchoolPortal.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class ResultArchiveManagementController : Controller
    {
        #region services


        private ApplicationDbContext db = new ApplicationDbContext();
        private ISessionService _sessionService = new SessionService();
        private IClassLevelService _classlevelService = new ClassLevelService();
        private IPublishResultService _publishService = new PublishResultService();
        private IEnrollmentService _enrolService = new EnrollmentService();
        private IResultArchiveService _resultService = new ResultArchiveService();
        private IStudentProfileService _studentService = new StudentProfileService();
        private ISettingService _settingService = new SettingService();
        private IEnrolledSubjectArchiveService _enrolledService = new EnrolledSubjectArchiveService();


        public ResultArchiveManagementController()
        {

        }
        public ResultArchiveManagementController(
            SessionService sessionService,
            ClassLevelService classLevelService,
            PublishResultService publishService,
            ResultArchiveService resultService,
            StudentProfileService studentService,
            SettingService settingService,
            EnrolledSubjectArchiveService enrolledService,
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

        public async Task<ActionResult> Index()
        {
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel, "Id", "ClassLevelName");

            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session, "Id", "FullSession");
            return View();
        }

        #region Batch result



        //batch print
        public async Task<ActionResult> BatchResult()
        {
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel, "Id", "ClassLevelName");

            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session, "Id", "FullSession");
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
                var check = await db.RecognitiveDomainArchive.FirstOrDefaultAsync(x => x.EnrolmentId == abcd.EnrollmentId);
                if (check == null)
                {
                    RecognitiveDomainArchive recognitive = new RecognitiveDomainArchive();
                    recognitive.EnrolmentId = abcd.EnrollmentId;
                    db.RecognitiveDomainArchive.Add(recognitive);
                    await db.SaveChangesAsync();


                    //Add Tracking
                    var userId2 = User.Identity.GetUserId();
                    var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Updated Recognitive Domain Archive";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }

                var check2 = await db.AffectiveDomainArchive.FirstOrDefaultAsync(x => x.EnrolmentId == abcd.EnrollmentId);
                if (check2 == null)
                {
                    AffectiveDomainArchive affective = new AffectiveDomainArchive();
                    affective.EnrolmentId = abcd.EnrollmentId;
                    db.AffectiveDomainArchive.Add(affective);
                    await db.SaveChangesAsync();

                    //Add Tracking
                    var userId2 = User.Identity.GetUserId();
                    var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Updated Affective Domain Archive";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }

                var check3 = await db.PsychomotorDomainArchive.FirstOrDefaultAsync(x => x.EnrolmentId == abcd.EnrollmentId);
                if (check3 == null)
                {
                    PsychomotorDomainArchive psychomotor = new PsychomotorDomainArchive();
                    psychomotor.EnrolmentId = abcd.EnrollmentId;
                    db.PsychomotorDomainArchive.Add(psychomotor);
                    await db.SaveChangesAsync();


                    //Add Tracking
                    var userId2 = User.Identity.GetUserId();
                    var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Updated Psychomotor Domain Archive";
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
            var sessionlist = await _sessionService.GetAllSession();
            var publishcheck = await db.PublishResults.FirstOrDefaultAsync(x => x.ClassLevelId == classId);
            if (publishcheck == null)
            {
                TempData["error"] = "Result Not Yet Ready.";
                return RedirectToAction("BatchResult");
            }
            var batchIdInfo = "";
            var enrolledStudents = await _resultService.StudentsBySessIdAndByClassId(sessId, classId);
            var batcenro = enrolledStudents.Where(x => x.AverageScore > 0);
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
                    var userId2 = User.Identity.GetUserId();
                    var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Added Batch Result";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
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
            var items = await db.BatchResults.Where(x => x.BatchId == batchid).ToListAsync();
            return View(items);
        }

        [AllowAnonymous]
        public async Task<ActionResult> Preview2(string batchid)
        {
            var items = await db.BatchResults.Where(x => x.BatchId == batchid).ToListAsync();
            return View(items);
        }


        [AllowAnonymous]
        public async Task<ActionResult> Preview3(string batchid)
        {
            var items = await db.BatchResults.Where(x => x.BatchId == batchid).ToListAsync();
            return View(items);
        }
        #endregion
        public async Task<ActionResult> Students(int sessId, int classId)
        {
            var archive = db.ArchiveResults.Where(x => x.SessionId == sessId && x.ClassLevelId == classId).FirstOrDefault();
            if (archive == null)
            {
                TempData["msg"] = "Archive Not Found, Archive this result then come back again but if the problem persist then contact the Administrator! ";
                return RedirectToAction("Index");
            }

            var sett = db.Settings.FirstOrDefault();
            ViewBag.sett = sett;

            ViewBag.enablePreviewResult = sett.EnablePreviewResult;

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
                var checki = await db.RecognitiveDomainArchive.FirstOrDefaultAsync(x => x.EnrolmentId == abcd.EnrollmentId);
                if (checki == null)
                {
                    RecognitiveDomainArchive recognitive = new RecognitiveDomainArchive();
                    recognitive.EnrolmentId = abcd.EnrollmentId;
                    db.RecognitiveDomainArchive.Add(recognitive);
                    await db.SaveChangesAsync();


                    //Add Tracking
                    var userId2 = User.Identity.GetUserId();
                    var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Updated Recognitive Domain Archive";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }

            }

            //UpdateComment psychomotor domain
            foreach (var abcd in enrolledStudents)
            {
                var studentsss = await db.StudentProfiles.FirstOrDefaultAsync(x => x.StudentRegNumber == abcd.StudentRegNumber);
                var checki = await db.PsychomotorDomainArchive.FirstOrDefaultAsync(x => x.EnrolmentId == abcd.EnrollmentId);
                if (checki == null)
                {
                    PsychomotorDomainArchive psychomotor = new PsychomotorDomainArchive();
                    psychomotor.EnrolmentId = abcd.EnrollmentId;
                    db.PsychomotorDomainArchive.Add(psychomotor);
                    await db.SaveChangesAsync();


                    //Add Tracking
                    var userId2 = User.Identity.GetUserId();
                    var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Updated Psychomotor Domain Archive";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
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
                var checki = await db.AffectiveDomainArchive.FirstOrDefaultAsync(x => x.EnrolmentId == abcd.EnrollmentId);
                if (checki == null)
                {
                    AffectiveDomainArchive affective = new AffectiveDomainArchive();
                    affective.EnrolmentId = abcd.EnrollmentId;
                    db.AffectiveDomainArchive.Add(affective);
                    await db.SaveChangesAsync();

                    //Add Tracking
                    var userId2 = User.Identity.GetUserId();
                    var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Updated Affective Domain Archive";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
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

            //var classResult = await _classlevelService.Get(classId);
            //var showPosOnClassResult = classResult.ShowPositionOnClassResult;
            //ViewBag.showPosOnClassResult = showPosOnClassResult;
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

                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Updated Psychomotor Domains";
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
                tracker.Note = tracker.FullName + " " + "Updated Affective Domains";
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


            var listing = new List<EnrolledSubjectArchive>();
            var merge = new List<EnrolledSubjectArchive>();
            var sess = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var enrollments = db.Enrollments.Where(x => x.ClassLevelId == id && x.SessionId == sess.Id).ToList();
            foreach (var i in enrollments)
            {
                var list = await _resultService.EnrolledSubjectForEnrollment(i.Id);
                merge.AddRange(listing.Concat(list.ToList()));
            }

            var lis = merge.Select(x => x.SubjectName).Distinct();
            ViewBag.list = lis;
            return View(lis);
        }

        public async Task<ActionResult> RemoveEnrolsub(int id)
        {
            EnrolledSubjectArchive assignment = db.EnrolledSubjectArchive.FirstOrDefault(x => x.Id == id);
            db.EnrolledSubjectArchive.Remove(assignment);
            db.SaveChangesAsync();

            //Add Tracking
            var userId2 = User.Identity.GetUserId();
            var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
            Tracker tracker = new Tracker();
            tracker.UserId = userId2;
            tracker.UserName = user2.UserName;
            tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
            tracker.ActionDate = DateTime.UtcNow.AddHours(1);
            tracker.Note = tracker.FullName + " " + "Remove Enrolled Archive Subject";
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
            var enr = await db.EnrolledSubjectArchive.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
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
                tracker.Note = tracker.FullName + " " + "Updated Student is Offered";
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
                    tracker.Note = tracker.FullName + " " + "Added Student Is Offered";
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
            return View(item);
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EnrolledSubjectArchive models)
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
                    return View();
                }

                //var totalCA = models.Project + models.ClassExercise + models.TestScore + models.TestScore2 + models.Assessment;
                //if (models.ExamScore > setting.ExamScore || totalCA > setting.AccessmentScore)
                //{
                //    TempData["error"] = "Test Score or Exam Score not in Range";
                //    return View();
                //}
                var subject = await _resultService.GetSubjectByEnrolledSubId(models.Id);
                var classlevel = await _resultService.GetClassByClassId(cid.ClassLevelId);
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
        public async Task<ActionResult> UpdateScore(EnrolledSubjectArchive models)
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

            //var totalCA = models.Project + models.ClassExercise + models.TestScore + models.TestScore2 + models.Assessment;
            //if (models.ExamScore > setting.ExamScore || totalCA > setting.AccessmentScore)
            //{
            //    TempData["error"] = "Test Score or Exam Score not in Range";
            //    ViewBag.sessId = sessId;
            //    return View(models);
            //}
            var subject = await _resultService.GetSubjectByEnrolledSubId(models.Id);
            var classlevel = await _resultService.GetClassByClassId(cid.ClassLevelId);
            string userLevel = classlevel.ClassName;

            try
            {
                decimal? totalScore = models.TestScore + models.ExamScore;
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
                        var userId2 = User.Identity.GetUserId();
                        var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                        Tracker tracker = new Tracker();
                        tracker.UserId = userId2;
                        tracker.UserName = user2.UserName;
                        tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                        tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                        tracker.Note = tracker.FullName + " " + "Updated Enrollment";
                        //db.Trackers.Add(tracker);
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
            ViewBag.ClassLevelId = new SelectList(classlevel.OrderByDescending(x => x.ClassLevelName), "Id", "ClassLevelName");

            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            ViewBag.sessionId = session.Id;
            ViewBag.session = session.SessionYear + " - " + session.Term + " Term";

            return View();
        }

        public async Task<ActionResult> MasterListAllSessions()
        {
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel.OrderByDescending(x => x.ClassLevelName), "Id", "ClassLevelName");


            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session.OrderByDescending(x => x.FullSession), "Id", "FullSession");

            return View();
        }
        public async Task<ActionResult> MasterList(int sessId, int classId)
        {
            var enrolledStudents = await _resultService.StudentsBySessIdAndByClassId(sessId, classId);
            if (enrolledStudents.Count() <= 0)
            {
                var classlevel1 = await _classlevelService.ClassLevelList();
                ViewBag.ClassLevelId = new SelectList(classlevel1.OrderByDescending(x => x.ClassLevelName), "Id", "ClassLevelName", classId);

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
                tracker.Note = tracker.FullName + " " + "UnPublished Result";
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
                EnrolledSubjectArchive sub = await db.EnrolledSubjectArchive.FindAsync(enrolledsubid);
                db.EnrolledSubjectArchive.Remove(sub);
                await db.SaveChangesAsync();

                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Deleted Enrolled Subject Archive";
                //db.Trackers.Add(tracker);
                db.SaveChangesAsync();
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
            var result = db.Enrollments.Include(x => x.ClassLevel).Include(x => x.EnrolledSubjectArchive).Include(x => x.Session).Include(x => x.StudentProfile.user).Include(x => x.StudentProfile).Include(x => x.User);
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

        public ActionResult EnrolledSubjectLoop(int id)
        {


            var t = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var listenrolledd = db.Enrollments.Where(x => x.ClassLevelId == id && x.SessionId == t.Id).ToList();


            foreach (var iss in listenrolledd)
            {


                var assignment = db.EnrolledSubjectArchive.Where(x => x.EnrollmentId == iss.Id).ToList();
                foreach (var io in assignment)
                {
                    EnrolledSubjectArchive enrd = db.EnrolledSubjectArchive.FirstOrDefault(x => x.Id == io.Id);
                    if (enrd.TotalScore < 1)
                    {
                        db.EnrolledSubjectArchive.Remove(enrd);
                        db.SaveChanges();

                        //Add Tracking
                        var userId2 = User.Identity.GetUserId();
                        var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                        Tracker tracker = new Tracker();
                        tracker.UserId = userId2;
                        tracker.UserName = user2.UserName;
                        tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                        tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                        tracker.Note = tracker.FullName + " " + "Removed Enrolled Subject Archive";
                        //db.Trackers.Add(tracker);
                        db.SaveChangesAsync();
                    }
                    else
                    {
                        string j = enrd.TotalScore.ToString();
                        string jf = enrd.ExamScore.ToString();
                        string jfc = enrd.TestScore.ToString();
                        string xjf = enrd.TestScore.ToString();
                    }
                }

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
            ViewBag.ClassLevelId = new SelectList(classlevel.OrderByDescending(x => x.ClassLevelName), "Id", "ClassLevelName");


            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session.OrderByDescending(x => x.FullSession), "Id", "FullSession");

            return View();
        }
        public async Task<ActionResult> CummulativeMasterList(int sessId, int classId)
        {
            var enrolledStudents = await _resultService.CumulativeStudentsBySessIdAndByClassId(sessId, classId);
            if (enrolledStudents.Count() <= 0)
            {
                var classlevel1 = await _classlevelService.ClassLevelList();
                ViewBag.ClassLevelId = new SelectList(classlevel1.OrderByDescending(x => x.ClassLevelName), "Id", "ClassLevelName", classId);

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
            return View(enrolledStudents.OrderByDescending(x => x.CummulativeAverageScore));
        }


        #endregion



        #region Reconcile Enrollsubjects Main ICS where by enrol subject contain subject from two classes
        public async Task<ActionResult> ReconcileEnrollSubjects(int? classid)
        {
            string output = "";
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var classinfo = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == classid);
            var enrollments = await db.Enrollments.Where(x => x.SessionId == session.Id && x.ClassLevelId == classinfo.Id).ToListAsync();
            foreach (var student in enrollments)
            {
                var items = await db.EnrolledSubjectArchive.Include(x => x.Enrollments.StudentProfile.user).Include(x => x.Enrollments).Include(x => x.Enrollments.ClassLevel).Where(x => x.EnrollmentId == student.Id && x.SubjectName != null).OrderBy(x => x.SubjectName).ThenBy(x => x.Id).ToListAsync();
                foreach (var esub in items)
                {
                    if (esub.Enrollments.ClassLevelId != classid)
                    {
                        output = output + "enrol" + esub.SubjectName + "<br/>";
                        var enrolsub = await db.EnrolledSubjectArchive.FirstOrDefaultAsync(x => x.Id == esub.Id);
                        db.EnrolledSubjectArchive.Remove(enrolsub);

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
                tracker.Note = tracker.FullName + " " + "Reconcile Enrolled Subjects";
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