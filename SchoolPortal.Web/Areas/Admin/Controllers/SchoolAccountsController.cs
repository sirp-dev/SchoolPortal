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
    public class SchoolAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ISchoolAccountService _schoolAccountService = new SchoolAccountService();


        public SchoolAccountsController()
        {

        }
        public SchoolAccountsController(SchoolAccountService schoolAccountService)
        {
            _schoolAccountService = schoolAccountService;
        }
        // GET: Admin/ SchoolAccounts
        public async Task<ActionResult> Index()
        {
            var items = await _schoolAccountService.List();
            return View(items);
        }

        // GET: Admin/SchoolAccounts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var acct = await _schoolAccountService.Get(id);
            if (acct == null)
            {
                return HttpNotFound();
            }
            return View(acct);
        }

        // GET: Admin/SchoolAccounts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/SchoolAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SchoolAccount acct)
        {
            if (ModelState.IsValid)
            {
                await _schoolAccountService.Create(acct);

                return RedirectToAction("Index");
            }

            return View(acct);
        }

        // GET: Admin/SchoolAccounts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var acct = await _schoolAccountService.Get(id);
            if (acct == null)
            {
                return HttpNotFound();
            }
            return View(acct);
        }

        // POST: Admin/SchoolAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SchoolAccount acct)
        {
            if (ModelState.IsValid)
            {
                await _schoolAccountService.Edit(acct);
                return RedirectToAction("Index");
            }
            return View(acct);
        }

        // GET: Admin/SchoolAccounts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var acct = await _schoolAccountService.Get(id);
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
            await _schoolAccountService.Delete(id);

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
