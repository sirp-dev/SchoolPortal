using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Models.Entities
{
    public class OnlineCourseUpload
    {
        public int Id { get; set; }
        public string Topic { get; set; }
        public int? ClassLevelId { get; set; }
        public ClassLevel ClassLevel { get; set; }
        public int? SessionId { get; set; }
        public Session Session { get; set; }
        public int? SubjectId { get; set; }
        public Subject Subject { get; set; }

        [Display(Name = "Upload Type")]
        [Required]
        public UploadType UploadType { get; set; }

        [Required]
        public string Upload { get; set; }

        [AllowHtml]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Date Upload")]
        public DateTime? Date { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}