using SchoolPortal.Web.Areas.Data.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using SchoolPortal.Web.Models.Dtos;
using SchoolPortal.Web.Models.Entities;
using System.Threading.Tasks;
using SchoolPortal.Web.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace SchoolPortal.Web.Areas.Data.Services
{

    public class SessionService : ISessionService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public SessionService()
        {

        }

        public SessionService(ApplicationUserManager userManager,
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



        public async Task Create(Session model)
        {
            var existingSession = await db.Sessions.FirstOrDefaultAsync(x => x.SessionYear == model.SessionYear);
            var cSession = db.Sessions.Count();
            if (existingSession == null)
            {
                string[] terms = new string[] { "First", "Second", "Third" };
                for (int i = 0; i <= terms.Length - 1; i++)
                {
                    model.Term = terms[i];
                    if (cSession == 0)
                    {
                        if (model.Term == "First")
                        {
                            model.Status = SessionStatus.Current;
                        }
                        else
                        {
                            model.Status = SessionStatus.NotUsed;
                        }
                    }
                    else
                    {
                        model.Status = SessionStatus.NotUsed;
                    }


                    db.Sessions.Add(model);
                    await db.SaveChangesAsync();
                }

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
                    tracker.Note = tracker.FullName + " " + "Added a session";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
              
            }
            //else
            //{
            //    return null;
            //}

        }

        public async Task Delete(int? id)
        {
            var term = await db.Sessions.FirstOrDefaultAsync(x => x.Id == id);

            var sessionNames = db.Sessions.Where(x => x.SessionYear == term.SessionYear);

            foreach (var item in sessionNames)
            {
                db.Sessions.Remove(item);

            }
            db.SaveChanges();

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
                tracker.Note = tracker.FullName + " " + "Deleted a session";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task Edit(Session models)
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
                tracker.Note = tracker.FullName + " " + "Edited a session";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task<Session> Get(int? id)
        {
            var item = db.Sessions.FirstOrDefaultAsync(x => x.Id == id);
            return await item;
        }

        public async Task<List<SchoolSessionDto>> GetAllSession()
        {
            var item = db.Sessions.OrderByDescending(x => x.SessionYear).ThenBy(x => x.Term);
            var output = item.Select(x => new SchoolSessionDto
            {
                FullSession = x.SessionYear + " - " + x.Term + " Term",
                Id = x.Id,
                SessionStatus = x.Status,
                Year = x.SessionYear
            });
            return await output.ToListAsync();
        }

        public async Task<int> GetCurrentSessionId()
        {
            var session = db.Sessions.OrderByDescending(x => x.Id);
            var currentSession = await session.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            return currentSession.Id;
        }


        public async Task<string> GetCurrentSession()
        {
            var session = db.Sessions.OrderByDescending(x => x.Id);
            var currentSession = await session.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            return currentSession.SessionYear;
        }

        public async Task<string> GetCurrentSessionTerm()
        {
            var session = db.Sessions.OrderByDescending(x => x.Id);
            var currentSession = await session.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            return currentSession.Term;
        }

        public async Task<SchoolSessionDto> GetInfo(int id)
        {
            var item = db.Sessions.Where(x => x.Id == id);
            var output = item.Select(x => new SchoolSessionDto
            {
                FullSession = x.SessionYear + " - " + x.Term + " Term",
                Id = x.Id
            });
            return await output.FirstOrDefaultAsync();
        }

        public async Task<List<Session>> List()
        {
            var list = db.Sessions.OrderBy(x => x.Id);
            return await list.ToListAsync();
        }

        public async Task<bool> NextSession()
        {
            try
            {
                var session = db.Sessions.OrderByDescending(x => x.Id);
                if (session != null)
                {

                    var currentSession = session.Where(x => x.Status == SessionStatus.Current).Single();
                    var next = db.Sessions.Where(x => x.Id > currentSession.Id).Take(1);
                    //var nxt = db.Sessions
                    //var nxt = (from x in session where x.Id < currentSession.Id orderby x.Id descending select x).FirstOrDefault();

                    var nextsession = await db.Sessions.FirstOrDefaultAsync(x => x.Id == next.FirstOrDefault().Id);
                    nextsession.Status = SessionStatus.Current;

                    db.Entry(nextsession).State = EntityState.Modified;

                    var oldsession = await db.Sessions.FirstOrDefaultAsync(x => x.Id == currentSession.Id);
                    oldsession.Status = SessionStatus.Used;
                    db.Entry(oldsession).State = EntityState.Modified;
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
                        tracker.Note = tracker.FullName + " " + "Moved a session to the next";
                        //db.Trackers.Add(tracker);
                        await db.SaveChangesAsync();
                    }
                    

                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }


        public async Task<bool> PreviousSession()
        {
            try
            {


                var session = db.Sessions.OrderByDescending(x => x.Id);
                if (session != null)
                {
                    var currentSession = session.Where(x => x.Status == SessionStatus.Current).Single();
                    var prev = db.Sessions.Where(x => x.Id < currentSession.Id).OrderByDescending(x => x.Id).Take(1);
                    //var nxt = db.Sessions
                    //var nxt = (from x in session where x.Id < currentSession.Id orderby x.Id descending select x).FirstOrDefault();

                    var prevsession = await db.Sessions.FirstOrDefaultAsync(x => x.Id == prev.FirstOrDefault().Id);
                    prevsession.Status = SessionStatus.Current;


                    db.Entry(prevsession).State = EntityState.Modified;

                    var oldsession = await db.Sessions.FirstOrDefaultAsync(x => x.Id == currentSession.Id);
                    oldsession.Status = SessionStatus.Used;
                    db.Entry(oldsession).State = EntityState.Modified;
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
                        tracker.Note = tracker.FullName + " " + "Moved a session to the previous";
                        //db.Trackers.Add(tracker);
                        await db.SaveChangesAsync();
                    }
                 

                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}