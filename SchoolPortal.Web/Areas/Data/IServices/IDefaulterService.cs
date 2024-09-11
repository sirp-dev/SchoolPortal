using SchoolPortal.Web.Models.Dtos;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IDefaulterService
    {
        Task<string> Create(Defaulter model, int id=0);
        Task<Defaulter> GetDefaulter(int? id);
        Task<Defaulter> Get(int? id);

        Task Edit(Defaulter models);
        Task Delete(int? id);
        Task<StudentProfile> RemoveDefaulter(int? id);
        Task<List<Defaulter>> List();
    }
}
