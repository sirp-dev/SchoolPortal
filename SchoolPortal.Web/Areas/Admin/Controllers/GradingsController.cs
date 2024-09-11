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
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace SchoolPortal.Web.Areas.Admin.Controllers
{
    //[Authorize]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class GradingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IGradingService _gradingService = new GradingService();
      

        public GradingsController()
        {

        }
        public GradingsController(
            GradingService gradingService,
              ApplicationUserManager userManager
           )
        {
            _gradingService = gradingService;
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
        // GET: Admin/Gradings
        public async Task<ActionResult> Index()
        {
            var item = await _gradingService.List();
            return View(item);
        }

        // GET: Admin/Gradings/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var grading = await _gradingService.Get(id);
            if (grading == null)
            {
                return HttpNotFound();
            }
            return View(grading);
        }

        // GET: Admin/Gradings/Create
        public async Task<ActionResult> Create()
        {
            var setting = await db.Settings.FirstOrDefaultAsync();
            ViewBag.isPrimary = setting.IsPrimaryNursery;
            return View();
        }

        // POST: Admin/Gradings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Grading grading)
        {
            if (ModelState.IsValid)
            {
                
                db.Gradings.Add(grading);
                await db.SaveChangesAsync();

                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Added Grading";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();


                return RedirectToAction("Index");
            }

            return View(grading);
        }

        // GET: Admin/Gradings/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var grading = await _gradingService.Get(id);
            if (grading == null)
            {
                return HttpNotFound();
            }
            return View(grading);
        }

        // POST: Admin/Gradings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Grading grading)
        {
            if (ModelState.IsValid)
            {
                await _gradingService.Edit(grading);
                return RedirectToAction("Index");
            }
            return View(grading);
        }

        // GET: Admin/Gradings/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var grading = await _gradingService.Get(id);
            if (grading == null)
            {
                return HttpNotFound();
            }
            return View(grading);
        }

        // POST: Admin/Gradings/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _gradingService.Delete(id);
            return RedirectToAction("Index");
        }

        // GET: Admin/GradingDetails/Create
        public ActionResult AddGrade(int gradeId)
        {

            ViewBag.GradingId = db.Gradings.FirstOrDefault(x => x.Id == gradeId).Name;
            return View();
        }

        // POST: Admin/GradingDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> AddGrade(GradingDetails gradingDetails, int gradeId)
        {
            if (ModelState.IsValid)
            {
                var check = await db.GradingDetails.Where(x => x.GradingId == gradeId).ToListAsync();
                if(check.Select(x=>x.LowerLimit).Contains(gradingDetails.LowerLimit) || check.Select(x => x.UpperLimit).Contains(gradingDetails.UpperLimit) || check.Select(x => x.Remark).Contains(gradingDetails.Remark) || check.Select(x => x.Grade).Contains(gradingDetails.Grade))
                {
                    TempData["error"] = "Grading already exist";
                    return View(gradingDetails);
                }
                gradingDetails.GradingId = gradeId;
                await _gradingService.Add(gradingDetails);
                return RedirectToAction("Details", new { id = gradingDetails.GradingId });
            }

          //  ViewBag.GradingId = new SelectList(db.Gradings, "Id", "Name", gradingDetails.GradingId);
            return View(gradingDetails);
        }

        public async Task<ActionResult> DeleteGradeDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var gradingDetails = await _gradingService.GetGrade(id);
            if (gradingDetails == null)
            {
                return HttpNotFound();
            }
            return View(gradingDetails);
        }

        // POST: Admin/GradingDetails/Delete/5
        [HttpPost, ActionName("DeleteGradeDetail")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteGradeDetailConfirmed(int id)
        {
            await _gradingService.DeleteGrade(id);
            return RedirectToAction("Index");
        }


        // GET: Admin/GradingDetails/Edit/5
        public async Task<ActionResult> EditGrade(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var gradingDetails = await _gradingService.GetGrade(id);
            if (gradingDetails == null)
            {
                return HttpNotFound();
            }
            ViewBag.GradingId = new SelectList(db.Gradings, "Id", "Name", gradingDetails.GradingId);
            return View(gradingDetails);
        }

        // POST: Admin/GradingDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditGrade(GradingDetails gradingDetails)
        {
            if (ModelState.IsValid)
            {
                await _gradingService.EditGrade(gradingDetails);
                return RedirectToAction("Details", new { id = gradingDetails.GradingId });
            }
            ViewBag.GradingId = new SelectList(db.Gradings, "Id", "Name", gradingDetails.GradingId);
            return View(gradingDetails);
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
