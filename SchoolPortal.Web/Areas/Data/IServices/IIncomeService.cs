using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolPortal.Web.Models.Entities;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IIncomeService
    {
        Task Create(Income item);
        Task<Income> Get(int? id);

        Task Delete(int? id);
        Task<List<Income>> IncomeList();
        Task<List<Income>> GetUserIncome(string userId);

        Task Edit(Income item);
    }
}