using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Models.Entities
{
    public class WebsiteSettings
    {
        public int Id { get; set; }
        public string PortalUrl { get; set; }
        public string WebsiteUrl { get; set; }
        public byte[] WebsiteLogo { get; set; }
        public byte[] WebsiteIcon { get; set; }

        //setings
        public int Layout { get; set; }
        public string Color { get; set; }
        public int Slider { get; set; }
        public int AboutUs { get; set; }
        public int EventAndNews { get; set; }
        public int ContactUs { get; set; }


        public bool ShowBlogInHome { get; set; }
        public bool ShowHallOfFameInHome { get; set; }

    }
}