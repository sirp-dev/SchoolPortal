using SchoolPortal.Web.Models.Dtos;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Areas.Data.IServices
{
    interface IResultService
    {
        Task<IEnumerable<EnrolledStudentsByClassDto>> StudentsBySessIdAndByClassId(int sessId, int classId);

        Task<decimal?> SumEnrolledSubjectTotalScore(int enrollmentId);
        Task<List<EnrolledSubject>> EnrolledSubjectForEnrollment(int enrollmentId);
        Task<int> TotalEnrolledSubjectCount(int enrollmentId);
        Task<List<Enrollment>> Position(int sessionId, int? classLevelId);

        Task UpdateResult(int id);
        Task<List<EnrolledStudentsByClassDto>> StudentsByByClassId(int classId);

        Task<Subject> GetSubjectByEnrolledSubId(int? id);
        Task<ClassLevel> GetClassByClassId(int? id);

        Task PublishResult(int id, int classId);
        Task UnpublishResult(int id, int classId);

        Task<StudentProfile> GetUserByRegNumber(string regnumber);
        Task<StudentProfile> GetUserByUserId(string userId);

        Task<Session> GetSessionBySessionId(int sessId);

        Task<List<EnrolledStudentsByClassDto>> CumulativeStudentsBySessIdAndByClassId(int sessId, int classId);

        Task<string> CumulativeResultReconciliationByClassId(int sessId, int classId);

        Task<Enrollment> GetEnrollmentBySessIdStudentProfileId(int sessId, int ProfileId);

        Task<PinCodeModel> PinCodeByPinNumberAndSerialNumber(string pin, string serialnumber);

        Task<PublishResult> PublishResultBySessIdAndClassId(int sessId, int? classId);

        Task<Defaulter> GetDefaulterByProfileId(int profileId);

        Task Update();
        Task<int> SubjectCount(int? enrolId);
        //Task<PrintResultDto> PrintResult(Int32? id);
    }
}
