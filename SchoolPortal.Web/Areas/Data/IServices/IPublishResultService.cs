using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IPublishResultService
    {
        Task Create(PublishResult model);
        Task<PublishResult> Get(int? id);
        Task<bool> CheckPublishResult(int SessionId, int classId);
        Task Edit(PublishResult models);
        Task Delete(int? id);
        Task<List<PublishResult>> List();
    }
}
