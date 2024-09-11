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
    public class PostModal
    {
        public long Id { get; set; }
        public string ImageUrl { get; set; }
        public string ImageKey { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public ModalTime ModalTime { get; set; }
        public ModalOccurance ModalOccurance { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? StopTime { get; set; }

        public bool ShowOnlyImage { get; set; }
        public bool ShowTitle { get; set; }
        public bool ShowDescription { get; set; }
        public bool ShowImage { get; set; }
        public bool Publish { get; set; }
        [Display(Name = "Button Link")]
        public string ButtonLink { get; set; }
    }
}
