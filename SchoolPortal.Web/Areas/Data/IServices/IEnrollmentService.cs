using SchoolPortal.Web.Models.Dtos;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IEnrollmentService
    {
        Task UpdateEnrollmentStatus(int? classId);
        Task Create(Enrollment model);
        Task<Enrollment> Get(int? id);
        Task<StudentProfile> GetStudent(int? id);
        Task<List<EnrollStudentsDto>> EnrollmentAutoSearch();
        Task Edit(Enrollment models);
        Task Delete(int? id);
        Task<List<Enrollment>> List();
        Task<string> RemoveSingleEnrollmentDirect(int id);
        Task<List<Enrollment>> EnrolledStudentBySessionClassId(int sessId, int classId);
        Task<string> PromotionEnrol(string sess, int classid = 0, int oldclassid = 0, int oldsessionid = 0);
        Task<List<EnrollStudentsDto>> Enrollment(string searchString, string currentFilter, int? page);
        Task<List<EnrolledStudentsDto>> EnrolledStudents(string searchString, string currentFilter, int? page);
        Task JustEnrolToClass(int? ClassLevelId = 0, int id = 0, int termid = 0);

        Task EnrollStudentFromSession(int ClassLevelId = 0, int id = 0, int sid=0);
        Task EnrollStudent(int ClassLevelId = 0, int id = 0);
        Task EnrollStudentList(int ClassLevelId = 0, int id = 0, int SessionId = 0);
        Task<string> MoveStudent(int Ocid = 0, int ClassLevelId = 0, int id = 0);
        Task<string> MoveClassStudent(int Ocid = 0, int ClassLevelId = 0);
        Task<string> RemoveStudent(int id = 0);
        Task<string> RemoveStudentFromSelectedTerm(int id = 0, int sessionId = 0);
        Task<OutComeDto> EnrollStudentMain(int ClassLevelId = 0, int id = 0);
        Task<int> TotalEnrolledStudentByTerm();
        Task<int> TotalUnEnrolledStudentByTerm();

        Task<Enrollment> classLevelFromEnrollmentbyProfileId(int id);
        Task<Enrollment> classLevelFromEnrollmentbyEnrolId(int id);
        Task<string> ChangeToDropoutStudent(int id = 0);


        Task<List<EnrolledSubjectDto>> StudentsListBySubIdBySessionId(int subId, int sessionId);

        Task<List<EnrolledSubject>> StudentsListBySubIdBySessionIdPreview(int subId, int sessionId);
    }
}
