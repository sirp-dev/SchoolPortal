using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SchoolPortal.Web.Models.Entities
{
    public class OptionModel
    {
        public int Id { get; set; }

        [AllowHtml]
        public string Name { get; set; }

        [Display(Name = "Question")]
        public int? QuestionModelId { get; set; }
        public QuestionModel QuestionModel { get; set; }

        [Display(Name = "Correct Option")]
        public bool IsAnswer { get; set; }

        [Display(Name = "School Link")]
        public string SchoolLink { get; set; }

        [Display(Name = "School Session")]
        public string Session { get; set; }

        public string Term { get; set; }
    }
}