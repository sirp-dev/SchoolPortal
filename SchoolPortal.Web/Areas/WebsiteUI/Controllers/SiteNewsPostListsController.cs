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
    public class SiteNewsPostListsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WebsiteUI/SiteNewsPostLists
        public async Task<ActionResult> Index()
        {
            return View(await db.SiteNewsPostList.ToListAsync());
        }

        // GET: WebsiteUI/SiteNewsPostLists/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteNewsPostList siteNewsPostList = await db.SiteNewsPostList.FindAsync(id);
            if (siteNewsPostList == null)
            {
                return HttpNotFound();
            }
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
            //

            var post = await db.Posts.Include(x=>x.PostImages).Where(x => x.PostImages.FirstOrDefault() != null).OrderByDescending(x => x.DatePosted).ToListAsync();

            var output = post.Select(x => new PageDto
            {
                Id = x.Id,
                Content = siteNewsPostList.Content.Replace("{/TITLE/}", x.Title).Replace("{/IMAGE/}", x.PostImages.FirstOrDefault().ImageByte ?? "").Replace("{/LINK/}", x.Link).Replace("{/PREVIEWTEXT/}", x.PreviewContent).Replace("{/DATE/}", x.DatePosted.ToString("MMM dd, yyyy"))

            }) ;

            ViewBag.post = output.ToList();
            //header
            var footerJs = await db.SiteFooterJSs.FirstOrDefaultAsync();
            if (footerJs != null)
            {
                ViewBag.footerJs = footerJs.Content;
            }
            return View(siteNewsPostList);
        }

        // GET: WebsiteUI/SiteNewsPostLists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WebsiteUI/SiteNewsPostLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SiteNewsPostList siteNewsPostList)
        {
            if (ModelState.IsValid)
            {
                db.SiteNewsPostList.Add(siteNewsPostList);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(siteNewsPostList);
        }

        // GET: WebsiteUI/SiteNewsPostLists/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteNewsPostList siteNewsPostList = await db.SiteNewsPostList.FindAsync(id);
            if (siteNewsPostList == null)
            {
                return HttpNotFound();
            }
            return View(siteNewsPostList);
        }

        // POST: WebsiteUI/SiteNewsPostLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SiteNewsPostList siteNewsPostList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(siteNewsPostList).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(siteNewsPostList);
        }

        // GET: WebsiteUI/SiteNewsPostLists/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteNewsPostList siteNewsPostList = await db.SiteNewsPostList.FindAsync(id);
            if (siteNewsPostList == null)
            {
                return HttpNotFound();
            }
            return View(siteNewsPostList);
        }

        // POST: WebsiteUI/SiteNewsPostLists/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SiteNewsPostList siteNewsPostList = await db.SiteNewsPostList.FindAsync(id);
            db.SiteNewsPostList.Remove(siteNewsPostList);
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
