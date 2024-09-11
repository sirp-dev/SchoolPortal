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
using PagedList.Mvc;
using SchoolPortal.Web.Areas.Data.Services;
using PagedList;
using SchoolPortal.Web.Models.Dtos;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace SchoolPortal.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class PinManagementController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IPinService _pinService = new PinService();
        private ISessionService _sessionService = new SessionService();


        public PinManagementController()
        {


        }
        public PinManagementController(PinService pinService, SessionService sessionService, ApplicationUserManager userManager)
        {
            _pinService = pinService;
            _sessionService = sessionService;
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
        // GET: Admin/PinManagement

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
            var items = await _pinService.List(searchString, currentFilter, page);
            ViewBag.countU = items.Count();
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            ViewBag.Total = await _pinService.TotalPin();
            return View(items.OrderBy(x => x.Usage).ToPagedList(pageNumber, pageSize));

        }

        //public async Task<ActionResult> Index(string searchString, string searchStringSession, string currentFilter, int? page)
        //{


        //    if (searchString != null)
        //    {
        //        page = 1;
        //    }
        //    else
        //    {
        //        searchString = currentFilter + searchString;
        //    }
        //    ///
        //    //session dropdown

        //    var item = db.Sessions.OrderBy(x => x.Id);
        //    var session = item.Select(x => new SchoolSessionDto
        //    {
        //        FullSession = x.SessionYear + " - " + x.Term,
        //        Id = x.Id,
        //        SessionStatus = x.Status
        //    });
        //    ViewBag.sessionId = new SelectList(session, "Id", "FullSession");

        //    ///
        //    ViewBag.CurrentFilter = searchString;
        //    var items = await _pinService.UsedPin(searchString, searchStringSession, currentFilter, page);
        //    ViewBag.countU = items.Count();
        //    int pageSize = 100;
        //    int pageNumber = (page ?? 1);
        //    ViewBag.Total = await _pinService.TotalUsedPin();
        //    return View(items.ToPagedList(pageNumber, pageSize));

        //}

        public async Task<ActionResult> UpdatePinWithSessionId()
        {
            var pins = await db.PinCodeModels.Where(x => x.EnrollmentId != null).ToListAsync();
            foreach (var pin in pins)
            {
                try
                {


                    var enroll = await db.Enrollments.FirstOrDefaultAsync(x => x.Id == pin.EnrollmentId);
                    if (enroll != null)
                    {
                        pin.SessionId = enroll.SessionId;
                        db.Entry(pin).State = EntityState.Modified;
                        await db.SaveChangesAsync();



                        //Update Printed Status
                        enroll.Printed = true;
                        db.Entry(enroll).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        //Add Tracking
                        var userId2 = User.Identity.GetUserId();
                        if(userId2 != null)
                        {
                            var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                            Tracker tracker = new Tracker();
                            tracker.UserId = userId2;
                            tracker.UserName = user2.UserName;
                            tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                            tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                            tracker.Note = tracker.FullName + " " + "Updated Pin Enrollment number session";
                            //db.Trackers.Add(tracker);
                            await db.SaveChangesAsync();
                        }
                       

                    }
                    else
                    {

                    }

                }
                catch (Exception d)
                {

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

        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult> EditPin(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var pin = db.PinCodeModels.FirstOrDefault(x => x.Id == id);
            if (pin == null)
            {
                return HttpNotFound();
            }

            return View(pin);
        }

        // POST: Content/Assignments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPin(PinCodeModel pin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pin).State = EntityState.Modified;
                await db.SaveChangesAsync();

                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Updated Pin Card";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
                return RedirectToAction("AdminPinIndex");
            }

            return View(pin);
        }

        public ActionResult ResetAdd(int id)
        {
            var card = db.PinCodeModels.FirstOrDefault(x => x.Id == id);
            card.Usage += 1;
            db.Entry(card).State = EntityState.Modified;
            db.SaveChanges();

            //Add Tracking
            var userId2 = User.Identity.GetUserId();
            var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
            Tracker tracker = new Tracker();
            tracker.UserId = userId2;
            tracker.UserName = user2.UserName;
            tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
            tracker.ActionDate = DateTime.UtcNow.AddHours(1);
            tracker.Note = tracker.FullName + " " + "Reset Pin Card addition";
            //db.Trackers.Add(tracker);
            db.SaveChangesAsync();
            return RedirectToAction("AdminPinIndex");
        }
        public ActionResult ResetMinus(int id)
        {
            var card = db.PinCodeModels.FirstOrDefault(x => x.Id == id);
            card.Usage -= 1;
            db.Entry(card).State = EntityState.Modified;
            db.SaveChanges();

            //Add Tracking
            var userId2 = User.Identity.GetUserId();
            var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
            Tracker tracker = new Tracker();
            tracker.UserId = userId2;
            tracker.UserName = user2.UserName;
            tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
            tracker.ActionDate = DateTime.UtcNow.AddHours(1);
            tracker.Note = tracker.FullName + " " + "Reset Pin Card Subtraction";
            //db.Trackers.Add(tracker);
            db.SaveChangesAsync();
            return RedirectToAction("AdminPinIndex");
        }
        // GET: Admin/PinManagement
        public async Task<ActionResult> AdminPinIndex(string searchString, string currentFilter, int? page)
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
            var items = await _pinService.List(searchString, currentFilter, page);
            ViewBag.countAll = items.Count();
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            ViewBag.Total = await _pinService.TotalPin();
            return View(items.OrderBy(x => x.Usage).ToPagedList(pageNumber, pageSize));

        }

        public async Task<ActionResult> AdminPinIndexList()
        {
            IQueryable<PinCodeModel> items = from s in db.PinCodeModels
                                        .Include(x => x.Session).OrderByDescending(x => x.PinNumber)
                                            select s;
            ViewBag.countAll = items.Count();
           

            ViewBag.Total = await _pinService.TotalPin();
            return View(items.AsQueryable());

        }

        public async Task<ActionResult> IndexofUnused(string searchString, string currentFilter, int? page)
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
            var items = await _pinService.UnUsedPin(searchString, currentFilter, page);
            ViewBag.countUn = items.Count();
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            ViewBag.Total = await _pinService.TotalUnUsedPin();
            return View(items.ToPagedList(pageNumber, pageSize));
        }

        public async Task<ActionResult> IndexofUsed(string searchString, string currentFilter, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            ViewBag.CurrentFilter = searchString;
            var items = await _pinService.UsedPin(searchString, currentSession.Id.ToString(), currentFilter, page);
            ViewBag.countUn = items.Count();
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            ViewBag.Total = await _pinService.TotalUsedPin();
            return View(items.ToPagedList(pageNumber, pageSize));
        }

        public async Task<ActionResult> IndexofAll(string searchString, string currentFilter, int? page)
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
            var items = await _pinService.List(searchString, currentFilter, page);
            ViewBag.countAll = items.Count();
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            ViewBag.Total = await _pinService.TotalPin();
            return View(items.ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/PinManagement/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var pinCodeModel = await _pinService.Details(id);
            if (pinCodeModel == null)
            {
                return HttpNotFound();
            }
            return View(pinCodeModel);
        }
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Create()
        {

            return View();
        }

        // POST: Content/Assignments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PinCodeModel mm)
        {
            if (ModelState.IsValid)
            {
                mm.DateCreated = DateTime.UtcNow;
                db.PinCodeModels.Add(mm);
                await db.SaveChangesAsync();

                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Added Pin Card";
                //db.Trackers.Add(tracker);
                db.SaveChangesAsync();
                return RedirectToAction("AdminPinIndex");
            }

            return View(mm);
        }

        public ActionResult _CardAnalysis()
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            //all card
            var allcard = db.PinCodeModels.Count();
            ViewBag.allcards = allcard;

            //total used
            var totalused = db.PinCodeModels.Where(x => x.StudentPin != null).Count();
            ViewBag.usedcards = totalused;

            //available card 
            var totalava = db.PinCodeModels.Where(x => x.StudentPin == null).Count();
            ViewBag.available = totalava;

            //used current term
            var currentterm = db.PinCodeModels.Where(x => x.StudentPin != null && x.SessionId == currentSession.Id).Count();
            ViewBag.currentterm = currentterm;

            ViewBag.session = currentSession.SessionYear + "-" + currentSession.Term;

            return PartialView();

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
