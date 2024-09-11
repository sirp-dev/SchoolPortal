using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class Attendance
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int? ClassLevelId { get; set; }
        public ClassLevel ClassLevel { get; set; }
        public int? EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; }
        public int? SessionId { get; set; }
        public Session Session { get; set; }
        public bool Updated { get; set; }

        public ICollection<AttendanceDetail> AttendanceDetails { get; set; }
    }
}