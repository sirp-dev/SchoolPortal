using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IAttendanceService
    {
        Task Create(Attendance model);
        Task Edit(Attendance model);

        Task AddAllStudentsInClass(int id);
        Task UpdateAttendance(AttendanceDetail model);

        Task<List<AttendanceDetail>> ListAttendanceDetail(int id);
        Task<List<AttendanceDetail>> ListAttendanceDetailByStudent(int id);
        Task<List<Attendance>> ListAttendanceByClassBySession(int id);

    }
}
