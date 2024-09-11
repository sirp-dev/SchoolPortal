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
using SchoolPortal.Web.Models.UI;
using SchoolPortal.Web.Models.Dtos;

namespace SchoolPortal.Web.Areas.WebsiteUI.Controllers
{
    public class SiteGalleriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WebsiteUI/SiteGalleries
        public async Task<ActionResult> Index()
        {
            return View(await db.SiteGalleries.ToListAsync());
        }

        // GET: WebsiteUI/SiteGalleries/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteGallery siteGallery = await db.SiteGalleries.FindAsync(id);
            if (siteGallery == null)
            {
                return HttpNotFound();
            }
            //header
            var header = await db.SiteHeaders.FirstOrDefaultAsync();
            if (header != null)
            {
                ViewBag.head = header.Content;
            }

            var overridecss = await db.SiteOverrideCSSs.FirstOrDefaultAsync();
            if (overridecss != null)
            {
                ViewBag.overridecss = overridecss.Content;
            }

            var newslist = await db.SiteGalleryLists.FirstOrDefaultAsync();


            var post = await db.ImageGallery.ToListAsync();

            var output = post.Select(x => new PageDto
            {
                Id = x.Id,
                Content = newslist.Content.Replace("{/IMAGE/}", x.ImageByte)

            });

            ViewBag.post = output.ToList();

            //header
            var footerJs = await db.SiteFooterJSs.FirstOrDefaultAsync();
            if (footerJs != null)
            {
                ViewBag.footerJs = footerJs.Content;
            }

            return View(siteGallery);
        }

        // GET: WebsiteUI/SiteGalleries/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WebsiteUI/SiteGalleries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SiteGallery siteGallery)
        {
            if (ModelState.IsValid)
            {
                db.SiteGalleries.Add(siteGallery);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(siteGallery);
        }

        // GET: WebsiteUI/SiteGalleries/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteGallery siteGallery = await db.SiteGalleries.FindAsync(id);
            if (siteGallery == null)
            {
                return HttpNotFound();
            }
            return View(siteGallery);
        }

        // POST: WebsiteUI/SiteGalleries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SiteGallery siteGallery)
        {
            if (ModelState.IsValid)
            {
                db.Entry(siteGallery).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(siteGallery);
        }

        // GET: WebsiteUI/SiteGalleries/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteGallery siteGallery = await db.SiteGalleries.FindAsync(id);
            if (siteGallery == null)
            {
                return HttpNotFound();
            }
            return View(siteGallery);
        }

        // POST: WebsiteUI/SiteGalleries/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SiteGallery siteGallery = await db.SiteGalleries.FindAsync(id);
            db.SiteGalleries.Remove(siteGallery);
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
