using SchoolPortal.Web.Areas.Data.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using SchoolPortal.Web.Models.Entities;
using System.Threading.Tasks;
using SchoolPortal.Web.Models;
using System.Drawing;
using System.Web.Helpers;
using System.IO;
using System.Drawing.Imaging;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace SchoolPortal.Web.Areas.Data.Services
{
    public class ImageService : IImageService
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public ImageService()
        {

        }

        public ImageService(ApplicationUserManager userManager,
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


        //public HttpPostedFileBase ResizeBitmap(HttpPostedFileBase b, int nWidth, int nHeight)
        //{
        //    HttpPostedFileBase result = new HttpPostedFileBase(nWidth, nHeight);
        //    using (Graphics g = Graphics.FromImage((Image)result))
        //        g.DrawImage(b, 0, 0, nWidth, nHeight);
        //    return result;
        //}
        public async Task<int> Create(HttpPostedFileBase upload)
        {
            ImageModel model = new ImageModel();
            if (upload != null && upload.ContentLength > 0)
            {


                // Find its length and convert it to byte array
                int ContentLength = upload.ContentLength;

                // Create Byte Array
                byte[] bytImg = new byte[ContentLength];

                // Read Uploaded file in Byte Array
                upload.InputStream.Read(bytImg, 0, ContentLength);

                model.ImageContent = bytImg;
                model.ContentType = upload.ContentType;
                model.FileName = upload.FileName;

            }

            db.ImageModel.Add(model);
            await db.SaveChangesAsync();


            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Added an image";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }



            return model.Id;
        }

        public async Task Delete(int? id)
        {
            var img = await db.ImageModel.FirstOrDefaultAsync(x => x.Id == id);
            if (img != null)
            {
                db.ImageModel.Remove(img);
                await db.SaveChangesAsync();

                //Add Tracking
                var userId = HttpContext.Current.User.Identity.GetUserId();
                if (userId != null)
                {
                    var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId;
                    tracker.UserName = user.UserName;
                    tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "deleted an image";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }

            }


        }

        public async Task Edit(int id, HttpPostedFileBase upload)
        {
            var img = await db.ImageModel.FirstOrDefaultAsync(x => x.Id == id);
            if (img != null)
            {
                if (upload != null && upload.ContentLength > 0)
                {


                    // Find its length and convert it to byte array
                    int ContentLength = upload.ContentLength;

                    // Create Byte Array
                    byte[] bytImg = new byte[ContentLength];

                    // Read Uploaded file in Byte Array
                    upload.InputStream.Read(bytImg, 0, ContentLength);

                    img.ImageContent = bytImg;
                    img.ContentType = upload.ContentType;
                    img.FileName = upload.FileName;
                }
                db.Entry(img).State = EntityState.Modified;
                await db.SaveChangesAsync();


                //Add Tracking
                var userId = HttpContext.Current.User.Identity.GetUserId();
                if (userId != null)
                {
                    var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId;
                    tracker.UserName = user.UserName;
                    tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "deleted an image";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task<ImageModel> Get(int? id)
        {
            var img = await db.ImageModel.FirstOrDefaultAsync(x => x.Id == id);
            return img;
        }

        public async Task PostImageCreate(List<HttpPostedFileBase> upload, int PostId)
        {
            PostImage model = new PostImage();
            if (upload.Count() > 0)
            {
                foreach (var image in upload)
                {
                    // Find its length and convert it to byte array
                    int ContentLength = image.ContentLength;

                    // Create Byte Array
                    byte[] bytImg = new byte[ContentLength];

                    // Read Uploaded file in Byte Array
                    image.InputStream.Read(bytImg, 0, ContentLength);

                    model.ImageContent = bytImg;
                    string b4 = Convert.ToBase64String(model.ImageContent);
                    model.ImageByte = "data:image/jpg;base64," + b4;
                    model.ContentType = image.ContentType;
                    model.FileName = image.FileName;
                    model.PostId = PostId;

                    db.PostImages.Add(model);
                    await db.SaveChangesAsync();


                    //Add Tracking
                    var userId = HttpContext.Current.User.Identity.GetUserId();
                    if (userId != null)
                    {
                        var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                        Tracker tracker = new Tracker();
                        tracker.UserId = userId;
                        tracker.UserName = user.UserName;
                        tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                        tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                        tracker.Note = tracker.FullName + " " + "deleted an image";
                        //db.Trackers.Add(tracker);
                        await db.SaveChangesAsync();
                    }
                }



            }

        }

        public async Task PostImageDelete(int? PostId)
        {
            var img = await db.PostImages.Where(x => x.PostId == PostId).ToListAsync();
            if (img.Count() > 0)
            {
                foreach (var my in img)
                {
                    db.PostImages.Remove(my);

                }
                await db.SaveChangesAsync();


                //Add Tracking
                var userId = HttpContext.Current.User.Identity.GetUserId();
                if (userId != null)
                {
                    var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId;
                    tracker.UserName = user.UserName;
                    tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "deleted an image";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task<List<PostImage>> PostImageGet(int? PostId)
        {
            var img = await db.PostImages.Where(x => x.PostId == PostId).ToListAsync();
            return img;
        }
    }
}