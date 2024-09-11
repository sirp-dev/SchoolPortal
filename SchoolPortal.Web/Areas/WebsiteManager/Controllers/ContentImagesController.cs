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
using System.IO;

namespace SchoolPortal.Web.Areas.WebsiteManager.Controllers
{
    public class ContentImagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WebsiteManager/ContentImages
        public async Task<ActionResult> Index()
        {
            return View(await db.ContentImages.ToListAsync());
        }

        // GET: WebsiteManager/ContentImages/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentImage contentImage = await db.ContentImages.FindAsync(id);
            if (contentImage == null)
            {
                return HttpNotFound();
            }
            return View(contentImage);
        }

        // GET: WebsiteManager/ContentImages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WebsiteManager/ContentImages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ContentImage contentImage, List<HttpPostedFileBase> upload)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (upload.Count() > 0)
                    {
                        foreach (var image in upload)
                        {

                            if (image != null && image.ContentLength > 0)
                            {


                                string date1 = DateTime.UtcNow.AddHours(1).ToString("ddMMyyyyhhmm");
                                string name = date1 + "-" + image.FileName;
                                string fileName = Path.GetFileName(name);
                                contentImage.ImageUrl = fileName;
                                contentImage.FIleName = fileName;
                                fileName = Path.Combine(Server.MapPath("~/Aq_Image/"), fileName);
                                image.SaveAs(fileName);

                                db.ContentImages.Add(contentImage);
                                await db.SaveChangesAsync();
                            }
                        }
                        return RedirectToAction("Index");
                    }
                }catch(Exception c)
                {
                    TempData["error"] = c;
                }
            }

            return View(contentImage);
        }

        // GET: WebsiteManager/ContentImages/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentImage contentImage = await db.ContentImages.FindAsync(id);
            if (contentImage == null)
            {
                return HttpNotFound();
            }
            return View(contentImage);
        }

        // POST: WebsiteManager/ContentImages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ImageUrl,FIleName")] ContentImage contentImage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contentImage).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(contentImage);
        }

        // GET: WebsiteManager/ContentImages/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentImage contentImage = await db.ContentImages.FindAsync(id);
            if (contentImage == null)
            {
                return HttpNotFound();
            }
            return View(contentImage);
        }

        // POST: WebsiteManager/ContentImages/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ContentImage contentImage = await db.ContentImages.FindAsync(id);
            //string[] files = Directory.GetFiles("~/Aq_Image/");
            if (System.IO.File.Exists(Server.MapPath("~/Aq_Image/" + contentImage.ImageUrl)))
            {
                System.IO.File.Delete(Server.MapPath("~/Aq_Image/" + contentImage.ImageUrl));
            }
            db.ContentImages.Remove(contentImage);
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
