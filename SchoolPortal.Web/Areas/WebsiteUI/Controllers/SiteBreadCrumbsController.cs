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
    public class SiteBreadCrumbsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WebsiteUI/SiteBreadCrumbs
        public async Task<ActionResult> Index()
        {
            return View(await db.SiteBreadCrumbs.ToListAsync());
        }

        // GET: WebsiteUI/SiteBreadCrumbs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteBreadCrumb siteBreadCrumb = await db.SiteBreadCrumbs.FindAsync(id);
            if (siteBreadCrumb == null)
            {
                return HttpNotFound();
            }
            //header
            var header = await db.SiteHeaders.FirstOrDefaultAsync();
            if(header != null)
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


            return View(siteBreadCrumb);
        }

        // GET: WebsiteUI/SiteBreadCrumbs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WebsiteUI/SiteBreadCrumbs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Content,Show")] SiteBreadCrumb siteBreadCrumb)
        {
            if (ModelState.IsValid)
            {
                db.SiteBreadCrumbs.Add(siteBreadCrumb);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(siteBreadCrumb);
        }

        // GET: WebsiteUI/SiteBreadCrumbs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteBreadCrumb siteBreadCrumb = await db.SiteBreadCrumbs.FindAsync(id);
            if (siteBreadCrumb == null)
            {
                return HttpNotFound();
            }
            return View(siteBreadCrumb);
        }

        // POST: WebsiteUI/SiteBreadCrumbs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Content,Show")] SiteBreadCrumb siteBreadCrumb)
        {
            if (ModelState.IsValid)
            {
                db.Entry(siteBreadCrumb).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(siteBreadCrumb);
        }

        // GET: WebsiteUI/SiteBreadCrumbs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteBreadCrumb siteBreadCrumb = await db.SiteBreadCrumbs.FindAsync(id);
            if (siteBreadCrumb == null)
            {
                return HttpNotFound();
            }
            return View(siteBreadCrumb);
        }

        // POST: WebsiteUI/SiteBreadCrumbs/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SiteBreadCrumb siteBreadCrumb = await db.SiteBreadCrumbs.FindAsync(id);
            db.SiteBreadCrumbs.Remove(siteBreadCrumb);
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
