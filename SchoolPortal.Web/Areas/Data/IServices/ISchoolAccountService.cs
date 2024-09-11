using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface ISchoolAccountService
    {
        Task Create(SchoolAccount model);
        Task<SchoolAccount> Get(int? id);

        Task Edit(SchoolAccount models);
        Task Delete(int? id);
        Task<List<SchoolAccount>> List();
    }
}
