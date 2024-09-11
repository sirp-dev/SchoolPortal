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
    public class SiteFooterJSController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WebsiteUI/SiteFooterJS
        public async Task<ActionResult> Index()
        {
            return View(await db.SiteFooterJSs.ToListAsync());
        }

        // GET: WebsiteUI/SiteFooterJS/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteFooterJS siteFooterJS = await db.SiteFooterJSs.FindAsync(id);
            if (siteFooterJS == null)
            {
                return HttpNotFound();
            }
            return View(siteFooterJS);
        }

        // GET: WebsiteUI/SiteFooterJS/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WebsiteUI/SiteFooterJS/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Content,Show")] SiteFooterJS siteFooterJS)
        {
            if (ModelState.IsValid)
            {
                db.SiteFooterJSs.Add(siteFooterJS);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(siteFooterJS);
        }

        // GET: WebsiteUI/SiteFooterJS/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteFooterJS siteFooterJS = await db.SiteFooterJSs.FindAsync(id);
            if (siteFooterJS == null)
            {
                return HttpNotFound();
            }
            return View(siteFooterJS);
        }

        // POST: WebsiteUI/SiteFooterJS/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Content,Show")] SiteFooterJS siteFooterJS)
        {
            if (ModelState.IsValid)
            {
                db.Entry(siteFooterJS).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(siteFooterJS);
        }

        // GET: WebsiteUI/SiteFooterJS/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteFooterJS siteFooterJS = await db.SiteFooterJSs.FindAsync(id);
            if (siteFooterJS == null)
            {
                return HttpNotFound();
            }
            return View(siteFooterJS);
        }

        // POST: WebsiteUI/SiteFooterJS/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SiteFooterJS siteFooterJS = await db.SiteFooterJSs.FindAsync(id);
            db.SiteFooterJSs.Remove(siteFooterJS);
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
