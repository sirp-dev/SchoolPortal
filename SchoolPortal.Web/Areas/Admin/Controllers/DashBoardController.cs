using SchoolPortal.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PagedList;
using SchoolPortal.Web.Models.Dtos;
using SchoolPortal.Web.Models.Entities;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using SchoolPortal.Web.Areas.Service;
using SchoolPortal.Web.Models.Dtos.Api;
using Microsoft.AspNet.Identity.Owin;

namespace SchoolPortal.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class DashBoardController : Controller
    {

        // GET: Admin/DashBoard
        private ApplicationDbContext db = new ApplicationDbContext();

        public DashBoardController()
        {

        }

        public DashBoardController(ApplicationUserManager userManager)
        {
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
        public ActionResult Index(string searchString, string currentFilter, int? page)
        {
            var settings = db.Settings.FirstOrDefault();
            if (settings == null)
            {
                return RedirectToAction("Create", "Settings", new { area = "Admin" });
            }

            var check = db.Users.FirstOrDefault(x => x.UserName == "Education Sec");
            if (check == null)
            {
                return RedirectToAction("ReadOnly", "AdminUser", new { area = "SuperUser" });
            }

            //Get Current Session
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);

            if (currentSession == null)
            {
                return RedirectToAction("NewSession", "Sessions");

            }
            else
            {
                ViewBag.CurrentSession = currentSession.SessionYear + "-" + currentSession.Term + " Term";

            }



            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var help = db.Helps.Where(x => x.Title != "");

            if (!String.IsNullOrEmpty(searchString))
            {

                help = help.Where(s => s.Title.ToUpper().Contains(searchString.ToUpper()));

            }
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            ViewBag.Total = help.Count();
            return View(help.OrderBy(x => x.Id).ThenBy(x => x.SortOrder).ToPagedList(pageNumber, pageSize));

        }

        public JsonResult GetEvents()
        {
            var userid = User.Identity.GetUserId();
            var events = db.Events.Where(x => x.UserId == userid || x.GeneralEvent == true).ToList();
            return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AddEvent(string subject, string description, DateTime start, DateTime end, string color, bool general, bool fday)
        {
            try
            {
                string u = User.Identity.GetUserId();
                Event m = new Event();
                m.Subject = subject;
                m.DIscription = description;
                m.Start = start;
                m.End = end;
                m.Color = color;
                m.GeneralEvent = general;
                m.IsFullDay = fday;
                m.UserId = u;
                db.Events.Add(m);
                db.SaveChanges();

                //Add Tracking
                var userId = User.Identity.GetUserId();
                var user = UserManager.Users.Where(x => x.Id == userId && x.Status == EntityStatus.Active).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Added an event";
                //db.Trackers.Add(tracker);
                db.SaveChangesAsync();

                TempData["success"] = "Event Added";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Something Went Wrong. Try again.";
            }
            return RedirectToAction("Index");
        }

        public ActionResult Statistics()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult GetSchool(string url = null)
        {

            var schoolinfo = db.Settings.FirstOrDefault(x => x.WebsiteLink == url);
            if (schoolinfo != null)
            {
                var session = db.Sessions.FirstOrDefault(x => x.Status == Models.Entities.SessionStatus.Current);
                var classes = db.ClassLevels;
                var enrolment = db.Enrollments.Where(x => x.SessionId == session.Id);
                var unenrol = db.StudentProfiles;
                var staff = db.StaffProfiles.Count();
                byte[] image = db.ImageModel.FirstOrDefault(x => x.Id == schoolinfo.ImageId).ImageContent;

                //card count
                var card = db.PinCodeModels.Count();
                //ViewBag.card = card;

                var cardUnused = db.PinCodeModels.Where(x => x.StudentPin == null).Count();
                //ViewBag.cardUnused = cardUnused;

                var cardUsed = db.PinCodeModels.Where(x => x.EnrollmentId != null).Count();
                //ViewBag.cardused = cardUsed;


                var output = new ApiSchoolInfoDto
                {
                    SchoolName = schoolinfo.SchoolName,
                    Usedcard = cardUsed.ToString(),
                    NonUsedcard = cardUnused.ToString(),
                    Totalcard = card.ToString(),
                    TotalStaff = staff.ToString(),
                    CurrentSession = session.SessionYear.ToString() + " / " + session.Term.ToString(),
                    SchoolAddress = schoolinfo.SchoolAddress,
                    SchoolCurrentPrincipal = session.SchoolPrincipal,
                    ClassCount = classes.Count().ToString(),
                    EnrolStudentsCount = enrolment.Count().ToString() + " Enrolled " + GeneralService.StudentorPupil() + "s / " + unenrol.Count().ToString() + " Total " + GeneralService.StudentorPupil(),
                    Url = schoolinfo.WebsiteLink

                };
                var outputmain = output;
                return View(outputmain);
            }
            return View();
        }

        public ActionResult Calender()
        {
            return View();
        }

        public ActionResult _Dashboard()
        {
            var settings = db.Settings.FirstOrDefault();
            if (settings == null)
            {
                return RedirectToAction("Create", "Settings", new { area = "Admin" });
            }
            //Get Current Session
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);

            if (currentSession == null)
            {
                return RedirectToAction("NewSession", "Sessions");

            }
            else
            {
                ViewBag.CurrentSession = currentSession.SessionYear + "-" + currentSession.Term + " Term";

            }

            //Get Enrolled Students Count

            IQueryable<Enrollment> enrolledStudents = from s in db.Enrollments
                                                     .Include(x => x.StudentProfile)
                                                     .Include(x=>x.StudentProfile.user).Include(p => p.ClassLevel).Include(c => c.Session).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.Session.Id == currentSession.Id)
                                                      select s;


            ViewBag.EnrolledStudents = enrolledStudents.Count();

            //Get Staff Count
            var staff = db.StaffProfiles.Include(x=>x.user).Where(x => x.user.Status == EntityStatus.Active).Count();
            ViewBag.Staff = staff;

            //card count
            //var card = db.PinCodeModels.Count();
            //ViewBag.card = card;

            IQueryable<PinCodeModel> cardUnused = from s in db.PinCodeModels

                                                  select s;


            ViewBag.cardavialable = cardUnused.Where(x => x.StudentPin == null).Count();

            var cardUsed = cardUnused.Where(x => x.EnrollmentId != null && x.SessionId == currentSession.Id).Count();
            ViewBag.cardusedcurrentterm = cardUsed;

            string batchstatus = settings.EnableBatchResultPrinting.ToString().ToUpper();
            ViewBag.batchstatus = batchstatus;

            ////
            ///
            //batch


            IQueryable<Enrollment> result = from s in db.Enrollments
            .Where(x => x.SessionId == currentSession.Id).Where(x => x.AverageScore != null || x.AverageScore != 0)
                                            select s;
            ViewBag.avelableresult = result.Count();


            var classes = db.ClassLevels.Count();
            ViewBag.classes = classes;


            IQueryable<ClassLevel> classesJss = from s in db.ClassLevels
                .Where(x => x.ClassName.Contains("JSS"))
                                                select s;
            ViewBag.classesJss = classesJss.Count();


            IQueryable<ClassLevel> classesSss = from s in db.ClassLevels
                .Where(x => x.ClassName.Contains("SSS"))
                                                select s;

            ViewBag.classesSss = classesSss.Count();

            ///////


            IQueryable<Post> posts = from s in db.Posts
              .Where(x => x.Status == PostStatus.Published)
                                     select s;

            ViewBag.Published = posts.Count();
            //

            IQueryable<Income> pamount = from s in db.Incomes
                                         select s;
            ViewBag.income = pamount;



            ///
            IQueryable<Finance> ifianance = from s in db.Finances
                                            select s;
            var ib = ifianance.Where(x => x.FinanceSource == FinanceSource.Online && x.FinanceType == FinanceType.Credit);
            if(ib.Count() > 1)
            {
                ViewBag.b = ib.Sum(x => x.Amount);
            }
            var ic = ifianance.Where(x => x.FinanceSource == FinanceSource.Cash && x.FinanceType == FinanceType.Credit);
            if (ic.Count() > 1)
            {
                ViewBag.c = ic.Sum(x => x.Amount);
            }
            var icompletpayment = ifianance.Where(x => x.FinanceType == FinanceType.Credit && x.TransactionStatus == TransactionStatus.Paid);
            if (icompletpayment.Count() > 1)
            {
                ViewBag.completpayment = icompletpayment.Sum(x => x.Amount);
            }
            var ipartpayment = ifianance.Where(x => x.FinanceType == FinanceType.Credit && x.TransactionStatus == TransactionStatus.Paid && x.Skip == false);
            if (ipartpayment.Count() > 1)
            {
                ViewBag.partpayment = ipartpayment.Sum(x => x.Amount);
            }




            return PartialView();
        }


        public async Task<ActionResult> FinancePage(int incomeid)
        {
            IQueryable<Enrollment> ienrol = from s in db.Enrollments
                                            .Include(x=>x.Finances)
                                            select s;
            return View();
        }

    }
}
