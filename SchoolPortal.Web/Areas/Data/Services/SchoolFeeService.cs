using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SchoolPortal.Web.Areas.Data.Services
{
    public class SchoolFeeService : ISchoolFeeService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public SchoolFeeService()
        {

        }

        public SchoolFeeService(ApplicationUserManager userManager,
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
        public async Task Create(SchoolFees model)
        {
            db.SchoolFees.Add(model);
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
                tracker.Note = tracker.FullName + " " + "Added school account";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
            
        }

        public async Task Delete(int? id)
        {
            var acct = await db.SchoolFees.FirstOrDefaultAsync(x => x.Id == id);
            if (acct != null)
            {
                db.SchoolFees.Remove(acct);
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
                    tracker.Note = tracker.FullName + " " + "Deleted school account";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
               
            }
        }

        public async Task Edit(SchoolFees models)
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
                tracker.Note = tracker.FullName + " " + "Edited school account";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
          
        }

        public async Task<SchoolFees> Get(int? id)
        {
            var fee = await db.SchoolFees.FirstOrDefaultAsync(x => x.Id == id);
            return fee;
        }

        public async Task<List<SchoolFees>> List()
        {
            var fee = await db.SchoolFees.Include(x=>x.Session).OrderBy(x => x.Category).ToListAsync();
            return fee;
        }
    }
}