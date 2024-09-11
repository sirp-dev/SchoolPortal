using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class MessageReply
    {
        public int Id { get; set; }
        [Display(Name = "Reply Message")]
        public string ReplyMessage { get; set; }
        public int MessageId { get; set; }
        public ContactUs ContactUs { get; set; }
        public string userId { get; set; }
        public ApplicationUser user { get; set; }
        [Display(Name = "Reply Date")]
        public DateTime ReplyDate { get; set; }
       
    }
}