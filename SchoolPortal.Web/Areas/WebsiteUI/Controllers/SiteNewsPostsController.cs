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
    public class SiteNewsPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WebsiteUI/SiteNewsPosts
        public async Task<ActionResult> Index()
        {
            return View(await db.SiteNewsPosts.ToListAsync());
        }

        // GET: WebsiteUI/SiteNewsPosts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteNewsPost siteNewsPost = await db.SiteNewsPosts.FindAsync(id);
            if (siteNewsPost == null)
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
            //
            var newslist = await db.SiteNewsPostList.FirstOrDefaultAsync();


            var post = await db.Posts.Include(x => x.PostImages).Where(x => x.PostImages.FirstOrDefault() != null).OrderByDescending(x => x.DatePosted).Take(3).ToListAsync();


            var output = post.Select(x => new PageDto
            {
                Id = x.Id,
                Content = newslist.Content.Replace("{/TITLE/}", x.Title).Replace("{/IMAGE/}", x.PostImages.FirstOrDefault().ImageByte ?? "").Replace("{/LINK/}", x.Link).Replace("{/PREVIEWTEXT/}", x.PreviewContent).Replace("{/DATE/}", x.DatePosted.ToString("MMM dd, yyyy"))

            });

            ViewBag.post = output.ToList();

            //header
            var footerJs = await db.SiteFooterJSs.FirstOrDefaultAsync();
            if (footerJs != null)
            {
                ViewBag.footerJs = footerJs.Content;
            }

            return View(siteNewsPost);
        }

        // GET: WebsiteUI/SiteNewsPosts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WebsiteUI/SiteNewsPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,UpperSection,LowerSection")] SiteNewsPost siteNewsPost)
        {
            if (ModelState.IsValid)
            {
                db.SiteNewsPosts.Add(siteNewsPost);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(siteNewsPost);
        }

        // GET: WebsiteUI/SiteNewsPosts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteNewsPost siteNewsPost = await db.SiteNewsPosts.FindAsync(id);
            if (siteNewsPost == null)
            {
                return HttpNotFound();
            }
            return View(siteNewsPost);
        }

        // POST: WebsiteUI/SiteNewsPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UpperSection,LowerSection")] SiteNewsPost siteNewsPost)
        {
            if (ModelState.IsValid)
            {
                db.Entry(siteNewsPost).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(siteNewsPost);
        }

        // GET: WebsiteUI/SiteNewsPosts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteNewsPost siteNewsPost = await db.SiteNewsPosts.FindAsync(id);
            if (siteNewsPost == null)
            {
                return HttpNotFound();
            }
            return View(siteNewsPost);
        }

        // POST: WebsiteUI/SiteNewsPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SiteNewsPost siteNewsPost = await db.SiteNewsPosts.FindAsync(id);
            db.SiteNewsPosts.Remove(siteNewsPost);
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
