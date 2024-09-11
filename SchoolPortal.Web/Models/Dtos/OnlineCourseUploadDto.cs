using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Models.Dtos
{
    public class OnlineCourseUploadDto
    {
        public string Topic { get; set; }
        public int? ClassLevelId { get; set; }
        public int? SessionId { get; set; }
        public int? SubjectId { get; set; }
        public string Subject { get; set; }

        [Display(Name = "Upload Type")]
        [Required]
        public UploadType UploadType { get; set; }

        [Required]
        [Display(Name = "Course File")]
        public string Upload { get; set; }

        [AllowHtml]
        [Display(Name = "Course Description")]
        public string Description { get; set; }

        [Display(Name = "Date Upload")]
        public DateTime? Date { get; set; }

        //Assignment field
        [Display(Name = "Assignment Title")]
        public string AssignmentTitle { get; set; }
        [AllowHtml]
        [Display(Name = "Assignment Content")]
        public string AssignmentContent { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateSubmitionEnds { get; set; }
        public bool IsPublished { get; set; }
        public string UserId { get; set; }
    }
}