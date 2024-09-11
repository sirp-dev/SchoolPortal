using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface ISchoolFeeService
    {
        Task Create(SchoolFees model);
        Task<SchoolFees> Get(int? id);

        Task Edit(SchoolFees models);
        Task Delete(int? id);
        Task<List<SchoolFees>> List();
    }
}
