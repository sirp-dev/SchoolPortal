using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IInvoiceService
    {
        Task Create(Invoice item);
        Task<Invoice> Get(int? id);

        Task Delete(int? id);
        Task<List<Invoice>> InvoiceList();

        Task<List<Invoice>> PaidInvoiceList();

        Task<List<Invoice>> UnPaidInvoiceList();
        Task<List<Invoice>> GetUserInvoice(string userId);
        Task<List<Invoice>> GetUserInvoiceWithOutId();

        Task Edit(Invoice item);
    }
}
