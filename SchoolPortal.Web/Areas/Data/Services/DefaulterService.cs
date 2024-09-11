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
    public class DefaulterService : IDefaulterService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public DefaulterService()
        {

        }

        public DefaulterService(ApplicationUserManager userManager,
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


        public async Task<string> Create(Defaulter model, int id = 0)
        {
            var user = await db.StudentProfiles.Include(u => u.user).FirstOrDefaultAsync(x => x.Id == id);
            model.ProfileId = id;
            db.Defaulters.Add(model);
            await db.SaveChangesAsync();
            var username = user.user.Surname + " " + user.user.FirstName + " " + user.user.OtherName;

            //Add Tracking
            var userId2 = HttpContext.Current.User.Identity.GetUserId();
            if (userId2 != null)
            {
                var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Added defaulter";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }

            return username;


        }

        public async Task Delete(int? id)
        {
            var item = await db.Defaulters.FirstOrDefaultAsync(x => x.Id == id);
            if (item != null)
            {
                db.Defaulters.Remove(item);
                await db.SaveChangesAsync();

                //Add Tracking
                var userId2 = HttpContext.Current.User.Identity.GetUserId();
                if (userId2 != null)
                {
                    var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Deleted defaulter";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
            }
        }

        public Task Edit(Defaulter models)
        {
            throw new NotImplementedException();
        }

        public async Task<Defaulter> Get(int? id)
        {
            var item = await db.Defaulters.Include(x => x.StudentProfile).FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task<Defaulter> GetDefaulter(int? id)
        {
            var item = db.Defaulters.Include(x => x.StudentProfile);
            //var output = item.Select(x => new DefaulterDto
            //{
            //  Id = x.Id,
            //  Fullname = x.StudentProfile.user.Surname + " " + x.StudentProfile.user.FirstName + " "+ x.StudentProfile.user.OtherName,
            //  ProfileId = x.ProfileId
            //});
            return await item.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Defaulter>> List()
        {
            var item = await db.Defaulters.ToListAsync();
            return item;
        }

        public async Task<StudentProfile> RemoveDefaulter(int? id)
        {

            Defaulter model = db.Defaulters.Find(id);

            var user = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.Id == model.ProfileId);
            // var classLevel = await db.Enrollments.Include(x => x.Session).Include(x => x.StudentProfile).FirstOrDefaultAsync(x => x.StudentProfileId == user.Id && x.Session.Status == SessionStatus.Current);
            db.Defaulters.Remove(model);
            await db.SaveChangesAsync();


            //Add Tracking
            var userId2 = HttpContext.Current.User.Identity.GetUserId();
            if (userId2 != null)
            {
                var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Removed defaulter";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
            //  string fullname = user.user.Surname + " " + user.user.FirstName + " " + user.user.OtherName;
            return user;

        }
    }
}