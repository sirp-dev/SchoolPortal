using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Dtos;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;
using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using System.Net;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity.Validation;
using SchoolPortal.Web.Areas.Service;
using SchoolPortal.Web.Models.Entities;

namespace SchoolPortal.Web.Areas.Content.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class DocController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IEnrollmentService _enrollmentService = new EnrollmentService();
        private IClassLevelService _classlevelService = new ClassLevelService();
        private IStudentProfileService _studentService = new StudentProfileService();
        private IUserManagerService _userService = new UserManagerService();
        private ISessionService _sessionService = new SessionService();


        public DocController()
        {

        }
        public DocController(
            EnrollmentService enrollmentService,
            SessionService sessionService,
            ClassLevelService classLevelService,
            StudentProfileService studentService,
            ApplicationUserManager userManager,
            UserManagerService userService
            )
        {
            _userService = userService;
            UserManager = userManager;
            _enrollmentService = enrollmentService;
            _classlevelService = classLevelService;
            _studentService = studentService;
            _sessionService = sessionService;
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
        // GET: Content/Doc
        public ActionResult Index()
        {
            var item = db.ClassLevels.Include(x => x.User).Where(x=>x.ShowClass == true).ToList();
            var output = item.Select(x => new ClassLevelListDto
            {
                ClassLevelName = x.ClassName,
                Id = x.Id,
                userId = x.UserId,
                FormTeacher = x.User.Surname + " " + x.User.FirstName + " " + x.User.OtherName
            });


            return View(output.OrderBy(x => x.ClassLevelName).ToList());
        }
        public async Task<ActionResult> LessonNote()
        {

            var note = await db.LessonNotes.Include(x => x.Subject).Include(x => x.Subject.ClassLevel).Include(x => x.StaffProfile).Include(x => x.Session).Where(x => x.IsPublished == true).ToListAsync();

            return View(note);
        }

        public async Task<ActionResult> LessonNoteDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await db.LessonNotes.Include(x => x.Session).Include(x => x.Subject).Include(x => x.Subject.ClassLevel).FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }
        public ActionResult StudentsList()
        {
            var item = db.ClassLevels.Include(x => x.User).Where(x => x.ClassName.Contains("JSS")).ToList();
            var output = item.Select(x => new ClassLevelListDto
            {
                ClassLevelName = x.ClassName,
                Id = x.Id,
                userId = x.UserId,
                FormTeacher = x.User.Surname + " " + x.User.FirstName + " " + x.User.OtherName
            });


            return View(output.OrderBy(x => x.ClassLevelName).ToList());
        }
        public async Task<ActionResult> IndexForList()
        {
            //var classlevel = await _classlevelService.ClassLevelList();
            //ViewBag.ClassLevelId = new SelectList(classlevel.OrderBy(x => x.ClassLevelName), "Id", "ClassLevelName");

            //var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            //ViewBag.sessionId = session.Id;
            //ViewBag.session = session.SessionYear + " - " + session.Term + " Term";
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> EnrolFromSession(int sessionId)
        {

            try
            {
                var session = db.Sessions.FirstOrDefault(x => x.Id == sessionId);
                var enrolment = db.Enrollments.Where(x => x.SessionId == session.Id).ToList();
                foreach (var x in enrolment)
                {
                    await _enrollmentService.EnrollStudentFromSession(x.ClassLevelId ?? 0, x.StudentProfileId, sessionId);
                }



            }
            catch (Exception e)
            {
                TempData["error"] = "Enrollment was not successfull. Please try again.";
                return RedirectToAction("Index");
            }
            return RedirectToAction("OtherTerm");
        }
        public async Task<ActionResult> Dropout(int id = 0)
        {

            try
            {
                var item = await _enrollmentService.ChangeToDropoutStudent(id);

                return RedirectToAction("Success", new { result = item });


            }
            catch (Exception ex)
            {
                return RedirectToAction("Success", new { result = "Unable to Update" });

            }

        }
        public ActionResult Success(string result)
        {
            if (result == "successfully")
            {
                TempData["success"] = "Updated successfully";
            }
            else
            {
                TempData["error"] = "Unable to Update";
            }

            return View();
        }
        [HttpPost]
        public ActionResult IndexBySession(int sessionId)
        {
            var item = db.ClassLevels.Include(x => x.User).ToList();
            var output = item.Select(x => new ClassLevelListDto
            {
                ClassLevelName = x.ClassName,
                Id = x.Id,
                userId = x.UserId,
                FormTeacher = x.User.Surname + " " + x.User.FirstName + " " + x.User.OtherName
            });
            ViewBag.sId = sessionId;
            var xy = db.Sessions.Find(sessionId);
            ViewBag.sname = xy.SessionYear + " - " + xy.Term + " Term";
            return View(output.OrderBy(x => x.ClassLevelName).ToList());
        }

        public async Task<ActionResult> OtherTerm()
        {

            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session.OrderBy(x => x.FullSession), "Id", "FullSession");
            return View();
        }
        ///get the student in a particular session
        ///
        public async Task<ActionResult> StudentsInTerm(int? sessionId)
        {
            try
            {


                var session = await db.Sessions.ToListAsync();
                var sessionoutput = session.Select(x => new SchoolSessionDto
                {
                    Id = x.Id,
                    FullSession = x.SessionYear + " " + x.Term

                });
                ViewBag.sessionId = new SelectList(sessionoutput, "Id", "FullSession");

                ViewBag.session = sessionId;

                var item = db.ClassLevels.Include(x => x.User).ToList();
                var output = item.Select(x => new ClassLevelListDto
                {
                    ClassLevelName = x.ClassName,
                    Id = x.Id,
                    userId = x.UserId,
                    FormTeacher = x.User.Surname + " " + x.User.FirstName + " " + x.User.OtherName
                });
                return View(output.OrderBy(x => x.ClassLevelName).ToList());
            }
            catch (Exception e)
            {
                return RedirectToAction("StudentsInTerm");
            }
        }



        /// <summary>
        public ActionResult ResultSummary()
        {
            var item = db.ClassLevels.Include(x => x.User).ToList();
            var output = item.Select(x => new ClassLevelListDto
            {
                ClassLevelName = x.ClassName,
                Id = x.Id,
                userId = x.UserId,
                FormTeacher = x.User.Surname + " " + x.User.FirstName + " " + x.User.OtherName
            });
            return View(output.OrderBy(x => x.ClassLevelName).ToList());
        }

        public ActionResult IndexAllByClass()
        {
            var item = db.ClassLevels.Include(x => x.User).ToList();
            var output = item.Select(x => new ClassLevelListDto
            {
                ClassLevelName = x.ClassName,
                Id = x.Id,
                userId = x.UserId,
                FormTeacher = x.User.Surname + " " + x.User.FirstName + " " + x.User.OtherName
            });


            return View(output.OrderBy(x => x.ClassLevelName).ToList());
        }

        /// <summary>
        /// /
        /// </summary>
        /// <returns></returns>
        /// 


        public async Task<ActionResult> IndexByEnrolId()
        {
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == Models.Entities.SessionStatus.Current);
            var enr = await db.Enrollments.Include(x => x.ClassLevel).Include(x => x.Session).Include(x => x.User).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Where(x => x.Session.SessionYear == session.SessionYear).OrderBy(x => x.Id).ToListAsync();
            return View(enr);

        }


        /////
        ///
        public async Task<ActionResult> RemoveAll()
        {
            int i = 0;
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == Models.Entities.SessionStatus.Current);
            //  var enrolledStudent = null;
            var enrollment = await db.Enrollments.Include(x => x.Session).Where(x => x.Session.SessionYear == session.SessionYear).ToListAsync();
            foreach (var item in enrollment)
            {
                string name;
                var enrolledStudentclass = await _enrollmentService.classLevelFromEnrollmentbyEnrolId(item.Id);
                try
                {
                    //  enrolledStudent = await _enrollmentService.GetStudent(id);
                    //name = await _enrollmentService.RemoveStudent(id);
                    //remove student from current enrollment
                    name = await _enrollmentService.RemoveStudent(item.Id);
                    i++;



                }
                catch (Exception e)
                {
                    TempData["error"] = "Removal was not successfull. Please try again.";


                }


            }
            TempData["success"] = " has successfully been removed from Class " + i;
            return RedirectToAction("Index");
        }


        public async Task<ActionResult> RemoveAllFromCurrent()
        {
            int i = 0;
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == Models.Entities.SessionStatus.Current);
            //  var enrolledStudent = null;
            var enrollment = await db.Enrollments.Include(x => x.Session).Where(x => x.SessionId == session.Id).ToListAsync();
            foreach (var item in enrollment)
            {
                string name;
                var enrolledStudentclass = await _enrollmentService.classLevelFromEnrollmentbyEnrolId(item.Id);
                try
                {
                    //  enrolledStudent = await _enrollmentService.GetStudent(id);
                    //name = await _enrollmentService.RemoveStudent(id);
                    //remove student from current enrollment
                    name = await _enrollmentService.RemoveSingleEnrollmentDirect(item.Id);
                    i++;



                }
                catch (Exception e)
                {
                    TempData["error"] = "Removal was not successfull. Please try again.";


                }


            }
            TempData["success"] = " has successfully been removed from Class " + i;
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> EnrollmentWithScore(int id = 0)
        {
            var enrolledStudent = await db.Enrollments.Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.Session).Include(x => x.ClassLevel).Include(x => x.StudentProfile.user).Include(x => x.EnrolledSubjects).FirstOrDefaultAsync(x => x.StudentProfileId == id);
            return View(enrolledStudent);
        }

        public async Task<ActionResult> EnrollFromAnotherTermInSession()
        {
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == Models.Entities.SessionStatus.Current);
            var allterminsession = await db.Sessions.Where(x => x.SessionYear == session.SessionYear).Where(x => x.Term != session.Term).ToListAsync();

            var output = allterminsession.Select(x => new SchoolSessionDto
            {
                FullSession = x.SessionYear + " - " + x.Term + " Term",
                Id = x.Id,
                SessionStatus = x.Status
            });
            ViewBag.sessionId = new SelectList(output.OrderBy(x => x.FullSession), "Id", "FullSession");

            return View();
        }
        [HttpPost]
        public async Task<ActionResult> EnrollFromAnotherTermInSession(int sessId = 0)
        {
            var session = await db.Sessions.FindAsync(sessId);
            var enrollment = await db.Enrollments.Include(x => x.Session).Where(x => x.SessionId == sessId).ToListAsync();
            var currentsession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == Models.Entities.SessionStatus.Current);

            foreach (var item in enrollment)
            {
                await _enrollmentService.JustEnrolToClass(item.ClassLevelId, item.StudentProfileId, currentsession.Id);
            }
            //int ClassLevelId = 0, int id = 0, int termid = 0)
            return View();
        }



        public async Task<ActionResult> RemoveAllFromCurrentLatest()
        {
            int i = 0;
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == Models.Entities.SessionStatus.Current);
            //  var enrolledStudent = null;
            var enrollment = await db.Enrollments.Include(x => x.Session).Where(x => x.SessionId == session.Id).ToListAsync();
            foreach (var item in enrollment)
            {
                var enrolledStudent = await db.Enrollments.Include(x => x.ClassLevel).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.EnrolledSubjects).FirstOrDefaultAsync(x => x.StudentProfileId == item.StudentProfileId && x.SessionId == session.Id);
                string name = enrolledStudent.StudentProfile.user.Surname + " " + enrolledStudent.StudentProfile.user.FirstName + " " + enrolledStudent.StudentProfile.user.OtherName;
                string classs = enrolledStudent.ClassLevel.ClassName;


                //var sub = db.EnrolledSubjects.Where(x => x.EnrollmentId == post.Id);
                //lock (enrolledStudent.EnrolledSubjects)
                //{
                foreach (var a in enrolledStudent.EnrolledSubjects.ToList())
                {
                    db.EnrolledSubjects.Remove(a);
                    db.SaveChanges();
                }

                var attend = db.AttendanceDetails.Where(x => x.StudentId == enrolledStudent.StudentProfileId).ToList();
                foreach (var att in attend)
                {
                    db.AttendanceDetails.Remove(att);
                    db.SaveChanges();
                }
                db.Enrollments.Remove(enrolledStudent);
                db.SaveChanges();
                TempData["success"] = name + " has successfully been removed from " + classs + " Class";



            }
            TempData["success"] = " has successfully been removed from Class " + i;
            return RedirectToAction("Index");
        }


        public async Task<ActionResult> Removefromsession(int id = 0)
        {
            //  var enrolledStudent = null;
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == Models.Entities.SessionStatus.Current);
            var sessionsToRemove = db.Sessions.Where(x => x.SessionYear == session.SessionYear);
            if (session.Term.ToLower().Contains("first"))
            {
                sessionsToRemove = sessionsToRemove.OrderBy(x => x.Id);
            }
            else if (session.Term.ToLower().Contains("second"))
            {
                sessionsToRemove = sessionsToRemove.Where(x => x.Term.ToLower() != "first");
            }
            else if (session.Term.ToLower().Contains("third"))
            {
                sessionsToRemove = sessionsToRemove.Where(x => x.Term.ToLower() == "third");
            }

            foreach (var term in sessionsToRemove.ToList())
            {
                var enrolledStudent = await db.Enrollments.Include(x => x.ClassLevel).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.EnrolledSubjects).FirstOrDefaultAsync(x => x.StudentProfileId == id && x.SessionId == term.Id);
                string name = enrolledStudent.StudentProfile.user.Surname + " " + enrolledStudent.StudentProfile.user.FirstName + " " + enrolledStudent.StudentProfile.user.OtherName;
                string classs = enrolledStudent.ClassLevel.ClassName;

                if (enrolledStudent.AverageScore != null || enrolledStudent.CummulativeAverageScore != null)
                {
                    return RedirectToAction("EnrollmentWithScore", new { id = id });
                }
                else
                {
                    //var sub = db.EnrolledSubjects.Where(x => x.EnrollmentId == post.Id);
                    //lock (enrolledStudent.EnrolledSubjects)
                    //{
                    foreach (var a in enrolledStudent.EnrolledSubjects.ToList())
                    {
                        db.EnrolledSubjects.Remove(a);
                        db.SaveChanges();
                    }

                    var attend = db.AttendanceDetails.Where(x => x.StudentId == enrolledStudent.StudentProfileId).ToList();
                    foreach (var att in attend)
                    {
                        db.AttendanceDetails.Remove(att);
                        db.SaveChanges();
                    }
                    db.Enrollments.Remove(enrolledStudent);
                    db.SaveChanges();
                    TempData["success"] = name + " has successfully been removed from " + classs + " Class";

                }

            }
            return RedirectToAction("Index");
        }



        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        public async Task<ActionResult> RemoveWithScore(int id = 0)
        {
            //  var enrolledStudent = null;
            //var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == Models.Entities.SessionStatus.Current);
            //var enrolledStudent = await db.Enrollments.Include(x => x.ClassLevel).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.EnrolledSubjects).FirstOrDefaultAsync(x => x.Id == id);
            //string name = enrolledStudent.StudentProfile.user.Surname + " " + enrolledStudent.StudentProfile.user.FirstName + " " + enrolledStudent.StudentProfile.user.OtherName;
            //string classs = enrolledStudent.ClassLevel.ClassName;
            //try
            //{

            //    var sub = db.EnrolledSubjects.Where(x => x.EnrollmentId == enrolledStudent.Id);
            //    foreach (var a in sub)
            //    {
            //        db.EnrolledSubjects.Remove(a);
            //    }

            //    var attend = db.AttendanceDetails.Where(x => x.StudentId == enrolledStudent.StudentProfileId).ToList();
            //    foreach (var att in attend)
            //    {
            //        db.AttendanceDetails.Remove(att);
            //    }
            //    db.Enrollments.Remove(enrolledStudent);
            //    db.SaveChanges();
            //    TempData["success"] = name + " has successfully been removed from " + classs + " Class";



            //    return RedirectToAction("Index");
            //}
            //catch (Exception e)
            //{
            //    TempData["error"] = "Removal was not successfull. Please try again.";

            //    return RedirectToAction("Index");
            //}


            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == Models.Entities.SessionStatus.Current);
            var sessionsToRemove = db.Sessions.Where(x => x.SessionYear == session.SessionYear);
            if (session.Term.ToLower().Contains("first"))
            {
                sessionsToRemove = sessionsToRemove.OrderBy(x => x.Id);
            }
            else if (session.Term.ToLower().Contains("second"))
            {
                sessionsToRemove = sessionsToRemove.Where(x => x.Term.ToLower() != "first");
            }
            else if (session.Term.ToLower().Contains("third"))
            {
                sessionsToRemove = sessionsToRemove.Where(x => x.Term.ToLower() == "third");
            }

            foreach (var term in sessionsToRemove.ToList())
            {
                var enrolledStudent = await db.Enrollments.Include(x => x.ClassLevel).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.EnrolledSubjects).FirstOrDefaultAsync(x => x.StudentProfileId == id && x.SessionId == term.Id);
                string name = enrolledStudent.StudentProfile.user.Surname + " " + enrolledStudent.StudentProfile.user.FirstName + " " + enrolledStudent.StudentProfile.user.OtherName;
                string classs = enrolledStudent.ClassLevel.ClassName;

                //var sub = db.EnrolledSubjects.Where(x => x.EnrollmentId == post.Id);
                //lock (enrolledStudent.EnrolledSubjects)
                //{
                foreach (var a in enrolledStudent.EnrolledSubjects.ToList())
                {
                    db.EnrolledSubjects.Remove(a);
                    db.SaveChanges();
                }

                var attend = db.AttendanceDetails.Where(x => x.StudentId == enrolledStudent.StudentProfileId).ToList();
                foreach (var att in attend)
                {
                    db.AttendanceDetails.Remove(att);
                    db.SaveChanges();
                }
                db.Enrollments.Remove(enrolledStudent);
                db.SaveChanges();
                TempData["success"] = name + " has successfully been removed from " + classs + " Class";

            }
            return RedirectToAction("Index");

        }


        public async Task<ActionResult> RemoveSelectedStudent(int id = 0)
        {
            //  var enrolledStudent = null;
            string name;

            try
            {
                //  enrolledStudent = await _enrollmentService.GetStudent(id);
                //name = await _enrollmentService.RemoveStudent(id);
                //remove student from current enrollment
                name = await _enrollmentService.RemoveSingleEnrollmentDirect(id);
                if (name == "error")
                {
                    TempData["error"] = name;
                }
                else
                {
                    TempData["success"] = name;

                }

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["error"] = "Removal was not successfull. Please try again.";

                return RedirectToAction("Index");
            }



        }


        public async Task<ActionResult> EditInfo(string id, string ReturnUrl)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.StateName = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName");
            var user = await _userService.GetUserByUserId(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReturnUrl = ReturnUrl;
            return View(user);
        }

        // POST: Admin/Settings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditInfo(ApplicationUser model, string ReturnUrl)
        {
            if (model.Id != null)
            {
                try
                {
                    bool check = await _userService.UpdateUser(model);
                    if (check == true)
                    {
                        TempData["success"] = "User Updated Successfully";
                        if (ReturnUrl == null)
                        {
                            return RedirectToAction("Index", "Doc", new { area = "Content" });
                        }
                        else
                        {
                            Redirect(ReturnUrl);
                        }

                    }
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        }
                    }
                }


            }
            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName", model.StateOfOrigin);
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            TempData["error"] = "Update not successful";
            ViewBag.ReturnUrl = ReturnUrl;
            return View(model);
        }


        public async Task<ActionResult> StaffList()
        {

            var staff = await db.StaffProfiles.Include(x => x.user).ToListAsync();
            foreach (var st in staff)
            {
                var i = GeneralService.IsUserInRole(st.UserId, "Admin");
                if (i == true)
                {
                    staff = staff.Where(x => x.UserId != st.UserId).ToList();
                }
            }

            return View(staff);
        }

        public ActionResult StaffRegistrationLink()
        {
            var setting = db.Settings.FirstOrDefault();
            ViewBag.link = setting.PortalLink;
            ViewBag.name = setting.SchoolName;
            return View();
        }
        //

        public async Task<ActionResult> MoveEnrol(int id, int Ocid = 0)
        {
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel, "Id", "ClassLevelName");
            ViewBag.id = id;
            ViewBag.Ocid = Ocid;
            return View();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> MoveEnrol(int Ocid = 0, int ClassLevelId = 0, int id = 0)
        {

            try
            {
                var item = await _enrollmentService.MoveStudent(Ocid, ClassLevelId, id);
                //if (item == "true")
                //{
                //var student1 = await _studentService.Get(id);
                //var classLevel1 = await _classlevelService.Get(ClassLevelId);
                TempData["success"] = "student has been moved successfully";
                return RedirectToAction("Index");

                //}

            }
            catch (Exception ex)
            {
                TempData["error"] = "Enrollment was not successfull. Please try again.";
                return RedirectToAction("Index");
            }

            var student = await _studentService.Get(id);
            var classLevel = await _classlevelService.Get(ClassLevelId);
            TempData["success"] = student.Fullname + " has successfully been enrolled into " + classLevel.ClassName;
            return RedirectToAction("Index");
        }


        public async Task<ActionResult> MoveClassEnrol(int id)
        {
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel, "Id", "ClassLevelName");
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> MoveClassEnrol(int id = 0, int ClassLevelId = 0)
        {

            try
            {
                var items = await _enrollmentService.MoveClassStudent(id, ClassLevelId);
                TempData["success"] = items;
                //TempData["success"] = "students has successfully been enrolled successfully";
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                TempData["error"] = "Enrollment was not successfull. Please try again.";
                return RedirectToAction("Index");
            }
            //var student = await _studentService.Get(id);
            var classLevel = await _classlevelService.Get(ClassLevelId);
            //TempData["success"] = student.Fullname + " has successfully been enrolled into " + classLevel.ClassName;
            //TempData["success"] = student.Fullname + " has successfully been enrolled into " + classLevel.ClassName;
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> UpdateOnlineData(int id)
        {
            var enrol = await db.Enrollments.FirstOrDefaultAsync(x => x.Id == id);
            if (enrol.EnableLiveClass == true)
            {
                enrol.EnableLiveClass = false;
            }
            else
            {
                enrol.EnableLiveClass = true;

            }

            db.Entry(enrol).State = EntityState.Modified;
            await db.SaveChangesAsync();
            TempData["success"] = "Updated Successful";
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