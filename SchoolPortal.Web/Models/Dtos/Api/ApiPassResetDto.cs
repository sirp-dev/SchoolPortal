using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos.Api
{
    public class ApiPassResetDto
    {
        public string Data { get; set; }
        public DateTime DateCreated { get; set; }
        public string SchoolName { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
    }
}