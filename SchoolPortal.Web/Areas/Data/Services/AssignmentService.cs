using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Entities;
using System.Data;
using System.Threading.Tasks;
using SchoolPortal.Web.Areas.Data.IServices;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace SchoolPortal.Web.Areas.Data.Services
{
    public class AssignmentService : IAssignmentService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public AssignmentService()
        {

        }

        public AssignmentService(ApplicationUserManager userManager,
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

        public async Task Create(Assignment model)
        {
            db.Assignments.Add(model);
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
                tracker.Note = tracker.FullName + " " + "Added an assignment";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task CreateAnswer(AssignmentAnswer model)
        {
            db.AssignmentAnswers.Add(model);
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
                tracker.Note = tracker.FullName + " " + "Added an assignment answer";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
        }

        public async Task Delete(int? id)
        {
            var item = await db.Assignments.FirstOrDefaultAsync(x => x.Id == id);
            if (item != null)
            {
                db.Assignments.Remove(item);
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
                    tracker.Note = tracker.FullName + " " + "deleted an assignment";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteAnswer(int? id)
        {
            var item = await db.AssignmentAnswers.FirstOrDefaultAsync(x => x.Id == id);
            if (item != null)
            {
                db.AssignmentAnswers.Remove(item);
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
                    tracker.Note = tracker.FullName + " " + "deleted an assignment answer";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task Edit(Assignment model)
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
                tracker.Note = tracker.FullName + " " + "edited an assignment";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
        }

        public async Task EditAnswer(AssignmentAnswer model)
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
                tracker.Note = tracker.FullName + " " + "edited an assignment answer";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
        }

        public async Task<Assignment> Get(int? id)
        {
            var item = await db.Assignments.Include(x=>x.Session).Include(x=>x.Subject).Include(x=>x.ClassLevel).Include(x => x.AssignmentAnswers).FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task<AssignmentAnswer> GetAnswer(int? id, int studentId)
        {
            var item = await db.AssignmentAnswers.Include(x => x.Assignment).Include(x=>x.StudentProfile).Include(x=>x.Enrollement).FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task<List<Assignment>> List(int classId)
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var ass = db.Assignments.Include(x => x.AssignmentAnswers).Include(x => x.ClassLevel).Include(x => x.Session).Include(x => x.Subject).Where(x => x.ClassLevelId == classId && x.SessionId == currentSession.Id);
            return await ass.ToListAsync();
        }


        public async Task<List<Assignment>> ListAll()
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var ass = db.Assignments.Include(x => x.AssignmentAnswers).Include(x => x.ClassLevel).Include(x => x.Session).Include(x => x.Subject).Where(x => x.SessionId == currentSession.Id);
            return await ass.ToListAsync();
        }

        public async Task<List<AssignmentAnswer>> ListForStudent(int classId, int assignmentId)
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var ass1 = db.AssignmentAnswers.Include(x => x.Assignment).Include(x => x.User).Include(x => x.StudentProfile).Include(x => x.ClassLevel);
            var ass = ass1.Where(x => x.ClassId == classId && x.AssignmentId == assignmentId);
            return await ass.ToListAsync();
        }
    }
}