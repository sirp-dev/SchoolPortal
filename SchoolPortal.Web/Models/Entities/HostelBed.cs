using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class HostelBed
    {
        public int Id { get; set; }

        [Display(Name = "Bed Number")]
        public string BedNo { get; set; }

        [Display(Name = "Hostel Room")]
        public int? HostelRoomId { get; set; }
        public HostelRoom HostelRoom { get; set; }

        [Display(Name = "Hostel")]
        public int? HostelId { get; set; }
        public Hostel Hostel { get; set; }
        public HostelBedStatus Status { get; set; }

    }
}