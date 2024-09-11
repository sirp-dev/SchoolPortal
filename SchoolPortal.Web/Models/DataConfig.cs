using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models
{
    public class DataConfig
    {
        public long Id { get;set;}

        [Display(Name = "Login CSS")]
        public string LoginCSS { get; set; }


        [Display(Name = "Layout CSS")]
        public string LayoutCSS { get; set; }

        [Display(Name = "Dashboard CSS")]
        public string DashboardCSS { get; set; }

        [Display(Name = "Redirect To https//www.")]
        public bool RedirectTohttpswww { get; set; }

        [Display(Name = "Redirect To https//")]
        public bool RedirectTohttps { get; set; }

        [Display(Name = "Configuration")]
        public string Configuration { get; set; }

        [Display(Name = "Live Configuration")]
        public string LiveConfiguration { get; set; }

    }
}