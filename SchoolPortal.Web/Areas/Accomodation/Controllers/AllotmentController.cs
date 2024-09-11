using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Areas.Accomodation.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin,Student")]
    public class AllotmentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IAccomodationService _accomodationService = new AccomodationService();

        public AllotmentController()
        {
        }

        public AllotmentController(AccomodationService accomodationService)
        {
            _accomodationService = accomodationService;
        }
        // GET: Accomodation/Allotment
        public async Task<ActionResult> Index()
        {
            var item = await _accomodationService.HostelAllotmentList();
            return View(item);
        }


        // GET: Accomodation/MyAccomodation
        public async Task<ActionResult> MyAccomodation()
        {
            var item = await _accomodationService.StudentHostelAllotmentList();
            return View(item);
        }

        // GET: Accomodation/Allotment/Allocate
        public ActionResult Allocate(int? fid)
        {
            ViewBag.id = fid;
            return View();
        }

        // POST: Accomodation/Allotment/Allocate
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Allocate(HostelAllotment allotment, int? fid)
        {
            if (ModelState.IsValid)
            {
                await _accomodationService.AddHostelAllotment(allotment,fid);
                TempData["success"] = "Student Allocated Successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Unable to Allocate to a hostel, it might be that there is no more available space. Please contact the Admin for more information";
            return RedirectToAction("Index");
        }

        // GET: Accomodation/Allotment/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hostelBed = await _accomodationService.GetHostelAllotment(id);
            if (hostelBed == null)
            {
                return HttpNotFound();
            }
            return View(hostelBed);
        }

        // GET: Accomodation/Allotment/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var allotment = await _accomodationService.GetHostelAllotment(id);
            if (allotment == null)
            {
                return HttpNotFound();
            }
            ViewBag.HostelId = new SelectList(db.Hostels, "Id", "Name", allotment.HostelId);
            return View(allotment);
        }

        // POST: Accomodation/Allotment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //////[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(HostelAllotment allotment)
        {
            if (ModelState.IsValid)
            {
                await _accomodationService.EditHostelAllotment(allotment);
                TempData["success"] = "Allotment Edited Successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Unable to Edit Allotment";
            ViewBag.HostelId = new SelectList(db.Hostels, "Id", "Name", allotment.HostelId);
            return View(allotment);
        }

        // GET: Accomodation/Allotment/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hostelBed = await _accomodationService.GetHostelAllotment(id);
            if (hostelBed == null)
            {
                return HttpNotFound();
            }
            return View(hostelBed);
        }

        // POST: Accomodation/Allotment/Delete/5
        [HttpPost, ActionName("Delete")]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _accomodationService.DeleteHostelAllotment(id);
            TempData["success"] = "Allotment Deleted Successfully";
            return RedirectToAction("Index");
        }

        public JsonResult RoomList(int Id)
        {
            var hostelId = db.Hostels.FirstOrDefault(x => x.Id == Id).Id;
            var room = from s in db.HostelRooms
                       where s.HostelId == hostelId
                       select s;

            return Json(new SelectList(room.ToArray(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult BedList(int Id)
        {
            var roomId = db.HostelRooms.FirstOrDefault(x => x.Id == Id).Id;
            var room = from s in db.HostelBeds
                       where s.HostelRoomId == roomId
                       select s;

            return Json(new SelectList(room.ToArray(), "Id", "BedNo"), JsonRequestBehavior.AllowGet);
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