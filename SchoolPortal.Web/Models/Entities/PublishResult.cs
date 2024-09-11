using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class PublishResult
    {
        public int Id { get; set; }

        public int SessionId { get; set; }
        public Session Session { get; set; }

        public int ClassLevelId { get; set; }
        public ClassLevel ClassLevel { get; set; }

        [Display(Name = "Publish Date")]
        public DateTime PublishedDate { get; set; }
    }
}