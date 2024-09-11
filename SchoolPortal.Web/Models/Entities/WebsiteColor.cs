using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class WebsiteColor
    {
        public int Id { get; set; }
        public string ColorName {get;set;}
        public int WebsiteSettingId { get; set; }
    }
}