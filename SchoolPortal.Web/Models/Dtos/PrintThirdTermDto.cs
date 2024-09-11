using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos
{
    public class PrintThirdTermDto
    {

        public byte[] studentImage { get; set; }
        public byte[] SchoolLogo { get; set; }
        public byte[] SchoolStamp { get; set; }

        public string Fullname { get; set; }
        public string RegNumber { get; set; }
        public decimal? Average { get; set; }
        public decimal? TotalScore { get; set; }
        public decimal? OverallScore { get; set; }
        public int Position { get; set; }
        public string ClassName { get; set; }
        public int TotalStudent { get; set; }
        public string SessionTerm { get; set; }
        public string SchoolName { get; set; }
        public string ResultGrade { get; set; }
        public string ResultRemark { get; set; }
        public string AttendancePresent { get; set; }
        public string AttendanceAbsent { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string headteacher { get; set; }
        public string PromotionStatus { get; set; }
        public string EnrollmentRemark { get; set; }
        public string EnrollmentRemark2 { get; set; }
        //public string GeneralRemark { get; set; }
        //public string NextResumption { get; set; }
        public bool cumshowPosOnClassResult { get; set; }
        public bool cumshowSchAccountOnResult { get; set; }
        public bool cumshowSchFeeOnResult { get; set; }


        public List<EnrolledSubject> SubjectList { get; set; }
        public List<GradingDetails> GradingDetails { get; set; }
        public List<SchoolAccount> SchoolAccount { get; set; }
        public RecognitiveDomain RecognitiveDomain { get; set; }
        public PsychomotorDomain PsychomotorDomain { get; set; }
        public AffectiveDomain AffectiveDomain { get; set; }
        public List<SchoolFees> SchoolFees { get; set; }
        public List<NewsLetter> NewsLetter { get; set; }
        //public NewsLetter NewsLetter { get; set; }


        ///
        public string CummulativeAverage { get; set; }
        public int CummulativePosition { get; set; }
        public string CummulativeRemark { get; set; }
        public string CummulativeSessionTerm { get; set; }
        public List<EnrolledSubject> CummulativeEnrolledSubjects { get; set; }
        public List<EnrolledSubject> CummulativeFirstScore { get; set; }
        public List<EnrolledSubject> CummulativeSecondScore { get; set; }
        public List<EnrolledSubject> CummulativeThirdScore { get; set; }


        public decimal? AverageFirthTerm { get; set; }
        public decimal? AverageSecondTerm { get; set; }
        public decimal? AverageThirdTerm { get; set; }

        public string checkFirthTerm { get; set; }
        public string checkSecondTerm { get; set; }
        public string checkThirdTerm { get; set; }

        public bool IsEngMath { get; set; }

        public int StudentId { get; set; }
        public int SessionId { get; set; }
    }
}