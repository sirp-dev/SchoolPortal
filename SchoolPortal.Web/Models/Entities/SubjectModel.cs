using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class SubjectModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int? ClassModelId { get; set; }
        public ClassModel ClassModel { get; set; }

        [Display(Name = "School Link")]
        public string SchoolLink { get; set; }

        public int? SchoolSubjectId { get; set; }
        public int? SchoolClassId { get; set; }

        public ICollection<QuestionModel> QuestionModel { get; set; }
    }
}