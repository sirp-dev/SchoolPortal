using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos
{
    public class AdminPostDetalsDto
    {
        public string Title { get; set; }

        public string Content { get; set; }

      
        public DateTime DatePosted { get; set; }

    
        public string Status { get; set; }

       
        public string WhoCanSeePost { get; set; }

       
        public string PageType { get; set; }

        public string PostedBy { get; set; }


        public int? SortOrder { get; set; }
        public byte[] Image { get; set; }
    }
}