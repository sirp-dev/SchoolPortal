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
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace SchoolPortal.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class ImageGalleriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IImageGalleryService _ImageGalleryServices = new ImageGalleryService();

        public ImageGalleriesController()
        { }
        public ImageGalleriesController(ImageGalleryService imageServices, ApplicationUserManager userManager)
        {
            _ImageGalleryServices = imageServices;
            UserManager = userManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        // GET: Admin/ImageSliders
        public async Task<ActionResult> Index()
        {
            var imageGallery = await _ImageGalleryServices.List();
            return View(imageGallery);
        }

        // GET: Admin/ImageSliders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImageGallery imageGallery = await db.ImageGallery.FindAsync(id);
            if (imageGallery == null)
            {
                return HttpNotFound();
            }

            return View(imageGallery);
        }

        // GET: Admin/ImageSliders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/ImageSliders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ImageGallery models, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                models.CurrentGallery = true;
                await _ImageGalleryServices.New(models, upload);
                return RedirectToAction("Index");
            }

            return View(models);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImageGallery img = await db.ImageGallery.FindAsync(id);
            if (img == null)
            {
                return HttpNotFound();
            }
            return View(img);
        }

        // POST: Content/Assignments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ImageGallery imageGallery)
        {
            if (ModelState.IsValid)
            {
                db.Entry(imageGallery).State = EntityState.Modified;
                await db.SaveChangesAsync();

                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                if(userId2 != null)
                {
                    var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Edited Gallery Image";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
               

                return RedirectToAction("Index");
            }
            return View(imageGallery);
        }


        // POST: Admin/ImageSliders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> AddToGallery(int id)
        {
            var models = await db.ImageGallery.FindAsync(id);
            await _ImageGalleryServices.AddToGallery(models);
            return RedirectToAction("Index");

        }

        // GET: Admin/ImageSliders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImageGallery imageGallery = await db.ImageGallery.FindAsync(id);
            if (imageGallery == null)
            {
                return HttpNotFound();
            }
            return View(imageGallery);
        }

        // POST: Admin/ImageSliders/Delete/5
        //[HttpPost, ActionName("Delete")]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _ImageGalleryServices.Delete(id);
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
