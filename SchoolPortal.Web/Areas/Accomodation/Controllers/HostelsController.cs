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

namespace SchoolPortal.Web.Areas.Accomodation.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class HostelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IAccomodationService _accomodationService = new AccomodationService();

        public HostelsController()
        {

        }

        public HostelsController(AccomodationService accomodationService )
        {
            _accomodationService = accomodationService;
        }

        // GET: Accomodation/Hostels
        public async Task<ActionResult> Index()
        {
            var item = await _accomodationService.HostelList();
            return View(item);
        }

        // GET: Accomodation/Hostels/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hostel = await _accomodationService.GetHostel(id);
            if (hostel == null)
            {
                return HttpNotFound();
            }
            return View(hostel);
        }

        // GET: Accomodation/Hostels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Accomodation/Hostels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Hostel hostel)
        {
            if (ModelState.IsValid)
            {
                await _accomodationService.AddHostel(hostel);
                TempData["success"] = "Hostel Added Successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Unable to add Hostel";
            return View(hostel);
        }

        // GET: Accomodation/Hostels/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hostel = await _accomodationService.GetHostel(id);
            if (hostel == null)
            {
                return HttpNotFound();
            }
            return View(hostel);
        }

        // POST: Accomodation/Hostels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Hostel hostel)
        {
            if (ModelState.IsValid)
            {
                await _accomodationService.EditHostel(hostel);
                TempData["success"] = "Hostel Edited Successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Unable to Edit Hostel";
            return View(hostel);
        }

        // GET: Accomodation/Hostels/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hostel = await _accomodationService.GetHostel(id);
            if (hostel == null)
            {
                return HttpNotFound();
            }
            return View(hostel);
        }

        // POST: Accomodation/Hostels/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _accomodationService.DeleteHostel(id);
            TempData["success"] = "Hostel Deleted Successfully";
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
