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
    public class SiteGalleryListsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WebsiteUI/SiteGalleryLists
        public async Task<ActionResult> Index()
        {
            return View(await db.SiteGalleryLists.ToListAsync());
        }

        // GET: WebsiteUI/SiteGalleryLists/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteGalleryList siteGalleryList = await db.SiteGalleryLists.FindAsync(id);
            if (siteGalleryList == null)
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

            return View(siteGalleryList);
        }

        // GET: WebsiteUI/SiteGalleryLists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WebsiteUI/SiteGalleryLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Content,Show")] SiteGalleryList siteGalleryList)
        {
            if (ModelState.IsValid)
            {
                db.SiteGalleryLists.Add(siteGalleryList);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(siteGalleryList);
        }

        // GET: WebsiteUI/SiteGalleryLists/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteGalleryList siteGalleryList = await db.SiteGalleryLists.FindAsync(id);
            if (siteGalleryList == null)
            {
                return HttpNotFound();
            }
            return View(siteGalleryList);
        }

        // POST: WebsiteUI/SiteGalleryLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Content,Show")] SiteGalleryList siteGalleryList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(siteGalleryList).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(siteGalleryList);
        }

        // GET: WebsiteUI/SiteGalleryLists/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteGalleryList siteGalleryList = await db.SiteGalleryLists.FindAsync(id);
            if (siteGalleryList == null)
            {
                return HttpNotFound();
            }
            return View(siteGalleryList);
        }

        // POST: WebsiteUI/SiteGalleryLists/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SiteGalleryList siteGalleryList = await db.SiteGalleryLists.FindAsync(id);
            db.SiteGalleryLists.Remove(siteGalleryList);
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
