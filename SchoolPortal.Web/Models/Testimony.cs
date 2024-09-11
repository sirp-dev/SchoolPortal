using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Models
{
    public class Testimony
    {
       public long Id { get; set; }
        public string Title { get; set; }
        public string SortOrder { get; set; }
        public string Show { get; set; }
        public string ImageUrl { get; set; }
        public string ImageKey { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
