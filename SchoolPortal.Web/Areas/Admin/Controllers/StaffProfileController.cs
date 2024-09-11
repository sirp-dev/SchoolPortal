using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Dtos;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Staff,Admin")]
    public class StaffProfileController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        private IStaffService _staffProfileService = new StaffService();


        public StaffProfileController()
        {

        }
        public StaffProfileController(StaffService staffProfileService)
        {
            _staffProfileService = staffProfileService;
        }
        // GET: Admin/StaffProfile
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _staffProfileService.Get(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            return View(item);
        }
        [AllowAnonymous]
        public JsonResult LgaList(string Id)
        {
            var stateId = db.States.FirstOrDefault(x => x.StateName == Id).Id;
            var local = from s in db.LocalGovs
                        where s.StatesId == stateId
                        select s;

            return Json(new SelectList(local.ToArray(), "LGAName", "LGAName"), JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> EditUser(int? id)
        {
            ViewBag.StateName = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await _staffProfileService.Get(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(StaffInfoDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _staffProfileService.Edit(model);
                    TempData["success"] = "Update Successful.";
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    TempData["error"] = "Update Unsuccessful, (" + e.ToString() + ")";
                }

            }
            return View(model);
        }

        public async Task<ActionResult> MyQualification(int id)
        {
            var items = await _staffProfileService.ListQualification(id);
            return View(items);
        }


        public async Task<ActionResult> QualificationDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _staffProfileService.GetQualification(id);
            if (item == null)
            {
                return HttpNotFound();
            }
           
            return View(item);
        }
        public async Task<ActionResult> AddQualification()
        {
          
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> AddQualification(Qualification model)
        {
            if (ModelState.IsValid)
            {
               var id = await _staffProfileService.CreateQualification(model);
                TempData["success"] = "Qualification Added Successfully";
                return RedirectToAction("Index", "Panel", new { area = "Staff" });
            }
            TempData["error"] = "Unable to add Qualification";
            return View(model);
        }

        public async Task<ActionResult> EditQualification(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _staffProfileService.GetQualification(id);
            if (item == null)
            {
                return HttpNotFound();
            }
           
            return View(item);
        }


        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditQualification(Qualification model)
        {
            if (ModelState.IsValid)
            {
               var id = await _staffProfileService.EditQualification(model);
                TempData["success"] = "Qualification Updated Successfully";
                return RedirectToAction("Index", "Panel", new { area = "Staff" });
            }
            TempData["error"] = "Unable to edit Qualification";

            return View(model);
        }

        public async Task<ActionResult> DeleteQualification(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _staffProfileService.GetQualification(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Admin/Settings/Delete/5
        [HttpPost, ActionName("DeleteQualification")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteQualificationConfirmed(int id)
        {
           var sid = await _staffProfileService.DeleteQualification(id);
            TempData["success"] = "Qualification Deleted Successfully";
            return RedirectToAction("Index", "Panel", new { area = "Staff" });
        }



    }
}