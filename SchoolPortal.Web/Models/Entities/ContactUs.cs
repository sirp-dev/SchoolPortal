using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class ContactUs
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        [Display(Name = "Phone Number")]
        [Required]

        public string PhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }
        public int UserId { get; set; }
        public ApplicationUser user { get; set; }
        [Display(Name = "Message Status")]
        public MessageStatus messageStatus { get; set; }
        [Display(Name = "Message Reply")]
        public ICollection<MessageReply> MessageReply { get; set; }
    }
}