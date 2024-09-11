using SchoolPortal.Web.Models.Dtos;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    public interface IStaffService
    {
        Task<StaffInfoDto> Get(int? id);
        Task UpdateImageId(int id, int imgId);
        Task<StaffInfoDto> GetStaffWithoutId();
        Task Edit(StaffInfoDto models);

        Task<List<StaffProfile>> List();
        Task<List<StaffDropdownListDto>> StaffDropdownList();
        ////

        Task<int> CreateQualification(Qualification model);
        Task<Qualification> GetQualification(int? id);
        Task<int> EditQualification(Qualification models);
        Task<int> DeleteQualification(int? id);
        Task<List<Qualification>> ListQualification(int id);
        Task<List<Qualification>> ListQualificationForUser();


        Task<List<Subject>> SubjectsByStaff();
        Task<List<ClassLevel>> ClassesByStaff();
        Task ReloadStudents(int id = 0, int sessionId = 0, int classId = 0);
        Task AdminReloadStudents(int id = 0, int sessionId = 0, int classId = 0);

        Task AdminReloadStudents2(int id = 0, int sessionId = 0, int classId = 0);
        Task RemoveStudents(int id = 0, int sessionId = 0, int classId = 0);
        Task<StudentProfile> StudentsList(int subjectId, int sessionId);

        Task<List<ClassLevel>> StaffClassName();
        Task<int> StaffSubjects();





    }
}
