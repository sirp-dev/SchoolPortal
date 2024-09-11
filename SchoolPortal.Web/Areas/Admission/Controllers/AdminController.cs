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

namespace SchoolPortal.Web.Areas.Admission.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IRegistrationDataService _registerServices = new RegistrationDataService();
        private IImageService _imageServices = new ImageService();

        public AdminController()
        { }
        public AdminController(RegistrationDataService registerServices, ImageService imageService)
        {
            _registerServices = registerServices;
            _imageServices = imageService;
        }

        // GET: Admission/StudentDatas
        public async Task<ActionResult> Index()
        {
            var item = await _registerServices.List();
            return View(item);
        }

        public async Task<ActionResult> GiveAdmissionreturnindex(int id)
        {
            await _registerServices.AdmitStudent(id);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> GiveAdmissionreturndetails(int id)
        {
            await _registerServices.AdmitStudent(id);
            return RedirectToAction("Details", new { id = id });
        }

        // GET: Admission/StudentDatas/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var studentData = await _registerServices.Get(id);
            if (studentData == null)
            {
                return HttpNotFound();
            }
            return View(studentData);
        }

        // GET: Admission/StudentDatas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admission/StudentDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(StudentData studentData, HttpPostedFileBase upload, string refid)
        {
            if (ModelState.IsValid)
            {
                await _registerServices.Create(studentData,refid);
                var Imageid = await _imageServices.Create(upload);
                studentData.ImageId = Imageid;
                await _registerServices.Edit(studentData);
                return RedirectToAction("Index");
            }

            return View(studentData);
        }

        // GET: Admission/StudentDatas/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var studentData = await _registerServices.GetEdit(id);
            if (studentData == null)
            {
                return HttpNotFound();
            }
            return View(studentData);
        }

        // POST: Admission/StudentDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(StudentData studentData, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
               // studentData.DateOfBirth = DateTime.ParseExact(studentData.DateOfBirth, "dd/MM/yyyy", null);
                if (upload != null && upload.ContentLength > 0)
                {
                    var image = await _imageServices.Get(studentData.ImageId);
                    if(image.ImageContent != null)
                    {
                        await _imageServices.Delete(studentData.ImageId);
                    }
                  var newImageId = await _imageServices.Create(upload);
                    studentData.ImageId = newImageId;
                }
                await _registerServices.Edit(studentData);
                
                return RedirectToAction("Index");
            }
            return View(studentData);
        }

        public async Task<ActionResult> UpdateScore(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var studentData = await _registerServices.GetEdit(id);
            if (studentData == null)
            {
                return HttpNotFound();
            }
            return View(studentData);
        }

        // POST: Admission/StudentDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateScore(StudentData studentData)
        {
            if (ModelState.IsValid)
            {
               
                await _registerServices.Edit(studentData);

                return RedirectToAction("Index");
            }
            return View(studentData);
        }

        // GET: Admission/StudentDatas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentData studentData = await db.StudentDatas.FindAsync(id);
            if (studentData == null)
            {
                return HttpNotFound();
            }
            return View(studentData);
        }

        // POST: Admission/StudentDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            StudentData studentData = await db.StudentDatas.FindAsync(id);
            db.StudentDatas.Remove(studentData);
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
