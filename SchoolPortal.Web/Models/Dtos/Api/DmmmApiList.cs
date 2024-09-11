using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos.Api
{
    public class DmmmApiList
    {
        public string Url { get; set; }
        public string School { get; set; }
        public string Year { get; set; }
        public string Total { get; set; }
        public string Enrolled { get; set; }
        public string HasResult { get; set; }
        public string NoResult { get; set; }
        public string PrintedResult { get; set; }
    }
}