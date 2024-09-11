using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace SchoolPortal.Web.Areas.Data.Services
{
    public class PaymentAmountService : IPaymentAmountService
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public PaymentAmountService()
        {

        }

        public PaymentAmountService(ApplicationUserManager userManager,
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

        //public async Task CreateForAllClass(PaymentAmount item, bool All)
        //{
        //    //update class

        //    IQueryable<ClassLevel> iclass = from s in db.ClassLevels
        //                                             .Where(x => x.ShowClass == true)
        //                                       select s;
        //    foreach (var i in iclass.ToList())
        //    {
        //        var check = await db.PaymentAmounts.AsNoTracking().FirstOrDefaultAsync(x => x.ClassLevelId == item.ClassLevelId && x.IncomeId == item.IncomeId);
        //        if (check == null)
        //        {
        //            PaymentAmount py = new PaymentAmount();
        //            py.ClassLevelId = i.Id;
        //            py.IncomeId = item.IncomeId;
        //            py.Amount = item.Amount;
        //            db.PaymentAmounts.Add(py);
        //        }

        //    }

        //    await db.SaveChangesAsync();


        //    //Add Tracking
        //    var userId = HttpContext.Current.User.Identity.GetUserId();
        //    if (userId != null)
        //    {
        //        var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
        //        Tracker tracker = new Tracker();
        //        tracker.UserId = userId;
        //        tracker.UserName = user.UserName;
        //        tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
        //        tracker.ActionDate = DateTime.UtcNow.AddHours(1);
        //        tracker.Note = tracker.FullName + " " + "Added payment amount";
        //        //db.Trackers.Add(tracker);
        //        await db.SaveChangesAsync();
        //    }

        //}

        //public async Task Create(PaymentAmount item)
        //{
        //    var check = await db.PaymentAmounts.FirstOrDefaultAsync(x => x.ClassLevelId == item.ClassLevelId && x.IncomeId == item.IncomeId);
        //    if (check == null)
        //    {
        //        db.PaymentAmounts.Add(item);
        //        await db.SaveChangesAsync();

        //        //Add Tracking
        //        var userId = HttpContext.Current.User.Identity.GetUserId();
        //        if (userId != null)
        //        {
        //            var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
        //            Tracker tracker = new Tracker();
        //            tracker.UserId = userId;
        //            tracker.UserName = user.UserName;
        //            tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
        //            tracker.ActionDate = DateTime.UtcNow.AddHours(1);
        //            tracker.Note = tracker.FullName + " " + "Added payment amount";
        //            //db.Trackers.Add(tracker);
        //            await db.SaveChangesAsync();
        //        }
        //    }
        //}

        //public async Task Delete(int? id)
        //{
        //    var item = await db.PaymentAmounts.FirstOrDefaultAsync(x => x.Id == id);
        //    db.PaymentAmounts.Remove(item);
        //    await db.SaveChangesAsync();

        //    //Add Tracking
        //    var userId = HttpContext.Current.User.Identity.GetUserId();
        //    if (userId != null)
        //    {
        //        var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
        //        Tracker tracker = new Tracker();
        //        tracker.UserId = userId;
        //        tracker.UserName = user.UserName;
        //        tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
        //        tracker.ActionDate = DateTime.UtcNow.AddHours(1);
        //        tracker.Note = tracker.FullName + " " + "deleted payment amount";
        //        //db.Trackers.Add(tracker);
        //        await db.SaveChangesAsync();
        //    }

        //}

        //public async Task Edit(PaymentAmount item)
        //{
        //    db.Entry(item).State = EntityState.Modified;
        //    await db.SaveChangesAsync();

        //    //Add Tracking
        //    var userId = HttpContext.Current.User.Identity.GetUserId();
        //    if (userId != null)
        //    {
        //        var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
        //        Tracker tracker = new Tracker();
        //        tracker.UserId = userId;
        //        tracker.UserName = user.UserName;
        //        tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
        //        tracker.ActionDate = DateTime.UtcNow.AddHours(1);
        //        tracker.Note = tracker.FullName + " " + "edited payment amount";
        //        //db.Trackers.Add(tracker);
        //        await db.SaveChangesAsync();
        //    }

        //}

        //public Task<PaymentAmount> Get(int? id)
        //{
        //    var pay = db.PaymentAmounts.Include(x => x.ClassLevel).Include(x => x.Income).FirstOrDefaultAsync(x => x.Id == id);
        //    return pay;
        //}

        //public async Task<List<PaymentAmount>> PaymentAmountList()
        //{
        //    var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
        //    var uId = HttpContext.Current.User.Identity.GetUserId();
        //    var enrol = db.Enrollments.Include(x => x.ClassLevel).Include(x => x.StudentProfile).Include(x => x.User).FirstOrDefault(x => x.StudentProfile.UserId == uId && x.SessionId == currentSession.Id);
        //    var pay = await db.PaymentAmounts.Include(x => x.ClassLevel).Include(x => x.Income).Where(x => x.ClassLevelId == enrol.ClassLevelId).ToListAsync();
        //    return pay;
        //}

        //public async Task<List<PaymentAmount>> AmountList()
        //{
        //    var pay = await db.PaymentAmounts.Include(x => x.ClassLevel).Include(x => x.Income).ToListAsync();
        //    return pay;
        //}
        //public async Task<List<PaymentAmount>> StudentAmountListBySession(int sessionId)
        //{
        //    int sid = 0;
        //    if (sessionId > 0)
        //    {
        //        var Session = await db.Sessions.FirstOrDefaultAsync(x => x.Id == sessionId);
        //        sid = Session.Id;
        //    }
        //    else
        //    {
        //        var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
        //        sid = currentSession.Id;
        //    }
        //    var uId = HttpContext.Current.User.Identity.GetUserId();
        //    var std = db.StudentProfiles.Include(x => x.user).FirstOrDefault(x => x.UserId == uId);
        //    var enrol = db.Enrollments.Include(x => x.ClassLevel).Include(x => x.StudentProfile).Include(x => x.User).FirstOrDefault(x => x.StudentProfileId == std.Id && x.SessionId == sid);
        //    var pay = await db.PaymentAmounts.Include(x => x.ClassLevel).Include(x => x.Income).Where(x => x.ClassLevelId == enrol.ClassLevelId).ToListAsync();
        //    return pay;
        //}

        //public async Task<List<PaymentAmount>> StudentAmountList()
        //{
        //    var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
        //    var uId = HttpContext.Current.User.Identity.GetUserId();
        //    var std = db.StudentProfiles.Include(x => x.user).FirstOrDefault(x => x.UserId == uId);
        //    var enrol = db.Enrollments.Include(x => x.ClassLevel).Include(x => x.StudentProfile).Include(x => x.User).FirstOrDefault(x => x.StudentProfileId == std.Id && x.SessionId == currentSession.Id);
        //    var pay = await db.PaymentAmounts.Include(x => x.ClassLevel).Include(x => x.Income).Where(x => x.ClassLevelId == enrol.ClassLevelId).ToListAsync();
        //    return pay;
        //}

        public async Task<List<Income>> IncomeList()
        {

            var income = await db.Incomes.ToListAsync();
            return income;
        }

        public async Task<IQueryable<Finance>> ListFinanceByType(int PaymentTypeid, int id)
        {
            var income1 = db.Incomes.FirstOrDefault(x => x.Id == PaymentTypeid);

            IQueryable<Finance> Ilist = from s in db.Finances.Include(x => x.User)
                                            .Where(x => x.PaymentTypeId == income1.Id)
                                            .Where(x => x.SessionId == id)
                                        select s;
            return Ilist;
        }
    }
}