using SchoolPortal.Web.Areas.Data.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;
using SchoolPortal.Web.Models.Entities;
using SchoolPortal.Web.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace SchoolPortal.Web.Areas.Data.Services
{
    public class AttendanceService : IAttendanceService
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public AttendanceService()
        {

        }

        public AttendanceService(ApplicationUserManager userManager,
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


        public async Task AddAllStudentsInClass(int id)
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var enrolledStudents = db.Enrollments.Include(x => x.User).Include(e => e.StudentProfile).Include(x => x.Session).Where(s => s.ClassLevelId == id && s.Session.Status == SessionStatus.Current);

            foreach (var it in enrolledStudents)
            {
                AttendanceDetail attendanceDetail = new AttendanceDetail();
                attendanceDetail.StudentId = it.StudentProfileId;
                attendanceDetail.UserId = it.StudentProfile.UserId;
                db.AttendanceDetails.Add(attendanceDetail);
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
                    tracker.Note = tracker.FullName + " " + "Added an attendance";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
            }

        }

        public async Task Create(Attendance model)
        {
            db.Attendances.Add(model);
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
                tracker.Note = tracker.FullName + " " + "Added an attendance";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
        }

        public async Task Edit(Attendance model)
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
                tracker.Note = tracker.FullName + " " + "Edited attendance";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
        }

        public async Task<List<Attendance>> ListAttendanceByClassBySession(int id)
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var attend = db.Attendances.Include(x => x.AttendanceDetails).Where(x => x.ClassLevelId == id && x.SessionId == currentSession.Id);
            return await attend.ToListAsync();

        }

        public async Task<List<AttendanceDetail>> ListAttendanceDetail(int id)
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var attend = db.AttendanceDetails.Include(x => x.StudentProfile).Include(c => c.User).Where(x => x.SessionId == currentSession.Id && x.AttendanceId == id);
            return await attend.ToListAsync();
        }

        public async Task<List<AttendanceDetail>> ListAttendanceDetailByStudent(int id)
        {
            var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
            var attend = db.AttendanceDetails.Include(x => x.StudentProfile).Include(c => c.User).Where(x => x.SessionId == currentSession.Id && x.StudentId == id);
            return await attend.ToListAsync();
        }

        public async Task UpdateAttendance(AttendanceDetail model)
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
                tracker.Note = tracker.FullName + " " + "Updated an attendance";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
        }
        //public async Task AttendanceAllStudent(Attendance model)
        //{
        //    db.Attendances.Add(model);
        //    await db.SaveChangesAsync();
        //}


        //public async Task Edit(Attendance models)
        //{
        //    db.Entry(models).State = EntityState.Modified;
        //    await db.SaveChangesAsync();
        //}


    }
}