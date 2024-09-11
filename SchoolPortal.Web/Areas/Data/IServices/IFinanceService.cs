using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IFinanceService
    {

        Task AddTransaction(FinanceInitializer model);
        Task Create(Finance item, string PaymentType);
        Task ApprovePay(Finance item,int? id);
        Task CashPay(Finance item, int? id);
        Task DebitPay(Finance item);
        Task OnlinePay(Finance item, int? id);
        Task BankPay(Finance item, int? id);
        Task<Finance> Get(int? id);

        Task Delete(int? id);
        Task<IQueryable<Finance>> PaymentList();
        Task<List<Finance>> AllPaymentList();
        Task<IQueryable<Finance>> PaymentListPage(string searchString, string currentFilter, int? page, DateTime? DateOne, DateTime? DateTwo, string Duration);
        Task<IQueryable<Finance>> PaymentListPageSession(int sessionId, string searchString, string currentFilter, int? page, DateTime? DateOne, DateTime? DateTwo, string Duration);
        Task<IQueryable<Finance>> PaymentListAll(string searchString, string currentFilter, int? page, DateTime? DateOne, DateTime? DateTwo, string Duration);
        Task<List<Finance>> GetUserPaymentBySession(int sessionid);
        Task<List<Finance>> CurrentSessionPaymentList();
        Task<List<Finance>> CreditList();
        Task<List<Finance>> CreditCurrentSessionList();
        Task<List<Finance>> DebitList();
        Task<List<Finance>> DebitCurrentSessionList();
        Task<IQueryable<Finance>>  DebitCurrentSessionListPage(string searchString, string currentFilter, int? page, DateTime? DateOne, DateTime? DateTwo, string Duration);
        Task<List<Finance>> BankSourceList();
        Task<List<Finance>> OnlineSourceList();
        Task<List<Finance>> CashSourceList();

        Task<List<Finance>> BankSourceCurrentSessionList();
        Task<List<Finance>> OnlineSourceCurrentSessionList();
        Task<List<Finance>> CashSourceCurrentSessionList();
        Task<List<Finance>> GetUserPayment();

        Task Edit(Finance item);

        Task<List<Finance>> AccomodationList();
        Task<List<Finance>> PaidAccomodationList();
    }
}
