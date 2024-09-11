using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos
{
    public class PostDetalsDto
    {
        public string Title { get; set; }

        public string Content { get; set; }

      
        public DateTime DatePosted { get; set; }

       
        public string PostedBy { get; set; }
        
        public byte[] Image { get; set; }
    }
}