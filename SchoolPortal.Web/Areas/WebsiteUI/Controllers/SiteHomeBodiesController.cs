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
    public class SiteHomeBodiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WebsiteUI/SiteHomeBodies
        public async Task<ActionResult> Index()
        {
            return View(await db.SiteHomeBodys.ToListAsync());
        }

        // GET: WebsiteUI/SiteHomeBodies/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteHomeBody siteHomeBody = await db.SiteHomeBodys.FindAsync(id);
            if (siteHomeBody == null)
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

            return View(siteHomeBody);
        }

        // GET: WebsiteUI/SiteHomeBodies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WebsiteUI/SiteHomeBodies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Content,Show")] SiteHomeBody siteHomeBody)
        {
            if (ModelState.IsValid)
            {
                db.SiteHomeBodys.Add(siteHomeBody);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(siteHomeBody);
        }

        // GET: WebsiteUI/SiteHomeBodies/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteHomeBody siteHomeBody = await db.SiteHomeBodys.FindAsync(id);
            if (siteHomeBody == null)
            {
                return HttpNotFound();
            }
            return View(siteHomeBody);
        }

        // POST: WebsiteUI/SiteHomeBodies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Content,Show")] SiteHomeBody siteHomeBody)
        {
            if (ModelState.IsValid)
            {
                db.Entry(siteHomeBody).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(siteHomeBody);
        }

        // GET: WebsiteUI/SiteHomeBodies/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteHomeBody siteHomeBody = await db.SiteHomeBodys.FindAsync(id);
            if (siteHomeBody == null)
            {
                return HttpNotFound();
            }
            return View(siteHomeBody);
        }

        // POST: WebsiteUI/SiteHomeBodies/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SiteHomeBody siteHomeBody = await db.SiteHomeBodys.FindAsync(id);
            db.SiteHomeBodys.Remove(siteHomeBody);
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
