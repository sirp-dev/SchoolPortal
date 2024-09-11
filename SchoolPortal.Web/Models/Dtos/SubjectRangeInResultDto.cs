using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos
{
    public class SubjectRangeInResultDto
    {
        public decimal? HghestScoreInSubject { get; set; }
        public decimal? LowestScoreInSubject { get; set; }
    }
}