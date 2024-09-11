using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class EnrolledHostelRoom
    {
        public int Id { get; set; }

        public int? HostelRoomId { get; set; }
        public HostelRoom HostelRoom { get; set; }
        public string Name { get; set; }

        [Display(Name = "Room Number")]
        public string RoomNo { get; set; }

        [Display(Name = "No Of Student")]
        public int? NoOfStudent { get; set; }

        [Display(Name = "Hostel")]
        public int? EnrolledHostelId { get; set; }
        public EnrolledHostel EnrolledHostel { get; set; }

        public ICollection<EnrolledHostelBed> EnrolledHostelBed { get; set; }
        public HostelRoomStatus Status { get; set; }
    }
}