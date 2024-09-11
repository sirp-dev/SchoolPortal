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
    public class SiteHeadersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WebsiteUI/SiteHeaders
        public async Task<ActionResult> Index()
        {
            return View(await db.SiteHeaders.ToListAsync());
        }

        // GET: WebsiteUI/SiteHeaders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteHeader siteHeader = await db.SiteHeaders.FindAsync(id);
            if (siteHeader == null)
            {
                return HttpNotFound();
            }
            return View(siteHeader);
        }

        // GET: WebsiteUI/SiteHeaders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WebsiteUI/SiteHeaders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Content,Show")] SiteHeader siteHeader)
        {
            if (ModelState.IsValid)
            {
                db.SiteHeaders.Add(siteHeader);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(siteHeader);
        }

        // GET: WebsiteUI/SiteHeaders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteHeader siteHeader = await db.SiteHeaders.FindAsync(id);
            if (siteHeader == null)
            {
                return HttpNotFound();
            }
            return View(siteHeader);
        }

        // POST: WebsiteUI/SiteHeaders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Content,Show")] SiteHeader siteHeader)
        {
            if (ModelState.IsValid)
            {
                db.Entry(siteHeader).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(siteHeader);
        }

        // GET: WebsiteUI/SiteHeaders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteHeader siteHeader = await db.SiteHeaders.FindAsync(id);
            if (siteHeader == null)
            {
                return HttpNotFound();
            }
            return View(siteHeader);
        }

        // POST: WebsiteUI/SiteHeaders/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SiteHeader siteHeader = await db.SiteHeaders.FindAsync(id);
            db.SiteHeaders.Remove(siteHeader);
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
