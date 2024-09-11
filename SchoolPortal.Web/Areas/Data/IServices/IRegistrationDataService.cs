using SchoolPortal.Web.Models.Dtos;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IRegistrationDataService
    {
        Task Create(StudentData model, string pinNumber);
        Task<StudentDataDto> Get(int? id);
        Task<StudentData> GetEdit(int? id);
        Task Edit(StudentData model);
        Task Delete(int? id);
        Task AdmitStudent(int id);
        Task<List<StudentData>> List();
        Task<PinCodeModel> GetPin();
        Task<StudentData> FormData();
        Task<StudentData> FormEmail();
    }
}
