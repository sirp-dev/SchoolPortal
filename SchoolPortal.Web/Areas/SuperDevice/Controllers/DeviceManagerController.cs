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

namespace SchoolPortal.Web.Areas.SuperDevice.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class DeviceManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IDeviceService _deviceService = new DeviceService();
        public DeviceManagerController()
        {

        }

        public DeviceManagerController(DeviceService deviceService)
        {
            _deviceService = deviceService;
        }
        // GET: SuperDevice/DeviceManager
        public async Task<ActionResult> Index()
        {
            var device = await _deviceService.DeviceList();
            return View(device);
        }

        public ActionResult AddComputer()
        {
            return View();
        }

        // POST: SuperDevice/DeviceManager/AddDevice
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> AddComputer(ApprovedDevice device)
        {
            if (ModelState.IsValid)
            {
                await _deviceService.Create(device);
                TempData["success"] = "Device Added Successfully";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Unable to Add Device";
            return View(device);
        }


        public ActionResult AddPhone()
        {
            return View();
        }

        // POST: SuperDevice/DeviceManager/AddDevice
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhone(ApprovedDevice device)
        {
            if (ModelState.IsValid)
            {
                await _deviceService.Create(device);
                TempData["success"] = "Device Added Successfully";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Unable to Add Device";
            return View(device);
        }


        // GET: SuperDevice/DeviceManager/Edit/5
        public async Task<ActionResult> EditComputer(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hostel = await _deviceService.Get(id);
            if (hostel == null)
            {
                return HttpNotFound();
            }
            return View(hostel);
        }

        // POST: SuperDevice/DeviceManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditComputer(ApprovedDevice device)
        {
            if (ModelState.IsValid)
            {
                await _deviceService.Edit(device);
                TempData["success"] = "Device Edited Successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Unable to Edit Device";
            return View(device);
        }


        // GET: SuperDevice/DeviceManager/Edit/5
        public async Task<ActionResult> EditPhone(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hostel = await _deviceService.Get(id);
            if (hostel == null)
            {
                return HttpNotFound();
            }
            return View(hostel);
        }

        // POST: SuperDevice/DeviceManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPhone(ApprovedDevice device)
        {
            if (ModelState.IsValid)
            {
                await _deviceService.Edit(device);
                TempData["success"] = "Device Edited Successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Unable to Edit Device";
            return View(device);
        }

        // GET: SuperDevice/DeviceManager/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hostel = await _deviceService.Get(id);
            if (hostel == null)
            {
                return HttpNotFound();
            }
            return View(hostel);
        }


        // POST: SuperDevice/DeviceManager/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _deviceService.Delete(id);
            TempData["success"] = "Device Deleted Successfully";
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