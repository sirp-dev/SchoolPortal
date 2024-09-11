using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Dtos;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using System.Net.Mail;
using SchoolPortal.Web.Areas.Service;
using SchoolPortal.Web.Models.Dtos.Zoom;
using SchoolPortal.Web.Controllers;
using Newtonsoft.Json;
using LinqToExcel.Extensions;
using System.Globalization;

namespace SchoolPortal.Web.Areas.Staff.Controllers
{
    [Authorize(Roles = "Staff,Admin,SuperAdmin,FormTeacher")]
    public class PanelController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IStaffService _staffProfileService = new StaffService();
        private IClassLevelService _classLevelService = new ClassLevelService();
        private IResultService _resultService = new ResultService();
        private IEnrollmentService _enrollmentService = new EnrollmentService();
        private IStudentProfileService _studentProfileService = new StudentProfileService();
        private ISubjectService _subjectService = new SubjectService();
        private ISessionService _sessionService = new SessionService();
        private IEnrolledSubjectService _enrolledSubjectService = new EnrolledSubjectService();
        private ISettingService _settingService = new SettingService();
        private IPublishResultService _publishResultService = new PublishResultService();
        private IImageService _imageService = new ImageService();
        private IPostService _postService = new PostService();
        private IAssignmentService _assignmentService = new AssignmentService();
        private IMessageService _messageService = new MessageService();


        public PanelController()
        {

        }
        public PanelController(StaffService staffProfileService,
            ClassLevelService classLevelService,
            ResultService resultService,
            EnrollmentService enrollmentService,
            StudentProfileService studentProfileService,
            SubjectService subjectService,
            SessionService sessionService,
            EnrolledSubjectService enrolledSubjectService,
            SettingService settingService,
            PublishResultService publishResultService,
            ImageService imageService,
            PostService postService,
            AssignmentService assignmentService,
            MessageService messageService
            )
        {
            _imageService = imageService;
            _postService = postService;
            _staffProfileService = staffProfileService;
            _classLevelService = classLevelService;
            _resultService = resultService;
            _enrollmentService = enrollmentService;
            _studentProfileService = studentProfileService;
            _subjectService = subjectService;
            _sessionService = sessionService;
            _enrolledSubjectService = enrolledSubjectService;
            _settingService = settingService;
            _publishResultService = publishResultService;
            _assignmentService = assignmentService;
            _messageService = messageService;
        }
        // GET: Staff/Panel
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult> Index()
        {
            try
            {
                var profile = await _staffProfileService.GetStaffWithoutId();
                if (profile == null)
                {
                    TempData["error"] = "Profile Error, Try again";
                    return RedirectToAction("Login", "Account", new { area = "" });
                }
                ViewBag.staff = profile;

                var sub = await _staffProfileService.StaffSubjects();
                ViewBag.SubjectCount = sub;

                var clas = await _staffProfileService.StaffClassName();
                ViewBag.Class = clas;

                var qualification = await _staffProfileService.ListQualificationForUser();
                ViewBag.Qualication = qualification;
                var post = await _postService.ListPost(7);
                ViewBag.post = post;


            }
            catch (Exception e)
            {

            }
            return View();
        }

        #region post
        // GET: Content/Posts/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: Content/Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Post model, List<HttpPostedFileBase> upload)
        {
            if (ModelState.IsValid)
            {
                model.DatePosted = DateTime.UtcNow.AddHours(1);
                model.PostedBy = User.Identity.Name;

                var id = await _postService.Create(model);
                var checkimg = upload.Count(a => a != null && a.ContentLength > 0);
                if (checkimg > 0)
                {
                    await _imageService.PostImageCreate(upload, id);
                }

                return RedirectToAction("MyPost");
            }

            return View(model);
        }
        // GET: Content/Posts
        public async Task<ActionResult> MyPost(string searchString, string currentFilter, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var items = await _postService.StaffPost(searchString, currentFilter, page);

            int pageSize = 50;
            int pageNumber = (page ?? 1);
            ViewBag.Total = items.Count();
            return View(items.ToPagedList(pageNumber, pageSize));
        }

        // GET: Content/Posts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var post = await _postService.Details(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        public ActionResult _UserPost(string searchString, string currentFilter, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var items = db.Posts.Include(x => x.Comments).Where(x => x.Title != "" || x.PageType == PageType.Article || x.PageType == PageType.News);
            if (!String.IsNullOrEmpty(searchString))
            {

                items = items.Where(s => s.Content.ToUpper().Contains(searchString.ToUpper()) || s.Title.ToUpper().Contains(searchString.ToUpper()));

            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            ViewBag.Total = items.Count();
            return PartialView(items.OrderByDescending(x => x.DatePosted).ToPagedList(pageNumber, pageSize));

        }

        // GET: Content/Posts/Edit/5
        public async Task<ActionResult> EditMyPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var post = await _postService.Get(id);
            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        // POST: Content/Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditMyPost(Post model, List<HttpPostedFileBase> upload)
        {
            if (ModelState.IsValid)
            {
                model.PostedBy = User.Identity.Name;
                var checkimg = upload.Count(a => a != null && a.ContentLength > 0);
                if (checkimg > 0)
                {
                    await _imageService.PostImageDelete(model.Id);
                    await _imageService.PostImageCreate(upload, model.Id);
                }
                await _postService.Edit(model);
                //return Redirect(ReturnUrl);
                return RedirectToAction("MyPost");
            }
            return View(model);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var post = await _postService.Get(id);

            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        // POST: Content/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _imageService.PostImageDelete(id);
            await _postService.Delete(id);
            return RedirectToAction("MyPost");
        }


        public async Task<ActionResult> DeleteComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var comment = await _postService.GetComment(id);

            if (comment == null)
            {
                return HttpNotFound();
            }

            return View(comment);
        }


        [HttpPost, ActionName("DeleteComment")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCommentConfirmed(int id)
        {

            await _postService.DeleteComment(id);
            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> UpdateComment(Comment model, int id)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        model.Username = User.Identity.Name;
        //        model.DateCommented = DateTime.UtcNow.AddHours(1);
        //        model.PostId = id;
        //        await _postService.CreateComment(model);

        //        return RedirectToAction("Details", new { id = id });
        //    }

        //    return View(model);
        //}

        #endregion

        #region staff data
        //public async Task<ActionResult> ClassIndex()
        //{
        //    var items = await _staffProfileService.ClassesByStaff();
        //    return View(items);
        //}
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult> Qualification()
        {
            var items = await _staffProfileService.ListQualificationForUser();
            return View(items);
        }

        public async Task<ActionResult> StudentsInClass(int id)
        {
            int sessId = await _sessionService.GetCurrentSessionId();
            var items = await _resultService.StudentsByByClassId(id);
            var check = await _publishResultService.CheckPublishResult(sessId, id);
            var session = db.Sessions.OrderByDescending(x => x.Id);
            var currentSession = await session.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            if (check == true)
            {
                ViewBag.Check = "Published";
            }
            else
            {
                ViewBag.Check = "Not Published";
            }
            foreach (var name in items)
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
            var classname = await _classLevelService.Get(id);
            ViewBag.classname = classname.ClassName;
            ViewBag.ClassId = id;
            ViewBag.sessId = sessId;
            ViewBag.session = currentSession.SessionYear + "-" + currentSession.Term;
            return View(items.OrderBy(x => x.StudentName));
        }

        public async Task<ActionResult> StudentsListInClass(int id)
        {
            int sessId = await _sessionService.GetCurrentSessionId();
            var items = await _resultService.StudentsByByClassId(id);
            var session = db.Sessions.OrderByDescending(x => x.Id);
            var currentSession = await session.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);

            foreach (var name in items)
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
            var classname = await _classLevelService.Get(id);
            ViewBag.classname = classname.ClassName;
            ViewBag.ClassId = id;
            ViewBag.sessId = sessId;
            ViewBag.session = currentSession.SessionYear + "-" + currentSession.Term;
            return View(items.OrderBy(x => x.StudentName));
        }

        public async Task<ActionResult> EditWardUser(int? id, int cid)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await _studentProfileService.Get(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName", user.StateOfOrigin);
            ViewBag.ClassId = cid;
            return View(user);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditWardUser(StudentInfoDto model, HttpPostedFileBase upload, int cid)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _studentProfileService.Edit(model);

                    var getstudent = await _studentProfileService.Get(model.Id);
                    if (getstudent != null)
                    {
                        if (getstudent.ImageId != 0)
                        {
                            await _imageService.Delete(getstudent.ImageId);
                        }
                        var imgId = await _imageService.Create(upload);

                        await _studentProfileService.UpdateImageId(getstudent.Id, imgId);

                    }
                    TempData["success"] = "Update Successful.";
                    return RedirectToAction("StudentsListInClass", new { id = cid });
                }
                catch (Exception e)
                {
                    TempData["error"] = "Update Unsuccessful, (" + e.ToString() + ")";
                }

            }
            ViewBag.ClassId = cid;
            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName", model.StateOfOrigin);
            return View(model);
        }

        public async Task<ActionResult> StudentResult(int id)
        {
            var items = await _resultService.EnrolledSubjectForEnrollment(id);
            var name = await _enrollmentService.Get(id);
            var user = await _studentProfileService.Get(name.StudentProfileId);
            string stname = user.Fullname;
            ViewBag.Name = stname;
            ViewBag.ClassLevel = name.ClassLevel.ClassName;
            ViewBag.ClassId = name.ClassLevelId;
            ViewBag.EnrollmentId = id;
            ViewBag.Class = name.ClassLevel.ClassName;
            ViewBag.Session = name.Session.SessionYear + "-" + name.Session.Term;
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
            var positions = await _resultService.Position(name.SessionId, name.ClassLevelId);
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

            return View(items);
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
                    TempData["error"] = "Update failed. Update test score and exam score";
                }

            }
            else
            {
                TempData["error"] = "Update failed. Update test score and exam score";
            }
            return RedirectToAction("StudentResult", new { id = id });
        }

        public async Task<ActionResult> Subject()
        {
            var items = await _staffProfileService.SubjectsByStaff();
            return View(items);
        }

        public async Task<ActionResult> ClassSubject(int classId)
        {
            var items = await db.Subjects.Include(x => x.ClassLevel).Where(x => x.ClassLevelId == classId).OrderBy(x => x.SubjectName).ToListAsync();
            var classinfo = await _classLevelService.Get(classId);
            ViewBag.Classname = classinfo.ClassName;

            return View(items);
        }

        public async Task<ActionResult> IndexOfClass()
        {
            var items = await _staffProfileService.ClassesByStaff();
            return View(items);
        }

        public async Task<ActionResult> ResultIndex(int id, string returnUrl)
        {
            var subject = await _subjectService.Get(id);
            var session = await _sessionService.GetAllSession();
            ViewBag.SubjectsId = subject.Id;
            ViewBag.SubjectName = subject.SubjectName;
            ViewBag.ClassName = subject.ClassLevel.ClassName;
            ViewBag.ReturnUrl = returnUrl;
            return View(session);
        }

        #endregion
        //subject Master List
        #region subject Master List

        public async Task<ActionResult> SubjectResultSheet(int subId, int sessionId)
        {
            var studentsEnrolled = await _enrollmentService.StudentsListBySubIdBySessionId(subId, sessionId);
            var classinfo = await db.Subjects.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == subId);
            ViewBag.info = classinfo;
            var settings = await db.Settings.FirstOrDefaultAsync();
            ViewBag.setting = settings;
            foreach (var name in studentsEnrolled)
            {
                if (name.FullName.Substring(0, 2).Contains("\t"))
                {
                    name.FullName = name.FullName.Remove(0, 2);
                }
                if (name.FullName.Substring(0, 2).Contains(" "))
                {
                    name.FullName = name.FullName.TrimStart();
                }
            }

            studentsEnrolled = studentsEnrolled.OrderByDescending(x => x.TotalScore).ToList();
            return View(studentsEnrolled);
        }

        #endregion
        public async Task<ActionResult> StudentsList(int subId, int sessionId)
        {
            try
            {
                var sub1 = db.Subjects.FirstOrDefault(x => x.Id == subId);
                var subjectsetting = await db.Subjects.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == sub1.Id);
                var setting = db.Settings.FirstOrDefault();
                TempData["setting"] = "<span> Note:</span><br>";
                if (setting.EnableTestScore == true)
                {
                    TempData["setting2"] = "<span>Test Score Total = " + subjectsetting.TestScore + "</span><br>";
                }
                if (setting.EnableTestScore2 == true)
                {
                    TempData["setting1"] = "<span>2nd Test Score Total = " + subjectsetting.TestScore2 + "</span><br>";
                }

                if (setting.EnableProject == true)
                {
                    TempData["setting3"] = "<span>Project Score Total = " + subjectsetting.Project + "</span><br>";
                }
                if (setting.EnableAssessment == true)
                {
                    TempData["setting4"] = "<span>Class Exercise Score Total = " + subjectsetting.ClassExercise + "</span><br>";
                }
                if (setting.EnableClassExercise == true)
                {
                    TempData["setting5"] = "<span>Assessment Score Total = " + subjectsetting.Assessment + "</span><br>";
                }
                if (setting.EnableExamScore == true)
                {

                    TempData["setting6"] = "<span>Exam Score Total = " + subjectsetting.ExamScore + "</span><br>";
                }
                //TempData["setting"] = "<span style=\"font-size:18px;font-weight:800;\">Note: Exam Score Total = " + subjectsetting.ExamScore + " and Assessment Score Total = " + subjectsetting.TestScore + "</span>";

                //var setting = db.ClassLevels.FirstOrDefault(x => x.Id == sub1.ClassLevelId);
                //TempData["setting"] = "<span style=\"font-size:18px;font-weight:800;\">Note: Exam Score Total = " + setting.ExamScore + " and Assessment Score Total = " + setting.AccessmentScore + "</span>";
                var classinfo = await _subjectService.Get(subId);
                if (classinfo == null)
                {
                    return HttpNotFound();
                }
                var enrolledStudents = await _enrollmentService.EnrolledStudentBySessionClassId(sessionId, classinfo.ClassLevelId);
                //Check if there is any student in that class
                if (enrolledStudents.Count() == 0)
                {
                    TempData["error"] = "Class Empty";//Page not found
                    return RedirectToAction("ResultIndex", new { id = subId });
                }
                var subname = await _subjectService.Get(subId);
                var classLevel = await _classLevelService.Get(classinfo.ClassLevelId);
                var session = await _sessionService.GetInfo(sessionId);
                ViewBag.SubjectId = classinfo.Id;
                ViewBag.Subject = subname.SubjectName;

                ViewBag.Class = classLevel.ClassName;
                ViewBag.ClassLevelId = classinfo.ClassLevelId;
                ViewBag.Session = session.FullSession;
                ViewBag.SessionId = session.Id;
                //check if result has been published
                var publish = await db.PublishResults.FirstOrDefaultAsync(x => x.ClassLevelId == classLevel.Id && x.SessionId == sessionId);
                if (publish == null)
                {
                    ViewBag.booling = "false";
                }
                else
                {
                    ViewBag.booling = "true";
                }
                var studentsEnrolled = await _enrollmentService.StudentsListBySubIdBySessionId(subId, sessionId);
                var settings = await db.Settings.FirstOrDefaultAsync();
                ViewBag.setting = settings;
                foreach (var name in studentsEnrolled)
                {
                    if (name.FullName.Substring(0, 2).Contains("\t"))
                    {
                        name.FullName = name.FullName.Remove(0, 2);
                    }
                    if (name.FullName.Substring(0, 2).Contains(" "))
                    {
                        name.FullName = name.FullName.TrimStart();
                    }
                }
                //UpdateComment recognitive domain
                foreach (var abcd in studentsEnrolled)
                {
                    var check = db.RecognitiveDomains.Where(x => x.EnrolmentId == abcd.EnrollmentId);
                    if (check == null)
                    {
                        RecognitiveDomain recognitive = new RecognitiveDomain();
                        recognitive.EnrolmentId = abcd.EnrollmentId;
                        db.RecognitiveDomains.Add(recognitive);
                        await db.SaveChangesAsync();
                    }


                }


                //UpdateComment psychomotor domain
                foreach (var abcd in studentsEnrolled)
                {
                    var check = db.PsychomotorDomains.Where(x => x.EnrolmentId == abcd.EnrollmentId);
                    if (check == null)
                    {
                        PsychomotorDomain psychomotor = new PsychomotorDomain();
                        psychomotor.EnrolmentId = abcd.EnrollmentId;
                        db.PsychomotorDomains.Add(psychomotor);
                        await db.SaveChangesAsync();
                    }


                }


                //UpdateComment affective domain
                foreach (var abcd in studentsEnrolled)
                {
                    var check = db.AffectiveDomains.Where(x => x.EnrolmentId == abcd.EnrollmentId);
                    if (check == null)
                    {
                        AffectiveDomain affectivedomain = new AffectiveDomain();
                        affectivedomain.EnrolmentId = abcd.EnrollmentId;
                        db.AffectiveDomains.Add(affectivedomain);
                        await db.SaveChangesAsync();
                    }


                }


                return View(studentsEnrolled.OrderBy(x => x.FullName));
            }
            catch (Exception e)
            {
                TempData["error"] = "Unable to fetch Students";
            }
            return RedirectToAction("ResultIndex", new { id = subId });
        }


        public async Task<ActionResult> StudentsList2(int subId, int sessionId)
        {
            try
            {
                var sub1 = db.Subjects.FirstOrDefault(x => x.Id == subId);
                var subjectsetting = await db.Subjects.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == sub1.Id);

                var setting = db.Settings.FirstOrDefault();
                TempData["setting"] = "<span> Note:</span><br>";
                if (setting.EnableTestScore == true)
                {
                    TempData["setting2"] = "<span>Test Score Total = " + subjectsetting.TestScore + "</span><br>";
                }
                if (setting.EnableTestScore2 == true)
                {
                    TempData["setting1"] = "<span>2nd Test Score Total = " + subjectsetting.TestScore2 + "</span><br>";
                }

                if (setting.EnableProject == true)
                {
                    TempData["setting3"] = "<span>Project Score Total = " + subjectsetting.Project + "</span><br>";
                }
                if (setting.EnableAssessment == true)
                {
                    TempData["setting4"] = "<span>Class Exercise Score Total = " + subjectsetting.ClassExercise + "</span><br>";
                }
                if (setting.EnableClassExercise == true)
                {
                    TempData["setting5"] = "<span>Assessment Score Total = " + subjectsetting.Assessment + "</span><br>";
                }
                if (setting.EnableExamScore == true)
                {

                    TempData["setting6"] = "<span>Exam Score Total = " + subjectsetting.ExamScore + "</span><br>";
                }

                //TempData["setting"] = "<span style=\"font-size:18px;font-weight:800;\">Note: Exam Score Total = " + subjectsetting.ExamScore + " and Assessment Score Total = " + subjectsetting.TestScore + "</span>";

                //var setting = db.ClassLevels.FirstOrDefault(x => x.Id == sub1.ClassLevelId);
                //TempData["setting"] = "<span style=\"font-size:18px;font-weight:800;\">Note: Exam Score Total = " + setting.ExamScore + " and Assessment Score Total = " + setting.AccessmentScore + "</span>";
                var classinfo = await _subjectService.Get(subId);
                if (classinfo == null)
                {
                    return HttpNotFound();
                }
                var enrolledStudents = await _enrollmentService.EnrolledStudentBySessionClassId(sessionId, classinfo.ClassLevelId);
                //Check if there is any student in that class
                if (enrolledStudents.Count() == 0)
                {
                    TempData["error"] = "Class Empty";//Page not found
                    return RedirectToAction("ResultIndex", new { id = subId });
                }
                var subname = await _subjectService.Get(subId);
                var classLevel = await _classLevelService.Get(classinfo.ClassLevelId);
                var session = await _sessionService.GetInfo(sessionId);
                ViewBag.SubjectId = classinfo.Id;
                ViewBag.Subject = subname.SubjectName;

                ViewBag.Class = classLevel.ClassName;
                ViewBag.ClassLevelId = classinfo.ClassLevelId;
                ViewBag.Session = session.FullSession;
                ViewBag.SessionId = session.Id;
                //check if result has been published
                var publish = await db.PublishResults.FirstOrDefaultAsync(x => x.ClassLevelId == classLevel.Id && x.SessionId == sessionId);
                if (publish == null)
                {
                    ViewBag.booling = "false";
                }
                else
                {
                    ViewBag.booling = "true";
                }
                var studentsEnrolled = await _enrollmentService.StudentsListBySubIdBySessionId(subId, sessionId);
                var settings = await db.Settings.FirstOrDefaultAsync();
                ViewBag.setting = settings;
                foreach (var name in studentsEnrolled)
                {
                    if (name.FullName.Substring(0, 2).Contains("\t"))
                    {
                        name.FullName = name.FullName.Remove(0, 2);
                    }
                    if (name.FullName.Substring(0, 2).Contains(" "))
                    {
                        name.FullName = name.FullName.TrimStart();
                    }
                }
                //UpdateComment recognitive domain
                foreach (var abcd in studentsEnrolled)
                {
                    var check = db.RecognitiveDomains.Where(x => x.EnrolmentId == abcd.EnrollmentId);
                    if (check == null)
                    {
                        RecognitiveDomain recognitive = new RecognitiveDomain();
                        recognitive.EnrolmentId = abcd.EnrollmentId;
                        db.RecognitiveDomains.Add(recognitive);
                        await db.SaveChangesAsync();
                    }


                }


                //UpdateComment psychomotor domain
                foreach (var abcd in studentsEnrolled)
                {
                    var check = db.PsychomotorDomains.Where(x => x.EnrolmentId == abcd.EnrollmentId);
                    if (check == null)
                    {
                        PsychomotorDomain psychomotor = new PsychomotorDomain();
                        psychomotor.EnrolmentId = abcd.EnrollmentId;
                        db.PsychomotorDomains.Add(psychomotor);
                        await db.SaveChangesAsync();
                    }


                }


                //UpdateComment affective domain
                foreach (var abcd in studentsEnrolled)
                {
                    var check = db.AffectiveDomains.Where(x => x.EnrolmentId == abcd.EnrollmentId);
                    if (check == null)
                    {
                        AffectiveDomain affectivedomain = new AffectiveDomain();
                        affectivedomain.EnrolmentId = abcd.EnrollmentId;
                        db.AffectiveDomains.Add(affectivedomain);
                        await db.SaveChangesAsync();
                    }


                }


                return View(studentsEnrolled.OrderBy(x => x.FullName));
            }
            catch (Exception e)
            {
                TempData["error"] = "Unable to fetch Students";
            }
            return RedirectToAction("ResultIndex", new { id = subId });
        }

        /// <summary>
        /// 
        /// update recognitive domain
        /// </summary>
        /// <param name="subId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        /// 


        // GET: Content/Assignments/Edit/5
        //public async Task<ActionResult> EditRecognitive(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    RecognitiveDomain assignment = await db.RecognitiveDomains.FirstOrDefaultAsync(x=>x.StudentId == id);
        //    if (assignment == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    return PartialView(assignment);
        //}

        //// POST: Content/Assignments/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public async Task<ActionResult> EditRecognitive(RecognitiveDomain domain, int subId, int sessionId)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(domain).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        TempData["msg"] = "Recognitive ddomain Update Successful";
        //        return RedirectToAction("StudentsList", new { subId = subId, sessionId = sessionId });
        //    }
        //   return View(domain);
        //}



        public async Task<ActionResult> PreviewResult(int subId, int sessionId)
        {
            try
            {
                var classinfo = await _subjectService.Get(subId);
                if (classinfo == null)
                {
                    return HttpNotFound();
                }
                var enrolledStudents = await _enrollmentService.EnrolledStudentBySessionClassId(sessionId, classinfo.ClassLevelId);
                //Check if there is any student in that class
                if (enrolledStudents.Count() == 0)
                {
                    //Page not found
                    return HttpNotFound();
                }
                var subname = await _subjectService.Get(subId);
                var classLevel = await _classLevelService.Get(classinfo.ClassLevelId);
                var session = await _sessionService.GetInfo(sessionId);
                ViewBag.SubjectId = classinfo.Id;
                ViewBag.Subject = subname.SubjectName;

                ViewBag.Class = classLevel.ClassName;
                ViewBag.ClassLevelId = classinfo.ClassLevelId;
                ViewBag.Session = session.FullSession;
                ViewBag.SessionId = session.Id;
                //check if result has been published
                var publish = await db.PublishResults.FirstOrDefaultAsync(x => x.ClassLevelId == classLevel.Id && x.SessionId == sessionId);
                if (publish == null)
                {
                    ViewBag.booling = "false";
                }
                else
                {
                    ViewBag.booling = "true";
                }
                var studentsEnrolled = await _enrollmentService.StudentsListBySubIdBySessionId(subId, sessionId);
                foreach (var name in studentsEnrolled)
                {
                    if (name.FullName.Substring(0, 2).Contains("\t"))
                    {
                        name.FullName = name.FullName.Remove(0, 2);
                    }
                    if (name.FullName.Substring(0, 2).Contains(" "))
                    {
                        name.FullName = name.FullName.TrimStart();
                    }
                }
                return View(studentsEnrolled.OrderBy(x => x.FullName));
            }
            catch (Exception e)
            {
                TempData["error"] = "Unable to fetch Students";
            }
            return RedirectToAction("ResultIndex", new { id = subId });
        }


        //offline



        public JsonResult SubjectList(int Id)
        {
            var c = db.ClassLevels.FirstOrDefault(x => x.Id == Id).Id;
            var subject = from s in db.Subjects
                          where s.ClassLevelId == c
                          select s;

            return Json(new SelectList(subject.Where(x => x.ShowSubject == true).OrderBy(x => x.SubjectName).ToArray(), "Id", "SubjectName"), JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> Offline(int subId = 0, Int32 sessionId = 0, int classListId = 0)
        {
            var classinfo = await _subjectService.Get(subId);
            try
            {
                var session1 = await _sessionService.GetAllSession();
                var classList = await _classLevelService.ClassLevelList();
                ViewBag.Session = new SelectList(session1, "Id", "FullSession", sessionId);
                ViewBag.Classlist = new SelectList(classList, "Id", "ClassLevelName", classListId);

                if (classinfo == null)
                {
                    ViewBag.notfound = true;
                    return View();
                }
                var enrolledStudents = await _enrollmentService.EnrolledStudentBySessionClassId(sessionId, classinfo.ClassLevelId);
                //Check if there is any student in that class
                var studentsEnrolled = await _enrollmentService.StudentsListBySubIdBySessionId(subId, sessionId);
                if (enrolledStudents.Count() == 0)
                {
                    //Page not found
                    ViewBag.notfound = true;
                    return View();
                }
                else
                {
                    var subname = await _subjectService.Get(subId);
                    var classLevel = await _classLevelService.Get(classinfo.ClassLevelId);
                    var session = await _sessionService.GetInfo(sessionId);
                    ViewBag.SubjectId = classinfo.Id;
                    ViewBag.Subject = subname.SubjectName;

                    ViewBag.Class = classLevel.ClassName;
                    //ViewBag.exam = classLevel.ExamScore;
                    //ViewBag.acc = classLevel.AccessmentScore;
                    var totalCA = subname.Project + subname.Assessment + subname.ClassExercise + subname.TestScore + subname.TestScore2;
                    ViewBag.exam = subname.ExamScore;
                    ViewBag.acc = totalCA;
                    ViewBag.ClassLevelId = classinfo.ClassLevelId;
                    ViewBag.SessionOne = session.FullSession;
                    ViewBag.SessionId = session.Id;
                    //check if result has been published


                    foreach (var name in studentsEnrolled)
                    {
                        if (name.FullName.Substring(0, 2).Contains("\t"))
                        {
                            name.FullName = name.FullName.Remove(0, 2);
                        }
                        if (name.FullName.Substring(0, 2).Contains(" "))
                        {
                            name.FullName = name.FullName.TrimStart();
                        }
                    }
                }
                ViewBag.Session = new SelectList(session1, "Id", "FullSession", sessionId);
                ViewBag.Classlist = new SelectList(classList, "Id", "ClassLevelName", classListId);
                return View(studentsEnrolled.OrderBy(x => x.FullName));
            }
            catch (Exception e)
            {
                TempData["error"] = "Unable to fetch Students";
            }
            //return RedirectToAction("StudentsList", new { subId = subId, sessionId = sessionId, classListId = classinfo.ClassLevelId });
            return RedirectToAction("Offline", new { subId = subId, sessionId = sessionId, classListId = classinfo.ClassLevelId });
        }

        [HttpPost]
        public async Task<ActionResult> UpdateScore(EnrolledSubject models)
        {
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var sessId = session.Id;
            var cid = await db.Enrollments.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == models.EnrollmentId);
            //var setting = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == cid.ClassLevelId);
            var subsetting = await db.Subjects.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == models.SubjectId);
            //if (models.ExamScore > setting.ExamScore || models.TestScore > setting.AccessmentScore)
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
            var totalSubCA = subsetting.Project + subsetting.ClassExercise + subsetting.TestScore + subsetting.TestScore2 + subsetting.Assessment;
            if (models.ExamScore > subsetting.ExamScore || totalCA > totalSubCA)
            {

                ViewBag.sessId = sessId;
                TempData["error"] = "Accessment or Exam Score not in Range";
                return Content("<script language='javascript' type='text/javascript'>alert('Test Score or Exam Score not in Range');</script>");
            }

            var subject = await _resultService.GetSubjectByEnrolledSubId(models.SubjectId);
            var classlevel = await _resultService.GetClassByClassId(subject.ClassLevelId);
            string userLevel = classlevel.ClassName;

            try
            {

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

                //await _enrolledSubjectService.Edit(models);
                db.Entry(models).State = EntityState.Modified;
                await db.SaveChangesAsync();
                await _resultService.UpdateResult(models.EnrollmentId);
                TempData["success"] = "Saved successfully";
                return Content("<script language='javascript' type='text/javascript'>alert('Saved Successful');</script>");
                //TempData["msg"] = "success";
                //return RedirectToAction("StudentsList", new { subId = models.SubjectId, sessionId = sessId });


            }
            catch (Exception e)
            {

            }

            //return new EmptyResult();
            return Content("<script language='javascript' type='text/javascript'>alert('Saved Successful');</script>");
        }

        public async Task<ActionResult> IsOffered(EnrolledSubject models)
        {

            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var sessId = session.Id;


            try
            {
                var enr = await db.EnrolledSubjects.FirstOrDefaultAsync(x => x.Id == models.Id);
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


            }
            catch (Exception e)
            {

            }

            //return new EmptyResult();
            return Content("<script language='javascript' type='text/javascript'>alert('Saved Successful');</script>");
        }


        public async Task<ActionResult> DeleteEnrollSubject(int id = 0, int sessionId = 0, int classId = 0)
        {
            await _staffProfileService.RemoveStudents(id, sessionId, classId);
            return RedirectToAction("StudentsList", new { subId = id, sessionId = sessionId });
        }


        public async Task<ActionResult> Edit(int id, int sessId)
        {
            var item = await _enrolledSubjectService.Get(id);
            var user = await _enrollmentService.Get(item.EnrollmentId);
            var name = await _studentProfileService.Get(user.StudentProfileId);
            string fullname = name.Fullname;
            ViewBag.StudentsName = fullname;
            ViewBag.reg = name.StudentRegNumber;
            ViewBag.sessId = sessId;
            ViewBag.s = user.Session.SessionYear + "-" + user.Session.Term;
            ViewBag.c = user.ClassLevel;
            return View(item);
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EnrolledSubject models, int sessId)
        {
            if (ModelState.IsValid)
            {
                var cid = await db.Enrollments.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == models.EnrollmentId);
                //var setting = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == cid.ClassLevelId);
                var subjectsetting = await db.Subjects.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == models.SubjectId);
                //if (models.ExamScore > setting.ExamScore || models.TestScore > setting.AccessmentScore)
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
                var totalSubCA = subjectsetting.Project + subjectsetting.ClassExercise + subjectsetting.TestScore + subjectsetting.TestScore2 + subjectsetting.Assessment;
                var setting = db.Settings.FirstOrDefault();
                if (models.ExamScore > subjectsetting.ExamScore || totalCA > totalSubCA)
                {
                    TempData["error"] = "Assessment Score or Exam Score not in Range";
                    if (setting.EnableTestScore == true)
                    {
                        TempData["error1"] = "Test Score  not in Range";
                    }
                    if (setting.EnableTestScore2 == true)
                    {
                        TempData["error2"] = "2nd Test Score  not in Range";
                    }

                    if (setting.EnableProject == true)
                    {
                        TempData["error3"] = "Project Score  not in Range";
                    }
                    if (setting.EnableAssessment == true)
                    {
                        TempData["error4"] = "Assessment Score not in Range";
                    }
                    if (setting.EnableClassExercise == true)
                    {
                        TempData["error5"] = "Class Exercise Score not in Range";
                    }
                    if (setting.EnableExamScore == true)
                    {

                        TempData["error6"] = "Exam Score not in Range";
                    }
                    //TempData["error"] = "Assessment Score or Exam Score not in Range";
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
                    models.TotalScore = totalScore;
                    if (totalScore > 0.00m)
                    {
                        models.IsOffered = true;
                    }
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
                        else if (userLevel.Substring(0, 3).Contains("PG"))
                        {
                            models.GradingOption = GradingOption.PG;
                        }
                    }

                    await _enrolledSubjectService.Edit(models);
                    await _resultService.UpdateResult(models.EnrollmentId);
                    TempData["msg"] = "success";
                    return RedirectToAction("StudentsList", new { subId = models.SubjectId, sessionId = sessId });


                }
                catch (Exception e)
                {

                }
            }
            ViewBag.sessId = sessId;
            string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
            return View(models);

        }


        public async Task<ActionResult> ReloadStudents(int id = 0, int sessionId = 0, int classId = 0)
        {
            await _staffProfileService.ReloadStudents(id, sessionId, classId);
            var setting = await db.Settings.FirstOrDefaultAsync();
            if (setting.PrintOutOption == PrintOutOption.PrintOutThree)
            {
                return RedirectToAction("StudentsList2", new { subId = id, sessionId = sessionId });
            }
            else
            {
                return RedirectToAction("StudentsList", new { subId = id, sessionId = sessionId });
            }

        }

        //reload for score sheet printing
        public async Task<ActionResult> ReloadScoreSheet(int id = 0, int sessionId = 0, int classId = 0)
        {
            await _staffProfileService.ReloadStudents(id, sessionId, classId);

            return RedirectToAction("ScoreSheet", new { subId = id, sessionId = sessionId });
        }

        //reload enterscore action
        public async Task<ActionResult> ReloadEnterscore(int id = 0, int sessionId = 0, int classId = 0)
        {
            await _staffProfileService.ReloadStudents(id, sessionId, classId);
            var setting = await db.Settings.FirstOrDefaultAsync();
            if (setting.PrintOutOption == PrintOutOption.PrintOutThree)
            {
                return RedirectToAction("EnterScore2", new { subId = id, sessionId = sessionId });
            }
            else
            {
                return RedirectToAction("EnterScore", new { subId = id, sessionId = sessionId });
            }

        }

        public async Task<ActionResult> AdminReloadEnterscore(int id = 0, int sessionId = 0, int classId = 0)
        {
            await _staffProfileService.AdminReloadStudents(id, sessionId, classId);
            var setting = await db.Settings.FirstOrDefaultAsync();
            if (setting.PrintOutOption == PrintOutOption.PrintOutThree)
            {
                return RedirectToAction("EnterScore2", new { subId = id, sessionId = sessionId });
            }
            else
            {
                return RedirectToAction("EnterScore", new { subId = id, sessionId = sessionId });
            }

        }

        public async Task<ActionResult> AdminReloadEnterscore2(int id = 0, int sessionId = 0, int classId = 0)
        {

            await _staffProfileService.AdminReloadStudents2(id, sessionId, classId);
            var setting = await db.Settings.FirstOrDefaultAsync();
            if (setting.PrintOutOption == PrintOutOption.PrintOutThree)
            {
                return RedirectToAction("EnterScore2", new { subId = id, sessionId = sessionId });
            }
            else
            {
                return RedirectToAction("EnterScore", new { subId = id, sessionId = sessionId });
            }

        }





        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>


        public async Task<ActionResult> ReloadAllStudent()
        {
            //var subject = db.EnrolledSubjects.Include(x => x.Enrollments).Where(x => x.SubjectId == id && x.Enrollments.SessionId == sessionId);

            //var studentClass = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == classId);

            ////get students enrolled in that class
            //var enrolledStudents = db.Enrollments.Where(c => c.ClassLevelId == studentClass.Id && c.SessionId == sessionId);
            var sess = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var session = db.Sessions.Where(x => x.SessionYear == sess.SessionYear);
            try
            {
                foreach (var sesion in session)
                {


                    var sub = db.Subjects.Where(x => x.ClassLevelId != 0);
                    try
                    {
                        foreach (var ss in sub)
                        {


                            var subject = db.EnrolledSubjects.Include(x => x.Enrollments).Where(x => x.SubjectId == ss.Id && x.Enrollments.SessionId == sesion.Id);
                            var ccc = db.ClassLevels.Where(x => x.Id != 0);
                            try
                            {
                                foreach (var cc in ccc)
                                {


                                    var studentClass = db.ClassLevels.FirstOrDefault(x => x.Id == cc.Id);

                                    //get students enrolled in that class
                                    var enrolledStudents = db.Enrollments.Where(c => c.ClassLevelId == studentClass.Id && c.SessionId == sesion.Id);
                                    var currentlevel = db.ClassLevels.FirstOrDefault(x => x.Id == studentClass.Id);
                                    try
                                    {
                                        foreach (var student in enrolledStudents.ToList())
                                        {
                                            var checkSubject = subject.FirstOrDefault(x => x.EnrollmentId == student.Id);
                                            if (checkSubject == null)
                                            {
                                                EnrolledSubject enrolledSubject = db.EnrolledSubjects.Create();
                                                enrolledSubject.SubjectId = ss.Id;
                                                if (currentlevel.ClassName.Substring(0, 3).Contains("SSS"))
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
                                                else if (currentlevel.ClassName.Substring(0, 3).Contains("PG"))
                                                {
                                                    enrolledSubject.GradingOption = GradingOption.PG;
                                                }
                                                enrolledSubject.EnrollmentId = student.Id;
                                                enrolledSubject.ExamScore = 0;
                                                enrolledSubject.TestScore = 0;
                                                enrolledSubject.TotalScore = 0;
                                                enrolledSubject.Project = 0;
                                                enrolledSubject.ClassExercise = 0;
                                                enrolledSubject.TestScore2 = 0;
                                                enrolledSubject.TotalCA = 0;
                                                enrolledSubject.IsOffered = false;
                                                db.EnrolledSubjects.Add(enrolledSubject);

                                            }

                                        }


                                    }
                                    catch (Exception r)
                                    {

                                    }

                                }

                            }
                            catch (Exception r)
                            {

                            }
                        }

                    }
                    catch (Exception r)
                    {
                        TempData["Report"] = "not success";
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                }
            }
            catch (Exception er)
            {

            }
            db.SaveChanges();
            TempData["Report"] = "success";
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }
        public async Task<ActionResult> PublishResult(int id, int classId)
        {
            try
            {
                if (id != 0 && classId != 0)
                {
                    await _resultService.PublishResult(id, classId);
                    TempData["Report"] = "The Result has been published. Students in this class can now check their results online.";
                    return RedirectToAction("StudentsInClass", new { id = classId });
                }


            }
            catch (Exception e)
            {
                TempData["Error"] = "The Result has not been published. Please a try again or contact the Administrator.";
            }

            return RedirectToAction("StudentsInClass", new { id = classId });
        }

        public async Task<ActionResult> UnpublishResult(int id, int classId)
        {
            try
            {
                if (id != 0 && classId != 0)
                {
                    await _resultService.UnpublishResult(id, classId);
                    TempData["Report"] = "The Result has been unpublished. Students in this class can't check their results online until the result is published.";
                    return RedirectToAction("StudentsInClass", new { id = classId });

                }


            }
            catch (Exception e)
            {
                TempData["Error"] = "The Result has not been unpublished. Please a try again or contact the Administrator.";
            }
            return RedirectToAction("StudentsInClass", new { id = classId });
        }

        public ActionResult Upload(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        // POST: Admin/Settings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(HttpPostedFileBase upload, int id)
        {
            try
            {
                var getstaff = await _staffProfileService.Get(id);
                if (getstaff != null)
                {
                    if (getstaff.ImageId != 0)
                    {
                        await _imageService.Delete(getstaff.ImageId);
                    }
                    var imgId = await _imageService.Create(upload);

                    await _staffProfileService.UpdateImageId(getstaff.Id, imgId);
                    TempData["success"] = "Update Successful.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                TempData["error"] = "Update Unsuccessful.";
            }
            return View();
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
            ViewBag.StateName = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await _staffProfileService.Get(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(StaffInfoDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _staffProfileService.Edit(model);
                    TempData["success"] = "Update Successful.";
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    TempData["error"] = "Update Unsuccessful, (" + e.ToString() + ")";
                }

            }
            return View(model);
        }

        #region
        //Assignment creation by subject teacher

        public async Task<ActionResult> AllAssignments()
        {

            var items = await _assignmentService.ListAll();
            return View(items);
        }

        public ActionResult AddAssignment(string subjectname, string ReturnUrl, int classId = 0, int subId = 0)
        {
            var userid = User.Identity.GetUserId();
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            if (subId == 0)
            {
                var staffsub = db.Subjects.Include(x => x.User).Where(x => x.ClassLevelId == classId);
                ViewBag.SubjectId = new SelectList(staffsub.OrderByDescending(x => x.SubjectName), "Id", "SubjectName");
            }
            if (classId == 0)
            {
                var classid = db.ClassLevels;
                ViewBag.ClassLevelId = new SelectList(classid.OrderByDescending(x => x.ClassName), "Id", "ClassName");
            }
            var nameclass = db.ClassLevels.FirstOrDefault(x => x.Id == classId);
            ViewBag.classid = classId;
            if (classId != 0)
            {
                ViewBag.classname = nameclass.ClassName;
            }

            ViewBag.subname = subId;

            //var staffsub = db.Subjects.Include(x => x.User).Where(x => x.UserId == userid);
            //ViewBag.SubjectId = new SelectList(staffsub, "Id", "SubjectName");
            ViewBag.subjectname = subjectname;
            return View();
        }



        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAssignment(Assignment model, int classId, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var userid = User.Identity.GetUserId();
                var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
                model.ClassLevelId = classId;
                model.SessionId = currentSession.Id;
                model.DateCreated = DateTime.UtcNow.AddHours(1);
                model.SubjectId = model.SubjectId;
                await _assignmentService.Create(model);
                TempData["success"] = "Successful";
                // return RedirectToAction("AssignmentList", new { classId = classId, subId = model.SubjectId });
                return Redirect(ReturnUrl);
            }
            var staffsub = db.Subjects.Include(x => x.User).Where(x => x.ClassLevelId == classId);
            ViewBag.SubjectId = new SelectList(staffsub.OrderByDescending(x => x.SubjectName), "Id", "SubjectName");

            var classid = db.ClassLevels;
            ViewBag.ClassLevelId = new SelectList(classid.OrderByDescending(x => x.ClassName), "Id", "ClassName");

            return View(model);
        }


        
        public async Task<ActionResult> LessonNoteEdit(string ReturnUrl, int id = 0)
        {
                                   var item = await db.LessonNotes.Include(x=>x.Session).Include(x=>x.Subject).Include(x=>x.Subject.ClassLevel).FirstOrDefaultAsync(x => x.Id == id);
            if(item == null)
            {
                return HttpNotFound();
            }
            var userid = User.Identity.GetUserId();
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
             var subject = await db.Subjects.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == item.SubjectId);
            
            ViewBag.c = subject.ClassLevel.ClassName;
            ViewBag.s = subject.SubjectName;
            return View();
        }



        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> LessonNoteEdit(LessonNote model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                
                model.LastEdited = DateTime.UtcNow.AddHours(1);
                 db.Entry(model).State = EntityState.Modified;
                await db.SaveChangesAsync();
                TempData["success"] = "Successful";
                return Redirect(ReturnUrl);
            }

            return View(model);
        }



        public async Task<ActionResult> AddLesson(string ReturnUrl, int subId = 0)
        {
            var userid = User.Identity.GetUserId();
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
             var subject = await db.Subjects.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == subId);
           
            ViewBag.subid = subId;
            ViewBag.c = subject.ClassLevel.ClassName;
            ViewBag.s = subject.SubjectName;
            return View();
        }



        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> AddLesson(LessonNote model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var userid = User.Identity.GetUserId();
                var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
                model.SessionId = currentSession.Id;
                model.DateCreated = DateTime.UtcNow.AddHours(1);
                model.LastEdited = DateTime.UtcNow.AddHours(1);
                model.SubjectId = model.SubjectId;
                db.LessonNotes.Add(model);
                await db.SaveChangesAsync();
                TempData["success"] = "Successful";
                return RedirectToAction("LessonNote", new {subId = model.SubjectId });
            }

            return View(model);
        }

        
        public async Task<ActionResult> LessonNoteDetail(int? id, string ReturnUrl)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
                       var item = await db.LessonNotes.Include(x=>x.Session).Include(x=>x.Subject).Include(x=>x.Subject.ClassLevel).FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReturnUrl = ReturnUrl;
            return View(item);
        }

                public async Task<ActionResult> PublishOrUnpublishNote(int id, string ReturnUrl)
        {
                       var check = await db.LessonNotes.Include(x=>x.Session).Include(x=>x.Subject).Include(x=>x.Subject.ClassLevel).FirstOrDefaultAsync(x => x.Id == id);
            if (check.IsPublished == true)
            {
                check.IsPublished = false;
            }
            else
            {
                check.IsPublished = true;
            }
             db.Entry(check).State = EntityState.Modified;
            await db.SaveChangesAsync();
            TempData["success"] = "Successful";
             
            return RedirectToAction("LessonNote", new {subId = check.SubjectId });
        }
        public async Task<ActionResult> LessonNote(int? subId)
        {
            if (subId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var currentUser = User.Identity.GetUserId();
            var note = await db.LessonNotes.Include(x => x.Subject).Include(x => x.StaffProfile).Include(x => x.Session).Where(x => x.SubjectId == subId).ToListAsync();
            var subject = await db.Subjects.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == subId);
            ViewBag.c = subject.ClassLevel.ClassName;
            ViewBag.s = subject.SubjectName;
            ViewBag.subId = subId;
            return View(note);
        }
        public async Task<ActionResult> AssignmentList(int? classId, int? subId)
        {
            if (subId == null || classId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var assignment = await db.Assignments.Include(x => x.ClassLevel).Include(x => x.AssignmentAnswers).Include(x => x.Subject).Include(x => x.Session).Where(x => x.SubjectId == subId && x.ClassLevelId == classId).ToListAsync();
            var c = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == classId);
            var s = await db.Subjects.FirstOrDefaultAsync(x => x.Id == subId);
            ViewBag.c = c.ClassName;
            ViewBag.s = s.SubjectName;
            ViewBag.classId = classId;
            ViewBag.subId = subId;
            return View(assignment);
        }

        public async Task<ActionResult> PublishOrUnpublish(int id, int ClassId, int subjectId, string ReturnUrl)
        {
            var check = await _assignmentService.Get(id);
            if (check.IsPublished == true)
            {
                check.IsPublished = false;
            }
            else
            {
                check.IsPublished = true;
            }
            await _assignmentService.Edit(check);
            TempData["success"] = "Successful";
            if (ReturnUrl != null)
            {
                return Redirect(ReturnUrl);
            }
            return RedirectToAction("AssignmentList", new { classId = ClassId, subId = subjectId });
        }

        public async Task<ActionResult> AnsweredAssignment(int classId, int assId, string ReturnUrl)
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var c = db.Assignments.Include(x => x.ClassLevel).Include(x => x.Session).FirstOrDefault(x => x.Id == assId && x.SessionId == currentSession.Id);
            ViewBag.AssignmentInfo = c;
            var items = await _assignmentService.ListForStudent(classId, assId);

            ViewBag.ReturnUrl = ReturnUrl;

            return View(items);

        }

        public async Task<ActionResult> AssignmentDetail(int? id, string ReturnUrl)
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
            ViewBag.ReturnUrl = ReturnUrl;
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

        #region event new
        //event new
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> NewEvent(string Subject, string DIscription, string Color, bool GeneralEvent, bool IsFullDay, DateTime Start, DateTime End)
        {
            Event myevent = new Event();
            var userid = User.Identity.GetUserId();
            myevent.UserId = userid;
            myevent.Subject = Subject;
            myevent.DIscription = DIscription;
            myevent.Color = Color;
            myevent.GeneralEvent = GeneralEvent;
            myevent.IsFullDay = IsFullDay;
            myevent.Start = Start;
            myevent.End = End;
            db.Events.Add(myevent);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");

        }

        #endregion

        #region
        public async Task<ActionResult> TimeTable(int? name)
        {
            try
            {
                var time = db.TimeTables.Include(x => x.ClassLevel);
                //ViewBag.Timetable = new SelectList(db.TimeTables, "Id", "Name", name);
                ViewBag.Timetable = new SelectList(time, "Id", "Name", name);
                var t = await db.TimeTables.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == name);
                ViewBag.time = t;
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

        }



        #endregion

        //sylabus
        #region
        public async Task<ActionResult> Sylabus()
        {
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var items = await db.syllables.Include(x => x.Session).Where(x => x.SessionId == currentSession.Id).ToListAsync();
            return View(items);
        }

        public async Task<ActionResult> SylabusDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Syllable abc = await db.syllables.Include(x => x.Session).FirstOrDefaultAsync(x => x.Id == id);
            if (abc == null)
            {
                return HttpNotFound();
            }
            return View(abc);
        }


        #endregion

        #region Import data excel
        //
        public async Task<ActionResult> Import(int? subId, int? sessionId, HttpPostedFileBase excelfile)
        {
            int? subid = 0;
            int? sessid = 0;
            string path = "";
            if (excelfile == null || excelfile.ContentLength == 0)
            {
                TempData["error"] = "Please select a excel file";
                return RedirectToAction("Offline");

            }
            else
            {
                if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx"))
                {
                    System.Random randomInteger = new System.Random();
                    int genNumber = randomInteger.Next(100000);
                    path = Server.MapPath("~/ExcelUpload/" + genNumber + excelfile.FileName);
                    //if (System.IO.File.Exists(path))
                    //    System.IO.File.Delete(path);
                    string nameswitheerror = "";
                    int sn = 0;

                    excelfile.SaveAs(path);

                    using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(path, false))
                    {
                        WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                        WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                        SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                        int rowCount = sheetData.Elements<Row>().Skip(2).Count();


                        foreach (Row r in sheetData.Descendants<Row>().Skip(2))
                        {

                            try
                            {
                                var enroId = r.ChildElements[2].InnerText;
                                var Test = r.ChildElements[4].InnerText;
                                var Exam = r.ChildElements[5].InnerText;


                                int enrolSubId = Convert.ToInt32(enroId);
                                int examscore = Convert.ToInt32(Exam);
                                int testscore = Convert.ToInt32(Test);

                                var enrolled = await db.EnrolledSubjects.AsNoTracking().FirstOrDefaultAsync(x => x.Id == enrolSubId);
                                var enrollement = await db.Enrollments.Include(x => x.User).Include(x => x.StudentProfile).Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == enrolled.EnrollmentId);
                                var cid = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == enrollement.ClassLevelId);
                                var subidinfo = await db.EnrolledSubjects.Include(x => x.Subject).Include(x => x.Enrollments.Session).FirstOrDefaultAsync(x => x.Id == enrolSubId);
                                subid = subidinfo.SubjectId;

                                sessid = subidinfo.Enrollments.Session.Id;
                                var setting = cid;

                                var subjectCA = await db.Subjects.FirstOrDefaultAsync(x => x.Id == subidinfo.SubjectId);
                                var totalCA = testscore;
                                //var totalCA = subidinfo.Project + subidinfo.ClassExercise + subidinfo.TestScore + subidinfo.TestScore2 + subidinfo.Assessment;
                                var totalSubCA = subjectCA.Project + subjectCA.ClassExercise + subjectCA.TestScore + subjectCA.TestScore2 + subjectCA.Assessment;

                                //if (examscore > enrollement.ClassLevel.ExamScore || testscore > enrollement.ClassLevel.AccessmentScore)
                                if (examscore > subjectCA.ExamScore || totalCA > totalSubCA)
                                {
                                    nameswitheerror = nameswitheerror + "(" + sn++ + ")" + enrollement.StudentProfile.StudentRegNumber + ", has exam " + examscore + " and C.A " + testscore + " that are out of range /// <br/>";
                                    //TempData["error"] = "Please Some Scores are out of range";
                                    //  return RedirectToAction("Offline", new { subId = subid, sessionId = sessionId });
                                }
                                else
                                {


                                    var subject = await _resultService.GetSubjectByEnrolledSubId(subId);
                                    var classlevel = await _resultService.GetClassByClassId(subject.ClassLevelId);
                                    string userLevel = classlevel.ClassName;


                                    if (enrolled.Assessment == null)
                                    {
                                        enrolled.Assessment = 0;
                                    }

                                    if (enrolled.TestScore2 == null)
                                    {
                                        enrolled.TestScore2 = 0;
                                    }
                                    if (enrolled.ClassExercise == null)
                                    {
                                        enrolled.ClassExercise = 0;
                                    }
                                    if (enrolled.Project == null)
                                    {
                                        enrolled.Project = 0;
                                    }
                                    if (enrolled.TestScore == null)
                                    {
                                        enrolled.TestScore = 0;
                                    }

                                    if (enrolled.ExamScore == null)
                                    {
                                        enrolled.ExamScore = 0;
                                    }

                                    if (enrolled.TotalCA == null)
                                    {
                                        enrolled.TotalCA = 0;
                                    }

                                    if (enrolled.TotalScore == null)
                                    {
                                        enrolled.TotalScore = 0;
                                    }



                                    decimal? totalScore = examscore + testscore;
                                    enrolled.TotalScore = totalScore;
                                    enrolled.TestScore = testscore;
                                    enrolled.ExamScore = examscore;
                                    enrolled.TotalCA = testscore;



                                    if (enrolled.TestScore != 0 || enrolled.ExamScore != 0)
                                    {
                                        enrolled.IsOffered = true;
                                    }
                                    else
                                    {
                                        enrolled.IsOffered = false;
                                    }

                                    if (enrolled.GradingOption == GradingOption.NONE)
                                    {
                                        if (userLevel.Substring(0, 3).Contains("SSS"))
                                        {
                                            enrolled.GradingOption = GradingOption.SSS;
                                        }
                                        else if (userLevel.Substring(0, 3).Contains("JSS"))
                                        {
                                            enrolled.GradingOption = GradingOption.JSS;
                                        }
                                        else if (userLevel.Substring(0, 3).Contains("NUR"))
                                        {
                                            enrolled.GradingOption = GradingOption.NUR;
                                        }
                                        else if (userLevel.Substring(0, 3).Contains("PRI"))
                                        {
                                            enrolled.GradingOption = GradingOption.PRI;
                                        }
                                        else if (userLevel.Substring(0, 3).Contains("PRE"))
                                        {
                                            enrolled.GradingOption = GradingOption.PRE;
                                        }
                                        else if (userLevel.Substring(0, 3).Contains("PG"))
                                        {
                                            enrolled.GradingOption = GradingOption.PG;
                                        }
                                    }

                                    await _enrolledSubjectService.Edit(enrolled);
                                    await _resultService.UpdateResult(enrolled.EnrollmentId);
                                }
                            }
                            catch (Exception e)
                            {

                            }




                        }
                    }
                    TempData["msg"] = "Upload successfull Scroll down to review.";
                    TempData["error"] = nameswitheerror;
                }
                else
                {
                    TempData["error"] = "file type incorrect";
                }
                var classinfo = await db.Subjects.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == subid);
                //int subId = 0, Int32 sessionId = 0, int classListId = 0

                //return RedirectToAction("PreviewResult", new { subId = subId, sessionId = sessionId });
                return RedirectToAction("Offline", new { subId = subid, sessionId = sessid, classListId = classinfo.ClassLevelId });

            }
        }

        public async Task<ActionResult> Import2(int? subId, int? sessionId, HttpPostedFileBase excelfile)
        {
            int? subid = 0;
            int? sessid = 0;
            string path = "";
            if (excelfile == null || excelfile.ContentLength == 0)
            {
                TempData["error"] = "Please select a excel file";
                return RedirectToAction("Offline");

            }
            else
            {
                if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx"))
                {
                    System.Random randomInteger = new System.Random();
                    int genNumber = randomInteger.Next(100000);
                    path = Server.MapPath("~/ExcelUpload/" + genNumber + excelfile.FileName);
                    //if (System.IO.File.Exists(path))
                    //    System.IO.File.Delete(path);
                    string nameswitheerror = "";
                    int sn = 0;

                    excelfile.SaveAs(path);

                    using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(path, false))
                    {
                        WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                        WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                        SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                        int rowCount = sheetData.Elements<Row>().Skip(2).Count();


                        foreach (Row r in sheetData.Descendants<Row>().Skip(2))
                        {

                            try
                            {
                                var enroId = r.ChildElements[2].InnerText;
                                var project = r.ChildElements[4].InnerText;
                                var exercise = r.ChildElements[5].InnerText;
                                var Test = r.ChildElements[6].InnerText;
                                var Test2 = r.ChildElements[7].InnerText;
                                var assesment = r.ChildElements[8].InnerText;
                                var Exam = r.ChildElements[9].InnerText;


                                int enrolSubId = Convert.ToInt32(enroId);
                                int examscore = Convert.ToInt32(Exam);
                                int testscore = Convert.ToInt32(Test);
                                int project1 = Convert.ToInt32(project);
                                int exercise1 = Convert.ToInt32(exercise);
                                int Test21 = Convert.ToInt32(Test2);
                                int assesment1 = Convert.ToInt32(assesment);

                                var enrolled = await db.EnrolledSubjects.AsNoTracking().FirstOrDefaultAsync(x => x.Id == enrolSubId);
                                var enrollement = await db.Enrollments.Include(x => x.User).Include(x => x.StudentProfile).Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == enrolled.EnrollmentId);
                                var cid = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == enrollement.ClassLevelId);
                                var subidinfo = await db.EnrolledSubjects.Include(x => x.Subject).Include(x => x.Enrollments.Session).FirstOrDefaultAsync(x => x.Id == enrolSubId);
                                subid = subidinfo.SubjectId;

                                sessid = subidinfo.Enrollments.Session.Id;
                                var setting = cid;

                                //decimal? totalCA = project1 + exercise1 + testscore + Test21 + assesment1;
                                //if (examscore > enrollement.ClassLevel.ExamScore || totalCA > enrollement.ClassLevel.AccessmentScore)
                                //{
                                var subjectCA = await db.Subjects.FirstOrDefaultAsync(x => x.Id == subidinfo.SubjectId);
                                var totalCA = project1 + exercise1 + testscore + Test21 + assesment1;
                                var totalSubCA = subjectCA.Project + subjectCA.ClassExercise + subjectCA.TestScore + subjectCA.TestScore2 + subjectCA.Assessment;

                                //if (examscore > enrollement.ClassLevel.ExamScore || testscore > enrollement.ClassLevel.AccessmentScore)
                                if (examscore > subjectCA.ExamScore || totalCA > totalSubCA)
                                {
                                    nameswitheerror = nameswitheerror + "(" + sn++ + ")" + enrollement.StudentProfile.StudentRegNumber + ", has exam " + examscore + " and C.A " + totalCA + " that are out of range /// <br/>";
                                    //TempData["error"] = "Please Some Scores are out of range";
                                    //  return RedirectToAction("Offline", new { subId = subid, sessionId = sessionId });
                                }
                                else
                                {
                                    var subject = await _resultService.GetSubjectByEnrolledSubId(subId);
                                    var classlevel = await _resultService.GetClassByClassId(subject.ClassLevelId);
                                    string userLevel = classlevel.ClassName;

                                    if (enrolled.Assessment == null)
                                    {
                                        enrolled.Assessment = 0;
                                    }

                                    if (enrolled.TestScore2 == null)
                                    {
                                        enrolled.TestScore2 = 0;
                                    }
                                    if (enrolled.ClassExercise == null)
                                    {
                                        enrolled.ClassExercise = 0;
                                    }
                                    if (enrolled.Project == null)
                                    {
                                        enrolled.Project = 0;
                                    }
                                    if (enrolled.TestScore == null)
                                    {
                                        enrolled.TestScore = 0;
                                    }

                                    if (enrolled.ExamScore == null)
                                    {
                                        enrolled.ExamScore = 0;
                                    }

                                    if (enrolled.TotalCA == null)
                                    {
                                        enrolled.TotalCA = 0;
                                    }

                                    if (enrolled.TotalScore == null)
                                    {
                                        enrolled.TotalScore = 0;
                                    }


                                    decimal? totalScore = examscore + totalCA;
                                    enrolled.TotalScore = totalScore;
                                    enrolled.TestScore = testscore;
                                    enrolled.ExamScore = examscore;
                                    enrolled.Project = project1;
                                    enrolled.ClassExercise = exercise1;
                                    enrolled.TestScore2 = Test21;
                                    enrolled.Assessment = assesment1;
                                    enrolled.TotalCA = totalCA;


                                    if (enrolled.TestScore != 0 || enrolled.ExamScore != 0)
                                    {
                                        enrolled.IsOffered = true;
                                    }
                                    else
                                    {
                                        enrolled.IsOffered = false;
                                    }

                                    if (enrolled.GradingOption == GradingOption.NONE)
                                    {
                                        if (userLevel.Substring(0, 3).Contains("SSS"))
                                        {
                                            enrolled.GradingOption = GradingOption.SSS;
                                        }
                                        else if (userLevel.Substring(0, 3).Contains("JSS"))
                                        {
                                            enrolled.GradingOption = GradingOption.JSS;
                                        }
                                        else if (userLevel.Substring(0, 3).Contains("NUR"))
                                        {
                                            enrolled.GradingOption = GradingOption.NUR;
                                        }
                                        else if (userLevel.Substring(0, 3).Contains("PRI"))
                                        {
                                            enrolled.GradingOption = GradingOption.PRI;
                                        }
                                        else if (userLevel.Substring(0, 3).Contains("PRE"))
                                        {
                                            enrolled.GradingOption = GradingOption.PRE;
                                        }
                                        else if (userLevel.Substring(0, 3).Contains("PG"))
                                        {
                                            enrolled.GradingOption = GradingOption.PG;
                                        }
                                    }

                                    await _enrolledSubjectService.Edit(enrolled);
                                    await _resultService.UpdateResult(enrolled.EnrollmentId);
                                }
                            }
                            catch (Exception e)
                            {

                            }

                        }
                    }
                    TempData["msg"] = "Upload successfull Scroll down to review.";
                    TempData["error"] = nameswitheerror;
                }
                else
                {
                    TempData["error"] = "file type incorrect";
                }
                var classinfo = await db.Subjects.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == subid);
                //int subId = 0, Int32 sessionId = 0, int classListId = 0

                //return RedirectToAction("PreviewResult", new { subId = subId, sessionId = sessionId });
                return RedirectToAction("Offline", new { subId = subid, sessionId = sessid, classListId = classinfo.ClassLevelId });

            }
        }
        public static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            string value = cell.CellValue.InnerXml;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            else
            {
                return value;
            }
        }
        //choose class by subject class and session

        #endregion
        #region

        public async Task<ActionResult> SubjectInClass()
        {
            var items = await _staffProfileService.SubjectsByStaff();
            return View(items);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> SubjectInClass(int a)
        {
            var items = await _staffProfileService.SubjectsByStaff();
            return View(items);
        }

        #endregion


        #region enter score

        public async Task<ActionResult> EnterScore(int subId = 0)
        {
            var uId = User.Identity.GetUserId();
            //var classlevel = await _classLevelService.ClassLevelList();
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var item = db.ClassLevels.Include(x => x.User).Where(x => x.ShowClass == true);
            var formTeacher = db.ClassLevels.Include(x => x.User).Where(x => x.UserId == uId);
            item = item.OrderBy(x => x.ClassName);

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
            ViewBag.ClassL = new SelectList(classlevel, "Id", "ClassLevelName");
            var sessionq = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var sessionId = sessionq.Id;
            var session = await _sessionService.GetInfo(sessionId);
            if (subId != 0)
            {
                //fetch students after selecting class and subject
                try
                {
                    var sub1 = db.Subjects.Include(x => x.User).FirstOrDefault(x => x.Id == subId);



                    //var setting = db.ClassLevels.FirstOrDefault(x => x.Id == sub1.ClassLevelId);
                    //TempData["setting"] = "<span> Note: Exam Score Total = " + setting.ExamScore + " and Assessment Score Total = " + setting.AccessmentScore + "</span>";

                    var subjectsetting = await db.Subjects.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == sub1.Id);
                    var totalSub = subjectsetting.Assessment + subjectsetting.ClassExercise + subjectsetting.Project + subjectsetting.TestScore2 + subjectsetting.TestScore;
                    var setting = db.Settings.FirstOrDefault();
                    TempData["setting"] = "<span> Note:</span><br>";
                    if (setting.EnableTestScore == true)
                    {
                        TempData["setting2"] = "<span>Test Score Total = " + subjectsetting.TestScore + "</span><br>";
                    }
                    if (setting.EnableTestScore2 == true)
                    {
                        TempData["setting1"] = "<span>2nd Test Score Total = " + subjectsetting.TestScore2 + "</span><br>";
                    }

                    if (setting.EnableProject == true)
                    {
                        TempData["setting3"] = "<span>Project Score Total = " + subjectsetting.Project + "</span><br>";
                    }
                    if (setting.EnableAssessment == true)
                    {
                        TempData["setting4"] = "<span>Class Exercise Score Total = " + subjectsetting.ClassExercise + "</span><br>";
                    }
                    if (setting.EnableClassExercise == true)
                    {
                        TempData["setting5"] = "<span>Assessment Score Total = " + subjectsetting.Assessment + "</span><br>";
                    }
                    if (setting.EnableExamScore == true)
                    {

                        TempData["setting6"] = "<span>Exam Score Total = " + subjectsetting.ExamScore + "</span><br>";
                    }
                    //TempData["setting6"] = "<span> Note: Exam Score Total = " + subjectsetting.ExamScore + " and Assessment Score Total = " + totalSub + "</span>";
                    var classinfo = await _subjectService.Get(subId);
                    if (classinfo == null)
                    {
                        return HttpNotFound();
                    }
                    var enrolledStudents = await _enrollmentService.EnrolledStudentBySessionClassId(sessionId, classinfo.ClassLevelId);
                    //Check if there is any student in that class
                    if (enrolledStudents.Count() == 0)
                    {
                        TempData["error"] = "Class Empty";//Page not found
                        return RedirectToAction("EnterScore");
                    }
                    var subname = await _subjectService.Get(subId);
                    var classLevel = await _classLevelService.Get(classinfo.ClassLevelId);
                    ViewBag.SubjectId = classinfo.Id;
                    ViewBag.Subject = subname.SubjectName;
                    ViewBag.subteacher = sub1.User.Surname + " " + sub1.User.FirstName + " " + sub1.User.OtherName;
                    ViewBag.Class = classLevel.ClassName;
                    ViewBag.ClassLevelId = classinfo.ClassLevelId;
                    ViewBag.Session = session.FullSession;
                    ViewBag.SessionId = session.Id;
                    //check if result has been published
                    var publish = await db.PublishResults.FirstOrDefaultAsync(x => x.ClassLevelId == classLevel.Id && x.SessionId == sessionId);
                    if (publish == null)
                    {
                        ViewBag.booling = "false";
                    }
                    else
                    {
                        ViewBag.booling = "true";
                    }
                    var studentsEnrolled = await _enrollmentService.StudentsListBySubIdBySessionId(subId, sessionId);
                    var settings = await db.Settings.FirstOrDefaultAsync();
                    ViewBag.setting = settings;
                    foreach (var name in studentsEnrolled)
                    {
                        if (name.FullName.Substring(0, 2).Contains("\t"))
                        {
                            name.FullName = name.FullName.Remove(0, 2);
                        }
                        if (name.FullName.Substring(0, 2).Contains(" "))
                        {
                            name.FullName = name.FullName.TrimStart();
                        }
                    }
                    //UpdateComment recognitive domain
                    foreach (var abcd in studentsEnrolled)
                    {
                        var check = db.RecognitiveDomains.Where(x => x.EnrolmentId == abcd.EnrollmentId);
                        if (check == null)
                        {
                            RecognitiveDomain recognitive = new RecognitiveDomain();
                            recognitive.EnrolmentId = abcd.EnrollmentId;
                            db.RecognitiveDomains.Add(recognitive);
                            await db.SaveChangesAsync();
                        }
                    }
                    //UpdateComment psychomotor domain
                    foreach (var abcd in studentsEnrolled)
                    {
                        var check = db.PsychomotorDomains.Where(x => x.EnrolmentId == abcd.EnrollmentId);
                        if (check == null)
                        {
                            PsychomotorDomain psychomotor = new PsychomotorDomain();
                            psychomotor.EnrolmentId = abcd.EnrollmentId;
                            db.PsychomotorDomains.Add(psychomotor);
                            await db.SaveChangesAsync();
                        }
                    }
                    //UpdateComment affective domain
                    foreach (var abcd in studentsEnrolled)
                    {
                        var check = db.AffectiveDomains.Where(x => x.EnrolmentId == abcd.EnrollmentId);
                        if (check == null)
                        {
                            AffectiveDomain affective = new AffectiveDomain();
                            affective.EnrolmentId = abcd.EnrollmentId;
                            db.AffectiveDomains.Add(affective);
                            await db.SaveChangesAsync();
                        }
                    }

                    if (classinfo.ClassLevel.SortByOrder == true)
                    {
                        studentsEnrolled = studentsEnrolled.OrderBy(x => x.SortOrder).ToList();

                    }
                    else
                    {
                        studentsEnrolled = studentsEnrolled.OrderBy(x => x.FullName).ToList();
                    }
                    return View(studentsEnrolled);
                }
                catch (Exception e)
                {
                    TempData["error"] = "Unable to fetch Students";
                }
            }
            // var classlevel = await _classLevelService.ClassLevelList();
            ViewBag.ClassL = new SelectList(classlevel, "Id", "ClassLevelName");
            return View();
        }


        public async Task<ActionResult> EnterScore2(int subId = 0)
        {
            var uId = User.Identity.GetUserId();
            //var classlevel = await _classLevelService.ClassLevelList();
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var item = db.ClassLevels.Include(x => x.User).Where(x => x.ShowClass == true);
            var formTeacher = db.ClassLevels.Include(x => x.User).Where(x => x.UserId == uId);
            item = item.OrderBy(x => x.ClassName);

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
            ViewBag.ClassL = new SelectList(classlevel, "Id", "ClassLevelName");
            var sessionq = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var sessionId = sessionq.Id;
            var session = await _sessionService.GetInfo(sessionId);
            if (subId != 0)
            {
                //fetch students after selecting class and subject
                try
                {
                    var sub1 = db.Subjects.Include(x => x.User).FirstOrDefault(x => x.Id == subId);



                    //var setting = db.ClassLevels.FirstOrDefault(x => x.Id == sub1.ClassLevelId);
                    //TempData["setting"] = "<span> Note: Exam Score Total = " + setting.ExamScore + " and Assessment Score Total = " + setting.AccessmentScore + "</span>";

                    var subjectsetting = await db.Subjects.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == sub1.Id);
                    var totalSub = subjectsetting.Assessment + subjectsetting.ClassExercise + subjectsetting.Project + subjectsetting.TestScore2 + subjectsetting.TestScore;
                    var setting = db.Settings.FirstOrDefault();
                    TempData["setting"] = "<span> Note:</span><br>";
                    if (setting.EnableTestScore == true)
                    {
                        TempData["setting2"] = "<span>Test Score Total = " + subjectsetting.TestScore + "</span><br>";
                    }
                    if (setting.EnableTestScore2 == true)
                    {
                        TempData["setting1"] = "<span>2nd Test Score Total = " + subjectsetting.TestScore2 + "</span><br>";
                    }

                    if (setting.EnableProject == true)
                    {
                        TempData["setting3"] = "<span>Project Score Total = " + subjectsetting.Project + "</span><br>";
                    }
                    if (setting.EnableAssessment == true)
                    {
                        TempData["setting4"] = "<span>Class Exercise Score Total = " + subjectsetting.ClassExercise + "</span><br>";
                    }
                    if (setting.EnableClassExercise == true)
                    {
                        TempData["setting5"] = "<span>Assessment Score Total = " + subjectsetting.Assessment + "</span><br>";
                    }
                    if (setting.EnableExamScore == true)
                    {

                        TempData["setting6"] = "<span>Exam Score Total = " + subjectsetting.ExamScore + "</span><br>";
                    }
                    //TempData["setting"] = "<span> Note: Exam Score Total = " + subjectsetting.ExamScore + " and Assessment Score Total = " + totalSub + "</span>";
                    var classinfo = await _subjectService.Get(subId);
                    if (classinfo == null)
                    {
                        return HttpNotFound();
                    }
                    var enrolledStudents = await _enrollmentService.EnrolledStudentBySessionClassId(sessionId, classinfo.ClassLevelId);
                    //Check if there is any student in that class
                    if (enrolledStudents.Count() == 0)
                    {
                        TempData["error"] = "Class Empty";//Page not found
                        return RedirectToAction("EnterScore2");
                    }
                    var subname = await _subjectService.Get(subId);
                    var classLevel = await _classLevelService.Get(classinfo.ClassLevelId);
                    ViewBag.SubjectId = classinfo.Id;
                    ViewBag.Subject = subname.SubjectName;
                    ViewBag.subteacher = sub1.User.Surname + " " + sub1.User.FirstName + " " + sub1.User.OtherName;
                    ViewBag.Class = classLevel.ClassName;
                    ViewBag.ClassLevelId = classinfo.ClassLevelId;
                    ViewBag.Session = session.FullSession;
                    ViewBag.SessionId = session.Id;
                    //check if result has been published
                    var publish = await db.PublishResults.FirstOrDefaultAsync(x => x.ClassLevelId == classLevel.Id && x.SessionId == sessionId);
                    if (publish == null)
                    {
                        ViewBag.booling = "false";
                    }
                    else
                    {
                        ViewBag.booling = "true";
                    }
                    var studentsEnrolled = await _enrollmentService.StudentsListBySubIdBySessionId(subId, sessionId);
                    var settings = await db.Settings.FirstOrDefaultAsync();
                    ViewBag.setting = settings;
                    foreach (var name in studentsEnrolled)
                    {
                        if (name.FullName.Substring(0, 2).Contains("\t"))
                        {
                            name.FullName = name.FullName.Remove(0, 2);
                        }
                        if (name.FullName.Substring(0, 2).Contains(" "))
                        {
                            name.FullName = name.FullName.TrimStart();
                        }
                    }
                    //UpdateComment recognitive domain
                    foreach (var abcd in studentsEnrolled)
                    {
                        var check = db.RecognitiveDomains.Where(x => x.EnrolmentId == abcd.EnrollmentId);
                        if (check == null)
                        {
                            RecognitiveDomain recognitive = new RecognitiveDomain();
                            recognitive.EnrolmentId = abcd.EnrollmentId;
                            db.RecognitiveDomains.Add(recognitive);
                            await db.SaveChangesAsync();
                        }
                    }
                    //UpdateComment psychomotor domain
                    foreach (var abcd in studentsEnrolled)
                    {
                        var check = db.PsychomotorDomains.Where(x => x.EnrolmentId == abcd.EnrollmentId);
                        if (check == null)
                        {
                            PsychomotorDomain psychomotor = new PsychomotorDomain();
                            psychomotor.EnrolmentId = abcd.EnrollmentId;
                            db.PsychomotorDomains.Add(psychomotor);
                            await db.SaveChangesAsync();
                        }
                    }
                    //UpdateComment affective domain
                    foreach (var abcd in studentsEnrolled)
                    {
                        var check = db.AffectiveDomains.Where(x => x.EnrolmentId == abcd.EnrollmentId);
                        if (check == null)
                        {
                            AffectiveDomain affective = new AffectiveDomain();
                            affective.EnrolmentId = abcd.EnrollmentId;
                            db.AffectiveDomains.Add(affective);
                            await db.SaveChangesAsync();
                        }
                    }

                    if (classinfo.ClassLevel.SortByOrder == true)
                    {
                        studentsEnrolled = studentsEnrolled.OrderBy(x => x.SortOrder).ToList();

                    }
                    else
                    {
                        studentsEnrolled = studentsEnrolled.OrderBy(x => x.FullName).ToList();
                    }
                    return View(studentsEnrolled);
                }
                catch (Exception e)
                {
                    TempData["error"] = "Unable to fetch Students";
                }
            }
            // var classlevel = await _classLevelService.ClassLevelList();
            ViewBag.ClassL = new SelectList(classlevel, "Id", "ClassLevelName");
            return View();
        }


        //print preview
        //score sheet with and without data

        public async Task<ActionResult> ScoreSheet(int subId = 0)
        {

            var classlevel = await _classLevelService.ClassLevelList();
            ViewBag.ClassL = new SelectList(classlevel, "Id", "ClassLevelName");

            var sessionq = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var sessionId = sessionq.Id;
            var session = await _sessionService.GetInfo(sessionId);
            if (subId != 0)
            {
                ///fetch students after selecting class and subject
                ///
                try
                {
                    var sub1 = db.Subjects.Include(x => x.User).FirstOrDefault(x => x.Id == subId);
                    var setting = db.ClassLevels.FirstOrDefault(x => x.Id == sub1.ClassLevelId);
                    var gsetting = await db.Settings.FirstOrDefaultAsync();
                    var totalCA = sub1.Assessment + sub1.ClassExercise + sub1.Project + sub1.TestScore2 + sub1.TestScore;
                    TempData["setting"] = "<span> Note:</span><br>";
                    if (gsetting.EnableTestScore == true)
                    {
                        TempData["setting2"] = "<span>Test Score Total = " + sub1.TestScore + "</span><br>";
                    }
                    if (gsetting.EnableTestScore2 == true)
                    {
                        TempData["setting1"] = "<span>2nd Test Score Total = " + sub1.TestScore2 + "</span><br>";
                    }

                    if (gsetting.EnableProject == true)
                    {
                        TempData["setting3"] = "<span>Project Score Total = " + sub1.Project + "</span><br>";
                    }
                    if (gsetting.EnableAssessment == true)
                    {
                        TempData["setting4"] = "<span>Class Exercise Score Total = " + sub1.ClassExercise + "</span><br>";
                    }
                    if (gsetting.EnableClassExercise == true)
                    {
                        TempData["setting5"] = "<span>Assessment Score Total = " + sub1.Assessment + "</span><br>";
                    }
                    if (gsetting.EnableExamScore == true)
                    {

                        TempData["setting6"] = "<span>Exam Score Total = " + sub1.ExamScore + "</span><br>";
                    }
                    //TempData["setting"] = "<span style=\"font-size:18px;font-weight:800;\">Note: Exam Score Total = " + setting.ExamScore + " and Assessment Score Total = " + totalCA + "</span>";
                    var classinfo = await _subjectService.Get(subId);
                    if (classinfo == null)
                    {
                        return HttpNotFound();
                    }
                    var enrolledStudents = await _enrollmentService.EnrolledStudentBySessionClassId(sessionId, classinfo.ClassLevelId);
                    //Check if there is any student in that class
                    if (enrolledStudents.Count() == 0)
                    {
                        TempData["error"] = "Class Empty";//Page not found
                        if (gsetting.PrintOutOption == PrintOutOption.PrintOutThree)
                        {
                            return RedirectToAction("EnterScore2");
                        }
                        else
                        {
                            return RedirectToAction("EnterScore");
                        }

                    }
                    var subname = await _subjectService.Get(subId);
                    var classLevel = await _classLevelService.Get(classinfo.ClassLevelId);

                    ViewBag.subteacher = sub1.User.Surname + " " + sub1.User.FirstName + " " + sub1.User.OtherName;
                    ViewBag.SubjectId = classinfo.Id;
                    ViewBag.Subject = subname.SubjectName;

                    ViewBag.Class = classLevel.ClassName;
                    ViewBag.ClassLevelId = classinfo.ClassLevelId;
                    ViewBag.Session = session.FullSession;
                    ViewBag.SessionId = session.Id;

                    var studentsEnrolled = await _enrollmentService.StudentsListBySubIdBySessionId(subId, sessionId);
                    var settings = await db.Settings.FirstOrDefaultAsync();
                    ViewBag.setting = settings;
                    foreach (var name in studentsEnrolled)
                    {
                        if (name.FullName.Substring(0, 2).Contains("\t"))
                        {
                            name.FullName = name.FullName.Remove(0, 2);
                        }
                        if (name.FullName.Substring(0, 2).Contains(" "))
                        {
                            name.FullName = name.FullName.TrimStart();
                        }
                    }

                    studentsEnrolled = studentsEnrolled.OrderBy(x => x.FullName).ToList();

                    return View(studentsEnrolled);
                }
                catch (Exception e)
                {
                    TempData["error"] = "Unable to fetch Students";
                }
            }

            // var classlevel = await _classLevelService.ClassLevelList();
            ViewBag.ClassL = new SelectList(classlevel, "Id", "ClassLevelName");

            return View();
        }

        public ActionResult EmptyScoreSheet()
        {
            Response.ContentType = "Application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=ESP Scoresheet.pdf");
            Response.TransmitFile(Server.MapPath("~/Areas/Service/Scoresheet Pdf/ESP Scoresheet.pdf"));
            Response.End();
            return RedirectToAction("ScoreSheet");
        }
        #endregion


        #region sorting by sorting
        public async Task<ActionResult> AddSort(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = db.EnrolledSubjects.Include(x => x.Subject).Include(x => x.Enrollments.StudentProfile.user).Include(x => x.Enrollments.StudentProfile).FirstOrDefault(x => x.Id == id);

            return View(item);
        }

        // POST: Content/Assignments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> AddSort(int id, int SortOrder)
        {
            var data = db.EnrolledSubjects.FirstOrDefault(x => x.Id == id);
            data.SortOrder = SortOrder;
            db.Entry(data).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("AddSort");
        }


        #endregion


        #region live video

        public async Task<ActionResult> LiveClassList()
        {
            var setting = await db.Settings.FirstOrDefaultAsync();

            var model = await db.OnlineZooms.Include(x => x.Session).Include(x => x.ClassLevel).Include(x => x.Subject).Include(x => x.User).ToListAsync();
            string uid = User.Identity.GetUserId();
            if (User.IsInRole("SuperAdmin"))
            {

                return View(model);
            }
            if (User.IsInRole("Admin"))
            {

                return View(model);
            }
            model = model.Where(x => x.UserId == uid).ToList();

            return View(model);
        }


        public ActionResult LiveClassType()
        {
            return View();
        }
        public async Task<ActionResult> SingleLiveClass()
        {
            if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
            {
                var staff = await _staffProfileService.StaffDropdownList();
                ViewBag.User = new SelectList(staff, "UserId", "FullName");
            }
            ViewBag.Classlist = new SelectList(db.ClassLevels, "Id", "ClassName");

            return View();
        }

        public async Task<ActionResult> MultipleLiveClass()
        {
            if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
            {
                var staff = await _staffProfileService.StaffDropdownList();
                ViewBag.User = new SelectList(staff, "UserId", "FullName");
            }
            ViewBag.Classlist = new SelectList(db.ClassLevels, "Id", "ClassName");

            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> NewLiveClass(OnlineZoomDto zmodel, string multy)
        {
            long iddetail = 0;
            string result1 = "";
            string result2 = "";
            string result3 = "";
            string eresult1 = "";
            string eresult2 = "";
            string eresult3 = "";
            if (ModelState.IsValid)
            {

                if (multy == "multy")
                {
                    //for class 1

                    try
                    {
                        OnlineZoom model = new OnlineZoom();
                        var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
                        var sett = db.Settings.FirstOrDefault();
                        model.SessionId = currentSession.Id;
                        model.DateCreated = DateTime.UtcNow.AddHours(1);
                        model.HostEmail = sett.ZoomHostOne;
                        model.UserId = zmodel.UserId1;
                        model.ClassDate = zmodel.ClassDate;
                        model.ClassTime = zmodel.ClassTime;
                        model.Duration = zmodel.Duration1;
                        model.ClassLevelId = zmodel.ClassLevelId1;
                        model.SubjectId = zmodel.SubjectId1;
                        model.Description = zmodel.Description1;
                        model.ClassPassword = zmodel.ClassPassword1;

                        db.OnlineZooms.Add(model);
                        await db.SaveChangesAsync();

                        //end create on local

                        //create zoom online
                        var subupdate = await db.Subjects.FirstOrDefaultAsync(x => x.Id == model.SubjectId);
                        RootobjectNewMeeting datamodel = new RootobjectNewMeeting();
                        datamodel.topic = sett.SchoolName + " Live Class on " + subupdate.SubjectName;
                        datamodel.type = model.MeetingType;
                        DateTime oDate = Convert.ToDateTime(model.ClassDate);
                        DateTime oTime = Convert.ToDateTime(model.ClassTime);
                        // DateTime DateValueConvert = DateTime.ParseExact(oDate.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);

                        datamodel.start_time = oDate.ToString("yyyy-MM-dd") + "T" + oTime.ToString("HH:mm:ss") + "Z";
                        datamodel.duration = model.Duration;
                        datamodel.password = model.ClassPassword;
                        datamodel.agenda = model.Description;
                        var content = TokenManager.NewMeeting(datamodel, sett.ZoomHostOne);
                        RootobjectDetails data = JsonConvert.DeserializeObject<RootobjectDetails>(content);
                        //end
                        iddetail = data.id;
                        //UPDAte local
                        var classmodel = await db.OnlineZooms.Include(x => x.Subject).Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == model.Id);
                        classmodel.MeetingId = data.id;
                        classmodel.MeetingUUId = data.uuid;
                        db.Entry(classmodel).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        result1 = "Successfully created live class on " + classmodel.Subject.SubjectName + " for " + classmodel.ClassLevel.ClassName;
                        string mass = "";
                        try
                        {

                            MailMessage mail = new MailMessage();

                            //set the addresses 
                            mail.From = new MailAddress("learnonline@iskools.com"); //IMPORTANT: This must be same as your smtp authentication address.
                            mail.To.Add("espErrorMail@exwhyzee.ng");
                            mail.To.Add("iskoolsportal@gmail.com");
                            mail.To.Add("onwukaemeka41@gmail.com");


                            //set the content 

                            mail.Subject = " Live Class Request from " + sett.SchoolName;

                            mass = sett.SchoolName + " - " + sett.PortalLink + "/Staff/Panel/DetailsLiveClass/" + data.id + " - visit for more info";

                            mail.Body = mass;
                            //send the message 
                            SmtpClient smtp = new SmtpClient("mail.iskools.com");

                            //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
                            NetworkCredential Credentials = new NetworkCredential("learnonline@iskools.com", "Exwhyzee@123");
                            smtp.Credentials = Credentials;
                            smtp.Send(mail);

                        }
                        catch (Exception ex)
                        {


                        }

                        try
                        {
                            string urlString = "http://xyzsms.com/api/ApiXyzSms/ComposeMessage?username=onwuka1&password=nation&recipients=08165680904&senderId=ISKOOLS&smsmessage=" + mass + "&smssendoption=SendNow";
                            //  string urlString = "http://www.xyzsms.com/components/com_spc/smsapi.php?username=" + senderUserName + "&password=" + senderPassword + "&sender=" + senderId + "&recipient=" + recipient + "&message=" + message;
                            string response = "";
                            try
                            {
                                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(urlString);
                                httpWebRequest.Method = "GET";
                                httpWebRequest.ContentType = "application/json";

                                //getting the respounce from the request
                                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                Stream responseStream = httpWebResponse.GetResponseStream();
                                StreamReader streamReader = new StreamReader(responseStream);
                                response = streamReader.ReadToEnd();
                            }
                            catch (Exception d)
                            {

                            }
                        }
                        catch (Exception c) { }
                    }
                    catch (Exception c)
                    {
                        eresult1 = "first class creation fail. try using single method to recreate a new one";
                    }

                    //for class 2

                    try
                    {
                        OnlineZoom model = new OnlineZoom();
                        var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
                        var sett = db.Settings.FirstOrDefault();
                        model.SessionId = currentSession.Id;
                        model.DateCreated = DateTime.UtcNow.AddHours(1);
                        model.HostEmail = sett.ZoomHostTwo;
                        model.UserId = zmodel.UserId2;
                        model.ClassDate = zmodel.ClassDate;
                        model.ClassTime = zmodel.ClassTime;
                        model.Duration = zmodel.Duration2;
                        model.ClassLevelId = zmodel.ClassLevelId2;
                        model.SubjectId = zmodel.SubjectId2;
                        model.Description = zmodel.Description2;
                        model.ClassPassword = zmodel.ClassPassword2;

                        db.OnlineZooms.Add(model);
                        await db.SaveChangesAsync();

                        //end create on local

                        //create zoom online
                        var subupdate = await db.Subjects.FirstOrDefaultAsync(x => x.Id == model.SubjectId);
                        RootobjectNewMeeting datamodel = new RootobjectNewMeeting();
                        datamodel.topic = sett.SchoolName + " Live Class on " + subupdate.SubjectName;
                        datamodel.type = model.MeetingType;
                        DateTime oDate = Convert.ToDateTime(model.ClassDate);
                        DateTime oTime = Convert.ToDateTime(model.ClassTime);
                        // DateTime DateValueConvert = DateTime.ParseExact(oDate.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);

                        datamodel.start_time = oDate.ToString("yyyy-MM-dd") + "T" + oTime.ToString("HH:mm:ss") + "Z";
                        datamodel.duration = model.Duration;
                        datamodel.password = model.ClassPassword;
                        datamodel.agenda = model.Description;
                        var content = TokenManager.NewMeeting(datamodel, sett.ZoomHostTwo);
                        RootobjectDetails data = JsonConvert.DeserializeObject<RootobjectDetails>(content);
                        //end
                        iddetail = data.id;
                        //UPDAte local
                        var classmodel = await db.OnlineZooms.Include(x => x.Subject).Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == model.Id);
                        classmodel.MeetingId = data.id;
                        classmodel.MeetingUUId = data.uuid;
                        db.Entry(classmodel).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        result2 = "Successfully created live class on " + classmodel.Subject.SubjectName + " for " + classmodel.ClassLevel.ClassName;
                        string mass = "";
                        try
                        {

                            MailMessage mail = new MailMessage();

                            //set the addresses 
                            mail.From = new MailAddress("learnonline@iskools.com"); //IMPORTANT: This must be same as your smtp authentication address.
                            mail.To.Add("espErrorMail@exwhyzee.ng");
                            mail.To.Add("iskoolsportal@gmail.com");
                            mail.To.Add("onwukaemeka41@gmail.com");


                            //set the content 

                            mail.Subject = " Live Class Request from " + sett.SchoolName;

                            mass = sett.SchoolName + " - " + sett.PortalLink + "/Staff/Panel/DetailsLiveClass/" + data.id + " - visit for more info";

                            mail.Body = mass;
                            //send the message 
                            SmtpClient smtp = new SmtpClient("mail.iskools.com");

                            //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
                            NetworkCredential Credentials = new NetworkCredential("learnonline@iskools.com", "Exwhyzee@123");
                            smtp.Credentials = Credentials;
                            smtp.Send(mail);

                        }
                        catch (Exception ex)
                        {


                        }

                        try
                        {
                            string urlString = "http://xyzsms.com/api/ApiXyzSms/ComposeMessage?username=onwuka1&password=nation&recipients=08165680904&senderId=ISKOOLS&smsmessage=" + mass + "&smssendoption=SendNow";
                            //  string urlString = "http://www.xyzsms.com/components/com_spc/smsapi.php?username=" + senderUserName + "&password=" + senderPassword + "&sender=" + senderId + "&recipient=" + recipient + "&message=" + message;
                            string response = "";
                            try
                            {
                                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(urlString);
                                httpWebRequest.Method = "GET";
                                httpWebRequest.ContentType = "application/json";

                                //getting the respounce from the request
                                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                Stream responseStream = httpWebResponse.GetResponseStream();
                                StreamReader streamReader = new StreamReader(responseStream);
                                response = streamReader.ReadToEnd();
                            }
                            catch (Exception d)
                            {

                            }
                        }
                        catch (Exception c) { }
                    }
                    catch (Exception c)
                    {
                        eresult2 = "second class creation fail. try using single method to recreate a new one";

                    }

                    //for class 3

                    try
                    {
                        OnlineZoom model = new OnlineZoom();
                        var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
                        var sett = db.Settings.FirstOrDefault();
                        model.SessionId = currentSession.Id;
                        model.DateCreated = DateTime.UtcNow.AddHours(1);
                        model.HostEmail = sett.ZoomHostThree;
                        model.UserId = zmodel.UserId3;
                        model.ClassDate = zmodel.ClassDate;
                        model.ClassTime = zmodel.ClassTime;
                        model.Duration = zmodel.Duration3;
                        model.ClassLevelId = zmodel.ClassLevelId3;
                        model.SubjectId = zmodel.SubjectId3;
                        model.Description = zmodel.Description3;
                        model.ClassPassword = zmodel.ClassPassword3;

                        db.OnlineZooms.Add(model);
                        await db.SaveChangesAsync();

                        //end create on local

                        //create zoom online
                        var subupdate = await db.Subjects.FirstOrDefaultAsync(x => x.Id == model.SubjectId);
                        RootobjectNewMeeting datamodel = new RootobjectNewMeeting();
                        datamodel.topic = sett.SchoolName + " Live Class on " + subupdate.SubjectName;
                        datamodel.type = model.MeetingType;
                        DateTime oDate = Convert.ToDateTime(model.ClassDate);
                        DateTime oTime = Convert.ToDateTime(model.ClassTime);
                        // DateTime DateValueConvert = DateTime.ParseExact(oDate.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);

                        datamodel.start_time = oDate.ToString("yyyy-MM-dd") + "T" + oTime.ToString("HH:mm:ss") + "Z";
                        datamodel.duration = model.Duration;
                        datamodel.password = model.ClassPassword;
                        datamodel.agenda = model.Description;
                        var content = TokenManager.NewMeeting(datamodel, sett.ZoomHostThree);
                        RootobjectDetails data = JsonConvert.DeserializeObject<RootobjectDetails>(content);
                        //end
                        iddetail = data.id;
                        //UPDAte local
                        var classmodel = await db.OnlineZooms.Include(x => x.Subject).Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.Id == model.Id);
                        classmodel.MeetingId = data.id;
                        classmodel.MeetingUUId = data.uuid;
                        db.Entry(classmodel).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        result3 = "Successfully created live class on " + classmodel.Subject.SubjectName + " for " + classmodel.ClassLevel.ClassName;
                        string mass = "";
                        try
                        {

                            MailMessage mail = new MailMessage();

                            //set the addresses 
                            mail.From = new MailAddress("learnonline@iskools.com"); //IMPORTANT: This must be same as your smtp authentication address.
                            mail.To.Add("espErrorMail@exwhyzee.ng");
                            mail.To.Add("iskoolsportal@gmail.com");
                            mail.To.Add("onwukaemeka41@gmail.com");


                            //set the content 

                            mail.Subject = " Live Class Request from " + sett.SchoolName;

                            mass = sett.SchoolName + " - " + sett.PortalLink + "/Staff/Panel/DetailsLiveClass/" + data.id + " - visit for more info";

                            mail.Body = mass;
                            //send the message 
                            SmtpClient smtp = new SmtpClient("mail.iskools.com");

                            //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
                            NetworkCredential Credentials = new NetworkCredential("learnonline@iskools.com", "Exwhyzee@123");
                            smtp.Credentials = Credentials;
                            smtp.Send(mail);

                        }
                        catch (Exception ex)
                        {


                        }

                        try
                        {
                            string urlString = "http://xyzsms.com/api/ApiXyzSms/ComposeMessage?username=onwuka1&password=nation&recipients=08165680904&senderId=ISKOOLS&smsmessage=" + mass + "&smssendoption=SendNow";
                            //  string urlString = "http://www.xyzsms.com/components/com_spc/smsapi.php?username=" + senderUserName + "&password=" + senderPassword + "&sender=" + senderId + "&recipient=" + recipient + "&message=" + message;
                            string response = "";
                            try
                            {
                                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(urlString);
                                httpWebRequest.Method = "GET";
                                httpWebRequest.ContentType = "application/json";

                                //getting the respounce from the request
                                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                Stream responseStream = httpWebResponse.GetResponseStream();
                                StreamReader streamReader = new StreamReader(responseStream);
                                response = streamReader.ReadToEnd();
                            }
                            catch (Exception d)
                            {

                            }
                        }
                        catch (Exception c) { }
                    }
                    catch (Exception c)
                    {
                        eresult3 = "third class creation fail. try using single method to recreate a new one";

                    }
                }
                else
                {


                    //create zoom on local
                    //
                    try
                    {
                        OnlineZoom model = new OnlineZoom();
                        var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
                        var sett = db.Settings.FirstOrDefault();
                        model.SessionId = currentSession.Id;
                        model.DateCreated = DateTime.UtcNow.AddHours(1);
                        model.HostEmail = sett.ZoomHostOne;
                        model.UserId = zmodel.UserId1;
                        model.ClassDate = zmodel.ClassDate;
                        model.ClassTime = zmodel.ClassTime;
                        model.Duration = zmodel.Duration1;
                        model.ClassLevelId = zmodel.ClassLevelId1;
                        model.SubjectId = zmodel.SubjectId1;
                        model.Description = zmodel.Description1;
                        model.ClassPassword = zmodel.ClassPassword1;

                        db.OnlineZooms.Add(model);
                        await db.SaveChangesAsync();

                        //end create on local

                        //create zoom online
                        var subupdate = await db.Subjects.FirstOrDefaultAsync(x => x.Id == model.SubjectId);
                        RootobjectNewMeeting datamodel = new RootobjectNewMeeting();
                        datamodel.topic = sett.SchoolName + " Live Class on " + subupdate.SubjectName;
                        datamodel.type = model.MeetingType;
                        DateTime oDate = Convert.ToDateTime(model.ClassDate);
                        DateTime oTime = Convert.ToDateTime(model.ClassTime);
                        // DateTime DateValueConvert = DateTime.ParseExact(oDate.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);

                        datamodel.start_time = oDate.ToString("yyyy-MM-dd") + "T" + oTime.ToString("HH:mm:ss") + "Z";
                        datamodel.duration = model.Duration;
                        datamodel.password = model.ClassPassword;
                        datamodel.agenda = model.Description;
                        var content = TokenManager.NewMeeting(datamodel, sett.ZoomHostOne);
                        RootobjectDetails data = JsonConvert.DeserializeObject<RootobjectDetails>(content);
                        //end
                        iddetail = data.id;
                        //UPDAte local
                        var classmodel = await db.OnlineZooms.FirstOrDefaultAsync(x => x.Id == model.Id);
                        classmodel.MeetingId = data.id;
                        classmodel.MeetingUUId = data.uuid;
                        db.Entry(classmodel).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        TempData["success"] = "Successfully created.";
                        string mass = "";
                        try
                        {

                            MailMessage mail = new MailMessage();

                            //set the addresses 
                            mail.From = new MailAddress("learnonline@iskools.com"); //IMPORTANT: This must be same as your smtp authentication address.
                            mail.To.Add("espErrorMail@exwhyzee.ng");
                            mail.To.Add("iskoolsportal@gmail.com");
                            mail.To.Add("onwukaemeka41@gmail.com");


                            //set the content 

                            mail.Subject = " Live Class Request from " + sett.SchoolName;

                            mass = sett.SchoolName + " - " + sett.PortalLink + "/Staff/Panel/DetailsLiveClass/" + data.id + " - visit for more info";

                            mail.Body = mass;
                            //send the message 
                            SmtpClient smtp = new SmtpClient("mail.iskools.com");

                            //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
                            NetworkCredential Credentials = new NetworkCredential("learnonline@iskools.com", "Exwhyzee@123");
                            smtp.Credentials = Credentials;
                            smtp.Send(mail);

                        }
                        catch (Exception ex)
                        {


                        }

                        try
                        {
                            string urlString = "http://xyzsms.com/api/ApiXyzSms/ComposeMessage?username=onwuka1&password=nation&recipients=08165680904&senderId=ISKOOLS&smsmessage=" + mass + "&smssendoption=SendNow";
                            //  string urlString = "http://www.xyzsms.com/components/com_spc/smsapi.php?username=" + senderUserName + "&password=" + senderPassword + "&sender=" + senderId + "&recipient=" + recipient + "&message=" + message;
                            string response = "";
                            try
                            {
                                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(urlString);
                                httpWebRequest.Method = "GET";
                                httpWebRequest.ContentType = "application/json";

                                //getting the respounce from the request
                                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                Stream responseStream = httpWebResponse.GetResponseStream();
                                StreamReader streamReader = new StreamReader(responseStream);
                                response = streamReader.ReadToEnd();
                            }
                            catch (Exception d)
                            {

                            }
                        }
                        catch (Exception c) { }
                    }
                    catch (Exception c)
                    {

                    }

                    //




                }

                if (multy == "multy")
                {
                    TempData["result1"] = result1;
                    TempData["result2"] = result2;
                    TempData["result3"] = result3;

                    return RedirectToAction("LiveClassList");
                }
                else
                {
                    return RedirectToAction("DetailsLiveClass", new { id = iddetail });
                }
            }
            if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
            {
                var staff = await _staffProfileService.StaffDropdownList();
                ViewBag.User = new SelectList(staff, "UserId", "FullName");
            }
            ViewBag.Classlist = new SelectList(db.ClassLevels, "Id", "ClassName");
            if (multy == "multy")
            {

                TempData["eresult1"] = eresult1;
                TempData["eresult2"] = eresult2;
                TempData["eresult3"] = eresult3;
                return RedirectToAction("LiveClassList");
            }
            else
            {
                return View(zmodel);
            }

        }

        // GET: Content/Assignments/Edit/5
        public async Task<ActionResult> EditLiveClass(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = await db.LiveClassOnlines.Include(x => x.Session).Include(x => x.ClassLevel).Include(x => x.Subject).Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
            {
                var staff = await _staffProfileService.StaffDropdownList();
                ViewBag.User = new SelectList(staff, "UserId", "FullName", model.UserId);
            }
            ViewBag.Classlist = new SelectList(db.ClassLevels, "Id", "ClassName", model.ClassLevelId);

            return View(model);
        }

        // POST: Content/Assignments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditLiveClass(LiveClassOnline data, string LiveStatusString)
        {
            if (ModelState.IsValid)
            {
                var model = await db.LiveClassOnlines.FirstOrDefaultAsync(x => x.Id == data.Id);
                //if (LiveStatusString == "active")
                //{
                //    model.LiveStatus = LiveStatus.Active;
                //}
                //if (LiveStatusString == "waiting")
                //{
                //    model.LiveStatus = LiveStatus.Waiting;
                //}
                //if (LiveStatusString == "ended")
                //{
                //    model.LiveStatus = LiveStatus.Ended;
                //}
                model.ClassLevelId = data.ClassLevelId;
                if (data.SubjectId != null)
                {
                    model.SubjectId = data.SubjectId;
                }
                model.UrlLive = data.UrlLive;
                model.ClassDate = data.ClassDate;
                model.ClassTime = data.ClassTime;
                model.Duration = data.Duration;


                db.Entry(model).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("LiveClassList");
            }
            if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
            {
                var staff = await _staffProfileService.StaffDropdownList();
                ViewBag.User = new SelectList(staff, "UserId", "FullName", data.UserId);
            }
            ViewBag.Classlist = new SelectList(db.ClassLevels, "Id", "ClassName", data.ClassLevelId);

            return View(data);
        }


        public async Task<ActionResult> DetailsLiveClass(long id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {

                List<User> userdata = new List<User>();
                var content = TokenManager.GetAMeeting(id);

                //userdata = JsonConvert.DeserializeObject<List<ZoomUserDto>>(response.Content);
                RootobjectDetails data = JsonConvert.DeserializeObject<RootobjectDetails>(content);
                var prof = await db.OnlineZooms.Include(x => x.Session).Include(x => x.ClassLevel).Include(x => x.Subject).Include(x => x.User).FirstOrDefaultAsync(x => x.MeetingId == data.id);
                ViewBag.prof = prof;
                return View(data);
            }
            catch (Exception c)
            {

            }
            return View();
        }

        public async Task<ActionResult> LiveClassRecording(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {

                List<User> userdata = new List<User>();
                var content = TokenManager.GetAMeetingRecording(id);

                //userdata = JsonConvert.DeserializeObject<List<ZoomUserDto>>(response.Content);
                RootobjectMeetingRecord data = JsonConvert.DeserializeObject<RootobjectMeetingRecord>(content);
                var prof = await db.OnlineZooms.Include(x => x.Session).Include(x => x.ClassLevel).Include(x => x.Subject).Include(x => x.User).FirstOrDefaultAsync(x => x.MeetingId == data.id);
                ViewBag.prof = prof;
                return View(data.recording_files);
            }
            catch (Exception c)
            {

            }
            return View();
        }

        public async Task<ActionResult> LiveClassParticipant(long id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {

                List<User> userdata = new List<User>();
                var content = TokenManager.GetAMeetingParticipant(id);

                //userdata = JsonConvert.DeserializeObject<List<ZoomUserDto>>(response.Content);
                RootobjectParticipant data = JsonConvert.DeserializeObject<RootobjectParticipant>(content);
                var prof = await db.OnlineZooms.Include(x => x.Session).Include(x => x.ClassLevel).Include(x => x.Subject).Include(x => x.User).FirstOrDefaultAsync(x => x.MeetingId == id);
                ViewBag.prof = prof;
                return View(data.participants.ToList());
            }
            catch (Exception c)
            {

            }
            return View();
        }

        #endregion

        #region online file upload
        public async Task<ActionResult> NewOnLineCourse()
        {
            if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
            {
                var staff = await _staffProfileService.StaffDropdownList();
                ViewBag.User = new SelectList(staff, "UserId", "FullName");
            }
            ViewBag.ClassId = new SelectList(db.ClassLevels, "Id", "ClassName");

            return View();
        }

        // POST: Content/Assignments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> NewOnLineCourse(OnlineCourseUploadDto model, int? subId, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
                    OnlineCourseUpload course = new OnlineCourseUpload();
                    course.Topic = model.Topic;
                    course.ClassLevelId = model.ClassLevelId;
                    course.SubjectId = subId;
                    course.SessionId = currentSession.Id;
                    course.Description = model.Description;
                    course.Date = DateTime.UtcNow.AddHours(1);
                    course.UserId = model.UserId;
                    course.UploadType = model.UploadType;

                    if (upload != null)

                    {

                        System.Random randomInteger = new System.Random();
                        int genNumber = randomInteger.Next(1000);

                        if (upload.ContentLength > 0)

                        {

                            if (course.UploadType == UploadType.Audio || upload.ContentType.ToUpper().Contains("MP3"))
                            {

                                string fileName = Path.GetFileName(course.Topic + "_" + genNumber + "_" + upload.FileName);
                                course.Upload = "~/Aq_Image/Course/" + fileName;
                                fileName = Path.Combine(Server.MapPath("~/Aq_Image/Course/"), fileName);
                                upload.SaveAs(fileName);
                            }
                            else if (course.UploadType == UploadType.Excel || upload.ContentType.ToUpper().Contains("XLS") || upload.ContentType.ToUpper().Contains("XLSX"))
                            {

                                string fileName = Path.GetFileName(course.Topic + "_" + genNumber + "_" + upload.FileName);
                                course.Upload = "~/Aq_Image/Course/" + fileName;
                                fileName = Path.Combine(Server.MapPath("~/Aq_Image/Course/"), fileName);
                                upload.SaveAs(fileName);
                            }
                            else if (course.UploadType == UploadType.PDF || upload.ContentType.ToUpper().Contains("PDF"))
                            {

                                string fileName = Path.GetFileName(course.Topic + "_" + genNumber + "_" + upload.FileName);
                                course.Upload = "~/Aq_Image/Course/" + fileName;
                                fileName = Path.Combine(Server.MapPath("~/Aq_Image/Course/"), fileName);
                                upload.SaveAs(fileName);
                            }
                            else if (course.UploadType == UploadType.Powerpoint || upload.ContentType.ToUpper().Contains("PPT") || upload.ContentType.ToUpper().Contains("PPTX"))
                            {

                                string fileName = Path.GetFileName(course.Topic + "_" + genNumber + "_" + upload.FileName);
                                course.Upload = "~/Aq_Image/Course/" + fileName;
                                fileName = Path.Combine(Server.MapPath("~/Aq_Image/Course/"), fileName);
                                upload.SaveAs(fileName);
                            }
                            else if (course.UploadType == UploadType.Video || upload.ContentType.ToUpper().Contains("MP4") || upload.ContentType.ToUpper().Contains("3GP"))
                            {

                                string fileName = Path.GetFileName(course.Topic + "_" + genNumber + "_" + upload.FileName);
                                course.Upload = "~/Aq_Image/Course/" + fileName;
                                fileName = Path.Combine(Server.MapPath("~/Aq_Image/Course/"), fileName);
                                upload.SaveAs(fileName);
                            }
                            else if (course.UploadType == UploadType.Word || upload.ContentType.ToUpper().Contains("DOC") || upload.ContentType.ToUpper().Contains("DOCX"))
                            {

                                string fileName = Path.GetFileName(course.Topic + "_" + genNumber + "_" + upload.FileName);
                                course.Upload = "~/Aq_Image/Course/" + fileName;
                                fileName = Path.Combine(Server.MapPath("~/Aq_Image/Course/"), fileName);
                                upload.SaveAs(fileName);
                            }
                            else
                            {
                                TempData["error"] = "File Format is not supported";
                                return RedirectToAction("NewOnLineCourse");
                            }

                        }
                    }

                    db.OnlineCourseUpload.Add(course);
                    await db.SaveChangesAsync();

                    var coursechecck = db.OnlineCourseUpload.Include(x => x.ClassLevel).Include(x => x.Subject).FirstOrDefault(x => x.Id == course.Id).Id;

                    if (model.AssignmentTitle != null && course.Id == coursechecck)
                    {
                        Assignment assignment = new Assignment();
                        assignment.ClassLevelId = model.ClassLevelId;
                        assignment.SessionId = currentSession.Id;
                        assignment.SubjectId = subId;
                        assignment.Title = model.AssignmentTitle;
                        assignment.Description = model.AssignmentContent;
                        assignment.DateCreated = DateTime.UtcNow.AddHours(1);
                        assignment.DateSubmitionEnds = model.DateSubmitionEnds;
                        assignment.IsPublished = true;

                        db.Assignments.Add(assignment);
                        await db.SaveChangesAsync();
                    }

                }
                catch (Exception e)
                {

                }

                TempData["success"] = "Successfully created.";
                string mass = "";

                try
                {
                    var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
                    var enrol = db.Enrollments.Include(x => x.ClassLevel).Include(x => x.User).Include(x => x.StudentProfile).Where(x => x.ClassLevelId == model.ClassLevelId && x.SessionId == currentSession.Id);
                    var sett = db.Settings.FirstOrDefault();
                    string[] enrolnumbers = enrol.Select(x => x.StudentProfile.user.Phone).ToArray();

                    string numberstosend = string.Join(",", enrolnumbers);

                    string courseTitle = "New Course has been added to your class";
                    var subjectName = db.Subjects.Include(x => x.ClassLevel).Where(x => x.Id == subId).FirstOrDefault();
                    string subject = subjectName.SubjectName;
                    mass = sett.SchoolName + " - " + courseTitle + " " + "in" + " " + subject + " - " + sett.PortalLink + " - visit for more info";
                    var init = sett.SchoolInitials;

                    string urlString1 = "http://xyzsms.com/api/ApiXyzSms/ComposeMessage?username=onwuka1&password=nation&recipients=08165680904," + numberstosend + "&senderId=ISKOOLS&smsmessage=" + mass + "&smssendoption=SendNow";

                    //  string urlString = "http://www.xyzsms.com/components/com_spc/smsapi.php?username=" + senderUserName + "&password=" + senderPassword + "&sender=" + senderId + "&recipient=" + recipient + "&message=" + message;
                    string response = "";
                    try
                    {
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(urlString1);
                        httpWebRequest.Method = "GET";
                        httpWebRequest.ContentType = "application/json";

                        //getting the respounce from the request
                        HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        Stream responseStream = httpWebResponse.GetResponseStream();
                        StreamReader streamReader = new StreamReader(responseStream);
                        response = streamReader.ReadToEnd();
                    }
                    catch (Exception c)
                    {

                    }





                }
                catch (Exception c) { }

                return RedirectToAction("NewOnLineCourse");
            }

            if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
            {
                var staff = await _staffProfileService.StaffDropdownList();
                ViewBag.User = new SelectList(staff, "UserId", "FullName");
            }

            ViewBag.ClassId = new SelectList(db.ClassLevels, "Id", "ClassName", model.ClassLevelId);

            return View(model);
        }


        public async Task<ActionResult> OnlineClass()
        {
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var Subject = await db.ClassLevels.Include(c => c.Subjects).ToListAsync();
            return View(Subject);
        }

        public async Task<ActionResult> OnlineSubject(int? classId)
        {
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var Subject = await db.Subjects.Include(c => c.ClassLevel).Where(x => x.ClassLevelId == classId).ToListAsync();
            ViewBag.sess = currentSession.Id;
            ViewBag.classId = classId;
            return View(Subject);
        }


        public async Task<ActionResult> OnlineSubjectTopics(int? subId, int? page)
        {
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var Topics = db.OnlineCourseUpload.Include(c => c.Session).Include(x => x.ClassLevel).Include(x => x.Subject).Where(x => x.SubjectId == subId && x.SessionId == currentSession.Id);
            ViewBag.SubjectId = subId;
            int pageSize = 1;
            int pageNumber = (page ?? 1);
            return View(Topics.OrderBy(x => x.Id).ToPagedList(pageNumber, pageSize));
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