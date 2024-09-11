using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.Web.Models.Entities
{
    public class ClassLevel
    {
        public ClassLevel()
        {
            ShowPositionOnClassResult = true;
        }

        public int Id { get; set; }

        [Display(Name = "Class:")]
        [Required(ErrorMessage = "Class is required")]
        public string ClassName { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public StaffProfile StaffProfile { get; set; }
        public int? SessionId { get; set; }
        public Session Session { get; set; }


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

        [Display(Name = "Accessment Total Score")]
        public decimal? Assessment { get; set; }

        [Display(Name = "Show Position on Result")]
        public bool ShowPositionOnClassResult { get; set; }

        [Display(Name = "School Fees")]
        public decimal SchoolFee { get; set; }

        [Display(Name = "Sort Score by Sortorder")]
        public bool SortByOrder { get; set; }

        [Display(Name = "Select If Exam Score, Test Score and Passmark is the same for all subjects in class")]
        public bool SubjectSettings { get; set; }

        [Display(Name = "Show Class")]
        public bool ShowClass { get; set; }

        [Display(Name = "Show Average Over PositionInClass")]
        public bool ShowAverageOverPositionInClass { get; set; }

        public virtual ICollection<Subject> Subjects { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; }


      
    }
}