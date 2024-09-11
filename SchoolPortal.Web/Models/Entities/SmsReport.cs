using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class SmsReport
    {
        public int Id { get; set; }

        [Display(Name = "Sent To")]
        public string SendTo { get; set; }

        [Display(Name="Sender's ID")]
        [StringLength(11)]
        public string SenderId { get; set; }

        [Display(Name = "Group Name")]
        public string GroupName { get; set; }
        public string Message { get; set; }
        public string Comment { get; set; }

        [Display(Name = "DateSent")]
        public DateTime DateSent { get; set; }
    }
}