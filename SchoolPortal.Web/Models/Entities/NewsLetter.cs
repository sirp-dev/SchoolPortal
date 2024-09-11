using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class NewsLetter
    {
        public int Id { get; set; }

        [Display(Name = "Next Term Resumption Date")]
        public string NextTResumptionDate { get; set; }

        [Display(Name = "General Remark")]
        public string GenRemark { get; set; }

        [Display(Name = "Date of PTA meeting")]
        public string PTAMeetingDate { get; set; }

        [Display(Name = "PTA Fee")]
        public string PTAFee { get; set; }

        public int? SessionId { get; set; }
        public Session Session { get; set; }
    }
}

