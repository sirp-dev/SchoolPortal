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

namespace SchoolPortal.Web.Areas.WebsiteManager.Controllers
{
    public class ContentPagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WebsiteManager/ContentPages
        public async Task<ActionResult> Index()
        {
            var ContentPages = db.ContentPages.Include(c => c.CategoryPage);
            return View(await ContentPages.ToListAsync());
        }

        // GET: WebsiteManager/ContentPages/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentPage ContentPage = await db.ContentPages.FindAsync(id);
            if (ContentPage == null)
            {
                return HttpNotFound();
            }
            return View(ContentPage);
        }

        // GET: WebsiteManager/ContentPages/Create
        public ActionResult Create()
        {
            ViewBag.CategoryPageId = new SelectList(db.CategoryPages.Where(x => x.MenuDescription == MenuDescription.Dropdown), "Id", "Title");
            return View();
        }

        // POST: WebsiteManager/ContentPages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ContentPage ContentPage)
        {
            if (ModelState.IsValid)
            {
                db.ContentPages.Add(ContentPage);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryPageId = new SelectList(db.CategoryPages.Where(x=>x.MenuDescription == MenuDescription.Dropdown), "Id", "Title", ContentPage.CategoryPageId);
            return View(ContentPage);
        }

        public async Task<ActionResult> UpdatePublishPage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentPage ContentPage = await db.ContentPages.FindAsync(id);
            if (ContentPage == null)
            {
                return HttpNotFound();
            }
            if(ContentPage.Publish == PagePublish.NotPublish)
            {
                ContentPage.Publish = PagePublish.Publish;
            }else
            {
                ContentPage.Publish = PagePublish.NotPublish;
            }

            db.Entry(ContentPage).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
            
        }

        // GET: WebsiteManager/ContentPages/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentPage ContentPage = await db.ContentPages.FindAsync(id);
            if (ContentPage == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryPageId = new SelectList(db.CategoryPages.Where(x => x.MenuDescription == MenuDescription.Dropdown), "Id", "Title", ContentPage.CategoryPageId);
            return View(ContentPage);
        }

        // POST: WebsiteManager/ContentPages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ContentPage ContentPage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ContentPage).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryPageId = new SelectList(db.CategoryPages.Where(x => x.MenuDescription == MenuDescription.Dropdown), "Id", "Title", ContentPage.CategoryPageId);
            return View(ContentPage);
        }

        // GET: WebsiteManager/ContentPages/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentPage ContentPage = await db.ContentPages.FindAsync(id);
            if (ContentPage == null)
            {
                return HttpNotFound();
            }
            return View(ContentPage);
        }

        // POST: WebsiteManager/ContentPages/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ContentPage ContentPage = await db.ContentPages.FindAsync(id);
            db.ContentPages.Remove(ContentPage);
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
