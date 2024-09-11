using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos
{
    public class PostDto
    {
        public int Id { get; set; }

      
        public string Title { get; set; }

        public string Content { get; set; }

    
        public string PreviewContent { get; set; }

      
        public DateTime DatePosted { get; set; }

       
        public PostStatus Status { get; set; }

     
        public WhoSeePost WhoCanSeePost { get; set; }

      
        public string PostedBy { get; set; }

        
        public int? SortOrder { get; set; }

       
        public string PostImage { get; set; }
        public ICollection<PostImage> PostImages { get; set; }
        public byte[] PostByteImages { get; set; }
    }
}