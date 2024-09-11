using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Entities;

namespace SchoolPortal.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class SchoolFeesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ISchoolFeeService _schoolFeeService = new SchoolFeeService();
        private ISessionService _sessionService = new SessionService();


        public SchoolFeesController()
        {

        }
        public SchoolFeesController(SchoolFeeService schoolFeeService, SessionService sessionService)
        {
            _schoolFeeService = schoolFeeService;
            _sessionService = sessionService;

        }
        // GET: Admin/ SchoolAccounts
        public async Task<ActionResult> Index()
        {
            var items = await _schoolFeeService.List();
            return View(items);
        }

        // GET: Admin/SchoolAccounts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var fee = await _schoolFeeService.Get(id);
            if (fee == null)
            {
                return HttpNotFound();
            }
            return View(fee);
        }

        // GET: Admin/SchoolAccounts/Create
        public async Task<ActionResult> Create()
        {
            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session, "Id", "FullSession");
            return View();
        }

        // POST: Admin/SchoolAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SchoolFees fee)
        {
            if (ModelState.IsValid)
            {
                await _schoolFeeService.Create(fee);

                return RedirectToAction("Index");
            }


            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session, "Id", "FullSession",fee.SessionId);
            return View(fee);
        }

        // GET: Admin/SchoolAccounts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var fee = await _schoolFeeService.Get(id);
            if (fee == null)
            {
                return HttpNotFound();
            }
            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session, "Id", "FullSession", fee.SessionId);
            return View(fee);
        }

        // POST: Admin/SchoolAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SchoolFees fee)
        {
            if (ModelState.IsValid)
            {
                await _schoolFeeService.Edit(fee);
                return RedirectToAction("Index");
            }
            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session, "Id", "FullSession", fee.SessionId);
            return View(fee);
        }

        // GET: Admin/SchoolAccounts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var acct = await _schoolFeeService.Get(id);
            if (acct == null)
            {
                return HttpNotFound();
            }

            return View(acct);
        }

        // POST: Admin/SchoolAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _schoolFeeService.Delete(id);

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
