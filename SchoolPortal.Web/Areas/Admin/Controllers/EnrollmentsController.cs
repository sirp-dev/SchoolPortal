using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Entities;
using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using PagedList;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace SchoolPortal.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class EnrollmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IEnrollmentService _enrollmentService = new EnrollmentService();
        private IClassLevelService _classlevelService = new ClassLevelService();
        private IStudentProfileService _studentService = new StudentProfileService();


        public EnrollmentsController()
        {

        }
        public EnrollmentsController(
            EnrollmentService enrollmentService,
            ClassLevelService classLevelService,
            StudentProfileService studentService,
            ApplicationUserManager userManager
            )
        {
            _enrollmentService = enrollmentService;
            _classlevelService = classLevelService;
            _studentService = studentService;
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
        // GET: Admin/Enrollments
        [AllowAnonymous]
        public async Task<ActionResult> ResultAnalysis()
        {
            var sess = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var total = await db.Enrollments.Include(x=>x.User).Include(x => x.StudentProfile.user).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.SessionId == sess.Id).ToListAsync();
            ViewBag.total = total.Count();
            //
            var Resulttotal = await db.Enrollments.Include(x=>x.User).Include(x => x.StudentProfile.user).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.SessionId == sess.Id && x.AverageScore != null).ToListAsync();
            ViewBag.Resulttotal = Resulttotal.Count();

            var name = await db.Settings.FirstOrDefaultAsync();
            ViewBag.name = name.SchoolName;
            return View();
        }
        public async Task<ActionResult> IndexEnrol(string searchString, string currentFilter, int? page)
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
            var items = await db.Enrollments.Include(x=>x.User).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.ClassLevel).Include(x => x.Session).OrderBy(x => x.Session.SessionYear).ThenBy(x => x.Id).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).ToListAsync();
            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    if (CountString(searchString) > 1)
            //    {
            //        string[] searchStringCollection = searchString.Split(' ');

            //        foreach (var item in searchStringCollection)
            //        {
            //            items = items.Where(s => s.StudentProfile.user.Surname.ToUpper().Contains(item.ToUpper()) || s.StudentProfile.user.FirstName.ToUpper().Contains(item.ToUpper()) || s.StudentProfile.user.OtherName.ToUpper().Contains(item.ToUpper())
            //                                                      || s.StudentProfile.user.UserName.ToUpper().Contains(item.ToUpper()) || s.StudentProfile.StudentRegNumber.ToUpper().Contains(item.ToUpper()));
            //        }
            //    }
            //    else
            //    {
            //        items = items.Where(s => s.StudentProfile.user.Surname.ToUpper().Contains(searchString.ToUpper()) || s.StudentProfile.user.FirstName.ToUpper().Contains(searchString.ToUpper()) || s.StudentProfile.user.OtherName.ToUpper().Contains(searchString.ToUpper())
            //                                                   || s.StudentProfile.user.UserName.ToUpper().Contains(searchString.ToUpper()) || s.StudentProfile.StudentRegNumber.ToUpper().Contains(searchString.ToUpper()));
            //    }

            //}
            int pageSize = 1000000000;
            int pageNumber = (page ?? 1);
            ViewBag.Total = await _enrollmentService.TotalUnEnrolledStudentByTerm();
            return View(items.ToPagedList(pageNumber, pageSize));
        }


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



        public async Task<ActionResult> IndexAutoSearch()
        {
            var items = await _enrollmentService.EnrollmentAutoSearch();

            ViewBag.Total = await _enrollmentService.TotalUnEnrolledStudentByTerm();
            return View(items.OrderBy(x => x.Surname));
        }

        public async Task<ActionResult> EnrolByClass()
        {
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel.OrderBy(x => x.ClassLevelName), "Id", "ClassLevelName");

            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            ViewBag.sessionId = session.Id;
            ViewBag.session = session.SessionYear + " - " + session.Term + " Term";
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> EnrollmentListForClass(int classId = 0)
        {
           
            var items = await _enrollmentService.Enrollment(null, null, 1);
            var classname = await db.ClassLevels.FindAsync(classId);
            if(classname == null)
            {
                return HttpNotFound();
            }
            ViewBag.cid = classname.Id;
            ViewBag.cname = classname.ClassName;
            int pageSize = 50000;
            int pageNumber = (1);
            ViewBag.Total = await _enrollmentService.TotalUnEnrolledStudentByTerm();
            return View(items.OrderBy(x => x.Surname).ToPagedList(pageNumber, pageSize));
        }

        public async Task<ActionResult> Index(string searchString, string currentFilter, int? page)
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
            var items = await _enrollmentService.Enrollment(searchString, currentFilter, page);

            int pageSize = 50;
            int pageNumber = (page ?? 1);
            ViewBag.Total = await _enrollmentService.TotalUnEnrolledStudentByTerm();
            return View(items.OrderBy(x => x.Surname).ToPagedList(pageNumber, pageSize));
        }

        public async Task<ActionResult> AllEnrollment(string UserId)
        {
            var enroll = await db.Enrollments.Include(x=>x.User).Include(x => x.StudentProfile.user).Include(x => x.StudentProfile).Include(x => x.ClassLevel).Include(x => x.Session).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.StudentProfile.user.Id == UserId).ToListAsync();
            return View(enroll);
        }
        public async Task<ActionResult> DelAllUnEnrolleds()
        {

            var session = db.Sessions.OrderByDescending(x => x.Id);
            if (session != null)
            {

                var currentSession = session.FirstOrDefault(x => x.Status == SessionStatus.Current);

                var allStudents = db.StudentProfiles.AsNoTracking().Include(x => x.user).Where(x => x.user.Status == EntityStatus.Active);
                var enrolledStudents = db.Enrollments.AsNoTracking().Include(x=>x.User).Include(x => x.StudentProfile.user).Include(c => c.Session).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.Session.SessionYear == currentSession.SessionYear).Select(u => u.StudentProfileId).ToList();
                var yetToEnroll = allStudents.Where(x => !enrolledStudents.Contains(x.Id));
                var studentsto = from s in yetToEnroll
                                 select s;
                foreach (var userid in studentsto.ToList())
                {


                    try
                    {


                        var user = db.Users.FirstOrDefault(x => x.Id == userid.UserId);

                        var studentp = db.StudentProfiles.Include(x => x.user).FirstOrDefault(x => x.user.Id == user.Id);

                        var enro = db.Enrollments.Include(x=>x.User).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.StudentProfileId == studentp.Id).ToList();
                        var enro1 = db.Enrollments.Include(x => x.StudentProfile).FirstOrDefault(x => x.StudentProfileId == studentp.Id);
                        try
                        {


                            if (enro1 != null)
                            {
                                var enroSub = db.EnrolledSubjects.Where(x => x.EnrollmentId == enro1.Id).ToList();
                            }
                        }
                        catch (Exception r)
                        {

                        }
                        try
                        {


                            var evnt = db.Events.Where(x => x.UserId == user.Id).ToList();
                            if (evnt.Count() > 0)
                            {
                                foreach (var i in evnt)
                                {
                                    db.Events.Remove(i);
                                    db.SaveChanges();
                                }

                            }
                        }
                        catch (Exception r)
                        {

                        }

                        try
                        {


                            var df = db.Defaulters.Where(x => x.ProfileId == studentp.Id).ToList();
                            if (df.Count() > 0)
                            {
                                foreach (var i in df)
                                {
                                    db.Defaulters.Remove(i);
                                    db.SaveChanges();
                                }

                            }
                        }
                        catch (Exception r)
                        {

                        }
                        try
                        {


                            if (enro1 != null)
                            {
                                var ate1 = db.AttendanceDetails.Where(x => x.EnrollmentId == enro1.Id).ToList();
                                if (ate1.Count() > 0)
                                {
                                    foreach (var i in ate1)
                                    {
                                        db.AttendanceDetails.Remove(i);
                                        db.SaveChanges();
                                    }

                                }
                                var ate = db.Attendances.Where(x => x.EnrollmentId == enro1.Id).ToList();
                                if (ate.Count() > 0)
                                {
                                    foreach (var i in ate)
                                    {
                                        db.Attendances.Remove(i);
                                        db.SaveChanges();
                                    }

                                }
                                var atee = db.AssignmentAnswers.Where(x => x.EnrollementId == enro1.Id).ToList();
                                if (atee.Count() > 0)
                                {
                                    foreach (var i in atee)
                                    {
                                        db.AssignmentAnswers.Remove(i);
                                        db.SaveChanges();
                                    }

                                }
                            }
                        }
                        catch (Exception r)
                        {

                        }
                        try
                        {


                            var img = db.ImageModel.FirstOrDefault(x => x.Id == studentp.ImageId);

                            db.ImageModel.Remove(img);
                            db.SaveChanges();
                        }
                        catch (Exception r)
                        {

                        }
                        try
                        {



                            await UserManager.RemoveFromRoleAsync(userid.UserId, "Student");

                            db.StudentProfiles.Remove(studentp);
                            db.SaveChanges();

                            if (user != null)
                            {
                                db.Users.Remove(user);
                                db.SaveChanges();
                                TempData["success"] = "Deleted";
                            }
                            else
                            {

                            }
                        }
                        catch (Exception r)
                        {

                        }
                        //  return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        TempData["error"] = "Not Successful" + e;
                        // return RedirectToAction("Index");
                    }

                    //Add Tracking
                    var userId2 = User.Identity.GetUserId();
                    var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Deleted student from the system";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }

            }
            return RedirectToAction("Index");
        }
        public async Task<ActionResult> EnrolledStudents(string searchString, string currentFilter, int? page)
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
            var items = await _enrollmentService.EnrolledStudents(searchString, currentFilter, page);
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            ViewBag.Total = await _enrollmentService.TotalEnrolledStudentByTerm();
            return View(items.OrderBy(x => x.Surname).ToPagedList(pageNumber, pageSize));
        }

        public async Task<ActionResult> EnrollStudent(int id)
        {
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel, "Id", "ClassLevelName");
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EnrollStudent(int ClassLevelId = 0, int id = 0)
        {
            try
            {
                await _enrollmentService.EnrollStudent(ClassLevelId, id);

            }
            catch (Exception e)
            {
                TempData["error"] = "Enrollment was not successfull. Please try again.";
                return RedirectToAction("Index");
            }
            var student = await _studentService.Get(id);
            var classLevel = await _classlevelService.Get(ClassLevelId);
            TempData["success"] = student.Fullname + " has successfully been enrolled into " + classLevel.ClassName;
            return RedirectToAction("Index");
        }
        public async Task<ActionResult> EnrollStudentToClassDirect(int ClassLevelId = 0, int id = 0)
        {
            try
            {
                await _enrollmentService.EnrollStudent(ClassLevelId, id);

            }
            catch (Exception e)
            {
                TempData["error"] = "Enrollment was not successfull. Please try again.";
                return RedirectToAction("Index");
            }
            var student = await _studentService.Get(id);
            var classLevel = await _classlevelService.Get(ClassLevelId);
            TempData["success"] = student.Fullname + " has successfully been enrolled into " + classLevel.ClassName;
            return RedirectToAction("Index");
        }

        //  [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EnrollStudentList(int ClassLevelId = 0, int id = 0, int SessionId = 0)
        {
            try
            {
                await _enrollmentService.EnrollStudentList(ClassLevelId, id, SessionId);

            }
            catch (Exception e)
            {
                TempData["error"] = "Enrollment was not successfull. Please try again.";
                return RedirectToAction("StatusResult");
            }
            var student = await _studentService.Get(id);
            var classLevel = await _classlevelService.Get(ClassLevelId);
            TempData["success"] = student.Fullname + " has successfully been enrolled into " + classLevel.ClassName;
            return RedirectToAction("StatusResult");
        }
        public async Task<ActionResult> StatusResult()
        {
            return View();
        }
        // GET: Admin/Enrollments/Details/5
        //public async Task<ActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Enrollment enrollment = await db.Enrollments.FindAsync(id);
        //    if (enrollment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(enrollment);
        //}
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult> EditEnrollment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment assignment = await db.Enrollments.Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.Session).Include(x => x.ClassLevel).Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
            if (assignment == null)
            {
                return HttpNotFound();
            }

            return View(assignment);
        }

        // POST: Content/Assignments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditEnrollment(Enrollment enro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(enro).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("EnrolledStudents");
            }

            return View(enro);
        }

        public async Task<ActionResult> RemoveStudentFromSession(string ReturnUrl, int id = 0)
        {
            //  var enrolledStudent = null;
            string name;
            var enrolledStudentclass = await _enrollmentService.classLevelFromEnrollmentbyProfileId(id);
            try
            {
                //  enrolledStudent = await _enrollmentService.GetStudent(id);
                //name = await _enrollmentService.RemoveStudent(id);
                //remove student from current enrollment
                name = await _enrollmentService.RemoveStudent(id);
                TempData["success"] = name + " has successfully been removed from Class ";
                if (ReturnUrl != null)
                {
                    return Redirect(ReturnUrl);
                }
                return RedirectToAction("EnrolledStudents");
            }
            catch (Exception e)
            {
                TempData["error"] = "Removal was not successfull. Please try again.";
                if (ReturnUrl != null)
                {
                    return Redirect(ReturnUrl);
                }
                return RedirectToAction("EnrolledStudents");
            }



        }



        public async Task<ActionResult> RemoveStudent(string ReturnUrl, int id = 0)
        {
            //  var enrolledStudent = null;
            string name;
            var enrolledStudentclass = await _enrollmentService.classLevelFromEnrollmentbyEnrolId(id);
            try
            {
                //  enrolledStudent = await _enrollmentService.GetStudent(id);
                //name = await _enrollmentService.RemoveStudent(id);
                //remove student from current enrollment
                name = await _enrollmentService.RemoveStudent(id);
                TempData["success"] = name + " has successfully been removed from Class ";
                if (ReturnUrl != null)
                {
                    return Redirect(ReturnUrl);
                }
                return RedirectToAction("EnrolledStudents");
            }
            catch (Exception e)
            {
                TempData["error"] = "Removal was not successfull. Please try again.";
                if (ReturnUrl != null)
                {
                    return Redirect(ReturnUrl);
                }
                return RedirectToAction("EnrolledStudents");
            }



        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>

        // GET: Content/Post/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment post = db.Enrollments.Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.ClassLevel).Include(x => x.Session).FirstOrDefault(x => x.Id == id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Content/Post/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Enrollment post = db.Enrollments.Find(id);
            var sub = db.EnrolledSubjects.Where(x => x.EnrollmentId == post.Id);
            foreach (var a in sub)
            {
                db.EnrolledSubjects.Remove(a);
            }
            db.Enrollments.Remove(post);
            var attend = db.AttendanceDetails.Where(x => x.StudentId == post.StudentProfileId).ToList();
            foreach (var att in attend)
            {
                db.AttendanceDetails.Remove(att);
            }
            db.SaveChanges();

            //Add Tracking
            var userId2 = User.Identity.GetUserId();
            var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
            Tracker tracker = new Tracker();
            tracker.UserId = userId2;
            tracker.UserName = user2.UserName;
            tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
            tracker.ActionDate = DateTime.UtcNow.AddHours(1);
            tracker.Note = tracker.FullName + " " + "Deleted enrolled subject";
            //db.Trackers.Add(tracker);
            db.SaveChangesAsync();

            return RedirectToAction("IndexEnrol");


        }

        //user and enrolment validation

        public ActionResult FetchUserandTermlyResult()
        {
            var user = db.StudentProfiles.Include(x => x.user).OrderByDescending(v => v.user.Surname).ToList();
            return View(user);
        }
        //
        public ActionResult FetchUserEnrollment()
        {
            var user = db.Enrollments.Include(x => x.StudentProfile.user).Include(x => x.StudentProfile).Include(x => x.ClassLevel).OrderByDescending(v => v.ClassLevel.ClassName).ThenBy(x => x.StudentProfile.user.Surname).ToList();
            return View(user);
        }

        public ActionResult FetchUserCurrentEnrollment()
        {
            var user = db.Enrollments.Include(x => x.StudentProfile.user).Include(x => x.StudentProfile).Include(x => x.ClassLevel).OrderByDescending(v => v.ClassLevel.ClassName).ThenBy(x => x.StudentProfile.user.Surname).Where(c => c.Session.Status == SessionStatus.Current).ToList();
            return View(user);
        }

        public ActionResult FetchUserLast2018Enrollment()
        {
            var user = db.Enrollments.Include(x => x.StudentProfile.user).Include(x => x.StudentProfile).Include(x => x.ClassLevel).OrderByDescending(v => v.ClassLevel.ClassName).ThenBy(x => x.StudentProfile.user.Surname).Where(c => c.Session.SessionYear.Contains("2017/2018") && c.Session.Term.Contains("Third")).ToList();
            return View(user);
        }



        public async Task<ActionResult> EditEnrolment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment assignment = await db.Enrollments.FindAsync(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }

            return View(assignment);
        }

        // POST: Content/Assignments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditEnrolment(Enrollment assignment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(assignment).State = EntityState.Modified;
                //var enrSub = db.EnrolledSubjects.Where(x => x.EnrollmentId == assignment.Id);
                //foreach(var i in enrSub)
                //{
                //    EnrolledSubject ss = await db.EnrolledSubjects.FindAsync(i.Id);
                //    ss.EnrollmentId = assignment.Id;
                //    db.Entry(ss).State = EntityState.Modified;
                //}
                await db.SaveChangesAsync();


                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Edited enrollment";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(assignment);
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
