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
    public class EnrolledSubjectService : IEnrolledSubjectService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public EnrolledSubjectService()
        {

        }

        public EnrolledSubjectService(ApplicationUserManager userManager,
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


        public async Task Create(EnrolledSubject model)
        {
            db.EnrolledSubjects.Add(model);
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
                tracker.Note = tracker.FullName + " " + "Added Enrolled Subject";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
            
        }

        public async Task Delete(int? id)
        {
            var item = await db.EnrolledSubjects.FirstOrDefaultAsync(x => x.Id == id);
            if (item != null)
            {
                db.EnrolledSubjects.Remove(item);
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
                    tracker.Note = tracker.FullName + " " + "Deleted Enrolled Subject";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
                
            }
        }

        //public async Task Edit(EnrolledSubject models)
        //{
            
        //    db.Entry(models).State = EntityState.Modified;
        //    await db.SaveChangesAsync();
        //}

        public async Task Edit(EnrolledSubject models)
        {
            try
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
                    tracker.Note = tracker.FullName + " " + "Edited Enrolled Subject";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
                
            }
            catch (Exception e)
            {
                try
                {
                    var DFD = await db.EnrolledSubjects.FindAsync(models.Id);
                    DFD.ExamScore = models.ExamScore;
                    DFD.TestScore = models.TestScore;
                    DFD.TotalScore = models.TotalScore;
                    DFD.GradingOption = models.GradingOption;
                    DFD.IsOffered = models.IsOffered;
                    db.Entry(DFD).State = EntityState.Modified;
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
                        tracker.Note = tracker.FullName + " " + "Edited Enrolled Subject";
                        //db.Trackers.Add(tracker);
                        await db.SaveChangesAsync();
                    }
                    
                }
                catch (Exception es)
                {

                }


            }

        }
        public async Task EditScore(int id)
        {
            var models = await db.EnrolledSubjects.FirstOrDefaultAsync(x => x.Id == id);

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
                tracker.Note = tracker.FullName + " " + "Edited Enrolled Subject Score";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task<EnrolledSubject> Get(int? id)
        {
            var item = await db.EnrolledSubjects.Include(x=>x.Enrollments).Include(x => x.Subject).FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task<List<EnrolledSubject>> List()
        {
            var item = await db.EnrolledSubjects.ToListAsync();
            return item;
        }
    }
}