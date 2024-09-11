using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models
{
    public class Schools
    {
        public int Id { get; set; }
        public string SchoolName { get; set; }
        public string Abriviation { get; set; }
        public string SchoolAddress { get; set; }
        public string WebsiteUrl { get; set; }
        public string PortalUrl { get; set; }
        public DateTime DateCreated { get; set; }
    }
}