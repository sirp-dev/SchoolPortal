using Microsoft.AspNet.Identity;
using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;

namespace SchoolPortal.Web.Areas.Data.Services
{
    public class InvoiceService : IInvoiceService
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public async Task Create(Invoice item)
        {
            var uId = HttpContext.Current.User.Identity.GetUserId();
            var std = db.StudentProfiles.Include(x => x.user).FirstOrDefault(x => x.UserId == uId);
            var enrol = db.Enrollments.Include(x => x.ClassLevel).Include(x => x.Session).Include(x => x.StudentProfile).Include(x => x.User).FirstOrDefault(x => x.StudentProfileId == std.Id);
            var income1 = db.Incomes.FirstOrDefault(x => x.Title == item.Title);
            var income = db.PaymentAmounts.Include(x => x.ClassLevel).Include(x => x.Income).FirstOrDefault(x => x.IncomeId == income1.Id && x.ClassLevelId == enrol.ClassLevelId);
            var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            item.SessionId = currentSession.Id;
            item.InvoiceNumber = item.InvoiceNumber;
            item.RegistrationNumber = enrol.StudentProfile.StudentRegNumber;
            item.EnrollmentId = enrol.Id;
            item.Amount = income.Amount;
            item.ClassName = enrol.ClassLevel.ClassName;
            item.CreatedDate = DateTime.UtcNow.AddHours(1);
            item.UserId = uId;
            item.Title = income.Income.Title;
            item.Status = PaymentStatus.UnPaid;
            db.Invoices.Add(item);
            await db.SaveChangesAsync();
        }

        public async Task Delete(int? id)
        {
            var item = await db.Invoices.FirstOrDefaultAsync(x => x.Id == id);
            db.Invoices.Remove(item);
            await db.SaveChangesAsync();
        }

        public async Task Edit(Invoice item)
        {
            db.Entry(item).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }

        public async Task<Invoice> Get(int? id)
        {
            var item = await db.Invoices.Include(x => x.Session).Include(x => x.Enrollment).Include(x=>x.User).FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task<List<Invoice>> GetUserInvoice(string userId)
        {
            var item = await db.Invoices.Include(x => x.Session).Include(x => x.Enrollment).Include(x=>x.User).Where(x => x.UserId == userId && x.Status == PaymentStatus.UnPaid).OrderByDescending(x=>x.CreatedDate).ToListAsync();
            return item;
        }

        public async Task<List<Invoice>> GetUserInvoiceWithOutId()
        {
            var uId = HttpContext.Current.User.Identity.GetUserId();
            var item = await db.Invoices.Include(x => x.Session).Include(x => x.Enrollment).Include(x=>x.User).Where(x => x.UserId == uId && x.Status == PaymentStatus.UnPaid).OrderByDescending(x => x.CreatedDate).ToListAsync();
            return item;
        }      

        public async Task<List<Invoice>> InvoiceList()
        {
            var item = await db.Invoices.Include(x => x.Session).Include(x => x.Enrollment).Include(x=>x.User).OrderByDescending(x => x.CreatedDate).ToListAsync();
            return item;
        }

        public async Task<List<Invoice>> PaidInvoiceList()
        {
            var item = await db.Invoices.Include(x => x.Session).Include(x => x.Enrollment).Include(x=>x.User).OrderByDescending(x => x.CreatedDate).Where(x=>x.Status == PaymentStatus.Paid).ToListAsync();
            return item;
        }

        public async Task<List<Invoice>> UnPaidInvoiceList()
        {
            var item = await db.Invoices.Include(x => x.Session).Include(x => x.Enrollment).Include(x=>x.User).OrderByDescending(x => x.CreatedDate).Where(x => x.Status == PaymentStatus.UnPaid).ToListAsync();
            return item;
        }
    }
}