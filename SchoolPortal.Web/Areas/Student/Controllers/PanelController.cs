using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SchoolPortal.Web.Models.Entities;
using SchoolPortal.Web.Areas.Service;
using System.Net;
using System.Data.Entity;
using System.Data;
using PagedList;
using System.Data.Entity.Validation;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SchoolPortal.Web.Models.Dtos.Zoom;
using Newtonsoft.Json;
using SchoolPortal.Web.Controllers;
using System.Net.Http;
using System.Net.Http.Headers;
using SchoolPortal.Web.Models.Dtos.Api;

namespace SchoolPortal.Web.Areas.Student.Controllers
{
    [Authorize(Roles = "Student")]
    public class PanelController : Controller
    {

        #region services
        private ApplicationDbContext db = new ApplicationDbContext();
        private IStaffService _staffProfileService = new StaffService();
        private IFinanceService _financeService = new FinanceService();

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
        private IUserManagerService _userManagerService = new UserManagerService();
        private IPostService _postService = new PostService();
        private IAssignmentService _assignmentService = new AssignmentService();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private HttpClient client = new HttpClient();
        private IPaymentAmountService _paymentAmountService = new PaymentAmountService();


        public PanelController()
        {
            //client.BaseAddress = new Uri("http://localhost:58920/");
            //client.BaseAddress = new Uri("http://cbttest.iskools.com/");
            //client.BaseAddress = new Uri("http://cbt.iskools.com/");
            var baseUrl = db.Settings.FirstOrDefault().CBTLink;
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }
        public PanelController(StaffService staffProfileService,
            ClassLevelService classLevelService,
            PostService postService,
            PaymentAmountService paymentAmountService,
            ResultService resultService,
            EnrollmentService enrollmentService,
            StudentProfileService studentProfileService,
            SubjectService subjectService,
            SessionService sessionService,
            EnrolledSubjectService enrolledSubjectService,
            SettingService settingService,
            PublishResultService publishResultService,
            ImageService imageService,
            UserManagerService userManagerService,
            AssignmentService assignmentService, ApplicationUserManager userManager, ApplicationSignInManager signInManager,
                   FinanceService financeService

            )
        {
            _userManagerService = userManagerService;
            _imageService = imageService;
            _staffProfileService = staffProfileService;
            _classLevelService = classLevelService;
            _postService = postService;
            _resultService = resultService;
            _enrollmentService = enrollmentService;
            _studentProfileService = studentProfileService;
            _subjectService = subjectService;
            _sessionService = sessionService;
            _enrolledSubjectService = enrolledSubjectService;
            _settingService = settingService;
            _publishResultService = publishResultService;
            _assignmentService = assignmentService;
            _financeService = financeService;
            UserManager = userManager;
            SignInManager = signInManager;
            _paymentAmountService = paymentAmountService;

        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        #endregion
        // GET: Student/Panel

        #region Index
        public async Task<ActionResult> Index()
        {

            return View();
        }

        public async Task<ActionResult> Mydocuments(int sessionId = 0)
        {
            int sid = 0;
            if (sessionId > 0)
            {
                var Session = await db.Sessions.FirstOrDefaultAsync(x => x.Id == sessionId);
                sid = Session.Id;
            }
            else
            {
                var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
                sid = currentSession.Id;
            }
            var output = await db.Incomes.ToListAsync();

            ViewBag.Income = output.ToList();


            ViewBag.sid = sid;
            var userid = User.Identity.GetUserId();
            var studentId = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.UserId == userid);
            var enroment = await db.Enrollments.FirstOrDefaultAsync(x => x.SessionId == sid && x.StudentProfileId == studentId.Id);
            return View();
        }

        public async Task<ActionResult> Mycertificate(int sessionId = 0)
        {
            int sid = 0;
            if (sessionId > 0)
            {
                var Session = await db.Sessions.FirstOrDefaultAsync(x => x.Id == sessionId);
                sid = Session.Id;
            }
            else
            {
                var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
                sid = currentSession.Id;
            }
            var output = await db.Incomes.ToListAsync();

            ViewBag.Income = output.ToList();


            ViewBag.sid = sid;
            var userid = User.Identity.GetUserId();
            var studentId = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.UserId == userid);
            var enroment = await db.Enrollments.FirstOrDefaultAsync(x => x.SessionId == sid && x.StudentProfileId == studentId.Id);
            return View();
        }

        public async Task<ActionResult> Personaltimetable(int sessionId = 0)
        {
            int sid = 0;
            if (sessionId > 0)
            {
                var Session = await db.Sessions.FirstOrDefaultAsync(x => x.Id == sessionId);
                sid = Session.Id;
            }
            else
            {
                var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
                sid = currentSession.Id;
            }
            var output = await db.Incomes.ToListAsync();

            ViewBag.Income = output.ToList();


            ViewBag.sid = sid;
            var userid = User.Identity.GetUserId();
            var studentId = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.UserId == userid);
            var enroment = await db.Enrollments.FirstOrDefaultAsync(x => x.SessionId == sid && x.StudentProfileId == studentId.Id);
            return View();
        }


        public async Task<ActionResult> SchoolCalender(int sessionId = 0)
        {
            int sid = 0;
            if (sessionId > 0)
            {
                var Session = await db.Sessions.FirstOrDefaultAsync(x => x.Id == sessionId);
                sid = Session.Id;
            }
            else
            {
                var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
                sid = currentSession.Id;
            }
            var output = await db.Incomes.ToListAsync();

            ViewBag.Income = output.ToList();


            ViewBag.sid = sid;
            var userid = User.Identity.GetUserId();
            var studentId = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.UserId == userid);
            var enroment = await db.Enrollments.FirstOrDefaultAsync(x => x.SessionId == sid && x.StudentProfileId == studentId.Id);
            return View();
        }


        public async Task<ActionResult> Myclassmates(int sessionId = 0)
        {
            int sid = 0;
            if (sessionId > 0)
            {
                var Session = await db.Sessions.FirstOrDefaultAsync(x => x.Id == sessionId);
                sid = Session.Id;
            }
            else
            {
                var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
                sid = currentSession.Id;
            }
            var output = await db.Incomes.ToListAsync();

            ViewBag.Income = output.ToList();


            ViewBag.sid = sid;
            var userid = User.Identity.GetUserId();
            var studentId = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.UserId == userid);
            var enroment = await db.Enrollments.FirstOrDefaultAsync(x => x.SessionId == sid && x.StudentProfileId == studentId.Id);
            return View();
        }


        public async Task<ActionResult> Myteachers(int sessionId = 0)
        {
            int sid = 0;
            if (sessionId > 0)
            {
                var Session = await db.Sessions.FirstOrDefaultAsync(x => x.Id == sessionId);
                sid = Session.Id;
            }
            else
            {
                var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
                sid = currentSession.Id;
            }
            //var income = await _paymentAmountService.StudentAmountListBySession(sid);

            //var output = income.Select(x => new PaymentListByClassDto
            //{
            //    Id = x.Income.Id,
            //    Title = x.Income.Title,
            //    Description = x.Income.Title + " (" + x.Amount + ")",
            //    Amount = x.Amount
            //});
            //ViewBag.Income = output.ToList();


            ViewBag.sid = sid;
            var userid = User.Identity.GetUserId();
            var studentId = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.UserId == userid);
            var enroment = await db.Enrollments.FirstOrDefaultAsync(x => x.SessionId == sid && x.StudentProfileId == studentId.Id);
            return View();
        }


        public async Task<ActionResult> MakePayment(int sessionId = 0)
        {
            int sid = 0;
            if (sessionId > 0)
            {
                var Session = await db.Sessions.FirstOrDefaultAsync(x => x.Id == sessionId);
                sid = Session.Id;
            }
            else
            {
                var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
                sid = currentSession.Id;
            }
            var income = await db.Incomes.ToListAsync();

            var output = income.Select(x => new PaymentListByClassDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Title + " (" + x.Amount + ")",
                Amount = x.Amount
            });
            ViewBag.Income = output.ToList();


            ViewBag.sid = sid;
            var userid = User.Identity.GetUserId();
            var studentId = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.UserId == userid);
            var enroment = await db.Enrollments
                 .Include(x => x.ClassLevel)
                                                      .Include(x => x.ClassLevel)
                .FirstOrDefaultAsync(x => x.SessionId == sid && x.StudentProfileId == studentId.Id);
            ViewBag.eid = studentId.Id;
            IQueryable<FinanceInitializer> pfinance = from s in db.FinanceInitializers
                                                     .Include(x => x.Income)
                                                     .Include(x => x.Enrollment)
                                                     .Where(x => x.EnrollmentId == enroment.Id && x.SessionId == sid)
                                                     .Where(x => x.TransactionStatus == TransactionStatus.Pending)
                                                      select s;
           // ViewBag.totalFee = enroment.ClassLevel.PaymentAmounts.Sum(x=>x.Amount);
            ViewBag.FeePaid = enroment.Finances.Sum(x => x.Amount);
            ViewBag.FeeBalance = ViewBag.totalFee - ViewBag.FeePaid;
            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session.OrderByDescending(x => x.FullSession), "Id", "FullSession");

            if (pfinance.Count() > 0)
            {
                ViewBag.total = await pfinance.SumAsync(x => x.Amount);
            }
            ViewBag.finance = pfinance;
            ViewBag.cid = enroment.ClassLevelId;
            return View();
        }
        public async Task<ActionResult> Profile()
        {
            try
            {
                var profile = await _studentProfileService.GetStudentWithoutId();
                ViewBag.student = profile;

                var studentimage = await _imageService.Get(profile.ImageId);
                if (studentimage != null)
                {
                    ViewBag.img = studentimage.ImageContent;
                }
                var sub = await _studentProfileService.SubjectCountStudent();
                ViewBag.SubjectCount = sub;

                var post = await _postService.ListPost(7);
                ViewBag.post = post;

                var clas = await _studentProfileService.ClassNameForStudent();
                ViewBag.Class = clas;


                string unixconverify = User.Identity.Name;
                string xgink = GeneralService.PortalLink();
                string role = "student";
                var userId = User.Identity.GetUserId();
                var student = db.StudentProfiles.Include(x => x.user).Where(x => x.UserId == userId).FirstOrDefault();

                var defaulter = db.Defaulters.Include(x => x.StudentProfile).Where(x => x.ProfileId == student.Id).FirstOrDefault();
                ViewBag.defaulter = defaulter;

                HttpResponseMessage response = client.GetAsync("/api/ExamBoardApi/GetStudentExam?unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role + "&regNumber=" + student.StudentRegNumber + "&UserName=" + student.user.UserName).Result;
                var response2 = response.Content.ReadAsStringAsync().Result;
                List<CBTExaminationDto> data = JsonConvert.DeserializeObject<List<CBTExaminationDto>>(response2);
                ViewBag.data = data;

            }
            catch (Exception ex)
            {
                var a = ex;
            }
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> LoginAccess(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }


        //[HttpPost]
        //[AllowAnonymous]
        //////[ValidateAntiForgeryToken]
        //public async Task<ActionResult> LoginAccess(LoginAccessDto model, string returnUrl)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var student = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.StudentRegNumber == model.Regnumber && x.SecurityAnswer.ToLower() == model.SecurityAnswer.ToLower());
        //    if (student == null)
        //    {
        //        TempData["error"] = "Invalid Registartion Number or Security Answer.";
        //        return View();
        //    }

        //    var user = await UserManager.FindByIdAsync(student.UserId);
        //    var result = SignInManager.SignInAsync(user, true, false);

        //    // This doesn't count login failures towards account lockout
        //    // To enable password failures to trigger account lockout, change to shouldLockout: true
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            if (user.IsLocked != true)
        //            {
        //                if (user.Status == Models.Entities.EntityStatus.NotActive)
        //                {
        //                    return RedirectToAction("UserLockout", new { userid = user.Id });
        //                }
        //                else
        //                {



        //                    if (returnUrl != null)
        //                    {
        //                        return Redirect(returnUrl);
        //                    }
        //                    else
        //                    {
        //                        if (await SignInManager.UserManager.IsInRoleAsync(user.Id, "Student"))
        //                        {
        //                            return RedirectToAction("Index", "Panel", new { area = "Student" });
        //                        }
        //                        else if (await SignInManager.UserManager.IsInRoleAsync(user.Id, "Staff"))
        //                        {
        //                            return RedirectToAction("Index", "Panel", new { area = "Staff" });
        //                        }
        //                        else
        //                        {
        //                            TempData["error"] = "Try Again or Contact Your Administration";
        //                            return RedirectToAction("Login", "Account", new { area = "" });
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                return RedirectToAction("UserLockout", new { userid = user.Id });
        //            }

        //        case SignInStatus.LockedOut:
        //            return RedirectToAction("UserLockout", new { userid = user.Id });

        //        case SignInStatus.Failure:
        //        default:
        //            //ModelState.AddModelError("", "Invalid login attempt.");
        //            string messages = string.Join("; ", ModelState.Values
        //                                .SelectMany(x => x.Errors)
        //                                .Select(x => x.ErrorMessage));
        //            TempData["error"] = "incorrect username or password, " + messages;
        //            return View(model);
        //    }

        //}








        #endregion

        #region
        // GET: Content/Posts/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: Content/Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        ////[ValidateAntiForgeryToken]
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
            var items = await _postService.StudentPost(searchString, currentFilter, page);

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
        ////[ValidateAntiForgeryToken]
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
        ////[ValidateAntiForgeryToken]
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
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCommentConfirmed(int id)
        {

            await _postService.DeleteComment(id);
            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateComment(Comment model, int id)
        {
            if (ModelState.IsValid)
            {

                model.Username = User.Identity.Name;
                model.DateCommented = DateTime.UtcNow.AddHours(1);
                model.PostId = id;
                await _postService.CreateComment(model);

                return RedirectToAction("Details", new { id = id });
            }

            return View(model);
        }

        #endregion


        #region Result
        [AllowAnonymous]
        //public async Task<ActionResult> Result(string skip)
        public async Task<ActionResult> Result()
        {
            var sett = db.Settings.FirstOrDefault();
            if(sett.DisableAllResultPrinting == true)
            {
                return RedirectToAction("/Verify");
            }

            var session = await _sessionService.GetAllSession();

            ViewBag.sessionId = new SelectList(session, "Id", "FullSession");
            //ViewBag.skip = skip;
            
            ViewBag.name = sett.SchoolName;
            if (User.IsInRole("Student"))
            {
                var user = await _resultService.GetUserByUserId(User.Identity.GetUserId());
                ViewBag.regnumber = user.StudentRegNumber;
            }

            return View();
        }
            [AllowAnonymous]
        //public async Task<ActionResult> Result(string skip)
        public async Task<ActionResult> Verify()
        {
           var sett = db.Settings.FirstOrDefault();
           
            ViewBag.notep = sett.DisableAllResultPrintingNote;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //public async Task<ActionResult> Result(CheckResultViewModelDto model, string skip)
        public async Task<ActionResult> Result(CheckResultViewModelDto model)
        {
            var sessionlist = await _sessionService.GetAllSession();
            TempData["validator"] = "asdflkjhgqwerpoiu";

            if (ModelState.IsValid)
            {
                try
                {
                    var userProfile = await _resultService.GetUserByRegNumber(model.StudentPIN);
                    if (userProfile == null)
                    {
                        ViewBag.Error = GeneralService.StudentorPupil() + " Wrong Registration Number.";
                        ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);

                        return View(model);
                    }
                    ///check if profile is complete
                    //
                    //if (skip == null)
                    //{
                    //    var usercompleteprofile = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.StudentRegNumber == userProfile.StudentRegNumber);
                    //    if (usercompleteprofile.IsUpdated == false)
                    //    {
                    //        if (string.IsNullOrEmpty(usercompleteprofile.AboutMe) || string.IsNullOrEmpty(usercompleteprofile.ParentGuardianOccupation) || string.IsNullOrEmpty(usercompleteprofile.user.Gender) || string.IsNullOrEmpty(usercompleteprofile.user.StateOfOrigin) || string.IsNullOrEmpty(usercompleteprofile.user.LocalGov))
                    //        {
                    //            return RedirectToAction("UpdateProfile", new { id = usercompleteprofile.Id });
                    //        }
                    //    }
                    //}

                    var session = await _resultService.GetSessionBySessionId(model.SessionId);
                    var enrollment = await _resultService.GetEnrollmentBySessIdStudentProfileId(model.SessionId, userProfile.Id);
                    var checkArchive = await db.ArchiveResults.FirstOrDefaultAsync(x => x.ClassLevelId == enrollment.ClassLevelId && x.SessionId == model.SessionId);
                    var checkTotalCA = await db.EnrolledSubjects.Include(x => x.Enrollments).FirstOrDefaultAsync(x => x.Enrollments.StudentProfileId == userProfile.Id && x.Enrollments.SessionId == session.Id);
                    //
                    //Check debt profile
                    if (enrollment == null)
                    {
                        ViewBag.Error =
                            "You Were Not Enrolled In This Session/Term. Contact Your Form Teacher";
                        ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                        return View(model);
                    }
                    var pinCode = await _resultService.PinCodeByPinNumberAndSerialNumber(model.PinNumber, model.SerialNumber);
                    var checkResultStatus = await _resultService.PublishResultBySessIdAndClassId(enrollment.SessionId, enrollment.ClassLevelId);
                    var checkDebtProfile = await _resultService.GetDefaulterByProfileId(userProfile.Id);
                    var PrintedStatus = await _enrollmentService.Get(enrollment.Id);
                    //Check debt profile
                    if (checkDebtProfile != null)
                    {
                        ViewBag.Error =
                            "You can not check your result. You may still be indebted to the school. Please Contact school Administrator for further details.";
                        ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                        return View(model);
                    }

                    //check if Student PIN is correct
                    if (userProfile == null)
                    {
                        ViewBag.Error = "The " + GeneralService.StudentorPupil() + "Registration Number does not exist. Please try again.";
                        ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                        return View(model);
                    }
                    if (checkResultStatus == null)
                    {
                        ViewBag.Error = "Your Result is not yet ready. Please try again later.";
                        ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                        return View(model);
                    }

                    if (enrollment == null)
                    {
                        ViewBag.Error = "You don't have a Result for the Session/Term you chose.";
                        ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                        return View(model);
                    }
                    var sett = db.Settings.FirstOrDefault();

                    //check if PIN Number exists
                    if (model.PinNumber == "098765098765" && model.SerialNumber == "654321")
                    {
                        if (sett.PrintOutOption == PrintOutOption.PrintOutOne)
                        {
                            if (checkArchive != null)
                            {
                                if (sett.ShowCumulativeResultForThirdTerm == false)
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult", "ArchiveResult", new { id = enrollment.Id });
                                }
                                else
                                {
                                    if (!session.Term.Contains("Third"))
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult", "ArchiveResult", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        return RedirectToAction("PrintThirdResult", "ArchiveResult",
                                            new { id = enrollment.Id, sess = session.Id });
                                    }
                                }
                            }
                            else
                            {
                                if (sett.ShowCumulativeResultForThirdTerm == false)
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                }
                                else
                                {
                                    if (!session.Term.Contains("Third"))
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        return RedirectToAction("PrintThirdResult",
                                            new { id = enrollment.Id, sess = session.Id });
                                    }
                                }
                            }

                        }
                        else if (sett.PrintOutOption == PrintOutOption.PrintOutTwo)
                        {
                            if (checkArchive != null)
                            {
                                if (sett.ShowCumulativeResultForThirdTerm == false)
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult2", "ArchiveResult", new { id = enrollment.Id });
                                }
                                else
                                {
                                    if (!session.Term.Contains("Third"))
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult2", "ArchiveResult", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        return RedirectToAction("PrintThirdResult2", "ArchiveResult",
                                            new { id = enrollment.Id, sess = session.Id });
                                    }
                                }
                            }
                            else
                            {
                                if (sett.ShowCumulativeResultForThirdTerm == false)
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                }
                                else
                                {
                                    if (!session.Term.Contains("Third"))
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        return RedirectToAction("PrintThirdResult2",
                                            new { id = enrollment.Id, sess = session.Id });
                                    }
                                }
                            }


                        }
                        else if (sett.PrintOutOption == PrintOutOption.PrintOutThree)
                        {
                            if (checkArchive != null)
                            {
                                if (sett.ShowCumulativeResultForThirdTerm == false)
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult3", "ArchiveResult", new { id = enrollment.Id });
                                }
                                else
                                {
                                    if (!session.Term.Contains("Third"))
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult3", "ArchiveResult", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        return RedirectToAction("PrintThirdResult3", "ArchiveResult",
                                            new { id = enrollment.Id, sess = session.Id });
                                    }
                                }
                            }
                            else if (checkTotalCA.TotalCA == null || checkTotalCA.TotalCA == 0)
                            {
                                if (sett.ShowCumulativeResultForThirdTerm == false)
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                }
                                else
                                {
                                    if (!session.Term.Contains("Third"))
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        return RedirectToAction("PrintThirdResult2",
                                            new { id = enrollment.Id, sess = session.Id });
                                    }
                                }
                            }
                            else
                            {
                                if (sett.ShowCumulativeResultForThirdTerm == false)
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult3", new { id = enrollment.Id });
                                }
                                else
                                {
                                    if (!session.Term.Contains("Third"))
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult3", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        return RedirectToAction("PrintThirdResult3",
                                            new { id = enrollment.Id, sess = session.Id });
                                    }
                                }
                            }


                        }
                        else
                        {
                            if (checkArchive != null)
                            {
                                if (sett.ShowCumulativeResultForThirdTerm == false)
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult", "ArchiveResult", new { id = enrollment.Id });
                                }
                                else
                                {
                                    if (!session.Term.Contains("Third"))
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult", "ArchiveResult", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        return RedirectToAction("PrintThirdResult", "ArchiveResult",
                                            new { id = enrollment.Id, sess = session.Id });
                                    }
                                }
                            }
                            else
                            {
                                if (sett.ShowCumulativeResultForThirdTerm == false)
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                }
                                else
                                {
                                    if (!session.Term.Contains("Third"))
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        return RedirectToAction("PrintThirdResult",
                                            new { id = enrollment.Id, sess = session.Id });
                                    }
                                }

                            }

                        }

                    }
                    if (pinCode == null)
                    {
                        ViewBag.Error = "Invalid PIN. Please check the PIN Number and Serial Number and then try again.";
                        ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                        return View(model);
                    }
                    else if (pinCode != null)
                    {
                        //.//
                        var termcondition = await db.Settings.FirstOrDefaultAsync();

                        //if (sett.PinValidOption == PinValidOption.Termly)
                        //{

                        //Check if it has been used by another user
                        if (pinCode.StudentPin != userProfile.StudentRegNumber && pinCode.StudentPin != null)
                        {
                            ViewBag.Error = "The PIN has been used by another user.";
                            ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);

                            return View(model);
                        }
                    
                    //Check If it has been used by you for another term
                    else if (pinCode.StudentPin == userProfile.StudentRegNumber && pinCode.EnrollmentId == enrollment.Id && pinCode.EnrollmentId != null && sett.PinValidOption == PinValidOption.Termly)
                        {

                            if (session != null)
                            {
                                //if (pinCode.EnrollmentId == enrollment.Id)
                                //{
                                //    ViewBag.Error = "The PIN has been used by you for another term.";
                                //    ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                                //    return View(model);
                                //}
                                if (pinCode.EnrollmentId == enrollment.Id && sett.PinValidOption == PinValidOption.Termly && pinCode.Usage <= 0)
                                {
                                    ViewBag.Error = "Exceeded PIN Usage.";
                                    ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                                    return View(model);
                                }
                                else
                                {
                                    if (pinCode.Usage > 0)
                                    {
                                        pinCode.Usage = pinCode.Usage - 1;
                                        await _resultService.Update();
                                    }

                                    //Update Printed Status
                                    PrintedStatus.Printed = true;
                                    await _enrollmentService.Edit(PrintedStatus);

                                    if (sett.PrintOutOption == PrintOutOption.PrintOutOne)
                                    {
                                        if (checkArchive != null)
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult", "ArchiveResult", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult", "ArchiveResult", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult", "ArchiveResult",
                                                        new { id = enrollment.Id, sess = session.Id });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult",
                                                        new { id = enrollment.Id, sess = session.Id });
                                                }
                                            }
                                        }

                                    }
                                    else if (sett.PrintOutOption == PrintOutOption.PrintOutTwo)
                                    {
                                        if (checkArchive != null)
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult2", "ArchiveResult", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult2", "ArchiveResult", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult2", "ArchiveResult",
                                                        new { id = enrollment.Id, sess = session.Id });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult2",
                                                        new { id = enrollment.Id, sess = session.Id });
                                                }
                                            }
                                        }

                                    }

                                    else if (sett.PrintOutOption == PrintOutOption.PrintOutThree)
                                    {
                                        if (checkArchive != null)
                                        {

                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult3", "ArchiveResult", new { id = enrollment.Id });
                                            }

                                            else
                                            {
                                                if (!session.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult3", "ArchiveResult", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult3", "ArchiveResult",
                                                        new { id = enrollment.Id, sess = session.Id });
                                                }
                                            }
                                        }
                                        else if (checkTotalCA.TotalCA == null || checkTotalCA.TotalCA == 0)
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult2",
                                                        new { id = enrollment.Id, sess = session.Id });
                                                }
                                            }
                                        }
                                        else
                                        {

                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult3", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult3", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult3",
                                                        new { id = enrollment.Id, sess = session.Id });
                                                }
                                            }
                                        }

                                    }

                                }
                            }
                        }

                        //Check if it has been used by you for another session
                        else if (pinCode.StudentPin == userProfile.StudentRegNumber && pinCode.EnrollmentId == enrollment.Id && pinCode.EnrollmentId != null && sett.PinValidOption == PinValidOption.Yearly)
                        {

                            if (session != null)
                            {
                                //if (session.SessionYear != selectedYear.SessionYear)
                                //if (pinCode.EnrollmentId == enrollment.Id)
                                //{
                                //    ViewBag.Error = "The PIN has been used by you for another Session.";
                                //    ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                                //    return View(model);
                                //}
                                if (pinCode.EnrollmentId == enrollment.Id && pinCode.Usage <= 0 && sett.PinValidOption == PinValidOption.Yearly)
                                {
                                    ViewBag.Error = "The PIN is no longer active. You have used it 5 times in another Session.";
                                    ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                                    return View(model);
                                }
                                else
                                {
                                    if (pinCode.Usage > 0)
                                    {
                                        pinCode.Usage = pinCode.Usage - 1;
                                        await _resultService.Update();
                                    }

                                    //Update Printed Status
                                    PrintedStatus.Printed = true;
                                    await _enrollmentService.Edit(PrintedStatus);


                                    if (sett.PrintOutOption == PrintOutOption.PrintOutOne)
                                    {
                                        if (checkArchive != null)
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult", "ArchiveResult", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult", "ArchiveResult", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult", "ArchiveResult",
                                                        new { id = enrollment.Id, sess = session.Id });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult",
                                                        new { id = enrollment.Id, sess = session.Id });
                                                }
                                            }
                                        }

                                    }
                                    else if (sett.PrintOutOption == PrintOutOption.PrintOutTwo)
                                    {
                                        if (checkArchive != null)
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult2", "ArchiveResult", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult2", "ArchiveResult", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult2", "ArchiveResult",
                                                        new { id = enrollment.Id, sess = session.Id });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult2",
                                                        new { id = enrollment.Id, sess = session.Id });
                                                }
                                            }
                                        }


                                    }

                                    else if (sett.PrintOutOption == PrintOutOption.PrintOutThree)
                                    {
                                        if (checkArchive != null)
                                        {

                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult3", "ArchiveResult", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult3", "ArchiveResult", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult3", "ArchiveResult",
                                                        new { id = enrollment.Id, sess = session.Id });
                                                }
                                            }
                                        }
                                        else if (checkTotalCA.TotalCA == null || checkTotalCA.TotalCA == 0)
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult2",
                                                        new { id = enrollment.Id, sess = session.Id });
                                                }
                                            }
                                        }
                                        else
                                        {

                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult3", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult3", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult3",
                                                        new { id = enrollment.Id, sess = session.Id });
                                                }
                                            }
                                        }


                                    }

                                }
                            }
                        }
                        else
                        {
                            if (pinCode.StudentPin == null)
                            {
                                pinCode.StudentPin = userProfile.StudentRegNumber;
                                pinCode.EnrollmentId = enrollment.Id;
                                pinCode.SessionId = enrollment.SessionId;
                            }
                            else if (pinCode.EnrollmentId == enrollment.Id && pinCode.Usage <= 0)
                            {
                                ViewBag.Error = "The PIN is no longer active. You have used it 2 times.";
                                ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                                return View(model);
                            }
                            else if (pinCode.EnrollmentId != enrollment.Id)
                            {
                                ViewBag.Error = "The PIN has been used by you for another term.";
                                ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                                return View(model);
                            }
                            if (pinCode.Usage > 0)
                            {
                                pinCode.Usage = pinCode.Usage - 1;
                                await _resultService.Update();
                            }
                            else
                            {
                                ViewBag.Error = "Exceeded PIN Usage.";
                                ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                                return View(model);
                            }

                            //Update Printed Status
                            PrintedStatus.Printed = true;
                            await _enrollmentService.Edit(PrintedStatus);

                            if (sett.PrintOutOption == PrintOutOption.PrintOutOne)
                            {
                                if (checkArchive != null)
                                {
                                    if (sett.ShowCumulativeResultForThirdTerm == false)
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult", "ArchiveResult", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        if (!session.Term.Contains("Third"))
                                        {
                                            TempData["Verified"] = "True";
                                            return RedirectToAction("PrintResult", "ArchiveResult", new { id = enrollment.Id });
                                        }
                                        else
                                        {
                                            return RedirectToAction("PrintThirdResult", "ArchiveResult",
                                                new { id = enrollment.Id, sess = session.Id });
                                        }
                                    }
                                }
                                else
                                {
                                    if (sett.ShowCumulativeResultForThirdTerm == false)
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        if (!session.Term.Contains("Third"))
                                        {
                                            TempData["Verified"] = "True";
                                            return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                        }
                                        else
                                        {
                                            return RedirectToAction("PrintThirdResult",
                                                new { id = enrollment.Id, sess = session.Id });
                                        }
                                    }
                                }

                            }
                            else if (sett.PrintOutOption == PrintOutOption.PrintOutTwo)
                            {
                                if (checkArchive != null)
                                {
                                    if (sett.ShowCumulativeResultForThirdTerm == false)
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult2", "ArchiveResult", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        if (!session.Term.Contains("Third"))
                                        {
                                            TempData["Verified"] = "True";
                                            return RedirectToAction("PrintResult2", "ArchiveResult", new { id = enrollment.Id });
                                        }
                                        else
                                        {
                                            return RedirectToAction("PrintThirdResult2", "ArchiveResult",
                                                new { id = enrollment.Id, sess = session.Id });
                                        }
                                    }
                                }
                                else
                                {
                                    if (sett.ShowCumulativeResultForThirdTerm == false)
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        if (!session.Term.Contains("Third"))
                                        {
                                            TempData["Verified"] = "True";
                                            return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                        }
                                        else
                                        {
                                            return RedirectToAction("PrintThirdResult2",
                                                new { id = enrollment.Id, sess = session.Id });
                                        }
                                    }
                                }


                            }

                            else if (sett.PrintOutOption == PrintOutOption.PrintOutThree)
                            {
                                if (checkArchive != null)
                                {
                                    if (checkTotalCA.TotalCA == null || checkTotalCA.TotalCA == 0)
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult2", "ArchiveResult", new { id = enrollment.Id });
                                    }
                                    else if (sett.ShowCumulativeResultForThirdTerm == false)
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult3", "ArchiveResult", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        if (!session.Term.Contains("Third"))
                                        {
                                            TempData["Verified"] = "True";
                                            return RedirectToAction("PrintResult3", "ArchiveResult", new { id = enrollment.Id });
                                        }
                                        else
                                        {
                                            return RedirectToAction("PrintThirdResult3", "ArchiveResult",
                                                new { id = enrollment.Id, sess = session.Id });
                                        }
                                    }
                                }
                                else if (checkTotalCA.TotalCA == null || checkTotalCA.TotalCA == 0)
                                {
                                    if (sett.ShowCumulativeResultForThirdTerm == false)
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        if (!session.Term.Contains("Third"))
                                        {
                                            TempData["Verified"] = "True";
                                            return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                        }
                                        else
                                        {
                                            return RedirectToAction("PrintThirdResult2",
                                                new { id = enrollment.Id, sess = session.Id });
                                        }
                                    }
                                }
                                else
                                {
                                    if (sett.ShowCumulativeResultForThirdTerm == false)
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult3", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        if (!session.Term.Contains("Third"))
                                        {
                                            TempData["Verified"] = "True";
                                            return RedirectToAction("PrintResult3", new { id = enrollment.Id });
                                        }
                                        else
                                        {
                                            return RedirectToAction("PrintThirdResult3",
                                                new { id = enrollment.Id, sess = session.Id });
                                        }
                                    }
                                }


                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var a = ex;
                    ViewBag.Error = "Unable to check result, Contact your Teacher.";
                }

            }

            ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
            return View(model);
        }


        [AllowAnonymous]
        //public async Task<ActionResult> Result(string skip)
        public async Task<ActionResult> ResultWithoutId()
        {

            //ViewBag.skip = skip;
            var sett = db.Settings.FirstOrDefault();
            ViewBag.name = sett.SchoolName;
            if (User.IsInRole("Student"))
            {
                var user = await _resultService.GetUserByUserId(User.Identity.GetUserId());
                ViewBag.regnumber = user.StudentRegNumber;
            }

            var session = await _sessionService.GetAllSession();

            ViewBag.sessionId = new SelectList(session, "Id", "FullSession");

            var classLevel = await _classLevelService.ClassLevelList();
            ViewBag.classId = new SelectList(classLevel, "Id", "ClassLevelName");

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //public async Task<ActionResult> Result(CheckResultViewModelDto model, string skip)
        public async Task<ActionResult> ResultWithoutId(CheckResultViewModelDto2 model)
        {
            var sessionlist = await _sessionService.GetAllSession();
            TempData["validator"] = "asdflkjhgqwerpoiu";

            if (ModelState.IsValid)
            {
                try
                {
                    var getReg = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.Id == model.StudentProfileId);
                    var userProfile = await _resultService.GetUserByRegNumber(getReg.StudentRegNumber);
                    if (userProfile == null)
                    {
                        ViewBag.Error = GeneralService.StudentorPupil() + " User Does Not Exist.";

                        var session1 = await _sessionService.GetAllSession();

                        ViewBag.sessionId = new SelectList(session1, "Id", "FullSession");

                        var classLevel1 = await _classLevelService.ClassLevelList();
                        ViewBag.classId = new SelectList(classLevel1, "Id", "ClassLevelName");
                        //ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);

                        return View(model);
                    }
                    ///check if profile is complete
                    //
                    //if (skip == null)
                    //{
                    //    var usercompleteprofile = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.StudentRegNumber == userProfile.StudentRegNumber);
                    //    if (usercompleteprofile.IsUpdated == false)
                    //    {
                    //        if (string.IsNullOrEmpty(usercompleteprofile.AboutMe) || string.IsNullOrEmpty(usercompleteprofile.ParentGuardianOccupation) || string.IsNullOrEmpty(usercompleteprofile.user.Gender) || string.IsNullOrEmpty(usercompleteprofile.user.StateOfOrigin) || string.IsNullOrEmpty(usercompleteprofile.user.LocalGov))
                    //        {
                    //            return RedirectToAction("UpdateProfile", new { id = usercompleteprofile.Id });
                    //        }
                    //    }
                    //}

                    var session2 = await _resultService.GetSessionBySessionId(model.SessionId);
                    var enrollment = await _resultService.GetEnrollmentBySessIdStudentProfileId(model.SessionId, userProfile.Id);
                    var checkArchive = await db.ArchiveResults.FirstOrDefaultAsync(x => x.ClassLevelId == enrollment.ClassLevelId && x.SessionId == model.SessionId);
                    var checkTotalCA = await db.EnrolledSubjects.Include(x => x.Enrollments).FirstOrDefaultAsync(x => x.Enrollments.StudentProfileId == userProfile.Id && x.Enrollments.SessionId == session2.Id);
                    ///
                     //Check debt profile
                    if (enrollment == null)
                    {
                        ViewBag.Error =
                            "You Were Not Enrolled In This Session/Term. Contact Your Form Teacher";

                        var session1 = await _sessionService.GetAllSession();

                        ViewBag.sessionId = new SelectList(session1, "Id", "FullSession");

                        var classLevel1 = await _classLevelService.ClassLevelList();
                        ViewBag.classId = new SelectList(classLevel1, "Id", "ClassLevelName");
                        //ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                        return View(model);
                    }
                    var pinCode = await _resultService.PinCodeByPinNumberAndSerialNumber(model.PinNumber, model.SerialNumber);
                    var checkResultStatus = await _resultService.PublishResultBySessIdAndClassId(enrollment.SessionId, enrollment.ClassLevelId);
                    var checkDebtProfile = await _resultService.GetDefaulterByProfileId(userProfile.Id);
                    var PrintedStatus = await _enrollmentService.Get(enrollment.Id);
                    //Check debt profile
                    if (checkDebtProfile != null)
                    {
                        ViewBag.Error = "You can not check your result. You may still be indebted to the school. Please Contact the Rector for further details.";

                        var session1 = await _sessionService.GetAllSession();

                        ViewBag.sessionId = new SelectList(session1, "Id", "FullSession");

                        var classLevel1 = await _classLevelService.ClassLevelList();
                        ViewBag.classId = new SelectList(classLevel1, "Id", "ClassLevelName");

                        //ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                        return View(model);
                    }

                    //check if Student PIN is correct
                    if (userProfile == null)
                    {
                        ViewBag.Error = "The " + GeneralService.StudentorPupil() + "Registration Number does not exist. Please try again.";

                        var session1 = await _sessionService.GetAllSession();

                        ViewBag.sessionId = new SelectList(session1, "Id", "FullSession");

                        var classLevel1 = await _classLevelService.ClassLevelList();
                        ViewBag.classId = new SelectList(classLevel1, "Id", "ClassLevelName");

                        //ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                        return View(model);
                    }
                    if (checkResultStatus == null)
                    {
                        ViewBag.Error = "Your Result is not yet ready. Please try again later.";

                        var session1 = await _sessionService.GetAllSession();

                        ViewBag.sessionId = new SelectList(session1, "Id", "FullSession");

                        var classLevel1 = await _classLevelService.ClassLevelList();
                        ViewBag.classId = new SelectList(classLevel1, "Id", "ClassLevelName");

                        //ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                        return View(model);
                    }

                    if (enrollment == null)
                    {
                        ViewBag.Error = "You don't have a Result for the Session/Term you chose.";

                        var session1 = await _sessionService.GetAllSession();

                        ViewBag.sessionId = new SelectList(session1, "Id", "FullSession");

                        var classLevel1 = await _classLevelService.ClassLevelList();
                        ViewBag.classId = new SelectList(classLevel1, "Id", "ClassLevelName");

                        //ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                        return View(model);
                    }
                    var sett = db.Settings.FirstOrDefault();

                    //check if PIN Number exists
                    if (model.PinNumber == "000000000000" && model.SerialNumber == "000000")
                    {
                        if (sett.PrintOutOption == PrintOutOption.PrintOutOne)
                        {
                            if (checkArchive != null)
                            {
                                if (sett.ShowCumulativeResultForThirdTerm == false)
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult", "ArchiveResult", new { id = enrollment.Id });
                                }
                                else
                                {
                                    if (!session2.Term.Contains("Third"))
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult", "ArchiveResult", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        return RedirectToAction("PrintThirdResult", "ArchiveResult",
                                            new { id = enrollment.Id, sess = session2.Id });
                                    }
                                }
                            }
                            else
                            {
                                if (sett.ShowCumulativeResultForThirdTerm == false)
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                }
                                else
                                {
                                    if (!session2.Term.Contains("Third"))
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        return RedirectToAction("PrintThirdResult",
                                            new { id = enrollment.Id, sess = session2.Id });
                                    }
                                }
                            }

                        }
                        else if (sett.PrintOutOption == PrintOutOption.PrintOutTwo)
                        {
                            if (checkArchive != null)
                            {
                                if (sett.ShowCumulativeResultForThirdTerm == false)
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult2", "ArchiveResult", new { id = enrollment.Id });
                                }
                                else
                                {
                                    if (!session2.Term.Contains("Third"))
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult2", "ArchiveResult", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        return RedirectToAction("PrintThirdResult2", "ArchiveResult",
                                            new { id = enrollment.Id, sess = session2.Id });
                                    }
                                }
                            }
                            else
                            {
                                if (sett.ShowCumulativeResultForThirdTerm == false)
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                }
                                else
                                {
                                    if (!session2.Term.Contains("Third"))
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        return RedirectToAction("PrintThirdResult2",
                                            new { id = enrollment.Id, sess = session2.Id });
                                    }
                                }
                            }


                        }
                        else if (sett.PrintOutOption == PrintOutOption.PrintOutThree)
                        {
                            if (checkArchive != null)
                            {
                                if (sett.ShowCumulativeResultForThirdTerm == false)
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult3", "ArchiveResult", new { id = enrollment.Id });
                                }
                                else
                                {
                                    if (!session2.Term.Contains("Third"))
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult3", "ArchiveResult", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        return RedirectToAction("PrintThirdResult3", "ArchiveResult",
                                            new { id = enrollment.Id, sess = session2.Id });
                                    }
                                }
                            }
                            else if (checkTotalCA.TotalCA == null || checkTotalCA.TotalCA == 0)
                            {
                                if (sett.ShowCumulativeResultForThirdTerm == false)
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                }
                                else
                                {
                                    if (!session2.Term.Contains("Third"))
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        return RedirectToAction("PrintThirdResult2",
                                            new { id = enrollment.Id, sess = session2.Id });
                                    }
                                }
                            }
                            else
                            {
                                if (sett.ShowCumulativeResultForThirdTerm == false)
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult3", new { id = enrollment.Id });
                                }
                                else
                                {
                                    if (!session2.Term.Contains("Third"))
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult3", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        return RedirectToAction("PrintThirdResult3",
                                            new { id = enrollment.Id, sess = session2.Id });
                                    }
                                }
                            }


                        }
                        else
                        {
                            if (checkArchive != null)
                            {
                                if (sett.ShowCumulativeResultForThirdTerm == false)
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult", "ArchiveResult", new { id = enrollment.Id });
                                }
                                else
                                {
                                    if (!session2.Term.Contains("Third"))
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult", "ArchiveResult", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        return RedirectToAction("PrintThirdResult", "ArchiveResult",
                                            new { id = enrollment.Id, sess = session2.Id });
                                    }
                                }
                            }
                            else
                            {
                                if (sett.ShowCumulativeResultForThirdTerm == false)
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                }
                                else
                                {
                                    if (!session2.Term.Contains("Third"))
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        return RedirectToAction("PrintThirdResult",
                                            new { id = enrollment.Id, sess = session2.Id });
                                    }
                                }

                            }

                        }

                    }
                    if (pinCode == null)
                    {
                        ViewBag.Error =
                            "The PIN does not exist. Please check the PIN Number and Serial Number and then try again.";

                        var session1 = await _sessionService.GetAllSession();

                        ViewBag.sessionId = new SelectList(session1, "Id", "FullSession");

                        var classLevel1 = await _classLevelService.ClassLevelList();
                        ViewBag.classId = new SelectList(classLevel1, "Id", "ClassLevelName");

                        //ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                        return View(model);
                    }
                    else if (pinCode != null)
                    {

                        //Check if it has been used by another user
                        if (pinCode.StudentPin != userProfile.StudentRegNumber && pinCode.StudentPin != null)
                        {
                            ViewBag.Error = "The PIN has been used by another user.";

                            var session1 = await _sessionService.GetAllSession();

                            ViewBag.sessionId = new SelectList(session1, "Id", "FullSession");

                            var classLevel1 = await _classLevelService.ClassLevelList();
                            ViewBag.classId = new SelectList(classLevel1, "Id", "ClassLevelName");

                            //ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);

                            return View(model);
                        }
                        //Check If it has been used by you for another term
                        else if (pinCode.StudentPin == userProfile.StudentRegNumber && pinCode.EnrollmentId == enrollment.Id && pinCode.EnrollmentId != null && sett.PinValidOption == PinValidOption.Termly)
                        {

                            if (session2 != null)
                            {
                                //if (pinCode.EnrollmentId == enrollment.Id)
                                //{
                                //    ViewBag.Error = "The PIN has been used by you for another term.";
                                //    ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                                //    return View(model);
                                //}
                                if (pinCode.EnrollmentId == enrollment.Id && sett.PinValidOption == PinValidOption.Termly && pinCode.Usage <= 0)
                                {
                                    ViewBag.Error = "The PIN is no longer active. You have used it in another term.";

                                    var session1 = await _sessionService.GetAllSession();

                                    ViewBag.sessionId = new SelectList(session1, "Id", "FullSession");

                                    var classLevel1 = await _classLevelService.ClassLevelList();
                                    ViewBag.classId = new SelectList(classLevel1, "Id", "ClassLevelName");

                                    //ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                                    return View(model);
                                }
                                else
                                {
                                    if (pinCode.Usage > 0)
                                    {
                                        pinCode.Usage = pinCode.Usage - 1;
                                        await _resultService.Update();
                                    }

                                    //Update Printed Status
                                    PrintedStatus.Printed = true;
                                    await _enrollmentService.Edit(PrintedStatus);

                                    if (sett.PrintOutOption == PrintOutOption.PrintOutOne)
                                    {
                                        if (checkArchive != null)
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult", "ArchiveResult", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session2.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult", "ArchiveResult", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult", "ArchiveResult",
                                                        new { id = enrollment.Id, sess = session2.Id });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session2.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult",
                                                        new { id = enrollment.Id, sess = session2.Id });
                                                }
                                            }
                                        }

                                    }
                                    else if (sett.PrintOutOption == PrintOutOption.PrintOutTwo)
                                    {
                                        if (checkArchive != null)
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult2", "ArchiveResult", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session2.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult2", "ArchiveResult", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult2", "ArchiveResult",
                                                        new { id = enrollment.Id, sess = session2.Id });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session2.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult2",
                                                        new { id = enrollment.Id, sess = session2.Id });
                                                }
                                            }
                                        }

                                    }

                                    else if (sett.PrintOutOption == PrintOutOption.PrintOutThree)
                                    {
                                        if (checkArchive != null)
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult3", "ArchiveResult", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session2.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult3", "ArchiveResult", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult3", "ArchiveResult",
                                                        new { id = enrollment.Id, sess = session2.Id });
                                                }
                                            }
                                        }
                                        else if (checkTotalCA.TotalCA == null || checkTotalCA.TotalCA == 0)
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session2.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult2",
                                                        new { id = enrollment.Id, sess = session2.Id });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult3", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session2.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult3", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult3",
                                                        new { id = enrollment.Id, sess = session2.Id });
                                                }
                                            }
                                        }

                                    }

                                }
                            }
                        }

                        //Check if it has been used by you for another session
                        else if (pinCode.StudentPin == userProfile.StudentRegNumber && pinCode.EnrollmentId == enrollment.Id && pinCode.EnrollmentId != null && sett.PinValidOption == PinValidOption.Yearly)
                        {

                            if (session2 != null)
                            {
                                //if (session.SessionYear != selectedYear.SessionYear)
                                //if (pinCode.EnrollmentId == enrollment.Id)
                                //{
                                //    ViewBag.Error = "The PIN has been used by you for another Session.";
                                //    ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                                //    return View(model);
                                //}
                                if (pinCode.EnrollmentId == enrollment.Id && pinCode.Usage <= 0 && sett.PinValidOption == PinValidOption.Yearly)
                                {
                                    ViewBag.Error = "The PIN is no longer active. You have used it 5 times in another Session.";

                                    var session1 = await _sessionService.GetAllSession();

                                    ViewBag.sessionId = new SelectList(session1, "Id", "FullSession");

                                    var classLevel1 = await _classLevelService.ClassLevelList();
                                    ViewBag.classId = new SelectList(classLevel1, "Id", "ClassLevelName");

                                    //ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                                    return View(model);
                                }
                                else
                                {
                                    if (pinCode.Usage > 0)
                                    {
                                        pinCode.Usage = pinCode.Usage - 1;
                                        await _resultService.Update();
                                    }

                                    //Update Printed Status
                                    PrintedStatus.Printed = true;
                                    await _enrollmentService.Edit(PrintedStatus);


                                    if (sett.PrintOutOption == PrintOutOption.PrintOutOne)
                                    {
                                        if (checkArchive != null)
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult", "ArchiveResult", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session2.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult", "ArchiveResult", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult", "ArchiveResult",
                                                        new { id = enrollment.Id, sess = session2.Id });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session2.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult",
                                                        new { id = enrollment.Id, sess = session2.Id });
                                                }
                                            }
                                        }

                                    }
                                    else if (sett.PrintOutOption == PrintOutOption.PrintOutTwo)
                                    {
                                        if (checkArchive != null)
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult2", "ArchiveResult", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session2.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult2", "ArchiveResult", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult2", "ArchiveResult",
                                                        new { id = enrollment.Id, sess = session2.Id });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session2.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult2",
                                                        new { id = enrollment.Id, sess = session2.Id });
                                                }
                                            }
                                        }


                                    }

                                    else if (sett.PrintOutOption == PrintOutOption.PrintOutThree)
                                    {
                                        if (checkArchive != null)
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult3", "ArchiveResult", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session2.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult3", "ArchiveResult", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult3", "ArchiveResult",
                                                        new { id = enrollment.Id, sess = session2.Id });
                                                }
                                            }
                                        }
                                        else if (checkTotalCA.TotalCA == null || checkTotalCA.TotalCA == 0)
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session2.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult2",
                                                        new { id = enrollment.Id, sess = session2.Id });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (sett.ShowCumulativeResultForThirdTerm == false)
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult3", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                if (!session2.Term.Contains("Third"))
                                                {
                                                    TempData["Verified"] = "True";
                                                    return RedirectToAction("PrintResult3", new { id = enrollment.Id });
                                                }
                                                else
                                                {
                                                    return RedirectToAction("PrintThirdResult3",
                                                        new { id = enrollment.Id, sess = session2.Id });
                                                }
                                            }
                                        }


                                    }

                                }
                            }
                        }
                        else
                        {
                            if (pinCode.StudentPin == null)
                            {
                                pinCode.StudentPin = userProfile.StudentRegNumber;
                                pinCode.EnrollmentId = enrollment.Id;
                                pinCode.SessionId = enrollment.SessionId;
                            }
                            else if (pinCode.EnrollmentId == enrollment.Id && pinCode.Usage <= 0)
                            {
                                ViewBag.Error = "The PIN is no longer active. You have used it 2 times.";

                                var session1 = await _sessionService.GetAllSession();

                                ViewBag.sessionId = new SelectList(session1, "Id", "FullSession");

                                var classLevel1 = await _classLevelService.ClassLevelList();
                                ViewBag.classId = new SelectList(classLevel1, "Id", "ClassLevelName");
                                //ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                                return View(model);
                            }

                            if (pinCode.Usage > 0)
                            {
                                pinCode.Usage = pinCode.Usage - 1;
                                await _resultService.Update();
                            }

                            //Update Printed Status
                            PrintedStatus.Printed = true;
                            await _enrollmentService.Edit(PrintedStatus);

                            if (sett.PrintOutOption == PrintOutOption.PrintOutOne)
                            {
                                if (checkArchive != null)
                                {
                                    if (sett.ShowCumulativeResultForThirdTerm == false)
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult", "ArchiveResult", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        if (!session2.Term.Contains("Third"))
                                        {
                                            TempData["Verified"] = "True";
                                            return RedirectToAction("PrintResult", "ArchiveResult", new { id = enrollment.Id });
                                        }
                                        else
                                        {
                                            return RedirectToAction("PrintThirdResult", "ArchiveResult",
                                                new { id = enrollment.Id, sess = session2.Id });
                                        }
                                    }
                                }
                                else
                                {
                                    if (sett.ShowCumulativeResultForThirdTerm == false)
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        if (!session2.Term.Contains("Third"))
                                        {
                                            TempData["Verified"] = "True";
                                            return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                        }
                                        else
                                        {
                                            return RedirectToAction("PrintThirdResult",
                                                new { id = enrollment.Id, sess = session2.Id });
                                        }
                                    }
                                }

                            }
                            else if (sett.PrintOutOption == PrintOutOption.PrintOutTwo)
                            {
                                if (checkArchive != null)
                                {
                                    if (sett.ShowCumulativeResultForThirdTerm == false)
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult2", "ArchiveResult", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        if (!session2.Term.Contains("Third"))
                                        {
                                            TempData["Verified"] = "True";
                                            return RedirectToAction("PrintResult2", "ArchiveResult", new { id = enrollment.Id });
                                        }
                                        else
                                        {
                                            return RedirectToAction("PrintThirdResult2", "ArchiveResult",
                                                new { id = enrollment.Id, sess = session2.Id });
                                        }
                                    }
                                }
                                else
                                {
                                    if (sett.ShowCumulativeResultForThirdTerm == false)
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        if (!session2.Term.Contains("Third"))
                                        {
                                            TempData["Verified"] = "True";
                                            return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                        }
                                        else
                                        {
                                            return RedirectToAction("PrintThirdResult2",
                                                new { id = enrollment.Id, sess = session2.Id });
                                        }
                                    }
                                }


                            }

                            else if (sett.PrintOutOption == PrintOutOption.PrintOutThree)
                            {
                                if (checkArchive != null)
                                {
                                    if (sett.ShowCumulativeResultForThirdTerm == false)
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult3", "ArchiveResult", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        if (!session2.Term.Contains("Third"))
                                        {
                                            TempData["Verified"] = "True";
                                            return RedirectToAction("PrintResult3", "ArchiveResult", new { id = enrollment.Id });
                                        }
                                        else
                                        {
                                            return RedirectToAction("PrintThirdResult3", "ArchiveResult",
                                                new { id = enrollment.Id, sess = session2.Id });
                                        }
                                    }
                                }
                                else if (checkTotalCA.TotalCA == null || checkTotalCA.TotalCA == 0)
                                {
                                    if (sett.ShowCumulativeResultForThirdTerm == false)
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        if (!session2.Term.Contains("Third"))
                                        {
                                            TempData["Verified"] = "True";
                                            return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                        }
                                        else
                                        {
                                            return RedirectToAction("PrintThirdResult2",
                                                new { id = enrollment.Id, sess = session2.Id });
                                        }
                                    }
                                }
                                else
                                {
                                    if (sett.ShowCumulativeResultForThirdTerm == false)
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult3", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        if (!session2.Term.Contains("Third"))
                                        {
                                            TempData["Verified"] = "True";
                                            return RedirectToAction("PrintResult3", new { id = enrollment.Id });
                                        }
                                        else
                                        {
                                            return RedirectToAction("PrintThirdResult3",
                                                new { id = enrollment.Id, sess = session2.Id });
                                        }
                                    }
                                }


                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var a = ex;
                    ViewBag.Error = "Unable to check result, Contact your Teacher.";
                }

            }



            var session = await _sessionService.GetAllSession();

            ViewBag.sessionId = new SelectList(session, "Id", "FullSession");

            var classLevel = await _classLevelService.ClassLevelList();
            ViewBag.classId = new SelectList(classLevel, "Id", "ClassLevelName");

            //ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
            return View(model);
        }
        #endregion

        #region Print Result
        [AllowAnonymous]
        public async Task<ActionResult> PrintResult(Int32? id, int Id)
        {
            try
            {
                string valstring = TempData["validator"].ToString();
                if (valstring != "asdflkjhgqwerpoiu")
                {

                    return RedirectToAction("Result");
                }
            }
            catch (Exception f)
            {

                return RedirectToAction("Result");
            }


            try
            {


                int subjectCount = await _resultService.SubjectCount(id);
                if (subjectCount == 0)
                {
                    ViewBag.Sum = "No Result score has been entered";
                }

                // var result = await _resultService.PrintResult(id);
                ViewBag.id = id;
                var PositionShow = db.Settings.FirstOrDefault();
                ViewBag.showpos = PositionShow;

                //var classResult = await _classLevelService.Get(Id);
                //var showPosOnClassResult = classResult.ShowPositionOnClassResult;
                //ViewBag.showPosOnClassResult = showPosOnClassResult;
                return View();
            }
            catch (Exception ex)
            {
                var session = await _sessionService.GetAllSession();
                ViewBag.sessionId = new SelectList(session, "Id", "FullSession");
                return View("Result");
            }
        }


        [AllowAnonymous]
        public async Task<ActionResult> PrintResult2(Int32? id, int Id)
        {
            try
            {
                string valstring = TempData["validator"].ToString();
                if (valstring != "asdflkjhgqwerpoiu")
                {

                    return RedirectToAction("Result");
                }
            }
            catch (Exception f)
            {

                return RedirectToAction("Result");
            }

            try
            {


                int subjectCount = await _resultService.SubjectCount(id);
                if (subjectCount == 0)
                {
                    ViewBag.Sum = "No Result score has been entered";
                }

                // var result = await _resultService.PrintResult(id);
                ViewBag.id = id;
                var PositionShow = db.Settings.FirstOrDefault();
                ViewBag.showpos = PositionShow;

                //var classResult = await _classLevelService.Get(Id);
                //var showPosOnClassResult = classResult.ShowPositionOnClassResult;
                //ViewBag.showPosOnClassResult = showPosOnClassResult;
                return View();
            }
            catch (Exception ex)
            {
                var session = await _sessionService.GetAllSession();
                ViewBag.sessionId = new SelectList(session, "Id", "FullSession");
                return View("Result");
            }
        }


        [AllowAnonymous]
        public async Task<ActionResult> PrintResult3(Int32? id, int Id)
        {
            try
            {
                string valstring = TempData["validator"].ToString();
                if (valstring != "asdflkjhgqwerpoiu")
                {

                    return RedirectToAction("Result");
                }
            }
            catch (Exception f)
            {

                return RedirectToAction("Result");
            }

            try
            {


                int subjectCount = await _resultService.SubjectCount(id);
                if (subjectCount == 0)
                {
                    ViewBag.Sum = "No Result score has been entered";
                }

                // var result = await _resultService.PrintResult(id);
                ViewBag.id = id;
                var PositionShow = db.Settings.FirstOrDefault();
                ViewBag.showpos = PositionShow;

                //var classResult = await _classLevelService.Get(Id);
                //var showPosOnClassResult = classResult.ShowPositionOnClassResult;
                //ViewBag.showPosOnClassResult = showPosOnClassResult;
                return View();
            }
            catch (Exception ex)
            {
                var session = await _sessionService.GetAllSession();
                ViewBag.sessionId = new SelectList(session, "Id", "FullSession");
                return View("Result");
            }
        }

        #endregion



        #region Cummulative

        [AllowAnonymous]
        public async Task<ActionResult> PrintThirdResult(Int32? id, int? sess, int Id)
        {
            try
            {
                string valstring = TempData["validator"].ToString();
                if (valstring != "asdflkjhgqwerpoiu")
                {

                    return RedirectToAction("Result");
                }
            }
            catch (Exception f)
            {

                return RedirectToAction("Result");
            }

            try
            {
                int subjectCount = await _resultService.SubjectCount(id);
                if (subjectCount == 0)
                {
                    ViewBag.Sum = "No Result score has been entered";
                }

                ViewBag.id = id;
                ViewBag.sessid = sess;

                //var classResult = await _classLevelService.Get(Id);
                //var showPosOnClassResult = classResult.ShowPositionOnClassResult;
                //ViewBag.showPosOnClassResult = showPosOnClassResult;

                return View();
            }
            catch (Exception ex)
            {
                var session = await _sessionService.GetAllSession();
                ViewBag.sessionId = new SelectList(session, "Id", "FullSession");
                return View("Result");
            }
        }




        [AllowAnonymous]
        public async Task<ActionResult> PrintThirdResult2(Int32? id, int? sess, int Id)
        {
            try
            {
                string valstring = TempData["validator"].ToString();
                if (valstring != "asdflkjhgqwerpoiu")
                {

                    return RedirectToAction("Result");
                }
            }
            catch (Exception f)
            {

                return RedirectToAction("Result");
            }

            try
            {


                int subjectCount = await _resultService.SubjectCount(id);
                if (subjectCount == 0)
                {
                    ViewBag.Sum = "No Result score has been entered";
                }

                ViewBag.id = id;
                ViewBag.sessid = sess;

                //var classResult = await _classLevelService.Get(Id);
                //var showPosOnClassResult = classResult.ShowPositionOnClassResult;
                //ViewBag.showPosOnClassResult = showPosOnClassResult;

                return View();
            }
            catch (Exception ex)
            {
                var session = await _sessionService.GetAllSession();
                ViewBag.sessionId = new SelectList(session, "Id", "FullSession");
                return View("Result");
            }
        }


        [AllowAnonymous]
        public async Task<ActionResult> PrintThirdResult3(Int32? id, int? sess, int Id)
        {
            try
            {
                string valstring = TempData["validator"].ToString();
                if (valstring != "asdflkjhgqwerpoiu")
                {

                    return RedirectToAction("Result");
                }
            }
            catch (Exception f)
            {

                return RedirectToAction("Result");
            }

            try
            {


                int subjectCount = await _resultService.SubjectCount(id);
                if (subjectCount == 0)
                {
                    ViewBag.Sum = "No Result score has been entered";
                }

                ViewBag.id = id;
                ViewBag.sessid = sess;

                //var classResult = await _classLevelService.Get(Id);
                //var showPosOnClassResult = classResult.ShowPositionOnClassResult;
                //ViewBag.showPosOnClassResult = showPosOnClassResult;

                return View();
            }
            catch (Exception ex)
            {
                var session = await _sessionService.GetAllSession();
                ViewBag.sessionId = new SelectList(session, "Id", "FullSession");
                return View("Result");
            }
        }




        #endregion cumulative

        #region Upload
        public ActionResult Upload(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        // POST: Admin/Settings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(HttpPostedFileBase upload, int id)
        {
            try
            {
                var getstudent = await _studentProfileService.Get(id);
                if (getstudent != null)
                {
                    if (getstudent.ImageId != 0)
                    {
                        await _imageService.Delete(getstudent.ImageId);
                    }
                    var imgId = await _imageService.Create(upload);

                    await _studentProfileService.UpdateImageId(getstudent.Id, imgId);
                    TempData["success"] = "Update Successful.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Update Unsuccessful." + ex;
            }
            return View();
        }


        [AllowAnonymous]
        public ActionResult ChangePhoto(string uid)
        {
            var student = db.StudentProfiles.Include(x => x.user).FirstOrDefault(x => x.UserId == uid);
            ViewBag.Id = student.Id;
            ViewBag.name = student.user.Surname + " " + student.user.FirstName + " " + student.user.OtherName + " (" + student.user.UserName + ")";
            return View();
        }

        // POST: Admin/Settings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AllowAnonymous]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePhoto(HttpPostedFileBase upload, int id)
        {
            var getstudent = await _studentProfileService.Get(id);
            try
            {

                if (getstudent != null)
                {
                    if (getstudent.ImageId != 0)
                    {
                        await _imageService.Delete(getstudent.ImageId);
                    }
                    var imgId = await _imageService.Create(upload);

                    await _studentProfileService.UpdateImageId(getstudent.Id, imgId);
                    TempData["success"] = "Update Successful.";
                    return RedirectToAction("Index", "UserManagers", new { area = "Admin" });
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Update Unsuccessful." + ex;
            }
            return RedirectToAction("ChangePhoto", new { uid = getstudent.userid });
        }

        #endregion

        #region Edit User
        public async Task<ActionResult> EditUser(int? id)
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

            return View(user);
        }

        [HttpPost]

        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(StudentInfoDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _studentProfileService.Edit(model);
                    TempData["success"] = "Update Successful.";
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    TempData["error"] = "Update Unsuccessful, (" + e.ToString() + ")";
                }

            }
            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName", model.StateOfOrigin);
            return View(model);
        }

        #endregion


        [AllowAnonymous]
        public JsonResult LgaList(string Id)
        {
            var stateId = db.States.FirstOrDefault(x => x.StateName == Id).Id;
            var local = from s in db.LocalGovs
                        where s.StatesId == stateId
                        select s;

            return Json(new SelectList(local.ToArray(), "LGAName", "LGAName"), JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateAmount(int percent, int id, int sessionId = 0)
        {
            var finance = await db.FinanceInitializers.Include(x => x.Income).FirstOrDefaultAsync(x => x.Id == id);
            decimal pamount = Convert.ToDecimal(percent) / Convert.ToDecimal(100);
            finance.Amount = pamount * finance.Amount;

            finance.Balance = finance.Amount - finance.Amount;
            db.Entry(finance).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("MakePayment", new { sessionId = finance.SessionId });
        }

        [HttpPost]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePay(int id, int sessionId = 0)
        {
            var finance = await db.FinanceInitializers.Include(x => x.Income).FirstOrDefaultAsync(x => x.Id == id);

            db.FinanceInitializers.Remove(finance);

            await db.SaveChangesAsync();
            return RedirectToAction("MakePayment", new { sessionId = finance.SessionId });
        }
        [HttpPost]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> AddToPay(int id, int sessionId = 0, bool payall = false, decimal bal = 0)
        {
            int sid = 0;
            if (sessionId > 0)
            {
                var Session = await db.Sessions.FirstOrDefaultAsync(x => x.Id == sessionId);
                sid = Session.Id;
            }
            else
            {
                var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
                sid = currentSession.Id;
            }
            var userid = User.Identity.GetUserId();
            var studentId = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.UserId == userid);
            var enroment = await db.Enrollments.FirstOrDefaultAsync(x => x.SessionId == sid && x.StudentProfileId == studentId.Id);
            //
            var income = await db.Incomes.FirstOrDefaultAsync(x => x.Id == id);

            var amountpay = await db.Incomes.FirstOrDefaultAsync(x => x.Id == income.Id);
            FinanceInitializer model = new FinanceInitializer();
            model.EnrollmentId = enroment.Id;
            model.IncomeId = income.Id;
            model.TransactionStatus = TransactionStatus.Pending;

            model.Percent = 100;
            model.SessionId = sid;
            model.TransactionDate = DateTime.UtcNow.AddHours(1);
            model.PaymentAmountId = amountpay.Id;
            model.PaymentTypeId = amountpay.Id;
            model.Payall = payall;
            if (payall == true)
            {
                model.Amount = bal;
                model.Balance = 0;
            }
            else
            {
                model.Amount = amountpay.Amount;
                model.Balance = 0;
            }
            db.FinanceInitializers.Add(model);
            await db.SaveChangesAsync();

            return RedirectToAction("MakePayment");
        }

        [AllowAnonymous]
        public async Task<JsonResult> StudentList(int classId, int sessionId)
        {
            var classid = db.ClassLevels.FirstOrDefault(x => x.Id == classId).Id;
            var sessionid = db.Sessions.FirstOrDefault(x => x.Id == sessionId).Id;
            //var enrolledStudents = _resultService.StudentsBySessIdAndByClassId(sessionid, classid);
            //var student = from s in db.Enrollments.Include(x=>x.StudentProfile).Include(x=>x.ClassLevel)
            //            where s.ClassLevelId == classId
            //            select s;
            //Student List
            List<EnrolledStudentsByClassDto> students = new List<EnrolledStudentsByClassDto>();

            IQueryable<Enrollment> enrolledStudents = from s in db.Enrollments
                                          .Include(x => x.Session).OrderBy(x => x.StudentProfile.user.Surname)
                                          .Include(x => x.EnrolledSubjects).Include(x => x.StudentProfile)
                                          .Where(s => s.ClassLevelId == classid && s.SessionId == sessionid && s.EnrolledSubjects.Count() > 0)
                                                      select s;

            var c = enrolledStudents.Count();
            var output = await enrolledStudents.OrderByDescending(x => x.AverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber).Select(x => new EnrolledStudentsByClassDto
            {
                Id = x.Id,
                StudentRegNumber = x.StudentProfile.StudentRegNumber,
                AverageScore = x.AverageScore,
                SubjectCount = x.EnrolledSubjects.Where(d => d.TotalScore > 0).Count(),
                StudentName = x.StudentProfile.user.Surname + " " + x.StudentProfile.user.FirstName + " " + x.StudentProfile.user.OtherName,
                EnrollmentId = x.Id,
                StudentId = x.StudentProfile.Id,
                SessionId = x.SessionId,
                SessionYear = x.Session.SessionYear,
                CummulativeAverageScore = x.CummulativeAverageScore,
                Term = x.Session.Term


            }).ToListAsync();

            students.AddRange(output.Select(entity => new EnrolledStudentsByClassDto()
            {
                StudentId = entity.StudentId,
                StudentName = entity.StudentName

            }));

            return Json(new SelectList(students.ToArray(), "StudentId", "StudentName"), JsonRequestBehavior.AllowGet);
        }

        #region Assignment

        public async Task<ActionResult> Assignment()
        {
            try
            {
                var userid = User.Identity.GetUserId();
                var studentId = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.UserId == userid);
                var enroment = db.Enrollments.Include(x => x.ClassLevel).Include(x => x.User).Where(x => x.StudentProfileId == studentId.Id).Select(x => x.ClassLevelId).ToList();

                var myass = db.Assignments.Include(x => x.ClassLevel).Include(x => x.Session).Include(x => x.Subject).Include(x => x.AssignmentAnswers).Where(x => enroment.Contains(x.ClassLevelId) && x.IsPublished == true);

                return View(myass);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

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
            var user = User.Identity.GetUserId();
            var check = await db.AssignmentAnswers.FirstOrDefaultAsync(x => x.AssignmentId == id && x.UserId == user);
            if (check != null)
            {
                if (item.DateSubmitionEnds > DateTime.UtcNow.Date)
                {
                    ViewBag.status = "<span class=\"\" style=\"font-weight:bolder;\">You Have Submited and can EDIT</span>";
                }
                else
                {
                    ViewBag.status = "<span class=\"text-success\" style=\"font-weight:bolder;\">You Have Submited and CANNOT EDIT </span>";
                }
            }
            else
            {
                if (item.DateSubmitionEnds > DateTime.UtcNow.Date)
                {
                    ViewBag.status = "<span class=\"text-warning\" style=\"font-weight:bolder;\">You Have Not Submited</span>";
                }
                else
                {
                    ViewBag.status = "<span class=\"text-danger\" style=\"font-weight:bolder;\">You Missed the Assignment</span>";
                }
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

        public async Task<ActionResult> AssignmentAnswer(int assId)
        {
            var user = User.Identity.GetUserId();
            var check = await db.AssignmentAnswers.FirstOrDefaultAsync(x => x.AssignmentId == assId && x.UserId == user);
            if (check != null)
            {
                return RedirectToAction("EditAssignmentAnswer", new { id = check.Id });
            }
            var item = await _assignmentService.Get(assId);
            ViewBag.assignment = item;
            return View();
        }

        [HttpPost]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignmentAnswer(AssignmentAnswer model, int assId)
        {
            var check = await db.AssignmentAnswers.FirstOrDefaultAsync(x => x.AssignmentId == model.Id);
            if (check != null)
            {
                return RedirectToAction("EditAssignmentAnswer", new { id = model.Id });
            }
            if (ModelState.IsValid)
            {

                var userid = User.Identity.GetUserId();
                var studentId = await db.StudentProfiles.FirstOrDefaultAsync(x => x.UserId == userid);
                var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
                var enrollement = await db.Enrollments.Include(x => x.ClassLevel).Include(x => x.Session).Include(x => x.StudentProfile).FirstOrDefaultAsync(x => x.SessionId == currentSession.Id && x.StudentProfileId == studentId.Id);
                model.AssignmentId = assId;
                model.ClassId = enrollement.ClassLevelId;
                model.EnrollementId = enrollement.Id;
                model.StudentId = studentId.Id;
                model.UserId = userid;

                model.DateAnswered = DateTime.UtcNow.AddHours(1);
                await _assignmentService.CreateAnswer(model);
                TempData["success"] = "Submitted Successful";
                return RedirectToAction("Assignment");
            }
            TempData["error"] = "Unable to Save";
            return View(model);
        }

        public async Task<ActionResult> EditAssignmentAnswer(int id)
        {
            var userid = User.Identity.GetUserId();
            var studentId = await db.StudentProfiles.FirstOrDefaultAsync(x => x.UserId == userid);
            var item = await _assignmentService.GetAnswer(id, studentId.Id);
            var assinfo = await _assignmentService.Get(item.AssignmentId);
            ViewBag.assignment = assinfo;
            return View(item);
        }

        [HttpPost]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAssignmentAnswer(AssignmentAnswer model, int id)
        {
            if (ModelState.IsValid)
            {

                model.DateModified = DateTime.UtcNow.AddHours(1);
                await _assignmentService.EditAnswer(model);
                TempData["success"] = "Submitted Successful";
                return RedirectToAction("Assignment");
            }
            TempData["error"] = "Unable to Save";
            return View(model);
        }

        #endregion

        #region Attendance

        public async Task<ActionResult> Attendance()
        {
            var userid = User.Identity.GetUserId();
            var my = await db.AttendanceDetails.Include(x => x.Attendance).Where(x => x.UserId == userid).OrderByDescending(x => x.Attendance.Date).ToListAsync();
            return View(my);
        }
        #endregion


        #region StudentTimeTable

        public async Task<ActionResult> StudentTimeTable()
        {
            try
            {
                var userid = User.Identity.GetUserId();
                var stu = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.UserId == userid);
                var ernro = await db.Enrollments.Include(x => x.User).FirstOrDefaultAsync(x => x.StudentProfileId == stu.Id);
                var timetable = await db.TimeTables.Include(x => x.ClassLevel).FirstOrDefaultAsync(x => x.ClassLevelId == ernro.ClassLevelId);
                return View(timetable);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Your not in a class yet";
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region TimeTable
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
        #region Sylabus
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

        #region update student info before checking result
        [AllowAnonymous]
        public async Task<ActionResult> UpdateProfile(int? id)
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

            return View(user);
        }

        [HttpPost]
        [AllowAnonymous]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateProfile(StudentInfoDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _studentProfileService.Edit(model);

                    var profil = await db.StudentProfiles.FirstOrDefaultAsync(x => x.StudentRegNumber == model.StudentRegNumber);


                    TempData["success"] = "Update Successful.Kindly proceed to check yout result";
                    profil.IsUpdated = true;
                    db.Entry(profil).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Result");
                }
                catch (Exception e)
                {
                    TempData["error"] = "Update Unsuccessful, (" + e.ToString() + ")";
                }

            }
            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName", model.StateOfOrigin);
            return View(model);
        }

        #endregion


        #region online class

        public async Task<ActionResult> LiveList()
        {
            var model = await db.OnlineZooms.Include(x => x.Session).Include(x => x.ClassLevel).Include(x => x.Subject).Include(x => x.User).ToListAsync();
            string uid = User.Identity.GetUserId();
            var iprofile = await db.StudentProfiles.FirstOrDefaultAsync(x => x.UserId == uid);
            var enrol = await db.Enrollments.Include(x => x.ClassLevel).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).FirstOrDefaultAsync(x => x.StudentProfileId == iprofile.Id);

            model = model.Where(x => x.ClassLevelId == enrol.ClassLevelId).ToList();
            ViewBag.data = enrol;
            return View(model);
        }

        [AllowAnonymous]
        public async Task<ActionResult> DetailsLive(long id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<User> userdata = new List<User>();
            var content = TokenManager.GetAMeeting(id);

            RootobjectDetails data = JsonConvert.DeserializeObject<RootobjectDetails>(content);
            var prof = await db.OnlineZooms.Include(x => x.Session).Include(x => x.ClassLevel).Include(x => x.Subject).Include(x => x.User).FirstOrDefaultAsync(x => x.MeetingId == data.id);
            ViewBag.prof = prof;
            return View(data);
        }

        public async Task<ActionResult> LiveClassRecord(string id)
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

        #endregion



        #region Online Course Upload
        //public async Task<ActionResult> OnlineClass()
        //{
        //    var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
        //    var Subject = await db.ClassLevels.Include(c => c.Subjects).ToListAsync();
        //    return View(Subject);
        //}

        public async Task<ActionResult> OnlineSubject()
        {
            var userId = User.Identity.GetUserId();
            var student = db.StudentProfiles.Include(x => x.user).FirstOrDefault(x => x.UserId == userId);
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var enrol = db.Enrollments.Include(x => x.ClassLevel).Include(x => x.StudentProfile).FirstOrDefault(x => x.StudentProfileId == student.Id && x.SessionId == currentSession.Id);

            var Subject = await db.Subjects.Include(c => c.ClassLevel).Include(x => x.OnlineCourseUpload).Where(x => x.ClassLevelId == enrol.ClassLevelId).ToListAsync();
            ViewBag.sess = currentSession.Id;
            ViewBag.classId = enrol.ClassLevelId;
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


        #region CBT Exam
        // GET: CBTExam/CBTExamBoard
        public async Task<ActionResult> StudentExamBoard(string unixconverify, string xgink, string role)
        {
            unixconverify = User.Identity.Name;
            xgink = GeneralService.PortalLink();
            role = "student";
            var userId = User.Identity.GetUserId();
            var student = db.StudentProfiles.Include(x => x.user).Where(x => x.UserId == userId).FirstOrDefault();

            var defaulter = db.Defaulters.Include(x => x.StudentProfile).Where(x => x.ProfileId == student.Id).FirstOrDefault();
            ViewBag.defaulter = defaulter;

            HttpResponseMessage response = client.GetAsync("/api/ExamBoardApi/GetStudentExam?unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role + "&regNumber=" + student.StudentRegNumber + "&UserName=" + student.user.UserName).Result;
            var response2 = response.Content.ReadAsStringAsync().Result;
            List<CBTExaminationDto> data = JsonConvert.DeserializeObject<List<CBTExaminationDto>>(response2);
            ViewBag.data = data;
            return View(data);
        }


        [HttpPost]
        public async Task<ActionResult> QuestionSetting(Examination exam, string xgink, string role, int? examId, int? settingId)
        {
            var iskoolLink = db.Settings.FirstOrDefault().IskoollLink;
            if (ModelState.IsValid)
            {

                xgink = GeneralService.PortalLink();
                role = "student";
                HttpResponseMessage response = client.PostAsJsonAsync("/api/ExamBoardApi/QuestionSetting?xgink=" + xgink + "&role=" + role + "&examId=" + examId + "&settingId=" + settingId, exam).Result;

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Result = "Data Is Successfully Saved!";

                    //Testing link do not delete
                    //return Redirect("http://cbttest.iskools.com/SetUp/Preview?examId=" + examId);

                    //Main Link do not delete
                    return Redirect(iskoolLink + "/SetUp/Preview?examId=" + examId);
                }


            }
            else
            {

                string unixconverify = User.Identity.Name;
                ViewBag.Result = "Error! Please try with valid data.";
                return RedirectToAction("StudentExamBoard", "Panel", new { unixconverify = unixconverify, xgink = xgink, role = role });
            }

            //Testing link do not delete
            return Redirect(iskoolLink + "/SetUp/Preview?examId=" + examId);

            //Main Link do not delete
            //return Redirect("http://cbt.iskools.com/SetUp/Preview?examId=" + examId);
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