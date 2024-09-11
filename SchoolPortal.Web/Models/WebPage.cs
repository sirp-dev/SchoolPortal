using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq; 

namespace SchoolPortal.Web.Models
{
    public class WebPage
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public ICollection<PageSection> PageSections { get; set; }
        public int SortOrder { get; set; }

        public bool Publish { get; set; }
        public bool SecurityPage { get; set; }

        public long? PageCategoryId { get; set; }
        public PageCategory PageCategory { get; set; }

        public string ImageUrl { get; set; }
        public string ImageKey { get; set; }

        [Display(Name = "Show In Main Top")]
        public bool ShowInMainTop { get; set; }

        [Display(Name = "Show In Menu DropDown")]
        public bool ShowInMenuDropDown { get; set; }

        [Display(Name = "Show In Main Menu")]
        public bool ShowInMainMenu { get; set; }

        [Display(Name = "Show In Footer")]
        public bool ShowInFooter { get; set; }

        [Display(Name = "Enable Direct Url")]
        public bool EnableDirectUrl { get; set; }
        [Display(Name = "Direct Url")]
        public string DirectUrl { get; set; }

        [Display(Name = "Home Sort From")]
        public HomeSortFrom HomeSortFrom { get; set; }
    }
}
