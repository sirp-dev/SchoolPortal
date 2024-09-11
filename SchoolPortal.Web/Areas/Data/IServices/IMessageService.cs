using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IMessageService
    {
        Task<string> AllStudentsContact();

        Task<string> AllStaffContact();

        Task<string> AllStudentInClassContact(int[] classLevelId);

        Task<string> AllParentInClassContact(int[] classLevelId);

        Task<string> AllParentInContact();

        Task<Property> SmsProperty();

      
        Task<List<SmsReport>> MessageHistory();

        Task<SmsReport> GetMessage(int id);

        Task<string> AccountBalance();
        //

        Task<string> GetPhoneNumbers(string category, string ClassSend);

        Task<string> SendSms(string SenderId, string Recipients, string Message, string SendOption, string ScheduleDate);

    }
}
