using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.Web.Models.Entities
{
    public class EnrolledHostel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int? HostelId { get; set; }
        public Hostel Hostel { get;set; }

        [Display(Name = "Hostel Type")]
        public string HostelType { get; set; }
        public HostelStatus Status { get; set; }

        [Display(Name = "Session")]
        public int? SessionId { get; set; }
        public Session Session { get; set; }
        public ICollection<EnrolledHostelRoom> EnrolledHostelRoom { get; set; }


    }
}