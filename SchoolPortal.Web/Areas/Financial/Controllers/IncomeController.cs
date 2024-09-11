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
using Microsoft.AspNet.Identity;

namespace SchoolPortal.Web.Areas.Financial.Controllers
{
    public class IncomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Financial/Income
        public async Task<ActionResult> Index()
        {
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);

            var finances = db.Finances.Include(f => f.Session).Where(x=>x.SessionId == currentSession.Id && x.FinanceType == FinanceType.Credit) ;
            return View(await finances.ToListAsync());
        }

        // GET: Financial/Income/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Finance finance = await db.Finances.FindAsync(id);
            if (finance == null)
            {
                return HttpNotFound();
            }
            return View(finance);
        }

        // GET: Financial/Income/Create
        public ActionResult Create()
        {
           
            return View();
        }

        // POST: Financial/Income/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Finance finance)
        {
            if (ModelState.IsValid)
            {
                string uid = User.Identity.GetUserId();
                var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
                finance.SessionId = currentSession.Id;
                finance.UserId = uid;
                finance.FinanceType = FinanceType.Credit;
                finance.TransactionStatus = TransactionStatus.Pending;
                finance.Date = DateTime.UtcNow.AddHours(1);
                db.Finances.Add(finance);
                await db.SaveChangesAsync();

               // if()

                return RedirectToAction("Index");
            }

            return View(finance);
        }

        // GET: Financial/Income/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Finance finance = await db.Finances.FindAsync(id);
            if (finance == null)
            {
                return HttpNotFound();
            }
            return View(finance);
        }

        // POST: Financial/Income/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Finance finance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(finance).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(finance);
        }

        // GET: Financial/Income/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Finance finance = await db.Finances.FindAsync(id);
            if (finance == null)
            {
                return HttpNotFound();
            }
            return View(finance);
        }

        // POST: Financial/Income/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Finance finance = await db.Finances.FindAsync(id);
            db.Finances.Remove(finance);
            await db.SaveChangesAsync();
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
