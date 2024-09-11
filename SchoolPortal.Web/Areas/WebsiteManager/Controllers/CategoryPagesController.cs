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
    public class CategoryPagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WebsiteManager/CategoryPages
        public async Task<ActionResult> Index()
        {
            return View(await db.CategoryPages.ToListAsync());
        }

        // GET: WebsiteManager/CategoryPages/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoryPage categoryPage = await db.CategoryPages.FindAsync(id);
            if (categoryPage == null)
            {
                return HttpNotFound();
            }
            return View(categoryPage);
        }

        public async Task<ActionResult> UpdatePublishPage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoryPage categoryPage = await db.CategoryPages.FindAsync(id);
            if (categoryPage == null)
            {
                return HttpNotFound();
            }
            if (categoryPage.Publish == PagePublish.NotPublish)
            {
                categoryPage.Publish = PagePublish.Publish;
            }
            else
            {
                categoryPage.Publish = PagePublish.NotPublish;
            }

            db.Entry(categoryPage).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
            
        }
        // GET: WebsiteManager/CategoryPages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WebsiteManager/CategoryPages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CategoryPage categoryPage)
        {
            if (ModelState.IsValid)
            {
                db.CategoryPages.Add(categoryPage);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(categoryPage);
        }

        /////// style sheet/ js/ tag
        ///

        ///.
        ///
        
        public ActionResult NewStyle()
        {
            return View();
        }

        // POST: WebsiteManager/CategoryPages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> NewStyle(CategoryPage categoryPage)
        {
            if (ModelState.IsValid)
            {
                db.CategoryPages.Add(categoryPage);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(categoryPage);
        }

        // GET: WebsiteManager/CategoryPages/Edit/5
        public async Task<ActionResult> StyleEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoryPage categoryPage = await db.CategoryPages.FindAsync(id);
            if (categoryPage == null)
            {
                return HttpNotFound();
            }
            return View(categoryPage);
        }

        // POST: WebsiteManager/CategoryPages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> StyleEdit(CategoryPage categoryPage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(categoryPage).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(categoryPage);
        }


        /// <summary>
        /// //
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: WebsiteManager/CategoryPages/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoryPage categoryPage = await db.CategoryPages.FindAsync(id);
            if (categoryPage == null)
            {
                return HttpNotFound();
            }
            return View(categoryPage);
        }

        // POST: WebsiteManager/CategoryPages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CategoryPage categoryPage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(categoryPage).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(categoryPage);
        }

        // GET: WebsiteManager/CategoryPages/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoryPage categoryPage = await db.CategoryPages.FindAsync(id);
            if (categoryPage == null)
            {
                return HttpNotFound();
            }
            return View(categoryPage);
        }

        // POST: WebsiteManager/CategoryPages/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CategoryPage categoryPage = await db.CategoryPages.FindAsync(id);
            db.CategoryPages.Remove(categoryPage);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public ActionResult PageLinks()
        {
            //header tag
            try
            {
                var item = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.MenuDescription == Models.Entities.MenuDescription.HeadTagContent).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadTagContent);
                ViewBag.taghead = item.ContentHome;
            }
            catch (Exception f) { }
            //header style
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 105).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.styletag = style.ContentHome;

            }
            catch (Exception c) { }

            //footer js
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 155).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.jstag = style.ContentHome;

            }
            catch (Exception c) { }

            var settings = db.Settings.FirstOrDefault().PortalLink;
            ViewBag.url = settings;
            var pages = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.None).ToList();
            return View(pages);
        }

        public ActionResult Help()
        {
            //header tag
            try
            {
                var item = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.MenuDescription == Models.Entities.MenuDescription.HeadTagContent).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadTagContent);
                ViewBag.taghead = item.ContentHome;
            }
            catch (Exception f) { }
            //header style
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 105).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.styletag = style.ContentHome;

            }
            catch (Exception c) { }

            //footer js
            try
            {
                var style = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.SortOrder == 155).OrderByDescending(x => x.SortOrder).FirstOrDefault(x => x.Publish == Models.Entities.PagePublish.Publish && x.MenuDescription == Models.Entities.MenuDescription.HeadStylesheet);
                ViewBag.jstag = style.ContentHome;

            }
            catch (Exception c) { }

            var settings = db.Settings.FirstOrDefault().PortalLink;
            ViewBag.url = settings;
            var pages = db.CategoryPages.Include(x => x.ContentPages).Where(x => x.Publish == Models.Entities.PagePublish.Publish).ToList();
            return View(pages);
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
