using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class SmsGroup
    {
        public int Id { get; set; }

        [Display(Name = "Group Name")]
        public string GroupName { get; set; }
        public string Numbers { get; set; }

        [Display(Name = "Numbers Count")]
        public int? NumbersCount { get; set; }
    }
}