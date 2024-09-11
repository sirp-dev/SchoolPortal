using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IEnrolledSubjectService
    {
        Task Create(EnrolledSubject model);
        Task<EnrolledSubject> Get(int? id);
        Task Edit(EnrolledSubject models);
        Task Delete(int? id);
        Task<List<EnrolledSubject>> List();
        Task EditScore(int id);
    }
}
