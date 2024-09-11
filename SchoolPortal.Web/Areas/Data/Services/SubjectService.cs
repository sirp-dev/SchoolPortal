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
    public class SubjectService : ISubjectService
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public SubjectService()
        {

        }

        public SubjectService(ApplicationUserManager userManager,
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

        public async Task Create(Subject model, int id)
        {
            model.ClassLevelId = id;
            db.Subjects.Add(model);
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
                tracker.Note = tracker.FullName + " " + "Added a subject";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
            
        }

        public async Task Delete(int? id)
        {
            var item = await db.Subjects.FirstOrDefaultAsync(x => x.Id == id);
            var enrolsub = db.EnrolledSubjects.Where(x=>x.SubjectId == item.Id);
            foreach(var sub in enrolsub)
            {
                db.EnrolledSubjects.Remove(sub);
               // await db.SaveChangesAsync();
            }
            db.Subjects.Remove(item);
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
                tracker.Note = tracker.FullName + " " + "Deleted a subject";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
          
        }

        public async Task Edit(Subject models)
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
                tracker.Note = tracker.FullName + " " + "Edited a subject";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task EnrolledSubject(EnrolledSubject model)
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

        public async Task<Subject> Get(int? id)
        {
            var item = await db.Subjects.Include(x=>x.ClassLevel).FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task<IQueryable<SubjectListDto>> List(int? id)
        {

            IQueryable<Subject> item = from s in db.Subjects
                                        .Include(x => x.User).Include(x => x.ClassLevel).Where(x => x.ClassLevelId == id && x.ShowSubject == true && x.ClassLevel.ShowClass == true)
                                                      select s;
           // var item = db.Subjects.Include(x=>x.User).Where(x=>x.ClassLevelId == id);
            var output = item.Select(x => new SubjectListDto
            {
                SubjectName = x.SubjectName,
                ClassLevelId = id,
                FormTeacher = x.User.Surname + " " + x.User.FirstName + " " + x.User.OtherName,
                SubjectId = x.Id,
                UserId = x.UserId,
                ShowSubject = x.ShowSubject
            });
            return output.OrderBy(x=>x.SubjectName).AsQueryable();
        }

        public async Task<IQueryable<SubjectListDto>> AllList(int? id)
        {

            IQueryable<Subject> item = from s in db.Subjects
                                        .Include(x => x.User).Where(x => x.ClassLevelId == id)
                                       select s;
            // var item = db.Subjects.Include(x=>x.User).Where(x=>x.ClassLevelId == id);
            var output = item.Select(x => new SubjectListDto
            {
                SubjectName = x.SubjectName,
                ClassLevelId = id,
                FormTeacher = x.User.Surname + " " + x.User.FirstName + " " + x.User.OtherName,
                SubjectId = x.Id,
                UserId = x.UserId,
                ShowSubject = x.ShowSubject
            });
            return output.OrderBy(x => x.SubjectName).AsQueryable();
        }
    }
}