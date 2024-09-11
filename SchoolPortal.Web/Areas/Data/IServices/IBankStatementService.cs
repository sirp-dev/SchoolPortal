using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IBankStatementService
    {
        Task Create(BankDetails item);
        Task<BankDetails> Get(int? id);

        Task Delete(int? id);
        Task<List<BankDetails>> BankStatementList();
        Task Edit(BankDetails item);
    }
}
