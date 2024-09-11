using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class AttendanceDetail
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int? StudentId { get; set; }
        public StudentProfile StudentProfile { get; set; }
        public int? SessionId { get; set; }
        public bool IsPresent { get; set; }
        public int? EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; }

        public int? AttendanceId { get; set; }
        public Attendance Attendance { get; set; }
    }
}