using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos
{
    public class EnrolledStudentsDto
    {
        public int ProfileId { get; set; }
        public string UserName { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string OtherName { get; set; }
        public string EnrolledClass { get; set; }
        public int EnrollmentId { get; set; }
        public string StudentRegNumberPin { get; set; }
        public string UserId { get; set; }

    }
}