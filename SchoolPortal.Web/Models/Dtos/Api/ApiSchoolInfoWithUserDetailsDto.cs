using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos.Api
{
    public class ApiSchoolInfoWithUserDetailsDto
    {
        public string SchoolName { get; set; }
        public string SchoolAddress { get; set; }

        public string RegNumber { get; set; }
        public string ClassName { get; set; }
        public string Session { get; set; }
        public string Term { get; set; }
        public string Username { get; set; }

        public string SchoolLink { get; set; }

        public string FullName { get; set; }

        public string PhoneNumber { get; set; }
        public string ParentPhone { get; set; }
        public string ParentEmail { get; set; }

        public byte[] Image { get; set; }
    }
}