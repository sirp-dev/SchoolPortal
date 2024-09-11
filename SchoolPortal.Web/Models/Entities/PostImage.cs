using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class PostImage
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string ImageByte { get; set; }
        public byte[] ImageContent { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}