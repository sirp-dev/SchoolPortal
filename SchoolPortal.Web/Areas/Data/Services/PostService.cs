using SchoolPortal.Web.Areas.Data.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SchoolPortal.Web.Models.Entities;
using System.Threading.Tasks;
using SchoolPortal.Web.Models;
using System.Data.Entity;
using SchoolPortal.Web.Models.Dtos;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace SchoolPortal.Web.Areas.Data.Services
{
    public class PostService : IPostService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public PostService()
        {

        }

        public PostService(ApplicationUserManager userManager,
           ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public async Task<Post> AdminDetails(int? id)
        {
            var item = await db.Posts.Include(x=>x.PostImages).Include(x => x.Comments).FirstOrDefaultAsync(x => x.Id == id);
         
            return item;
        }
        public byte[] ImageByte(int id)
        {
            var b = db.ImageModel.FirstOrDefault(p => p.Id == id);
            return b.ImageContent;
        }

        public async Task<int> Create(Post model)
        {
          

            db.Posts.Add(model);
            await db.SaveChangesAsync();

            string link = "";
            var site = await db.Settings.FirstOrDefaultAsync();
            if (site != null)
            {
                link = site.WebsiteLink;


            }
            var getpage = await db.Posts.FirstOrDefaultAsync(x => x.Id == model.Id);
            getpage.Link = link + "/UI/Post/" + getpage.Id + "?title=" + getpage.Title.Replace(" ", "-");
            db.Entry(getpage).State = EntityState.Modified;

            await db.SaveChangesAsync();
            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if(userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Added a post";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           

            return model.Id;
        }

        public async Task Delete(int? id)
        {
            var item = await db.Posts.FirstOrDefaultAsync(x => x.Id == id);
            if (item != null)
            {
                db.Posts.Remove(item);
                await db.SaveChangesAsync();

                //Add Tracking
                var userId = HttpContext.Current.User.Identity.GetUserId();
                if(userId != null)
                {
                    var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId;
                    tracker.UserName = user.UserName;
                    tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "deleted a post";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
               
            }
        }

        public async Task<Post> Details(int? id)
        {
            var item = await db.Posts.Include(x=>x.PostImages).Include(x=>x.Comments).FirstOrDefaultAsync(x => x.Id == id);


            return item;
        }

        public async Task Edit(Post models)
        {
            db.Entry(models).State = EntityState.Modified;
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if(userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Edited a post";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task<Post> Get(int? id)
        {
            var item = await db.Posts.Include(c=>c.PostImages).FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

    

        public async Task<List<Post>> List(string searchString, string currentFilter, int? page)
        {
            var items = db.Posts.Include(x=>x.Comments).Where(x=>x.Title != "");
            if (!String.IsNullOrEmpty(searchString))
            {
                
                items = items.Where(s => s.Content.ToUpper().Contains(searchString.ToUpper()) || s.Title.ToUpper().Contains(searchString.ToUpper()));

            }
            return await items.ToListAsync();
        }

        public async Task<List<Post>> StaffPost(string searchString, string currentFilter, int? page)
        {
            var items = db.Posts.Where(x=>x.WhoCanSeePost == WhoSeePost.Staff || x.WhoCanSeePost == WhoSeePost.All && x.Status == PostStatus.Published);
            if (!String.IsNullOrEmpty(searchString))
            {

                //  items = items.Where(s => s.Content.ToUpper().Contains(searchString.ToUpper()) || s.Title.ToUpper().Contains(searchString.ToUpper()));
                items = items.Where(s => s.Content.ToUpper().Contains(searchString.ToUpper()) || s.Title.ToUpper().Contains(searchString.ToUpper()));

            }
            return await items.ToListAsync();
        }

        public async Task<List<Post>> StudentPost(string searchString, string currentFilter, int? page)
        {
            var items = db.Posts.Where(x => x.WhoCanSeePost == WhoSeePost.Student || x.WhoCanSeePost == WhoSeePost.All && x.Status == PostStatus.Published);
            if (!String.IsNullOrEmpty(searchString))
            {

                //  items = items.Where(s => s.Content.ToUpper().Contains(searchString.ToUpper()) || s.Title.ToUpper().Contains(searchString.ToUpper()));
                items = items.Where(s => s.Content.ToUpper().Contains(searchString.ToUpper()) || s.Title.ToUpper().Contains(searchString.ToUpper()));

            }
            return await items.ToListAsync();
        }

        public async Task<List<Post>> ListPost(int count)
        {
            var post = await db.Posts.Include(x=>x.Comments).Where(x=>x.PageType == PageType.Article || x.PageType == PageType.News).OrderByDescending(x => x.DatePosted).Take(count).ToListAsync();
            return post;
        }

        public async Task CreateComment(Comment model)
        {
            db.Comments.Add(model);
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if(userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Added a comment to a post";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
          
        }

        public async Task<Comment> GetComment(int? id)
        {
            var comment = await db.Comments.Include(x => x.Post).FirstOrDefaultAsync(x => x.Id == id);
            return comment;
        }

        public async Task DeleteComment(int? id)
        {
            var item = await db.Comments.FirstOrDefaultAsync(x => x.Id == id);
            if (item != null)
            {
                db.Comments.Remove(item);
                await db.SaveChangesAsync();

                //Add Tracking
                var userId = HttpContext.Current.User.Identity.GetUserId();
                if(userId != null)
                {
                    var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId;
                    tracker.UserName = user.UserName;
                    tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Deleted a post comment";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
               
            }
        }
    }
}