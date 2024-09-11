using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Models.Entities
{
    public class Assignment
    {
        public int Id { get; set; }
        public int? ClassLevelId { get; set; }
        public ClassLevel ClassLevel { get; set; }
        public int? SessionId { get; set; }
        public Session Session { get; set; }
        public int? SubjectId { get; set; }
        public Subject Subject { get; set; }
        public string Title { get; set; }
        [AllowHtml]
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateSubmitionEnds { get; set; }
        public bool IsPublished { get; set; }

        public virtual ICollection<AssignmentAnswer> AssignmentAnswers { get; set; }
    }
}