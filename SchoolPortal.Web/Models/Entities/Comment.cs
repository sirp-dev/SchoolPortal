using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Username { get; set; }
       
        public string Content { get; set; }
        [Display(Name = "Date Commented:")]
        public DateTime DateCommented { get; set; }

        //navigation property
        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}