using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class EnrolledHostelBed
    {
        public int Id { get; set; }

        public int? HostelBedId { get; set; }
        public HostelBed HostelBed { get; set; }

        [Display(Name = "Bed Number")]
        public string BedNo { get; set; }

        [Display(Name = "Hostel Room")]
        public int? EnrolledHostelRoomId { get; set; }
        public EnrolledHostelRoom EnrolledHostelRoom { get; set; }

        [Display(Name = "Hostel")]
        public int? EnrolledHostelId { get; set; }
        public EnrolledHostel EnrolledHostel { get; set; }
        public HostelBedStatus Status { get; set; }

    }
}