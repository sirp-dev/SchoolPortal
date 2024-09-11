using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IPaymentAmountService
    {
        //Task Create(PaymentAmount item);
        //Task<PaymentAmount> Get(int? id);
        //Task Delete(int? id);
        //Task<List<PaymentAmount>> PaymentAmountList();
        //Task CreateForAllClass(PaymentAmount item, bool All);
        //Task<List<PaymentAmount>> AmountList();
        //Task<List<PaymentAmount>> StudentAmountList();
        //Task<List<PaymentAmount>> StudentAmountListBySession(int sessionId);
        //Task Edit(PaymentAmount item);
        Task<List<Income>> IncomeList();
        Task<IQueryable<Finance>> ListFinanceByType(int PaymentTypeid, int id);
    }
}
