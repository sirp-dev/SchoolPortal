using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Areas.Financial.Controllers
{
    [Authorize(Roles = "SuperAdmin,Finance")]

    public class FinanceAccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Financial/Account
        public async Task<ActionResult> Complete(int sessionId = 0)
        {
            if (sessionId == 0)
            {
                var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
                sessionId = currentSession.Id;
            }


            IQueryable<Finance> Payment = from s in db.Finances
                                             .OrderByDescending(x => x.Date)
                                             .Where(x => x.SessionId == sessionId)
                                             .Where(x => x.TransactionStatus == Models.Entities.TransactionStatus.Paid)
                                          select s;

            return View(Payment);
        }

        public async Task<ActionResult> Part(int sessionId = 0)
        {
            if (sessionId == 0)
            {
                var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
                sessionId = currentSession.Id;
            }


            IQueryable<Finance> Payment = from s in db.Finances
                                             .OrderByDescending(x => x.Date)
                                             .Where(x => x.SessionId == sessionId)
                                             .Where(x => x.TransactionStatus == Models.Entities.TransactionStatus.Part)
                                          select s;

            return View(Payment);
        }

        public async Task<ActionResult> NotPaid(int sessionId = 0)
        {
            if (sessionId == 0)
            {
                var currentSession = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
                sessionId = currentSession.Id;
            }

            IQueryable<Finance> Payment = from s in db.Finances
                                             .OrderByDescending(x => x.Date)
                                             .Where(x => x.SessionId == sessionId)
                                             .Where(x => x.TransactionStatus == Models.Entities.TransactionStatus.Part)
                                          select s;

            return View(Payment);
        }
    }
}