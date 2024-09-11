using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.UI
{
    public class SitePageCategory
    {
        public int Id { get; set; }
      
        public string Title { get; set; }
        public bool Show { get; set; }
        public ICollection<SitePage> SitePages { get; set; }
    }
}