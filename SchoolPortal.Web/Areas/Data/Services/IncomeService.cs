using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using SchoolPortal.Web.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity.Owin;

namespace SchoolPortal.Web.Areas.Data.Services
{
    public class IncomeService : IIncomeService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public IncomeService()
        {

        }

        public IncomeService(ApplicationUserManager userManager,
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

        public async Task Create(Income item)
        {
            var uId = HttpContext.Current.User.Identity.GetUserId();
            item.Date = DateTime.UtcNow.AddHours(1);
            item.UserId = uId;
            db.Incomes.Add(item);
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
                tracker.Note = tracker.FullName + " " + "Added an income source";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task Delete(int? id)
        {
            var item = await db.Incomes.FirstOrDefaultAsync(x => x.Id == id);
            db.Incomes.Remove(item);
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
                tracker.Note = tracker.FullName + " " + "Deleted an income source";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task Edit(Income item)
        {
            db.Entry(item).State = EntityState.Modified;
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
                tracker.Note = tracker.FullName + " " + "Edited an income source";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
          
        }

        public async Task<Income> Get(int? id)
        {
            var item = await db.Incomes.FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task<List<Income>> GetUserIncome(string userId)
        {
            var item = await db.Incomes.Where(x => x.UserId == userId).ToListAsync();
            return item;
        }

        public async Task<List<Income>> IncomeList()
        {
            var item = await db.Incomes.ToListAsync();
            return item;
        }

        //public async Task<List<Income>> PaymentAmountIncomeList()
        //{
        //    var uId = HttpContext.Current.User.Identity.GetUserId();
        //    var enrol = db.Enrollments.Include(x => x.ClassLevel).Include(x => x.StudentProfile).Include(x => x.User).FirstOrDefault(x => x.StudentProfile.UserId == uId);
        //    var pay = await db.PaymentAmounts.Include(x => x.ClassLevel).Include(x => x.Income).Where(x => x.ClassLevelId == enrol.ClassLevelId).ToListAsync();
        //    foreach(var incom in pay)
        //    {
        //        var item = db.Incomes.Where(x=>x.Id == incom.Id).ToList();
        //        return item;
        //    }
        //    return pay;
        //}
    }
}