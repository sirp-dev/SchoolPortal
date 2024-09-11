using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SchoolPortal.Web.Models
{
    public class PageSectionList
    {
        public long Id { get; set; }
        public string VideoUrl { get; set; }
        public string VideoKey { get; set; }

        public string ImageUrl { get; set; }
        public string ImageKey { get; set; }
        public string IconText { get; set; }
        public int SortOrder { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }


        [Display(Name = "Mini Title")]
        public string MiniTitle { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "More Description")]
        public string MoreDescription { get; set; }


        [Display(Name = "Button Text")]
        public string ButtonText { get; set; }

        [Display(Name = "Button Link")]
        public string ButtonLink { get; set; }
         [Display(Name = "Direct Url")]
        public string DirectUrl { get; set; }

        public bool Disable { get; set; }

        public long? PageSectionId { get; set; }
        public PageSection PageSection { get; set; }
    }
}
