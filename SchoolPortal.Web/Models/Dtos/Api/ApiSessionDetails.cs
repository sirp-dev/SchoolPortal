using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos.Api
{
    public class ApiSessionDetails
    {
        public string SchoolCurrentPrincipal { get; set; }
        public string ClassCount { get; set; }
        public string EnrolStudentsCount { get; set; }
        public string UnEnrolStudentsCount { get; set; }
        public string Usedcard { get; set; }
        public string TotalStaff { get; set; }
        public string CurrentSession { get; set; }
        public string TotalResults { get; set; }
        public string TotalCummulativeResults { get; set; }
        public string Session { get; set; }
        public string Term { get; set; }
    }
}