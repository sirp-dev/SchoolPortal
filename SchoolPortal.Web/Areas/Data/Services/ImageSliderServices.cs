using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SchoolPortal.Web.Areas.Data.IServices;
using System.Threading.Tasks;
using SchoolPortal.Web.Models.Entities;
using System.IO;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using SchoolPortal.Web.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace SchoolPortal.Web.Areas.Data.Services
{
    public class ImageSliderServices:IimageSliderServices
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ImageSliderServices()
        {

        }

        public ImageSliderServices(ApplicationUserManager userManager,
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

        public async Task New(ImageSlider models, HttpPostedFileBase upload)
        {
            if (upload != null && upload.ContentLength > 0)
            {


                // Find its length and convert it to byte array
                int ContentLength = upload.ContentLength;

                // Create Byte Array
                byte[] bytImg = new byte[ContentLength];

                // Read Uploaded file in Byte Array
                upload.InputStream.Read(bytImg, 0, ContentLength);
                
                models.Content = bytImg;
                models.ContentType = upload.ContentType;
                models.FileName = upload.FileName;

            }

            db.ImageSlider.Add(models);
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
                tracker.Note = tracker.FullName + " " + "Added an image slider";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task Delete(int? id)
        {
            ImageSlider models = await db.ImageSlider.FindAsync(id);
           
            db.ImageSlider.Remove(models);
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
                tracker.Note = tracker.FullName + " " + "Deleted an image slider";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task<List<ImageSlider>> List()
        {
            var slider = db.ImageSlider;
            return await slider.ToListAsync();
        }


        public async Task AddToSlider(ImageSlider models)
        {
            if (models.CurrentSlider == false)
            {
                models.CurrentSlider = true;
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
                    tracker.Note = tracker.FullName + " " + "Added slider to current";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
              
            }
            else
            {
                models.CurrentSlider = false;
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
                    tracker.Note = tracker.FullName + " " + "Removed slider from current";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
               
            }

        }
    }
}