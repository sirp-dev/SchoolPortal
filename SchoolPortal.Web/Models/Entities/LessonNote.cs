using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Models.Entities
{
    public class LessonNote
    {
        public int Id { get; set; }      
        public int? SubjectId { get; set; }
        public Subject Subject { get; set; }

         public int? SessionId { get; set; }
        public Session Session { get; set; }

         public int? StaffProfileId { get; set; }
        public StaffProfile StaffProfile { get; set; }
        public string Topic { get; set; }
        [AllowHtml]
        public string Note { get; set; }
        public DateTime DateCreated { get; set; } 
        public DateTime LastEdited { get; set; } 
        public bool IsPublished { get; set; }
    }
}