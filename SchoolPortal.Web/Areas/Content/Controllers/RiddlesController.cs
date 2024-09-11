using SchoolPortal.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using SchoolPortal.Web.Models.Entities;

namespace SchoolPortal.Web.Areas.Content.Controllers
{
    [Authorize]
    public class RiddlesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Content/Riddles
        // GET: Content/Riddles
        public async Task<ActionResult> Index()
        {
            return View(await db.Riddles.ToListAsync());
        }

        // GET: Content/Riddles/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Riddles riddles = await db.Riddles.FindAsync(id);
            if (riddles == null)
            {
                return HttpNotFound();
            }
            return View(riddles);
        }

        // GET: Content/Riddles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Content/Riddles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Riddles riddles)
        {
            if (ModelState.IsValid)
            {
                db.Riddles.Add(riddles);
               await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(riddles);
        }

        // GET: Content/Riddles/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Riddles riddles = await db.Riddles.FindAsync(id);
            if (riddles == null)
            {
                return HttpNotFound();
            }
            return View(riddles);
        }

        // POST: Content/Riddles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Riddles riddles)
        {
            if (ModelState.IsValid)
            {
                db.Entry(riddles).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(riddles);
        }

        // GET: Content/Riddles/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Riddles riddles = await db.Riddles.FindAsync(id);
            if (riddles == null)
            {
                return HttpNotFound();
            }
            return View(riddles);
        }

        // POST: Content/Riddles/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Riddles riddles = await db.Riddles.FindAsync(id);
            db.Riddles.Remove(riddles);
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