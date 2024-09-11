using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IDeviceService
    {
        Task Create(ApprovedDevice model);
        Task Edit(ApprovedDevice model);
        Task<List<ApprovedDevice>> DeviceList();
        Task Delete(int? id);
        Task<ApprovedDevice> Get(int? id);
    }
}
