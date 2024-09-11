using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Models.Entities
{
    public class QuestionModel
    {
        public int Id { get; set; }

        [AllowHtml]
        public string Name { get; set; }

        [Display(Name = "Exam Mode")]
        [UIHint("Enum")]
        public Mode ExamMode { get; set; }

        [Display(Name = "Subject")]
        public int? SubjectModelId { get; set; }
        public SubjectModel SubjectModel { get; set; }

        [Display(Name = "School Link")]
        public string SchoolLink { get; set; }

        [Display(Name = "School Session")]
        public string Session { get; set; }

        public string Term { get; set; }

        [Display(Name = "Class")]
        public string ClassName { get; set; }

        [Display(Name = "Subject")]
        public string Subject { get; set; }

        public ICollection<OptionModel> OptionModels { get; set; }
    }
}