using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace SchoolPortal.Web.Areas.Data.Services
{
    public class DeviceService : IDeviceService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public DeviceService()
        {

        }

        public DeviceService(ApplicationUserManager userManager,
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

        public async Task Create(ApprovedDevice model)
        {
            var uId = HttpContext.Current.User.Identity.GetUserId();
            model.UserId = uId;
            model.Date = DateTime.UtcNow.AddHours(1);
            db.ApprovedDevices.Add(model);
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
                tracker.Note = tracker.FullName + " " + "Added a approve device for logging in";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task Delete(int? id)
        {
            var device = await db.ApprovedDevices.FirstOrDefaultAsync(x => x.Id == id);
            if(device != null)
            {
                db.ApprovedDevices.Remove(device);
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
                    tracker.Note = tracker.FullName + " " + "deleted a approve device for logging in";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
            }
          
        }

        public async Task<List<ApprovedDevice>> DeviceList()
        {
            var device = await db.ApprovedDevices.Include(x=>x.User).ToListAsync();
            return device;
        }

        public async Task Edit(ApprovedDevice model)
        {
            db.Entry(model).State = EntityState.Modified;
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
                tracker.Note = tracker.FullName + " " + "edited an approve device for logging in";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
        }

        public async Task<ApprovedDevice> Get(int? id)
        {
            var device = await db.ApprovedDevices.Include(x=>x.User).FirstOrDefaultAsync(x => x.Id == id);
            return device;
        }
    }
}