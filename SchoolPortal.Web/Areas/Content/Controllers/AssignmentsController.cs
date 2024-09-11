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
    public class AssignmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Content/Assignments
        public async Task<ActionResult> Index()
        {
            var assignments = db.Assignments.Include(a => a.ClassLevel).Include(a => a.Session).Include(a => a.Subject);
            return View(await assignments.ToListAsync());
        }

        // GET: Content/Assignments/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignment assignment = await db.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            return View(assignment);
        }

        // GET: Content/Assignments/Create
        public ActionResult Create()
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            ViewBag.ClassLevelId = new SelectList(db.ClassLevels, "Id", "ClassName");
            ViewBag.SessionId = new SelectList(db.Sessions, "Id", "Term");
            ViewBag.SubjectId = new SelectList(db.Subjects, "Id", "SubjectName");
            return View();
        }

        // POST: Content/Assignments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,ClassLevelId,SessionId,SubjectId,Title,Description,DateCreated,DateSubmitionEnds,IsPublished")] Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                db.Assignments.Add(assignment);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            ViewBag.ClassLevelId = new SelectList(db.ClassLevels, "Id", "ClassName", assignment.ClassLevelId);
            ViewBag.SessionId = new SelectList(db.Sessions, "Id", "Term", assignment.SessionId);
            ViewBag.SubjectId = new SelectList(db.Subjects, "Id", "SubjectName", assignment.SubjectId);
            return View(assignment);
        }

        // GET: Content/Assignments/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignment assignment = await db.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClassLevelId = new SelectList(db.ClassLevels, "Id", "ClassName", assignment.ClassLevelId);
            ViewBag.SessionId = new SelectList(db.Sessions, "Id", "Term", assignment.SessionId);
            ViewBag.SubjectId = new SelectList(db.Subjects, "Id", "SubjectName", assignment.SubjectId);
            return View(assignment);
        }

        // POST: Content/Assignments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ClassLevelId,SessionId,SubjectId,Title,Description,DateCreated,DateSubmitionEnds,IsPublished")] Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(assignment).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            ViewBag.ClassLevelId = new SelectList(db.ClassLevels, "Id", "ClassName", assignment.ClassLevelId);
            ViewBag.SessionId = new SelectList(db.Sessions, "Id", "Term", assignment.SessionId);
            ViewBag.SubjectId = new SelectList(db.Subjects, "Id", "SubjectName", assignment.SubjectId);
            return View(assignment);
        }

        // GET: Content/Assignments/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignment assignment = await db.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            return View(assignment);
        }

        // POST: Content/Assignments/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Assignment assignment = await db.Assignments.FindAsync(id);
            db.Assignments.Remove(assignment);
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
