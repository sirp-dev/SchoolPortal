using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Models.UI
{
    public class SiteNavHeader
    {
        public int Id { get; set; }
        [AllowHtml]
        public string Content { get; set; }
        public bool Show { get; set; }
    }
}