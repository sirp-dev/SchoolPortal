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
using System.Configuration;

namespace SchoolPortal.Web.Areas.SuperUser.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class WebsiteSettingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SuperUser/WebsiteSettings
        public async Task<ActionResult> Index()
        {
            return View(await db.WebsiteSettings.ToListAsync());
        }

        // GET: SuperUser/WebsiteSettings/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebsiteSettings websiteSettings = await db.WebsiteSettings.FindAsync(id);
            if (websiteSettings == null)
            {
                return HttpNotFound();
            }
            return View(websiteSettings);
        }

        // GET: SuperUser/WebsiteSettings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SuperUser/WebsiteSettings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(WebsiteSettings websiteSettings, HttpPostedFileBase upload, HttpPostedFileBase icon)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {


                    // Find its length and convert it to byte array
                    int ContentLength = upload.ContentLength;

                    // Create Byte Array
                    byte[] bytImg = new byte[ContentLength];

                    // Read Uploaded file in Byte Array
                    upload.InputStream.Read(bytImg, 0, ContentLength);

                    websiteSettings.WebsiteLogo = bytImg;

                }

                if (icon != null && icon.ContentLength > 0)
                {


                    // Find its length and convert it to byte array
                    int ContentLength = icon.ContentLength;

                    // Create Byte Array
                    byte[] bytImg = new byte[ContentLength];

                    // Read Uploaded file in Byte Array
                    icon.InputStream.Read(bytImg, 0, ContentLength);

                    websiteSettings.WebsiteIcon = bytImg;

                }

                db.WebsiteSettings.Add(websiteSettings);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(websiteSettings);
        }

        // GET: SuperUser/WebsiteSettings/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebsiteSettings websiteSettings = await db.WebsiteSettings.FindAsync(id);
            if (websiteSettings == null)
            {
                return HttpNotFound();
            }
            return View(websiteSettings);
        }

        // POST: SuperUser/WebsiteSettings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(WebsiteSettings websiteSettings, HttpPostedFileBase upload, HttpPostedFileBase icon)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {


                    // Find its length and convert it to byte array
                    int ContentLength = upload.ContentLength;

                    // Create Byte Array
                    byte[] bytImg = new byte[ContentLength];

                    // Read Uploaded file in Byte Array
                    upload.InputStream.Read(bytImg, 0, ContentLength);

                    websiteSettings.WebsiteLogo = bytImg;

                }
                if (icon != null && icon.ContentLength > 0)
                {


                    // Find its length and convert it to byte array
                    int ContentLength = icon.ContentLength;

                    // Create Byte Array
                    byte[] bytImg = new byte[ContentLength];

                    // Read Uploaded file in Byte Array
                    icon.InputStream.Read(bytImg, 0, ContentLength);

                    websiteSettings.WebsiteIcon = bytImg;

                }


                db.Entry(websiteSettings).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(websiteSettings);
        }

        // GET: SuperUser/WebsiteSettings/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebsiteSettings websiteSettings = await db.WebsiteSettings.FindAsync(id);
            if (websiteSettings == null)
            {
                return HttpNotFound();
            }
            return View(websiteSettings);
        }

        // POST: SuperUser/WebsiteSettings/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            WebsiteSettings websiteSettings = await db.WebsiteSettings.FindAsync(id);
            db.WebsiteSettings.Remove(websiteSettings);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public ActionResult Config()
        {
            string St = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string conn;
            try
            {
                int pFrom = St.IndexOf("Initial") + "Initial".Length;
                int pTo = St.LastIndexOf("User Id");

                conn = St.Substring(pFrom, pTo - pFrom);
            }
            catch (Exception c)
            {
                conn = St;
            }

            return Content(conn);
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
