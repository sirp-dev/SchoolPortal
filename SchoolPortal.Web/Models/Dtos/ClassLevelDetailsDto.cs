using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos
{
    public class ClassLevelDetailsDto
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public string FormTeacher { get; set; }
        public int NumberOfSubjects { get; set; }
        public int NumberOfStudents { get; set; }
        public string UserId { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; }


        public decimal Passmark { get; set; }

        [Display(Name = "Mark for Promotion on Trial")]
        public decimal PromotionByTrial { get; set; }

        [Display(Name = "Test or Assessment Total Score")]
        public decimal AccessmentScore { get; set; }

        [Display(Name = "Exam Total Score")]
        public decimal ExamScore { get; set; }

         [Display(Name = "2nd Test Total Score")]
        public decimal? TestScore2 { get; set; }

        [Display(Name = "Project Total Score")]
        public decimal? Project { get; set; }

        [Display(Name = "Class Exercise Total Score")]
        public decimal? ClassExercise { get; set; }

        [Display(Name = "Assessment Total Score")]
        public decimal? Assessment { get; set; }

        [Display(Name = "Show Position on Result")]
        public bool ShowPositionOnClassResult { get; set; }

        public bool ShowClass { get; set; }
    }
}