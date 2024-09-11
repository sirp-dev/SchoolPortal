
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
    public class SitePagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WebsiteUI/SitePages
        public async Task<ActionResult> Index()
        {
            var sitePages = db.SitePages.Include(s => s.SitePageCategory);
            return View(await sitePages.ToListAsync());
        }

        // GET: WebsiteUI/SitePages/Details/5
        public async Task<ActionResult> DetailsMain(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SitePage sitePage = await db.SitePages.Include(x=>x.SitePageCategory).FirstOrDefaultAsync(x=>x.Id == id);
            if (sitePage == null)
            {
                return HttpNotFound();
            }
           

            return View(sitePage);
        }
        // GET: WebsiteUI/SitePages/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SitePage sitePage = await db.SitePages.FindAsync(id);
            if (sitePage == null)
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

            return View(sitePage);
        }

        // GET: WebsiteUI/SitePages/Create
        public ActionResult Create()
        {
            ViewBag.SitePageCategoryId = new SelectList(db.SitePageCategories, "Id", "Title");
            return View();
        }

        // POST: WebsiteUI/SitePages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SitePage sitePage)
        {
            if (ModelState.IsValid)
            {
                string link = "";
                var site = await db.Settings.FirstOrDefaultAsync();
                if (site != null)
                {
                     link = site.WebsiteLink;


                }
                sitePage.TitleLink = sitePage.Title.Replace(" ", "-");
                db.SitePages.Add(sitePage);
                await db.SaveChangesAsync();



                var getpage = await db.SitePages.FirstOrDefaultAsync(x => x.Id == sitePage.Id);
                getpage.PageLink = "https://" + link + "/UI/Pages/" + getpage.Id + "?title=" + getpage.TitleLink;
                db.Entry(getpage).State = EntityState.Modified;



                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.SitePageCategoryId = new SelectList(db.SitePageCategories, "Id", "Title", sitePage.SitePageCategoryId);
            return View(sitePage);
        }

        // GET: WebsiteUI/SitePages/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SitePage sitePage = await db.SitePages.FindAsync(id);
            if (sitePage == null)
            {
                return HttpNotFound();
            }
            ViewBag.SitePageCategoryId = new SelectList(db.SitePageCategories, "Id", "Title", sitePage.SitePageCategoryId);
            return View(sitePage);
        }

        // POST: WebsiteUI/SitePages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SitePage sitePage)
        {
            if (ModelState.IsValid)
            {
                string link = "";
                var site = await db.Settings.FirstOrDefaultAsync();
                if (site != null)
                {
                    link = site.WebsiteLink;


                }
                sitePage.TitleLink = sitePage.Title.Replace(" ", "-");

                sitePage.PageLink = "https://" + link + "/UI/Pages/" + sitePage.Id + "?title=" + sitePage.TitleLink;
                db.Entry(sitePage).State = EntityState.Modified;
                await db.SaveChangesAsync();

                db.Entry(sitePage).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.SitePageCategoryId = new SelectList(db.SitePageCategories, "Id", "Title", sitePage.SitePageCategoryId);
            return View(sitePage);
        }

        // GET: WebsiteUI/SitePages/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SitePage sitePage = await db.SitePages.FindAsync(id);
            if (sitePage == null)
            {
                return HttpNotFound();
            }
            return View(sitePage);
        }

        // POST: WebsiteUI/SitePages/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SitePage sitePage = await db.SitePages.FindAsync(id);
            db.SitePages.Remove(sitePage);
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
