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

namespace SchoolPortal.Web.Areas.Content.Controllers
{
    [Authorize]
    public class SyllablesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ISyllableService _syllableService = new SyllableService();


        public SyllablesController()
        {

        }
        public SyllablesController(
            SyllableService syllableService
            )
        {
            _syllableService = syllableService;
        }
        // GET: Content/Syllables
        public async Task<ActionResult> Index()
        {
            var syllables = await _syllableService.ListAll();
            return View(syllables);
        }

        // GET: Content/Syllables/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var syllable = await _syllableService.Get(id);
            if (syllable == null)
            {
                return HttpNotFound();
            }
            return View(syllable);
        }

        // GET: Content/Syllables/Create
        public ActionResult Create()
        {
            ViewBag.ClassLevelId = new SelectList(db.ClassLevels, "Id", "ClassName");
            return View();
        }

        // POST: Content/Syllables/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Syllable syllable)
        {
            if (ModelState.IsValid)
            {

                var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
                
                syllable.SessionId = currentSession.Id;
                syllable.DateAdded = DateTime.UtcNow.AddHours(1);
                await _syllableService.Create(syllable);
                return RedirectToAction("Index");
            }
            return View(syllable);
        }

        // GET: Content/Syllables/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var syllable = await _syllableService.Get(id);
            if (syllable == null)
            {
                return HttpNotFound();
            }
            return View(syllable);
        }

        // POST: Content/Syllables/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Syllable syllable)
        {
            if (ModelState.IsValid)
            {
                await _syllableService.Edit(syllable);
                return RedirectToAction("Index");
            }
            return View(syllable);
        }

        // GET: Content/Syllables/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var syllable = await _syllableService.Get(id);
            if (syllable == null)
            {
                return HttpNotFound();
            }
            return View(syllable);
        }

        // POST: Content/Syllables/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _syllableService.Delete(id);
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
