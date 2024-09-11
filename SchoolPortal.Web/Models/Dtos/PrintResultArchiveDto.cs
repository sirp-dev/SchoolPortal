using SchoolPortal.Web.Models.Entities;
using SchoolPortal.Web.Models.ResultArchive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Models.Dtos
{
    public class PrintResultArchiveDto
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
        public int NumberInClass { get; set; }
        public int TotalStudent { get; set; }
        public int SessionId { get; set; }
        public string SessionTerm { get; set; }
        public string SessionTerm1 { get; set; }
        public string SessionTerm2 { get; set; }
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
        public bool showPosOnClassResult { get; set; }
        public bool showSchAccountOnResult { get; set; }
        public bool showSchFeeOnResult { get; set; }

        public decimal? LowestTotalAverage { get; set; }

        public decimal? HighestTotalAverage { get; set; }

        public List<SubjectListPrintoutArchiveDto> SubjectList { get; set; }
        public List<GradingDetails> GradingDetails { get; set; }
        public List<SchoolAccount> SchoolAccount { get; set; }
        public RecognitiveDomainArchive RecognitiveDomain { get; set; }
        public PsychomotorDomainArchive PsychomotorDomain { get; set; }
        public List<NewsLetterArchive> NewsLetter { get; set; }
        public AffectiveDomainArchive AffectiveDomain { get; set; }
        public List<SchoolFeesArchive> SchoolFees { get; set; }
        public List<PrincipalArchive> PrincipalArchives { get; set; }

        [AllowHtml]
        public string NewsletterContent { get; set; }
        public bool ShowNewsletterPage { get; set; }

    }
}