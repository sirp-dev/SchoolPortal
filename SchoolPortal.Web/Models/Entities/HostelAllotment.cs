using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class HostelAllotment
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Display(Name = "Hostel Bed")]
        public int? HostelBedId { get; set; }
        public EnrolledHostelBed HostelBed { get; set; }

        [Display(Name = "Hostel Room")]
        public int? HostelRoomId { get; set; }
        public EnrolledHostelRoom HostelRoom { get; set; }

        [Display(Name = "Hostel")]
        public int? HostelId { get; set; }
        public EnrolledHostel Hostel { get; set; }

        [Display(Name = "Session")]
        public int? SessionId { get; set; }
        public Session Session { get; set; }
        public DateTime AllotedDate { get; set; }
    }
}