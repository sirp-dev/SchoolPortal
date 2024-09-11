using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SchoolPortal.Web.Models
{
    public class Blog
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string VideoUrl { get; set; }
        public string VideoKey { get; set; }

        public bool Publish { get; set; }
        public int SortOrder { get; set; }

        public string ImageUrl { get; set; }
        public string ImageKey { get; set; }
        public long? BlogCategoryId { get; set; }
        public BlogCategory BlogCategory { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Full Description")]
        public string FullDescription { get; set; }
    }
}
