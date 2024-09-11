using SchoolPortal.Web.Models.Dtos;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface ISubjectService
    {
        Task Create(Subject model, int id);
        Task<Subject> Get(int? id);
        Task Edit(Subject models);
        Task Delete(int? id);
        Task<IQueryable<SubjectListDto>> List(int? id);

        Task<IQueryable<SubjectListDto>> AllList(int? id);

        Task EnrolledSubject(EnrolledSubject model);

    }
}
