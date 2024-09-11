using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class GradingDetails
    {
        public int Id { get; set; }
        public int GradingId { get; set; }
        public Grading Grading { get; set; }
        [Display(Name = "Upper Limit")]
        public int UpperLimit { get; set; }
        [Display(Name = "Lower Limit")]
        public int LowerLimit { get; set; }
        public string Grade { get; set; }
        public string Remark { get; set; }
    }
}