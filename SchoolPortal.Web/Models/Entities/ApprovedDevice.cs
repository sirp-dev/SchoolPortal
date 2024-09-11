using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class ApprovedDevice
    {
        public int Id { get; set; }
        public string DeviceName { get; set; }
        public string MacAddress { get; set; }
        public string ImelNumber { get; set; }
        public string IpAddress { get; set; }
        public DateTime? Date { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}