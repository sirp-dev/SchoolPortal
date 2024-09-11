using SchoolPortal.Web.Models.Entities;
using SchoolPortal.Web.Models.ResultArchive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IEnrolledSubjectArchiveService
    {
        Task Create(EnrolledSubjectArchive model);
        Task<EnrolledSubjectArchive> Get(int? id);
        Task Edit(EnrolledSubjectArchive models);
        Task Delete(int? id);
        Task<List<EnrolledSubjectArchive>> List();
        Task EditScore(int id);
    }
}
