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
using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using PagedList;

namespace SchoolPortal.Web.Areas.Content.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IPostService _postService = new PostService();
        private IImageService _imageService = new ImageService();


        public PostsController()
        {

        }
        public PostsController(
            PostService postService,
            ImageService imageService
            )
        {
            _postService = postService;
            _imageService = imageService;
        }
        // GET: Content/Posts
        public async Task<ActionResult> Index(string searchString, string currentFilter, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var items = await _postService.List(searchString, currentFilter, page);

            int pageSize = 50;
            int pageNumber = (page ?? 1);
            ViewBag.Total = items.Count();
            return View(items.ToPagedList(pageNumber, pageSize));
        }

        // GET: Content/Posts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var post = await _postService.AdminDetails(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        public async Task<ActionResult> PostDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var post = await _postService.Details(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // GET: Content/Posts/Create
        public ActionResult Create()
        {
           
            return View();
        }

        // POST: Content/Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Post model, List<HttpPostedFileBase> upload)
        {
            if (ModelState.IsValid)
            {
                model.DatePosted = DateTime.UtcNow.AddHours(1);
                model.PostedBy = User.Identity.Name;
                
               var id = await _postService.Create(model);
                var checkimg = upload.Count(a => a != null && a.ContentLength > 0);
                if(checkimg > 0)
                {
 await _imageService.PostImageCreate(upload, id);
                }
               
                return RedirectToAction("Index");
            }
            
            return View(model);
        }

        // GET: Content/Posts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var post = await _postService.AdminDetails(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            
            return View(post);
        }

        // POST: Content/Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Post model, List<HttpPostedFileBase> upload)
        {
            if (ModelState.IsValid)
            {
                model.PostedBy = User.Identity.Name;
                var checkimg = upload.Count(a => a != null && a.ContentLength > 0);
                if (checkimg > 0 )
                {
                    await _imageService.PostImageDelete(model.Id);
                    await _imageService.PostImageCreate(upload, model.Id);
                }
                await _postService.Edit(model);
                //return Redirect(ReturnUrl);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Content/Posts/Delete/5
        public async Task<ActionResult> Delete(int? id, string ReturnUrl)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           var post = await _postService.AdminDetails(id);

            if (post == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReturnUrl = ReturnUrl;
            return View(post);
        }

        // POST: Content/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id, string ReturnUrl)
        {
            await _imageService.PostImageDelete(id);
            await _postService.Delete(id);
            return RedirectToAction("Index");
        }


        public async Task<ActionResult> DeleteComment(int? id, string ReturnUrl)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var comment = await _postService.GetComment(id);

            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReturnUrl = ReturnUrl;
            return View(comment);
        }

       
        [HttpPost, ActionName("DeleteComment")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCommentConfirmed(int id, string ReturnUrl)
        {
           
            await _postService.DeleteComment(id);
            return Redirect(ReturnUrl);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateComment(Comment model, int id, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
               
                model.Username = User.Identity.Name;
                model.DateCommented = DateTime.UtcNow.AddHours(1);
                model.PostId = id;
                await _postService.CreateComment(model);
               
                return Redirect(ReturnUrl);
            }

            return View(model);
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
