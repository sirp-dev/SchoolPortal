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
    public class SiteSettingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WebsiteUI/SiteSettings
        public async Task<ActionResult> Index()
        {
            return View(await db.SiteSettings.ToListAsync());
        }

        // GET: WebsiteUI/SiteSettings/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteSetting siteSetting = await db.SiteSettings.FindAsync(id);
            if (siteSetting == null)
            {
                return HttpNotFound();
            }
            return View(siteSetting);
        }

        // GET: WebsiteUI/SiteSettings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WebsiteUI/SiteSettings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Show,EmailOne,EmailTwo,EmailThree,PhoneOne,PhoneTwo,PhoneThree,AddressOne,AddressTwo,AddressThree,Host,PX,Sender,Receiver")] SiteSetting siteSetting)
        {
            if (ModelState.IsValid)
            {
                db.SiteSettings.Add(siteSetting);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(siteSetting);
        }

        // GET: WebsiteUI/SiteSettings/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteSetting siteSetting = await db.SiteSettings.FindAsync(id);
            if (siteSetting == null)
            {
                return HttpNotFound();
            }
            return View(siteSetting);
        }

        // POST: WebsiteUI/SiteSettings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Show,EmailOne,EmailTwo,EmailThree,PhoneOne,PhoneTwo,PhoneThree,AddressOne,AddressTwo,AddressThree,Host,PX,Sender,Receiver")] SiteSetting siteSetting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(siteSetting).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(siteSetting);
        }

        // GET: WebsiteUI/SiteSettings/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteSetting siteSetting = await db.SiteSettings.FindAsync(id);
            if (siteSetting == null)
            {
                return HttpNotFound();
            }
            return View(siteSetting);
        }

        // POST: WebsiteUI/SiteSettings/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SiteSetting siteSetting = await db.SiteSettings.FindAsync(id);
            db.SiteSettings.Remove(siteSetting);
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
