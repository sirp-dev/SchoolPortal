using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.Web.Models.Entities
{
    public class Hostel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Display(Name = "Hostel Type")]
        public string HostelType { get; set; }
        public HostelStatus Status { get; set; }
        public ICollection<HostelRoom> HostelRoom { get; set; }


    }
}