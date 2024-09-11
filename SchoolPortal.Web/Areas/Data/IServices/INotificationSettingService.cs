using SchoolPortal.Web.Models.Dtos;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface INotificationSettingService
    {
        Task Create(Notification model);
        Task<Notification> Get(int? id);
        Task<NotificationDto> GetNotification();
        Task Edit(Notification models);
    }
}
