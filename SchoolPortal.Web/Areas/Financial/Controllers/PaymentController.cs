using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Dtos;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Areas.Financial.Controllers
{
    [Authorize(Roles = "SuperAdmin,Finance,Student,FinanceEntry")]
    public class PaymentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IFinanceService _financeService = new FinanceService();
        private IIncomeService _incomeService = new IncomeService();
        private IExpenditureService _expendituerService = new ExpenditureService();
        private IPaystackTransactionService _paystackTransactionService = new PaystackTransactionService();
        private IPaymentAmountService _paymentAmountService = new PaymentAmountService();
        private ISessionService _sessionService = new SessionService();
        private IClassLevelService _classlevelService = new ClassLevelService();
        private ApplicationUserManager _userManager;
        private IEnrollmentService _enrollmentService = new EnrollmentService();

        public PaymentController()
        {

        }

        public PaymentController(FinanceService financeService,
            ExpenditureService expenditureService,
            IncomeService incomeService,
            PaystackTransactionService paystackTransactionService,
            SessionService sessionService,
            PaymentAmountService paymentAmountService, ClassLevelService classlevelService, ApplicationUserManager userManager, EnrollmentService enrollmentService)
        {
            _financeService = financeService;
            _incomeService = incomeService;
            _expendituerService = expenditureService;
            _paystackTransactionService = paystackTransactionService;
            _paymentAmountService = paymentAmountService;
            _sessionService = sessionService;
            _classlevelService = classlevelService;
            _userManager = userManager;
            _enrollmentService = enrollmentService;
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

        // GET: Financial/Payment
        [Authorize(Roles = "SuperAdmin,Finance,FinanceEntry")]
        public async Task<ActionResult> Index(string searchString, string currentFilter, int? page, DateTime? DateOne, DateTime? DateTwo, string Duration = "All")
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            string Durationoflist = Duration;
            #region date
            if (DateOne != null)
            {
                DateOne = DateOne.Value.Date;
            }
            else
            {
                DateOne = DateTime.UtcNow.AddHours(1).Date;
            }
            if (DateTwo != null)
            {
                DateTwo = DateTwo.Value.Date;
            }
            else
            {
                DateTwo = DateTime.UtcNow.AddHours(1).Date;
            }
            DateTime? StartDate = DateOne.Value;
            DateTime? EndDate = DateTwo.Value;

            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;

            #endregion


            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            ViewBag.session = session.SessionYear + "-" + session.Term +" Term";
            ViewBag.CurrentFilter = searchString;
            var items = await _financeService.PaymentListPage(searchString, currentFilter, page, DateOne, DateTwo, Duration);
            ViewBag.countall = items.Count();
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));

        }
        [Authorize(Roles = "SuperAdmin,Finance,FinanceEntry")]
        public async Task<ActionResult> IndexAll(string searchString, string currentFilter, int? page, DateTime? DateOne, DateTime? DateTwo, string Duration = "All")
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            string Durationoflist = Duration;
            #region date
            if (DateOne != null)
            {
                DateOne = DateOne.Value.Date;
            }
            else
            {
                DateOne = DateTime.UtcNow.AddHours(1).Date;
            }
            if (DateTwo != null)
            {
                DateTwo = DateTwo.Value.Date;
            }
            else
            {
                DateTwo = DateTime.UtcNow.AddHours(1).Date;
            }
            DateTime? StartDate = DateOne.Value;
            DateTime? EndDate = DateTwo.Value;

            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;

            #endregion


            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            ViewBag.session = session.SessionYear + "-" + session.Term + " Term";
            ViewBag.CurrentFilter = searchString;
            var items = await _financeService.PaymentListAll(searchString, currentFilter, page, DateOne, DateTwo, Duration);
            ViewBag.countall = items.Count();
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));

        }
        public async Task<ActionResult> IndexSession()
        {

            var session = await _sessionService.GetAllSession();
            ViewBag.SessionId = new SelectList(session.OrderByDescending(x => x.FullSession), "Id", "FullSession");

            return View();
        }

        public async Task<ActionResult> IndexBySession(int sessionId, string searchString, string currentFilter, int? page, DateTime? DateOne, DateTime? DateTwo, string Duration = "All")
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            string Durationoflist = Duration;
            #region date
            if (DateOne != null)
            {
                DateOne = DateOne.Value.Date;
            }
            else
            {
                DateOne = DateTime.UtcNow.AddHours(1).Date;
            }
            if (DateTwo != null)
            {
                DateTwo = DateTwo.Value.Date;
            }
            else
            {
                DateTwo = DateTime.UtcNow.AddHours(1).Date;
            }
            DateTime? StartDate = DateOne.Value;
            DateTime? EndDate = DateTwo.Value;

            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;

            #endregion
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Id == sessionId);
            ViewBag.session = session.SessionYear + "-" + session.Term;
            ViewBag.CurrentFilter = searchString;
            var items = await _financeService.PaymentListPageSession(sessionId, searchString, currentFilter, page, DateOne, DateTwo, Duration);
            ViewBag.countall = items.Count();
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));

        }

        [Authorize(Roles = "SuperAdmin,Finance,FinanceEntry")]
        public async Task<ActionResult> AccountStatement()
        {

            return View();
        }

            // GET: Financial/Payment/Accomodation
            public async Task<ActionResult> Accomodation()
        {
            var item = await _financeService.PaidAccomodationList();
            return View(item);
        }


        // GET: Financial/Payment/MyPayment
        public async Task<ActionResult> MyPayment(int sessionId = 0)
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
            var item = await _financeService.GetUserPaymentBySession(sid);
            var userid = User.Identity.GetUserId();
            var studentId = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.UserId == userid);
            var enroment = await db.Enrollments
                .Include(x => x.ClassLevel)
                .FirstOrDefaultAsync(x => x.SessionId == sid && x.StudentProfileId == studentId.Id);

            ViewBag.totalFee = 0;
            ViewBag.FeePaid = enroment.Finances.Sum(x => x.Amount);
            ViewBag.FeeBalance = ViewBag.totalFee - ViewBag.FeePaid;
            return View(item);
        }

        //GET: Financial/Payment/PaymentInfo
        public async Task<ActionResult> PaymentInfo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var finance = await _financeService.Get(id);
            ViewBag.finance = finance;


            if (finance == null)
            {
                return HttpNotFound();
            }
            return View(finance);
        }



        // GET: Financial/Payment/InvoiceDetails
        public async Task<ActionResult> InvoiceDetails(int sessionId = 0)
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
            var enroment = await db.Enrollments.Include(x => x.ClassLevel).Include(x => x.Session).Include(x => x.StudentProfile).Include(x => x.User).FirstOrDefaultAsync(x => x.SessionId == sid && x.StudentProfileId == studentId.Id);


            IQueryable<FinanceInitializer> pfinance = from s in db.FinanceInitializers
                                                     .Include(x => x.Income)
                                                     .Where(x => x.EnrollmentId == enroment.Id && x.SessionId == sid)
                                                     .Where(x => x.TransactionStatus == TransactionStatus.Pending)
                                                      select s;


            //var item = await _financeService.Get(id);
            //var std = db.StudentProfiles.Include(x => x.user).FirstOrDefault(x => x.UserId == item.UserId);
            //var enrol = db.Enrollments.Include(x => x.ClassLevel).Include(x => x.Session).Include(x => x.StudentProfile).Include(x => x.User).FirstOrDefault(x => x.StudentProfileId == std.Id);
            ViewBag.stdName = enroment;
            var set = db.Settings.FirstOrDefault();
            ViewBag.set = set;
            ViewBag.pro = studentId;
            ViewBag.ssid = sid;
            //var bankinfo = await db.BankDetails.ToListAsync();
            //ViewBag.bank = bankinfo;
            return View(await pfinance.ToListAsync());

        }

        // GET: Financial/Payment/InvoicePrint
        public async Task<ActionResult> InvoicePrint(int? id)
        {
            var item = await _financeService.Get(id);
            var std = db.StudentProfiles.Include(x => x.user).FirstOrDefault(x => x.UserId == item.UserId);
            var enrol = db.Enrollments.Include(x => x.ClassLevel).Include(x => x.Session).Include(x => x.StudentProfile).Include(x => x.User).FirstOrDefault(x => x.StudentProfileId == std.Id);
            ViewBag.stdName = enrol;
            var set = db.Settings.FirstOrDefault();
            ViewBag.set = set;
            ViewBag.pro = std;
            var bankinfo = await db.BankDetails.ToListAsync();
            ViewBag.bank = bankinfo;
            return View(item);
        }

        // GET: Financial/Payment/GenerateInvoice
        public async Task<ActionResult> GenerateInvoice()
        {
            var income = await db.Incomes.ToListAsync();

            //var output = income.Select(x => new PaymentListByClassDto
            //{
            //    Title = x.Income.Title,
            //    Description = x.Income.Title + " (" + x.Amount + ")"
            //});
            ViewBag.Income = new SelectList(income, "Title", "Description");
            return View();
        }

        // POST: Financial/Payment/GenerateInvoice
        [HttpPost]
        public async Task<ActionResult> GenerateInvoice(Finance item, int? payId, string PaymentType)
        {

            if (ModelState.IsValid)
            {
                await _financeService.Create(item, PaymentType);
                TempData["success"] = "Invoice Generated Successfully";
                return RedirectToAction("InvoiceDetails", "Payment", new { id = item.Id });
            }
            TempData["error"] = "Unable to Generate Invoice";

            var output = await db.Incomes.ToListAsync();

          
            ViewBag.Income = new SelectList(output, "Title", "Description");


            //var income = await _incomeService.IncomeList();
            //ViewBag.Income = new SelectList(income, "Title", "Title");

            return RedirectToAction("InvoiceDetails", "Payment", new { id = item.Id });

        }
        public JsonResult StudentList(int Id)
        {
            var session = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);

            var classlevel = db.ClassLevels.FirstOrDefault(x => x.Id == Id).Id;
            var student = from s in db.Enrollments

                          where s.ClassLevelId == classlevel && s.SessionId == session.Id

                          select s;

            var output = student.Select(x => new StudentEnrollementDto
            {
                Id = x.Id,
                Student = x.StudentProfile.user.Surname + " " + x.StudentProfile.user.FirstName + " " + x.StudentProfile.user.OtherName
            });

            return Json(new SelectList(output.OrderBy(x => x.Student).ToArray(), "Id", "Student"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult StudentListi(int sId, int cid)
        {
            var session = db.Sessions.FirstOrDefault(x => x.Id == sId);

            var classlevel = db.ClassLevels.FirstOrDefault(x => x.Id == cid).Id;
            var student = from s in db.Enrollments

                          where s.ClassLevelId == classlevel && s.SessionId == session.Id

                          select s;

            var output = student.Select(x => new StudentEnrollementDto
            {
                Id = x.Id,
                Student = x.StudentProfile.user.Surname + " " + x.StudentProfile.user.FirstName + " " + x.StudentProfile.user.OtherName
            });

            return Json(new SelectList(output.OrderBy(x => x.Student).ToArray(), "Id", "Student"), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult ClaassPaymentList(int Id)
        //{
        //    var pmt = db.PaymentAmounts.Where(x => x.ClassLevelId == Id);


        //    var output = pmt.Select(x => new PaymentListByClassDto
        //    {
        //        Id = x.Id,
        //        Description = x.Income.Title + " (" + x.Amount + ")"
        //    });

        //    return Json(new SelectList(output.ToArray(), "Id", "Description"), JsonRequestBehavior.AllowGet);
        //}
        

             [Authorize(Roles = "SuperAdmin,Finance,FinanceEntry")]

        public async Task<ActionResult> CreateNewPayment()
        {

            var classlevel = await db.ClassLevels.ToListAsync();
            ViewBag.ClassLevelId = new SelectList(classlevel.OrderBy(x => x.ClassName), "Id", "ClassName");

            var session = await _sessionService.GetAllSession();
            ViewBag.SessionId = new SelectList(session.OrderByDescending(x => x.FullSession), "Id", "FullSession");
            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName");

            var Income = await db.Incomes.ToListAsync();
            ViewBag.Income = new SelectList(Income, "Id", "Description");
            return View();
        }

        // POST: Financial/Income/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateNewPayment(Finance item, int PaymentAmountId, string teller, string Surname, string FirstName, string OtherName, string Email, string PhoneNumber, string Address, string State, int Classid)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string userId = User.Identity.GetUserId();


                    var setting = db.Settings.OrderByDescending(x => x.Id).First();
                    var officer = User.Identity.GetUserName();
                    if (officer == "SuperAdmin")
                    {
                        officer = "Admin";
                    }
                    var user = new ApplicationUser
                    {

                        UserName = Surname+FirstName,
                        Email = Email,
                        Surname = Surname,
                        FirstName = FirstName,
                        OtherName = OtherName,
                        DateOfBirth = DateTime.UtcNow,
                        Religion = "Christian",
                        Phone = PhoneNumber,
                        DateRegistered = DateTime.UtcNow.AddHours(1),
                        ContactAddress = Address,
                        City = "",
                        Status = EntityStatus.Active,
                        StateOfOrigin = State,
                        Nationality = "Nigerian",
                        RegisteredBy = officer
                    };
                    var result = await UserManager.CreateAsync(user, "123456");
                    if (result.Succeeded)
                    {
                        await UserManager.AddToRoleAsync(user.Id, "Student");

                        StudentProfile student = new StudentProfile();
                        student.UserId = user.Id;
                        student.LastPrimarySchoolAttended = "";
                        student.ParentGuardianName = "";
                        student.ParentGuardianAddress = "";
                        student.ParentGuardianPhoneNumber = "";
                        student.ParentGuardianOccupation = "";
                        student.Graduate = true;
                        db.StudentProfiles.Add(student);
                        await db.SaveChangesAsync();

                        var studentReg = await db.StudentProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                        string numberid = studentReg.Id.ToString("D6");
                        studentReg.StudentRegNumber = setting.SchoolInitials + "/" + numberid;
                        db.Entry(studentReg).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        //Add Tracking
                        var userIdl = User.Identity.GetUserId();
                        if (userIdl != null)
                        {
                            var user2 = UserManager.Users.Where(x => x.Id == userIdl).FirstOrDefault();
                            Tracker tracker = new Tracker();
                            tracker.UserId = userIdl;
                            tracker.UserName = user2.UserName;
                            tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                            tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                            tracker.Note = tracker.FullName + " " + "Added a new student";
                            //db.Trackers.Add(tracker);
                            await db.SaveChangesAsync();
                        }
                        var session = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);

                        Enrollment enrollment = db.Enrollments.Create();

                        //other data for enrollment table
                        enrollment.StudentProfileId = student.Id;
                        enrollment.SessionId = session.Id;
                        enrollment.ClassLevelId = Classid;
                        enrollment.EnrollmentRemark = setting.DefaultEnrollmentRemark;
                        db.Enrollments.Add(enrollment);
                        await db.SaveChangesAsync();

                        //await _enrollmentService.EnrollStudent(Classid, student.Id);


                        
                        var enroment = await db.Enrollments.Include(x => x.StudentProfile).Include(x => x.User).FirstOrDefaultAsync(x => x.Id == enrollment.Id);
                        var paymentamt = await db.Incomes.FirstOrDefaultAsync(x => x.Id == PaymentAmountId);
                        string unqid = enroment.StudentProfile.Id + DateTime.UtcNow.AddHours(1).ToString("MMddHHmmss");

                        var check = await db.Finances.FirstOrDefaultAsync(x => x.EnrollmentId == enroment.Id && x.PaymentTypeId == paymentamt.Id);
                        if (check == null)
                        {
                            item.SessionId = session.Id;
                            item.RegistrationNumber = enroment.StudentProfile.StudentRegNumber;
                            item.EnrollmentId = enroment.Id;
                            item.Amount = paymentamt.Amount;
                            item.Date = DateTime.UtcNow.AddHours(1);
                            item.UserId = enroment.StudentProfile.UserId;
                            item.Title = paymentamt.Title;
                            item.IncomeId = paymentamt.Id;
                            item.PaymentTypeId = paymentamt.Id;
                            item.UniqueIdCheck = unqid;
                            item.TellerNumber = teller;
                            item.ReferenceId = unqid;
                            item.FinanceType = FinanceType.Credit;
                            item.Payall = true;
                            item.TransactionStatus = TransactionStatus.Paid;
                            item.ApprovedById = userId;
                            item.FinanceSource = FinanceSource.Bank;
                            db.Finances.Add(item);
                            await db.SaveChangesAsync();
                            //add invoice number
                            var invoicen = await db.Finances.FindAsync(item.Id);
                            invoicen.InvoiceNumber = item.Id.ToString("000000");
                            db.Entry(invoicen).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                            //Add Tracking

                            if (userId != null)
                            {
                                var iuser = db.Users.Find(userId);
                                Tracker tracker = new Tracker();
                                tracker.UserId = userId;
                                tracker.UserName = iuser.UserName;
                                tracker.FullName = iuser.Surname + " " + iuser.FirstName + " " + iuser.OtherName;
                                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                                tracker.Note = tracker.FullName + " " + "Created Finance Payment for " + enroment.StudentProfile.StudentRegNumber;
                                //db.Trackers.Add(tracker);
                                await db.SaveChangesAsync();
                            }
                            TempData["success"] = "Payment Successfull";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["error"] = "Already Paid";
                            return View(item);
                        }
                    }
                }
                catch (Exception c)
                {
                    TempData["success"] = "Payment Failed";
                    return View(item);
                }



            }

            return View(item);
        }


        [Authorize(Roles = "SuperAdmin,Finance,FinanceEntry")]

        public async Task<ActionResult> CreatePayment()
        {

            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel.OrderBy(x => x.ClassLevelName), "Id", "ClassLevelName");

            var session = await _sessionService.GetAllSession();
            ViewBag.SessionId = new SelectList(session.OrderByDescending(x => x.FullSession), "Id", "FullSession");

            var Income = await db.Incomes.ToListAsync();
               ViewBag.Income = new SelectList(Income, "Id", "Description");
            return View();
        }

        // POST: Financial/Income/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> CreatePayment(Finance item, int StudentId, int PaymentAmountId, string teller)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string userId = User.Identity.GetUserId();
                    var session = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
                    var enroment = await db.Enrollments.Include(x => x.StudentProfile).FirstOrDefaultAsync(x => x.Id == StudentId);
                    var paymentamt = await db.Incomes.FirstOrDefaultAsync(x => x.Id == PaymentAmountId);
                    string unqid = enroment.StudentProfile.Id + DateTime.UtcNow.AddHours(1).ToString("MMddHHmmss");

                    var check = await db.Finances.FirstOrDefaultAsync(x => x.EnrollmentId == enroment.Id && x.PaymentTypeId == paymentamt.Id);
                    if (check == null)
                    {
                        item.SessionId = session.Id;
                        item.RegistrationNumber = enroment.StudentProfile.StudentRegNumber;
                        item.EnrollmentId = enroment.Id;
                        item.Amount = paymentamt.Amount;
                        item.Date = DateTime.UtcNow.AddHours(1);
                        item.UserId = enroment.StudentProfile.UserId;
                        item.Title = paymentamt.Title;
                        item.IncomeId = paymentamt.Id;
                        item.PaymentTypeId = paymentamt.Id;
                        item.UniqueIdCheck = unqid;
                        item.TellerNumber = teller;
                        item.ReferenceId = unqid;
                        item.FinanceType = FinanceType.Credit;
                        item.Payall = true;
                        item.TransactionStatus = TransactionStatus.Paid;
                        item.ApprovedById = userId;
                        item.FinanceSource = FinanceSource.Bank;
                        db.Finances.Add(item);
                        await db.SaveChangesAsync();
                        //add invoice number
                        var invoicen = await db.Finances.FindAsync(item.Id);
                        invoicen.InvoiceNumber = item.Id.ToString("000000");
                        db.Entry(invoicen).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        //Add Tracking

                        if (userId != null)
                        {
                            var user = db.Users.Find(userId);
                            Tracker tracker = new Tracker();
                            tracker.UserId = userId;
                            tracker.UserName = user.UserName;
                            tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                            tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                            tracker.Note = tracker.FullName + " " + "Created Finance Payment for " + enroment.StudentProfile.StudentRegNumber;
                            //db.Trackers.Add(tracker);
                            await db.SaveChangesAsync();
                        }
                        TempData["success"] = "Payment Successfull";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["error"] = "Already Paid";
                        return View(item);
                    }

                }
                catch (Exception c)
                {
                    TempData["success"] = "Payment Failed";
                    return View(item);
                }



            }

            return View(item);
        }


        [Authorize(Roles = "SuperAdmin,Finance,FinanceEntry")]

        public async Task<ActionResult> CreatePaymentSession()
        {

            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel.OrderBy(x => x.ClassLevelName), "Id", "ClassLevelName");

            var session = await _sessionService.GetAllSession();
            ViewBag.SessionId = new SelectList(session.OrderByDescending(x => x.FullSession), "Id", "FullSession");

            var Income = await db.Incomes.ToListAsync();
            ViewBag.Income = new SelectList(Income, "Id", "Description");
            return View();
        }

        // POST: Financial/Income/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> CreatePaymentSession(Finance item, int StudentId, int PaymentAmountId, int sessionId, string teller)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string userId = User.Identity.GetUserId();
                    var session = db.Sessions.FirstOrDefault(x => x.Id == sessionId);
                    var enroment = await db.Enrollments.Include(x => x.StudentProfile).FirstOrDefaultAsync(x => x.Id == StudentId);
                    var paymentamt = await db.Incomes.FirstOrDefaultAsync(x => x.Id == PaymentAmountId);
                    string unqid = enroment.StudentProfile.Id + DateTime.UtcNow.AddHours(1).ToString("MMddHHmmss");

                    var check = await db.Finances.FirstOrDefaultAsync(x => x.EnrollmentId == enroment.Id && x.PaymentTypeId == paymentamt.Id);
                    if (check == null)
                    {
                        item.SessionId = session.Id;
                        item.RegistrationNumber = enroment.StudentProfile.StudentRegNumber;
                        item.EnrollmentId = enroment.Id;
                        item.Amount = paymentamt.Amount;
                        item.Date = DateTime.UtcNow.AddHours(1);
                        item.UserId = enroment.StudentProfile.UserId;
                        item.Title = paymentamt.Title;
                        item.IncomeId = paymentamt.Id;
                        item.PaymentTypeId = paymentamt.Id;
                        item.UniqueIdCheck = unqid;
                        item.ReferenceId = unqid;
                        item.TellerNumber = teller;
                        item.FinanceType = FinanceType.Credit;
                        item.Payall = true;
                        item.TransactionStatus = TransactionStatus.Paid;
                        item.ApprovedById = userId;
                        item.FinanceSource = FinanceSource.Bank;
                        db.Finances.Add(item);
                        await db.SaveChangesAsync();
                        //add invoice number
                        var invoicen = await db.Finances.FindAsync(item.Id);
                        invoicen.InvoiceNumber = item.Id.ToString("000000");
                        db.Entry(invoicen).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        //Add Tracking

                        if (userId != null)
                        {
                            var user = db.Users.Find(userId);
                            Tracker tracker = new Tracker();
                            tracker.UserId = userId;
                            tracker.UserName = user.UserName;
                            tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                            tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                            tracker.Note = tracker.FullName + " " + "Created Finance Payment for " + enroment.StudentProfile.StudentRegNumber;
                            //db.Trackers.Add(tracker);
                            await db.SaveChangesAsync();
                        }
                        TempData["success"] = "Payment Successfull";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["error"] = "Already Paid";
                        return View(item);
                    }

                }
                catch (Exception c)
                {
                    TempData["success"] = "Payment Failed";
                    return View(item);
                }



            }

            return View(item);
        }

        public async Task<ActionResult> InitializePayment(int id, int sessionId = 0)
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
            var enroment = await db.Enrollments
                 .Include(x => x.ClassLevel)
                .FirstOrDefaultAsync(x => x.SessionId == sid && x.StudentProfileId == studentId.Id);
            ViewBag.eid = studentId.Id;
            IQueryable<FinanceInitializer> pfinance = from s in db.FinanceInitializers
                                                     .Include(x => x.Income)
                                                     .Include(x => x.Enrollment)
                                                     .Where(x => x.EnrollmentId == enroment.Id && x.SessionId == sid)
                                                     .Where(x => x.TransactionStatus == TransactionStatus.Pending)
                                                      select s;
            ViewBag.totalFee = 0;
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

            var data = await db.PaymentDatas.FirstOrDefaultAsync(x => x.Id == id);
            return View(data);
        }


        // GET:  Financial/Payment/ApproveCreditPay
        public ActionResult ApproveCreditPay(int? id)
        {
            ViewBag.id = id;
            return View();
        }

        // POST: Financial/Payment/ApproveCreditPay
        [HttpPost]
        public async Task<ActionResult> ApproveCreditPay(Finance item, int? id)
        {

            if (ModelState.IsValid)
            {
                await _financeService.ApprovePay(item, id);
                TempData["success"] = "Payment have been approved";
                return RedirectToAction("CurrentCreditPayment");
            }
            TempData["error"] = "Unable to Make Payment";
            return RedirectToAction("CurrentCreditPayment");

        }

        // GET:  Financial/Payment/ApproveDebitPay
        public ActionResult ApproveDebitPay(int? id)
        {
            ViewBag.id = id;
            return View();
        }

        // POST: Financial/Payment/ApproveCreditPay
        [HttpPost]
        public async Task<ActionResult> ApproveDebitPay(Finance item, int? id)
        {

            if (ModelState.IsValid)
            {
                await _financeService.ApprovePay(item, id);
                TempData["success"] = "Payment have been approved";
                return RedirectToAction("CurrentDebitPayment");
            }
            TempData["error"] = "Unable to Make Payment";
            return RedirectToAction("CurrentDebitPayment");

        }


        public async Task<ActionResult> PayNow(int sessionId = 0)
        {

            try
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
                var enroment = await db.Enrollments.Include(x => x.ClassLevel).Include(x => x.Session).Include(x => x.StudentProfile).Include(x => x.User).FirstOrDefaultAsync(x => x.SessionId == sid && x.StudentProfileId == studentId.Id);


                IQueryable<FinanceInitializer> pfinance = from s in db.FinanceInitializers
                                                         .Include(x => x.Income)
                                                         .Where(x => x.TransactionStatus == TransactionStatus.Pending)
                                                         .Where(x => x.EnrollmentId == enroment.Id && x.SessionId == sid)
                                                          select s;
                string unqid = studentId.Id + DateTime.UtcNow.AddHours(1).ToString("MMddHHmmss");
                foreach (var i in pfinance)
                {
                    i.UniqueId = unqid;
                    db.Entry(i).State = EntityState.Modified;

                }
                await db.SaveChangesAsync();

                if (studentId.user.Email == null)
                {
                    TempData["warning"] = $"Please update your email address.";
                }
                //
                var paystackkeySetting = await db.Settings.FirstOrDefaultAsync();
                var secretKey = paystackkeySetting.PaystackSecretKey;
                //var secretKey = "sk_test_797baf092b8ef69154e40d3e7df632a301c468b3";
                decimal amount = await pfinance.SumAsync(x => x.Amount);

                var percentcharge = (paystackkeySetting.PaystackChargePercentage / 100) * amount;
                int amountInKobo = (int)amount * 100 + (int)percentcharge;

                var response = await _paystackTransactionService.
                    InitializeTransaction(secretKey, studentId.user.Email,
                    amountInKobo, Convert.ToInt64(unqid), studentId.user.FirstName,
                    studentId.user.Surname);

                if (response.message.ToLower() == "invalid email address passed")
                {
                    TempData["warning"] = $"Please update your email address.";
                }

                if (response.status == true)
                {
                    return Redirect(response.data.authorization_url);
                }

                return RedirectToAction("InvoiceDetails", "Payment", new { sessionId = sid });
            }
            catch (Exception ex)
            {
                TempData["warning"] = $"Payment was not succesful.";
                throw ex;
            }

            return View();
        }


        public async Task<ActionResult> Complete(Finance item)
        {
            var paystackkeySetting = await db.Settings.FirstOrDefaultAsync();
            var secretKey = paystackkeySetting.PaystackSecretKey;
            //
            //var secretKey = _config["SecretKey"];
            //var secretKey = "sk_test_797baf092b8ef69154e40d3e7df632a301c468b3";
            var tranxRef = HttpContext.Request["reference"].ToString();
            if (tranxRef != null)
            {
                var response = await _paystackTransactionService.VerifyTransaction(tranxRef, secretKey);

                var id = response.data.metadata.CustomFields.FirstOrDefault(x => x.DisplayName == "Transaction Id").Value;


                IQueryable<FinanceInitializer> pfinance = from s in db.FinanceInitializers
                                                         .Include(x => x.Income)
                                                         .Include(x => x.Enrollment)
                                                         .Include(x => x.Enrollment.StudentProfile)
                                                         .Include(x => x.Enrollment.StudentProfile.user)
                                                         .Where(x => x.UniqueId == id.ToString())
                                                          select s;
                var io = pfinance.ToList();
                if (response.status == true)
                {
                    foreach (var i in pfinance)
                    {
                        i.ReferenceId = tranxRef;
                        i.FinanceSource = FinanceSource.Online;
                        i.FinanceType = FinanceType.Credit;
                        await _financeService.AddTransaction(i);

                    }
                    var fi = await db.FinanceInitializers
                        .Include(x => x.Income)
                                                         .Include(x => x.Enrollment)
                                                         .Include(x => x.Enrollment.User)
                                                         .Include(x => x.Enrollment.StudentProfile)
                                                         .Include(x => x.Enrollment.StudentProfile.user)
                        .FirstOrDefaultAsync(x => x.UniqueId == id.ToString());

                    PaymentData data = new PaymentData();
                    data.StudentProfileId = fi.Enrollment.StudentProfileId;
                    data.SessionId = fi.Enrollment.SessionId;
                    data.Amount = io.Sum(x => x.Amount);
                    data.FinanceSource = FinanceSource.Online;
                    data.UniqueId = id.ToString();
                    data.ApproveId = fi.Enrollment.StudentProfile.UserId;
                    db.PaymentDatas.Add(data);
                    await db.SaveChangesAsync();

                    return RedirectToAction("MyPayment", "Payment", new { area = "Financial" });
                }
                else
                {

                    TempData["error"] = $"Transaction with Reference {tranxRef} failed.";
                    var ssid = await db.FinanceInitializers.FirstOrDefaultAsync(x => x.UniqueId == id);
                    return RedirectToAction("InvoiceDetails", "Payment", new { sessionId = ssid.SessionId });

                }

            }

            TempData["error"] = $"Invoice Payment with Reference {tranxRef} failed.";

            return RedirectToAction("MyInvoice");
        }

        // GET: Financial/Payment/MakeOnlinePayment
        public async Task<ActionResult> MakeOnlinePayment(int? invoiceId)
        {
            //var Income = await _incomeService.IncomeList();
            //ViewBag.Income = new SelectList(Income, "Title", "Title");
            //var invoice = await _invoiceService.Get(invoiceId);
            return View();
        }

        // POST: Financial/Payment/MakeOnlinePayment
        [HttpPost]
        public async Task<ActionResult> MakeOnlinePayment(Finance item, int? invoiceId)
        {

            if (ModelState.IsValid)
            {
                await _financeService.OnlinePay(item, invoiceId);
                TempData["success"] = "Payment Added Successfully,it will display on the Dashboard after approval by the school";
                return RedirectToAction("MyPayment");
            }
            TempData["error"] = "Unable to Make Payment";
            return RedirectToAction("MyPayment");

        }

       
        [Authorize(Roles = "SuperAdmin")]

        public async Task<ActionResult> MixRemove(long id)
        {
            var exp = await db.Finances.FirstOrDefaultAsync(x => x.Id == id);
            if (exp == null)
            {
                return View("Expenditure");
            }
            return View(exp);
        }
        [Authorize(Roles = "SuperAdmin")]

        // POST: Financial/Payment/MakeDebitPayment
        [HttpPost]
        public async Task<ActionResult> MixRemove(int id)
        {

            if (ModelState.IsValid)
            {
                var uId = User.Identity.GetUserId();
                var finav = await db.Finances.FirstOrDefaultAsync(x => x.Id == id);
                db.Finances.Remove(finav);
                await db.SaveChangesAsync();
                //await _financeService.DebitPay(item);
                TempData["success"] = "Approved Expenditure";
                return RedirectToAction("Expenditure");
            }
            TempData["error"] = "Unable to Approve Expenditure";

            return RedirectToAction("Expenditure");

        }

        //GET: Financial/Payment/MakeDebitPayment
        [Authorize(Roles = "SuperAdmin,Approval")]

        public async Task<ActionResult> ApproveExpenditure(long id)
        {
            var exp = await db.Finances.FirstOrDefaultAsync(x => x.Id == id);
            if(exp == null)
            {
                return View("Expenditure");
            }
            return View(exp);
        }
        [Authorize(Roles = "SuperAdmin,Approval")]

        // POST: Financial/Payment/MakeDebitPayment
        [HttpPost]
        public async Task<ActionResult> ApproveExpenditure(int id, string Note, Finance item)
        {

            if (ModelState.IsValid)
            {
                var uId = User.Identity.GetUserId();
                var finav = await db.Finances.FirstOrDefaultAsync(x => x.Id == id);
                finav.AdminNote = Note;
                finav.ApprovedById = uId;
                finav.TransactionStatus = item.TransactionStatus;
                db.Entry(finav).State = EntityState.Modified;
                await db.SaveChangesAsync();
                //await _financeService.DebitPay(item);
                TempData["success"] = "Approved Expenditure";
                return RedirectToAction("Expenditure");
            }
            TempData["error"] = "Unable to Approve Expenditure";
        
            return RedirectToAction("Expenditure");

        }
        public async Task<ActionResult> MakeDebitPayment()
        {
            var income = await _expendituerService.ExpenditureList();
            ViewBag.income = new SelectList(income, "Title", "Title");
            return View();
        }

        // POST: Financial/Payment/MakeDebitPayment
        [HttpPost]
        public async Task<ActionResult> MakeDebitPayment(Finance item)
        {

            if (ModelState.IsValid)
            {
                await _financeService.DebitPay(item);
                TempData["success"] = "Payment Added Successfully";
                return RedirectToAction("CurrentDebitPayment");
            }
            TempData["error"] = "Unable to Make Payment";
            var income = await _expendituerService.ExpenditureList();
            ViewBag.income = new SelectList(income, "Title", "Title", item.Title);
            //return View(item);
            return RedirectToAction("CurrentDebitPayment");

        }

        //GET: Financial/Payment/MakeCashPayment
        public async Task<ActionResult> MakeCashPayment()
        {
            //var Income = await _incomeService.IncomeList();
            //ViewBag.Income = new SelectList(Income, "Title", "Title");
            return View();
        }

        // POST: Financial/Payment/MakeCashPayment
        [HttpPost]
        public async Task<ActionResult> MakeCashPayment(Finance item, int? invoiceId)
        {

            if (ModelState.IsValid)
            {
                await _financeService.CashPay(item, invoiceId);
                TempData["success"] = "Payment Added Successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Unable to Make Payment";
            //var Income = await _incomeService.IncomeList();
            //ViewBag.Income = new SelectList(Income, "Title", "Title",item.Title);
            //return View(item);
            return RedirectToAction("Index");

        }
        [Authorize(Roles = "SuperAdmin,Finance,FinanceEntry")]

        public async Task<ActionResult> CreateDebit()
        {
            var income = await _expendituerService.ExpenditureList();
            ViewBag.income = new SelectList(income, "Title", "Title");

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Finance,FinanceEntry")]

        public async Task<ActionResult> CreateDebit(Finance item)
        {
            if (ModelState.IsValid)
            {
                await _financeService.DebitPay(item);
                TempData["success"] = "Payment Added Successfully";
                return RedirectToAction("Expenditure");
            }
            TempData["error"] = "Unable to Make Payment";
            var income = await _expendituerService.ExpenditureList();
            ViewBag.income = new SelectList(income, "Title", "Title", item.Title);
            return RedirectToAction("CreateDebit");
        }


        // GET: Financial/Payment/MakeBankPayment
        public async Task<ActionResult> MakeBankPayment(int? id)
        {
            //var Income = await _incomeService.IncomeList();
            //ViewBag.Income = new SelectList(Income, "Title", "Title");
            var bank = await db.BankDetails.ToListAsync();
            ViewBag.bank = bank;

            ViewBag.InvoiceId = id;
            return View();
        }

        // POST: Financial/Payment/MakeBankPayment
        [HttpPost]
        public async Task<ActionResult> MakeBankPayment(Finance item, int? id)
        {

            if (ModelState.IsValid)
            {
                ViewBag.InvoiceId = id;
                await _financeService.BankPay(item, id);
                TempData["success"] = "Payment Added Successfully,it will display on the Dashboard after approval by the school";
                return RedirectToAction("MyPayment");
            }
            TempData["error"] = "Unable to Make Payment";
            //var Income = await _incomeService.IncomeList();
            //ViewBag.Income = new SelectList(Income, "Title", "Title", item.Title);
            //return View(item);
            return RedirectToAction("MyPayment");

        }

        public async Task<ActionResult> CurrentCreditPayment()
        {
            var item = await _financeService.CreditCurrentSessionList();
            return View(item);
        }

        public async Task<ActionResult> Fees()
        {
            var income = await _paymentAmountService.IncomeList();

            return View(income);
        }
        public async Task<ActionResult> PaymentHistory()
        {
            var item = await _financeService.CreditCurrentSessionList();
            return View(item);
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> PaymentHistoryByName(string PaymentType)
        {
            var py = await db.Incomes.FirstOrDefaultAsync(x => x.Title == PaymentType);
            return RedirectToAction("IListPayment", new { PaymentTypeid = py.Id });
        }
        [HttpGet]
        public async Task<ActionResult> IListPayment(int PaymentTypeid, int sessionId = 0)
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
            var py = await db.Incomes.FirstOrDefaultAsync(x => x.Id == PaymentTypeid);

            TempData["name"] = py.Title;
            TempData["id"] = py.Id;
            var item = await _paymentAmountService.ListFinanceByType(PaymentTypeid, sid);

            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session.OrderByDescending(x => x.FullSession), "Id", "FullSession");


            return View(item);
        }
        public async Task<ActionResult> CreditPayment()
        {
            var item = await _financeService.CreditList();
            return View(item);
        }
        [Authorize(Roles = "SuperAdmin,Finance,FinanceEntry")]


        public async Task<ActionResult> Expenditure(string searchString, string currentFilter, int? page, DateTime? DateOne, DateTime? DateTwo, string Duration = "All")
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            string Durationoflist = Duration;
            #region date
            if (DateOne != null)
            {
                DateOne = DateOne.Value.Date;
            }
            else
            {
                DateOne = DateTime.UtcNow.AddHours(1).Date;
            }
            if (DateTwo != null)
            {
                DateTwo = DateTwo.Value.Date;
            }
            else
            {
                DateTwo = DateTime.UtcNow.AddHours(1).Date;
            }
            DateTime? StartDate = DateOne.Value;
            DateTime? EndDate = DateTwo.Value;

            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;

            #endregion

            ViewBag.CurrentFilter = searchString;
            var items = await _financeService.DebitCurrentSessionListPage(searchString, currentFilter, page, DateOne, DateTwo, Duration);
            ViewBag.countall = items.Count();
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));


        }
        public async Task<ActionResult> CurrentDebitPayment()
        {
            var item = await _financeService.DebitCurrentSessionList();
            return View(item);
        }

        public async Task<ActionResult> DebitPayment()
        {
            var item = await _financeService.CreditList();
            return View(item);
        }

        public async Task<ActionResult> CurrentBankPayment()
        {
            var item = await _financeService.BankSourceCurrentSessionList();
            return View(item);
        }
        public async Task<ActionResult> BankPayment()
        {
            var item = await _financeService.BankSourceList();
            return View(item);
        }

        public async Task<ActionResult> CurrentCashPayment()
        {
            var item = await _financeService.CashSourceCurrentSessionList();
            return View(item);
        }
        public async Task<ActionResult> CashPayment()
        {
            var item = await _financeService.CashSourceList();
            return View(item);
        }

        public async Task<ActionResult> CurrentOnlinePayment()
        {
            var item = await _financeService.OnlineSourceCurrentSessionList();
            return View(item);
        }
        public async Task<ActionResult> OnlinePayment()
        {
            var item = await _financeService.OnlineSourceList();
            return View(item);
        }

    }
}