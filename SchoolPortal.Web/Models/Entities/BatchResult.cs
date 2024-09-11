using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class BatchResult
    {
        public int Id { get; set; }
        public string BatchId { get; set; }
        public string StudentRegNumber { get; set; }
        public int ProfileId { get; set; }
        public int SessionId { get; set; }
        public int ClassId { get; set; }
        public int EnrollmentId { get; set; }
        public decimal? AverageScore { get; set; }
    }
}