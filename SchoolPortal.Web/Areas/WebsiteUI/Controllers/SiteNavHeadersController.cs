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

namespace SchoolPortal.Web.Areas.WebsiteUI.Controllers
{
    public class SiteNavHeadersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WebsiteUI/SiteNavHeaders
        public async Task<ActionResult> Index()
        {
            return View(await db.SiteNavHeaders.ToListAsync());
        }

        // GET: WebsiteUI/SiteNavHeaders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteNavHeader siteNavHeader = await db.SiteNavHeaders.FindAsync(id);
            if (siteNavHeader == null)
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
            //header
            var footerJs = await db.SiteFooterJSs.FirstOrDefaultAsync();
            if (footerJs != null)
            {
                ViewBag.footerJs = footerJs.Content;
            }


            return View(siteNavHeader);
        }

        // GET: WebsiteUI/SiteNavHeaders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WebsiteUI/SiteNavHeaders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Content,Show")] SiteNavHeader siteNavHeader)
        {
            if (ModelState.IsValid)
            {
                db.SiteNavHeaders.Add(siteNavHeader);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(siteNavHeader);
        }

        // GET: WebsiteUI/SiteNavHeaders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteNavHeader siteNavHeader = await db.SiteNavHeaders.FindAsync(id);
            if (siteNavHeader == null)
            {
                return HttpNotFound();
            }
            return View(siteNavHeader);
        }

        // POST: WebsiteUI/SiteNavHeaders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Content,Show")] SiteNavHeader siteNavHeader)
        {
            if (ModelState.IsValid)
            {
                db.Entry(siteNavHeader).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(siteNavHeader);
        }

        // GET: WebsiteUI/SiteNavHeaders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteNavHeader siteNavHeader = await db.SiteNavHeaders.FindAsync(id);
            if (siteNavHeader == null)
            {
                return HttpNotFound();
            }
            return View(siteNavHeader);
        }

        // POST: WebsiteUI/SiteNavHeaders/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SiteNavHeader siteNavHeader = await db.SiteNavHeaders.FindAsync(id);
            db.SiteNavHeaders.Remove(siteNavHeader);
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
