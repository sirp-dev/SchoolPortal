using SchoolPortal.Web.Areas.Data.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SchoolPortal.Web.Models.Entities;
using System.Data.Entity;
using System.Threading.Tasks;
using SchoolPortal.Web.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace SchoolPortal.Web.Areas.Data.Services
{
    public class ContactUsService : IContactUsService
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ContactUsService()
        {

        }

        public ContactUsService(ApplicationUserManager userManager,
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
        public async Task Create(ContactUs model)
        {
            model.messageStatus = MessageStatus.NotReplied;
            model.DateCreated = DateTime.UtcNow.AddHours(1);
            db.ContactUs.Add(model);
            await db.SaveChangesAsync();

            //Add Tracking
            //var userId = HttpContext.Current.User.Identity.GetUserId();
            //var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
            //Tracker tracker = new Tracker();
            //tracker.UserId = userId;
            //tracker.UserName = user.UserName;
            //tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
            //tracker.ActionDate = DateTime.UtcNow.AddHours(1);
            //tracker.Note = tracker.FullName + " " + "Contact Us";
            ////db.Trackers.Add(tracker);
            //await db.SaveChangesAsync();
        }

        public async Task Reply(MessageReply model, int id)
        {
            model.MessageId = id;
            model.ReplyDate = DateTime.UtcNow.AddHours(1);
            db.MessageReply.Add(model);
            await db.SaveChangesAsync();

            //Add Tracking
            //var userId = HttpContext.Current.User.Identity.GetUserId();
            //var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
            //Tracker tracker = new Tracker();
            //tracker.UserId = userId;
            //tracker.UserName = user.UserName;
            //tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
            //tracker.ActionDate = DateTime.UtcNow.AddHours(1);
            //tracker.Note = tracker.FullName + " " + "Replied to a message";
            ////db.Trackers.Add(tracker);
            //await db.SaveChangesAsync();
        }

        public Task Delete(int? id)
        {
            throw new NotImplementedException();
        }

        public async Task<ContactUs> Get(int? id)
        {
            var item = await db.ContactUs.Include(x => x.MessageReply).FirstOrDefaultAsync(c => c.Id == id);
            return item;
        }

        public async Task<List<ContactUs>> List()
        {
            var items = await db.ContactUs.Include(d => d.MessageReply).ToListAsync();
            return items;
        }
    }
}