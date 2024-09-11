using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class ImageModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
       
        public string ContentType { get; set; }
        public byte[] ImageContent { get; set; }
        
    }
}