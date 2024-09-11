using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class HostelRoom
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Display(Name = "Room Number")]
        public string RoomNo { get; set; }

        [Display(Name = "No Of Student")]
        public int? NoOfStudent { get; set; }

        [Display(Name = "Hostel")]
        public int? HostelId { get; set; }
        public Hostel Hostel { get; set; }

        public ICollection<HostelBed> HostelBed { get; set; }
        public HostelRoomStatus Status { get; set; }
    }
}