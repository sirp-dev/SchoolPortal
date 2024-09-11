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

namespace SchoolPortal.Web.Areas.Content.Controllers
{
    [Authorize]
    public class TimeTablesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Content/TimeTables
        public async Task<ActionResult> Index()
        {
            var timeTables = db.TimeTables.Include(t => t.ClassLevel);
            return View(await timeTables.ToListAsync());
        }

        // GET: Content/TimeTables/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeTable timeTable = await db.TimeTables.FindAsync(id);
            if (timeTable == null)
            {
                return HttpNotFound();
            }
            return View(timeTable);
        }

        // GET: Content/TimeTables/Create
        public ActionResult Create()
        {
            ViewBag.ClassLevelId = new SelectList(db.ClassLevels, "Id", "ClassName");
            return View();
        }

        // POST: Content/TimeTables/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,ClassLevelId,Monday,M7_8,M8_9,M9_10,M10_11,M11_12,M12_13,M13_14,M14_15,M15_16,M16_17,M17_18,Tuesday,T7_8,T8_9,T9_10,T10_11,T11_12,T12_13,T13_14,T14_15,T15_16,T16_17,T17_18,Wednessday,W7_8,W8_9,W9_10,W10_11,W11_12,W12_13,W13_14,W14_15,W15_16,W16_17,W17_18,Thursday,Th7_8,Th8_9,Th9_10,Th10_11,Th11_12,Th12_13,Th13_14,Th14_15,Th15_16,Th16_17,Th17_18,Friday,F7_8,F8_9,F9_10,F10_11,F11_12,F12_13,F13_14,F14_15,F15_16,F16_17,F17_18")] TimeTable timeTable)
        {
            if (ModelState.IsValid)
            {
                db.TimeTables.Add(timeTable);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ClassLevelId = new SelectList(db.ClassLevels, "Id", "ClassName", timeTable.ClassLevelId);
            return View(timeTable);
        }

        // GET: Content/TimeTables/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeTable timeTable = await db.TimeTables.FindAsync(id);
            if (timeTable == null)
            {
                return HttpNotFound();
            }
            return View(timeTable);
        }

        // POST: Content/TimeTables/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ClassLevelId,Monday,M7_8,M8_9,M9_10,M10_11,M11_12,M12_13,M13_14,M14_15,M15_16,M16_17,M17_18,Tuesday,T7_8,T8_9,T9_10,T10_11,T11_12,T12_13,T13_14,T14_15,T15_16,T16_17,T17_18,Wednessday,W7_8,W8_9,W9_10,W10_11,W11_12,W12_13,W13_14,W14_15,W15_16,W16_17,W17_18,Thursday,Th7_8,Th8_9,Th9_10,Th10_11,Th11_12,Th12_13,Th13_14,Th14_15,Th15_16,Th16_17,Th17_18,Friday,F7_8,F8_9,F9_10,F10_11,F11_12,F12_13,F13_14,F14_15,F15_16,F16_17,F17_18")] TimeTable timeTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(timeTable).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(timeTable);
        }

        // GET: Content/TimeTables/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeTable timeTable = await db.TimeTables.FindAsync(id);
            if (timeTable == null)
            {
                return HttpNotFound();
            }
            return View(timeTable);
        }

        // POST: Content/TimeTables/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TimeTable timeTable = await db.TimeTables.FindAsync(id);
            db.TimeTables.Remove(timeTable);
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
