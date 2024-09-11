using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos.Api
{
    public class ApiSchoolInfoDto
    {
        public string SchoolName { get; set; }
        public string SchoolAddress { get; set; }
        public string SchoolCurrentPrincipal { get; set; }
        public string ClassCount { get; set; }
        public string EnrolStudentsCount { get; set; }
        public string UnEnrolStudentsCount { get; set; }
        public string TotalStudentsCount { get; set; }
        public string Url { get; set; }
        public string SchoolType { get; set; }
        public string Usedcard { get; set; }
        public string NonUsedcard { get; set; }
        public string Totalcard { get; set; }
        public string TotalStaff { get; set; }
        public string CurrentSession { get; set; }
        public string Session { get; set; }
        public string Term { get; set; }
        public string BatchPrint { get; set; }

        public Byte[] Image { get; set; }

    }
}