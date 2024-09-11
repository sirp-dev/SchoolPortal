using SchoolPortal.Web.Areas.Data.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SchoolPortal.Web.Models.Entities;
using System.Threading.Tasks;

using SchoolPortal.Web.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace SchoolPortal.Web.Areas.Data.Services
{
    public class HallOfFameService : IHallOfFameService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public HallOfFameService()
        {

        }

        public HallOfFameService(ApplicationUserManager userManager,
           ApplicationRoleManager roleManager, FinanceService financeService)
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

        public async Task Create(HallOfFame model, HttpPostedFileBase upload)
        {
            
            if (upload != null && upload.ContentLength > 0)
            {


                // Find its length and convert it to byte array
                int ContentLength = upload.ContentLength;

                // Create Byte Array
                byte[] bytImg = new byte[ContentLength];

                // Read Uploaded file in Byte Array
                upload.InputStream.Read(bytImg, 0, ContentLength);

                model.Image = bytImg;
                

            }
            model.DateCreated = DateTime.UtcNow.AddHours(1);
            db.HallOfFames.Add(model);
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
                tracker.Note = tracker.FullName + " " + "Added Hall Of Fame";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           

        }

        public async Task Delete(int? id)
        {
            var fame = await db.HallOfFames.FirstOrDefaultAsync(x => x.Id == id);
            if (fame != null)
            {
                db.HallOfFames.Remove(fame);
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
                    tracker.Note = tracker.FullName + " " + "Deleted Hall Of Fame";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
               
            }

        }

        public async Task Edit(HallOfFame models, HttpPostedFileBase upload)
        {

                if (upload != null && upload.ContentLength > 0)
                {


                    // Find its length and convert it to byte array
                    int ContentLength = upload.ContentLength;

                    // Create Byte Array
                    byte[] bytImg = new byte[ContentLength];

                    // Read Uploaded file in Byte Array
                    upload.InputStream.Read(bytImg, 0, ContentLength);

                    models.Image = bytImg;
                   
                }
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
                tracker.Note = tracker.FullName + " " + "Edited Hall Of Fame";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           

        }

        public async Task<HallOfFame> Get(int? id)
        {
            var fame = await db.HallOfFames.FirstOrDefaultAsync(x => x.Id == id);
            return fame;
        }

        public async Task<List<HallOfFame>> List()
        {
            var fame = await db.HallOfFames.OrderBy(x=>x.SortOrder).ToListAsync();
            return fame;
        }
    }
}