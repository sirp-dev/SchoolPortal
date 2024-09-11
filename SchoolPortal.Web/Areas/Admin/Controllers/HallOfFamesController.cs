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

namespace SchoolPortal.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class HallOfFamesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IHallOfFameService _hallOfFameService = new HallOfFameService();


        public HallOfFamesController()
        {

        }
        public HallOfFamesController(HallOfFameService hallOfFameService)
        {
            _hallOfFameService = hallOfFameService;
        }
        // GET: Admin/HallOfFames
        public async Task<ActionResult> Index()
        {
            var items = await _hallOfFameService.List();
            return View(items);
        }

        // GET: Admin/HallOfFames/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hallOfFame = await _hallOfFameService.Get(id);
            if (hallOfFame == null)
            {
                return HttpNotFound();
            }
            return View(hallOfFame);
        }

        // GET: Admin/HallOfFames/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/HallOfFames/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(HallOfFame hallOfFame, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                await _hallOfFameService.Create(hallOfFame, upload);
               
                return RedirectToAction("Index");
            }

            return View(hallOfFame);
        }

        // GET: Admin/HallOfFames/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hallOfFame = await _hallOfFameService.Get(id);
            if (hallOfFame == null)
            {
                return HttpNotFound();
            }
            return View(hallOfFame);
        }

        // POST: Admin/HallOfFames/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(HallOfFame hallOfFame, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                await _hallOfFameService.Edit(hallOfFame, upload);
                return RedirectToAction("Index");
            }
            return View(hallOfFame);
        }

        // GET: Admin/HallOfFames/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hallOfFame = await _hallOfFameService.Get(id);
            if (hallOfFame == null)
            {
                return HttpNotFound();
            }
            return View(hallOfFame);
        }

        // POST: Admin/HallOfFames/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _hallOfFameService.Delete(id);
            
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
