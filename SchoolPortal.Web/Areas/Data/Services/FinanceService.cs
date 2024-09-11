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
using System.Data.Entity.Core.Objects;

namespace SchoolPortal.Web.Areas.Data.Services
{
    public class FinanceService : IFinanceService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public FinanceService()
        {

        }

        public FinanceService(ApplicationUserManager userManager,
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
        //private IFinanceService _financeService = new FinanceService();

        //public FinanceService()
        //{

        //}

        //public FinanceService(FinanceService financeService)
        //{
        //    _financeService = financeService;

        //}
        public async Task AddTransaction(FinanceInitializer model)
        {
            try
            {
                var check = db.Finances.FirstOrDefault(x => x.UniqueIdCheck == model.UniqueId && x.ReferenceId == model.ReferenceId);
                if (check == null)
                {
                    Finance item = new Finance();
                    item.SessionId = model.SessionId;
                    item.InvoiceNumber = model.InvoiceNumber;
                    item.RegistrationNumber = model.Enrollment.StudentProfile.StudentRegNumber;
                    item.EnrollmentId = model.EnrollmentId;
                    item.Amount = model.Amount;
                    item.Date = DateTime.UtcNow.AddHours(1);
                    item.UserId = model.Enrollment.StudentProfile.UserId;
                    item.Title = model.Income.Title;
                    item.IncomeId = model.Income.Id;
                    item.PaymentTypeId = model.PaymentAmountId;

                    if (model.Payall == true)
                    {
                        item.Balance = 0;

                    }
                    else
                    {
                        item.Balance = model.Income.Amount - model.Amount;

                    }


                    item.UniqueIdCheck = model.UniqueId;
                    item.ReferenceId = model.ReferenceId;
                    item.FinanceSource = model.FinanceSource;
                    item.FinanceType = model.FinanceType;
                    item.Payall = model.Payall;
                    if (item.Payall == true)
                    {
                        item.TransactionStatus = TransactionStatus.CompletePart;
                    }
                    else
                    {
                        if (item.Balance == 0)
                        {
                            item.TransactionStatus = TransactionStatus.Paid;
                        }
                        else
                        {
                            item.TransactionStatus = TransactionStatus.Part;
                        }
                    }
                    db.Finances.Add(item);

                    var xfinance = await db.FinanceInitializers.FirstOrDefaultAsync(x => x.Id == model.Id);
                    if (xfinance.Payall == true)
                    {
                        xfinance.TransactionStatus = TransactionStatus.CompletePart;
                    }
                    else
                    {
                        if (xfinance.Balance == 0)
                        {
                            xfinance.TransactionStatus = TransactionStatus.Paid;
                        }
                        else
                        {
                            xfinance.TransactionStatus = TransactionStatus.Part;
                        }
                    }
                    db.Entry(xfinance).State = EntityState.Modified;
                    //
                    //var enrolmnt = await db.Enrollments.FirstOrDefaultAsync(x => x.Id == item.EnrollmentId);
                    //enrolmnt.AmountPaid += model.Amount;
                    //db.Entry(enrolmnt).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    if (item.Payall == true)
                    {
                        var prevpay = await db.Finances.FirstOrDefaultAsync(x => x.IncomeId == item.IncomeId && x.EnrollmentId == item.EnrollmentId && x.Payall != true);
                        prevpay.Skip = true;
                        db.Entry(prevpay).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }

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
                        tracker.Note = tracker.FullName + " " + "Created Finance Payment";
                        //db.Trackers.Add(tracker);
                        await db.SaveChangesAsync();
                    }
                }
            }catch(Exception d)
            {

            }

        }

        public async Task Create(Finance item, string PaymentType)
        {
            //string[] names = item.Title.Split(' ');
            var uId = HttpContext.Current.User.Identity.GetUserId();
            var std = db.StudentProfiles.Include(x => x.user).FirstOrDefault(x => x.UserId == uId);
            var enrol = db.Enrollments.Include(x => x.ClassLevel).Include(x => x.Session).Include(x => x.StudentProfile).Include(x => x.User).FirstOrDefault(x => x.StudentProfileId == std.Id);
            //var income1 = db.Incomes.FirstOrDefault(x => x.Title == names[0].ToString());
            var income = db.Incomes.FirstOrDefault(x => x.Title == PaymentType);
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            item.SessionId = currentSession.Id;
            item.InvoiceNumber = item.InvoiceNumber;
            item.RegistrationNumber = enrol.StudentProfile.StudentRegNumber;
            item.EnrollmentId = enrol.Id;
            item.Amount = income.Amount;
            item.Date = DateTime.UtcNow.AddHours(1);
            item.UserId = std.UserId;
            item.Title = income.Title;
            item.PaymentTypeId = income.Id;
            item.TransactionStatus = TransactionStatus.Pending;
            db.Finances.Add(item);
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
                tracker.Note = tracker.FullName + " " + "Created Finance Payment";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }
        public async Task<IQueryable<Finance>> PaymentList()
        {
            IQueryable<Finance> finance = from s in db.Finances
                                             .Include(x => x.Session).Include(x => x.Enrollment).Include(x => x.ApprovedBy).Include(x => x.Enrollment.ClassLevel).Include(x => x.Enrollment.StudentProfile.user).Include(x => x.User)
                                             .Where(x=>x.FinanceType == FinanceType.Credit)
                .OrderByDescending(x => x.Date)
                                                          select s;

            return finance;
        }
        public async Task<List<Finance>> AllPaymentList()
        {

            var finance = await db.Finances.Include(x => x.Session).Include(x => x.Enrollment).Include(x=>x.Enrollment.StudentProfile.user).Include(x=>x.User).OrderByDescending(x=>x.Date).ToListAsync();
            return finance;
        }

        public async Task ApprovePay(Finance item,int? id)
        {
            
            var uId = HttpContext.Current.User.Identity.GetUserId();
            var approve = db.Finances.FirstOrDefault(x => x.Id == id);
            approve.ApprovedById = uId;
            approve.TransactionStatus = TransactionStatus.Paid;
            db.Entry(approve).State = EntityState.Modified;
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
                tracker.Note = tracker.FullName + " " + "Approved Payment";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task BankPay(Finance item, int? id)
        {
            var set = db.Settings.FirstOrDefault();
            var setname = set.SchoolInitials;

            
            var invoice = await db.Finances.FirstOrDefaultAsync(x => x.Id == id);
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            invoice.SessionId = currentSession.Id;
            invoice.UserId = invoice.UserId;
            invoice.FinanceType = FinanceType.Credit;
            invoice.TransactionStatus = TransactionStatus.Pending;
            invoice.Date = DateTime.UtcNow.AddHours(1);
            invoice.FinanceSource = FinanceSource.Online;
            invoice.ReferenceId = item.ReferenceId +"BANK" + "-" + setname;
            invoice.EnrollmentId = invoice.EnrollmentId;
            invoice.Title = invoice.Title;
            invoice.RegistrationNumber = invoice.RegistrationNumber;
            invoice.Amount = invoice.Amount;
            db.Entry(invoice).State = EntityState.Modified;
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
                tracker.Note = tracker.FullName + " " + "Accepted bank payment receipt";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task<List<Finance>> BankSourceList()
        {
            var finance = await db.Finances.Include(x => x.Session).Include(x => x.Enrollment).Include(x => x.User).Where(x=>x.FinanceSource == FinanceSource.Online).OrderByDescending(x=>x.Date).ToListAsync();
            return finance;
        }

        public async Task<List<Finance>> BankSourceCurrentSessionList()
        {
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var finance = await db.Finances.Include(x => x.Session).Include(x => x.Enrollment).Include(x => x.User).Where(x => x.FinanceSource == FinanceSource.Online && x.SessionId ==currentSession.Id).OrderByDescending(x => x.Date).ToListAsync();
            return finance;
        }

        public async Task CashPay(Finance item, int? id)
        {
            var uId = HttpContext.Current.User.Identity.GetUserId();
            var set = db.Settings.FirstOrDefault();
            var setname = set.SchoolInitials;

            var invoice = await db.Finances.FirstOrDefaultAsync(x=>x.Id == id);
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            invoice.SessionId = currentSession.Id;
            invoice.UserId = invoice.UserId;
            invoice.FinanceType = FinanceType.Credit;
            invoice.FinanceSource = FinanceSource.Cash;
            invoice.TransactionStatus = TransactionStatus.Paid;
            invoice.Date = DateTime.UtcNow.AddHours(1);
            invoice.Title = invoice.Title;
            invoice.Amount = invoice.Amount;
            invoice.Description = invoice.Description;
            invoice.ReferenceId = DateTime.UtcNow.Date.Year.ToString() +
            DateTime.UtcNow.Date.Month.ToString() +
            DateTime.UtcNow.Date.Day.ToString() + Guid.NewGuid().ToString().Substring(0, 4).ToUpper() + "CASH" + "-" + setname;
            invoice.AdminNote = invoice.AdminNote;
            invoice.EnrollmentId = invoice.EnrollmentId;
            invoice.ApprovedById = uId;
            invoice.RegistrationNumber = invoice.RegistrationNumber;
            invoice.ReferenceId = invoice.ReferenceId;
            db.Entry(invoice).State = EntityState.Modified;
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
                tracker.Note = tracker.FullName + " " + "Accepted cash payment";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
            

        }


        public async Task DebitPay(Finance item)
        {
            var uId = HttpContext.Current.User.Identity.GetUserId();
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            item.SessionId = currentSession.Id;
            item.UserId = uId;
            item.FinanceType = FinanceType.Debit;
            item.TransactionStatus = TransactionStatus.Pending;
            item.Date = DateTime.UtcNow.AddHours(1);
            db.Finances.Add(item);
            await db.SaveChangesAsync();

            //add invoice number
            var invoicen = await db.Finances.FindAsync(item.Id);
            invoicen.InvoiceNumber = item.Id.ToString("000000");
            db.Entry(invoicen).State = EntityState.Modified;
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
                tracker.Note = tracker.FullName + " " + "Carried out debit payment for "+item.Title;
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
            
        }

        public async Task<List<Finance>> CashSourceList()
        {
            var finance = await db.Finances.Include(x => x.Session).Include(x => x.Enrollment).Include(x => x.User).Where(x => x.FinanceSource == FinanceSource.Cash).OrderByDescending(x => x.Date).ToListAsync();
            return finance;
        }
        public async Task<List<Finance>> CashSourceCurrentSessionList()
        {
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var finance = await db.Finances.Include(x => x.Session).Include(x => x.Enrollment).Include(x => x.User).Where(x => x.FinanceSource == FinanceSource.Cash && x.SessionId == currentSession.Id).OrderByDescending(x => x.Date).ToListAsync();
            return finance;
        }

        public async Task<List<Finance>> CreditList()
        {
            var finance = await db.Finances.Include(x => x.Session).Include(x => x.Enrollment).Include(x => x.User).Where(x => x.FinanceType == FinanceType.Credit).OrderByDescending(x => x.Date).ToListAsync();
            return finance;
        }

        public async Task<List<Finance>> CreditCurrentSessionList()
        {
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var finance = await db.Finances.Include(x => x.Session).Include(x => x.Enrollment).Include(x => x.User).Where(x => x.FinanceType == FinanceType.Credit && x.SessionId == currentSession.Id).OrderByDescending(x => x.Date).ToListAsync();
            return finance;
        }


        //public async Task<List<Finance>> CreditCurrentSessionList()
        //{
        //    var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
        //    var finance = await db.Finances.Include(x => x.Session).Include(x => x.Enrollment).Include(x => x.User).Where(x => x.FinanceType == FinanceType.Credit && x.SessionId == currentSession.Id).OrderByDescending(x => x.Date).ToListAsync();
        //    return finance;
        //}
        public async Task<List<Finance>> CurrentSessionPaymentList()
        {
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var finance = await db.Finances.Include(x => x.Session).Include(x => x.Enrollment).Include(x => x.User).Where(x=>x.SessionId == currentSession.Id).OrderByDescending(x => x.Date).ToListAsync();
            return finance;
        }

        public async Task<List<Finance>> DebitList()
        {
            var finance = await db.Finances.Include(x => x.Session).Include(x => x.Enrollment).Include(x => x.User).Where(x => x.FinanceType == FinanceType.Debit).OrderByDescending(x => x.Date).ToListAsync();
            return finance;
        }

        public async Task<List<Finance>> DebitCurrentSessionList()
        {
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var finance = await db.Finances.Include(x => x.Session).Include(x => x.Enrollment).Include(x => x.User).Where(x => x.FinanceType == FinanceType.Debit && x.SessionId == currentSession.Id).OrderByDescending(x => x.Date).ToListAsync();
            return finance;
        }

        public async Task Delete(int? id)
        {
            var item = await db.Finances.FirstOrDefaultAsync(x => x.Id == id);
            db.Finances.Remove(item);
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
                tracker.Note = tracker.FullName + " " + "deleted finance";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task Edit(Finance item)
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
                tracker.Note = tracker.FullName + " " + "edited finance";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task<Finance> Get(int? id)
        {
            var item = await db.Finances.Include(x=>x.Enrollment).Include(x => x.Session).Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }
        public async Task<List<Finance>> GetUserPaymentBySession(int sessionid)
        {
            var uId = HttpContext.Current.User.Identity.GetUserId();
            var item = await db.Finances.Include(x => x.Session).Include(x => x.Enrollment).Include(x => x.User).Where(x => x.UserId == uId && x.SessionId == sessionid).OrderByDescending(x => x.Date).ToListAsync();
            return item;
        }
        public async Task<List<Finance>> GetUserPayment()
        {
            var uId = HttpContext.Current.User.Identity.GetUserId();
            var item = await db.Finances.Include(x => x.Session).Include(x => x.Enrollment).Include(x => x.User).Where(x => x.UserId == uId).OrderByDescending(x => x.Date).ToListAsync();
            return item;
        }

        public async Task OnlinePay(Finance item, int? id)
        {
            var set = db.Settings.FirstOrDefault();
            var setname = set.SchoolInitials;
            var invoice = await db.Finances.FirstOrDefaultAsync(x=>x.Id == id);
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            invoice.SessionId = currentSession.Id;
            invoice.UserId = invoice.UserId;
            invoice.FinanceType = FinanceType.Credit;
            invoice.TransactionStatus = TransactionStatus.Paid;
            invoice.Date = DateTime.UtcNow.AddHours(1);
            invoice.FinanceSource = FinanceSource.Online;
            invoice.EnrollmentId = invoice.EnrollmentId;
            invoice.ApprovedById = invoice.UserId;
            invoice.RegistrationNumber = invoice.RegistrationNumber;
            invoice.Title = invoice.Title;
            invoice.Amount = invoice.Amount;
            item.ApprovedById = "Online";
            invoice.ReferenceId = invoice.ReferenceId +"ONLINE" + "-" + setname;
            db.Entry(invoice).State = EntityState.Modified;
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
                tracker.Note = tracker.FullName + " " + "Made an online payment";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
          

        }

        public async Task<List<Finance>> OnlineSourceList()
        {
            var finance = await db.Finances.Include(x => x.Session).Include(x=>x.Enrollment).Include(x => x.User).Where(x => x.FinanceSource == FinanceSource.Online).OrderByDescending(x => x.Date).ToListAsync();
            return finance;
        }

        public async Task<List<Finance>> OnlineSourceCurrentSessionList()
        {
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var finance = await db.Finances.Include(x => x.Session).Include(x => x.Enrollment).Include(x => x.User).Where(x => x.FinanceSource == FinanceSource.Online && x.SessionId == currentSession.Id).OrderByDescending(x => x.Date).ToListAsync();
            return finance;
        }

        public async Task<List<Finance>> AccomodationList()
        {
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var finance = await db.Finances.Include(x => x.Session).Include(x => x.Enrollment).Include(x => x.User)
                .Where(x => x.FinanceSource == FinanceSource.Online && x.SessionId == currentSession.Id || x.Title =="Accomodation" || x.Title=="Hostel")
                .OrderByDescending(x => x.Date).ToListAsync();
            return finance;
        }

        public async Task<List<Finance>> PaidAccomodationList()
        {
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            var finance = await db.Finances.Include(x => x.Session).Include(x => x.Enrollment).Include(x => x.User)
                .Where(x => x.FinanceSource == FinanceSource.Online && x.TransactionStatus == TransactionStatus.Paid 
                && x.SessionId == currentSession.Id && x.AllocationStatus != AllocationStatus.Allocated && x.Title == "Accomodation" || x.Title == "Hostel")
                .OrderByDescending(x => x.Date).ToListAsync();
            return finance;
        }
        public int CountString(string searchString)
        {
            int result = 0;

            searchString = searchString.Trim();

            if (searchString == "")
                return 0;

            while (searchString.Contains("  "))
                searchString = searchString.Replace("  ", " ");

            foreach (string y in searchString.Split(' '))

                result++;


            return result;
        }

        public async Task<IQueryable<Finance>> PaymentListAll(string searchString, string currentFilter, int? page, DateTime? DateOne, DateTime? DateTwo, string Duration)
        {
            IQueryable<Finance> finance = from s in db.Finances
                                                        .Include(x => x.Session).Include(x => x.Enrollment).Include(x => x.ApprovedBy).Include(x => x.Enrollment.ClassLevel).Include(x => x.Enrollment.StudentProfile.user).Include(x => x.User)
                                                        .Where(x => x.FinanceType == FinanceType.Credit)


                           .OrderByDescending(x => x.Date)
                                          select s;
            //if(Duration != "All")
            //{
            //    finance = finance.Where(x => DbFunctions.TruncateTime(x.Date.Date) <= DbFunctions.TruncateTime(DateOne.Value))
            //                .Where(x => DbFunctions.TruncateTime(x.Date.Date) >= DbFunctions.TruncateTime(DateTwo.Value)).AsQueryable();
            //    //finance = finance.Where(x => EntityFunctions.TruncateTime(x.Date.Date >= DateOne)
            //    //.Where(x => x.Date.Date <= DateTwo);
            //    //_db.CallLogs.Where(r => DbFunctions.TruncateTime(r.DateTime) == callDateTime.Date).ToLi
            //}




            if (!String.IsNullOrEmpty(searchString))
            {
                if (CountString(searchString) > 1)
                {
                    string[] searchStringCollection = searchString.Split(' ');

                    foreach (var item in searchStringCollection)
                    {
                        finance = finance.Where(s => s.Enrollment.StudentProfile.user.Surname.ToUpper().Contains(item.ToUpper()) || s.Enrollment.StudentProfile.user.FirstName.ToUpper().Contains(item.ToUpper())
                                                               || s.Enrollment.StudentProfile.user.OtherName.ToUpper().Contains(item.ToUpper())
                                                               || s.Amount.ToString().Contains(item.ToUpper())
                                                               || s.Enrollment.ClassLevel.ClassName.ToString().Contains(item.ToUpper())
                                                               || s.ApprovedBy.UserName.ToString().Contains(item.ToUpper())
                                                               || s.Title.ToString().Contains(item.ToUpper())
                                                               || s.InvoiceNumber.ToString().Contains(item.ToUpper())

                                                               );
                    }
                }
                else
                {
                    finance = finance.Where(s => s.Enrollment.StudentProfile.user.Surname.ToUpper().Contains(searchString.ToUpper()) || s.Enrollment.StudentProfile.user.FirstName.ToUpper().Contains(searchString.ToUpper())
                                                               || s.Enrollment.StudentProfile.user.OtherName.ToUpper().Contains(searchString.ToUpper())
                                                               || s.Amount.ToString().Contains(searchString.ToUpper())
                                                               || s.Enrollment.ClassLevel.ClassName.ToString().Contains(searchString.ToUpper())
                                                               || s.ApprovedBy.UserName.ToString().Contains(searchString.ToUpper())
                                                               || s.Title.ToString().Contains(searchString.ToUpper())
                                                               || s.InvoiceNumber.ToString().Contains(searchString.ToUpper())

                                                               );
                }

            }

            return finance;
        }

        public async Task<IQueryable<Finance>> PaymentListPage(string searchString, string currentFilter, int? page, DateTime? DateOne, DateTime? DateTwo, string Duration)
        {
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            IQueryable<Finance> finance = from s in db.Finances
                                                        .Include(x => x.Session).Include(x => x.Enrollment).Include(x => x.ApprovedBy).Include(x => x.Enrollment.ClassLevel).Include(x => x.Enrollment.StudentProfile.user).Include(x => x.User)
                                                        .Where(x => x.FinanceType == FinanceType.Credit)
                                                        .Where(x=>x.SessionId == session.Id)

                           .OrderByDescending(x => x.Date)
                                          select s;
            //if(Duration != "All")
            //{
            //    finance = finance.Where(x => DbFunctions.TruncateTime(x.Date.Date) <= DbFunctions.TruncateTime(DateOne.Value))
            //                .Where(x => DbFunctions.TruncateTime(x.Date.Date) >= DbFunctions.TruncateTime(DateTwo.Value)).AsQueryable();
            //    //finance = finance.Where(x => EntityFunctions.TruncateTime(x.Date.Date >= DateOne)
            //    //.Where(x => x.Date.Date <= DateTwo);
            //    //_db.CallLogs.Where(r => DbFunctions.TruncateTime(r.DateTime) == callDateTime.Date).ToLi
            //}




            if (!String.IsNullOrEmpty(searchString))
            {
                if (CountString(searchString) > 1)
                {
                    string[] searchStringCollection = searchString.Split(' ');

                    foreach (var item in searchStringCollection)
                    {
                        finance = finance.Where(s => s.Enrollment.StudentProfile.user.Surname.ToUpper().Contains(item.ToUpper()) || s.Enrollment.StudentProfile.user.FirstName.ToUpper().Contains(item.ToUpper())
                                                               || s.Enrollment.StudentProfile.user.OtherName.ToUpper().Contains(item.ToUpper())
                                                               || s.Amount.ToString().Contains(item.ToUpper())
                                                               || s.Enrollment.ClassLevel.ClassName.ToString().Contains(item.ToUpper())
                                                               || s.ApprovedBy.UserName.ToString().Contains(item.ToUpper())
                                                               || s.Title.ToString().Contains(item.ToUpper())
                                                               || s.InvoiceNumber.ToString().Contains(item.ToUpper())

                                                               );
                    }
                }
                else
                {
                    finance = finance.Where(s => s.Enrollment.StudentProfile.user.Surname.ToUpper().Contains(searchString.ToUpper()) || s.Enrollment.StudentProfile.user.FirstName.ToUpper().Contains(searchString.ToUpper())
                                                               || s.Enrollment.StudentProfile.user.OtherName.ToUpper().Contains(searchString.ToUpper())
                                                               || s.Amount.ToString().Contains(searchString.ToUpper())
                                                               || s.Enrollment.ClassLevel.ClassName.ToString().Contains(searchString.ToUpper())
                                                               || s.ApprovedBy.UserName.ToString().Contains(searchString.ToUpper())
                                                               || s.Title.ToString().Contains(searchString.ToUpper())
                                                               || s.InvoiceNumber.ToString().Contains(searchString.ToUpper())

                                                               );
                }

            }

            return finance;
        }


        public async Task<IQueryable<Finance>> PaymentListPageSession(int sessionId, string searchString, string currentFilter, int? page, DateTime? DateOne, DateTime? DateTwo, string Duration)
        {
            IQueryable<Finance> finance = from s in db.Finances
                                                        .Include(x => x.Session).Include(x => x.Enrollment).Include(x => x.ApprovedBy).Include(x => x.Enrollment.ClassLevel).Include(x => x.Enrollment.StudentProfile.user).Include(x => x.User)
                                                        .Where(x => x.FinanceType == FinanceType.Credit)
                                                       .Where(x=>x.SessionId == sessionId)

                           .OrderByDescending(x => x.Date)
                                          select s;
            //if(Duration != "All")
            //{
            //    finance = finance.Where(x => DbFunctions.TruncateTime(x.Date.Date) <= DbFunctions.TruncateTime(DateOne.Value))
            //                .Where(x => DbFunctions.TruncateTime(x.Date.Date) >= DbFunctions.TruncateTime(DateTwo.Value)).AsQueryable();
            //    //finance = finance.Where(x => EntityFunctions.TruncateTime(x.Date.Date >= DateOne)
            //    //.Where(x => x.Date.Date <= DateTwo);
            //    //_db.CallLogs.Where(r => DbFunctions.TruncateTime(r.DateTime) == callDateTime.Date).ToLi
            //}




            if (!String.IsNullOrEmpty(searchString))
            {
                if (CountString(searchString) > 1)
                {
                    string[] searchStringCollection = searchString.Split(' ');

                    foreach (var item in searchStringCollection)
                    {
                        finance = finance.Where(s => s.Enrollment.StudentProfile.user.Surname.ToUpper().Contains(item.ToUpper()) || s.Enrollment.StudentProfile.user.FirstName.ToUpper().Contains(item.ToUpper())
                                                               || s.Enrollment.StudentProfile.user.OtherName.ToUpper().Contains(item.ToUpper()) 
                                                               || s.Amount.ToString().Contains(item.ToUpper())
                                                               || s.Enrollment.ClassLevel.ClassName.ToString().Contains(item.ToUpper())
                                                               || s.ApprovedBy.UserName.ToString().Contains(item.ToUpper())
                                                               || s.Title.ToString().Contains(item.ToUpper())
                                                               || s.InvoiceNumber.ToString().Contains(item.ToUpper())

                                                               );
                    }
                }
                else
                {
                    finance = finance.Where(s => s.Enrollment.StudentProfile.user.Surname.ToUpper().Contains(searchString.ToUpper()) || s.Enrollment.StudentProfile.user.FirstName.ToUpper().Contains(searchString.ToUpper())
                                                               || s.Enrollment.StudentProfile.user.OtherName.ToUpper().Contains(searchString.ToUpper())
                                                               || s.Amount.ToString().Contains(searchString.ToUpper())
                                                               || s.Enrollment.ClassLevel.ClassName.ToString().Contains(searchString.ToUpper())
                                                               || s.ApprovedBy.UserName.ToString().Contains(searchString.ToUpper())
                                                               || s.Title.ToString().Contains(searchString.ToUpper())
                                                               || s.InvoiceNumber.ToString().Contains(searchString.ToUpper())

                                                               );
                }

            }

            return finance;
        }

        public async Task<IQueryable<Finance>> DebitCurrentSessionListPage(string searchString, string currentFilter, int? page, DateTime? DateOne, DateTime? DateTwo, string Duration)
        {
            IQueryable<Finance> finance = from s in db.Finances
                                                                   .Include(x => x.Session).Include(x => x.Enrollment).Include(x => x.ApprovedBy).Include(x => x.Enrollment.ClassLevel).Include(x => x.Enrollment.StudentProfile.user).Include(x => x.User)
                                                                   .Where(x => x.FinanceType == FinanceType.Debit)


                                      .OrderByDescending(x => x.Date)
                                          select s;
          
            if (!String.IsNullOrEmpty(searchString))
            {
                if (CountString(searchString) > 1)
                {
                    string[] searchStringCollection = searchString.Split(' ');

                    foreach (var item in searchStringCollection)
                    {
                        finance = finance.Where(s => s.Amount.ToString().Contains(item.ToUpper())
                                                               || s.User.UserName.ToString().Contains(item.ToUpper())
                                                               || s.ApprovedBy.UserName.ToString().Contains(item.ToUpper())
                                                               || s.Title.ToString().Contains(item.ToUpper())
                                                               || s.InvoiceNumber.ToString().Contains(item.ToUpper())

                                                               );
                    }
                }
                else
                {
                    finance = finance.Where(s => s.Amount.ToString().Contains(searchString.ToUpper())
                                                               || s.User.UserName.ToString().Contains(searchString.ToUpper())
                                                               || s.ApprovedBy.UserName.ToString().Contains(searchString.ToUpper())
                                                               || s.Title.ToString().Contains(searchString.ToUpper())
                                                               || s.InvoiceNumber.ToString().Contains(searchString.ToUpper())

                                                               );
                }

            }

            return finance;
        }
    }
}