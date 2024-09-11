using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IExpenditureService
    {
        Task Create(Expenditure item);
        Task<Expenditure> Get(int? id);

        Task Edit(Expenditure item);
        Task Delete(int? id);
        Task<List<Expenditure>> ExpenditureList();
        Task<List<Expenditure>> GetUserExpenditure(string userId);
    }
}
