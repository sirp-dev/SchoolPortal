using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.Web.Models.Entities
{
    public class Subject
    {
        public Subject()
        {
            ShowSubject = true;
        }
        public int Id { get; set; }

        [Display(Name = "Subject:")]
        [Required(ErrorMessage = "Subject is required")]
        public string SubjectName { get; set; }

        public int ClassLevelId { get; set; }
        public ClassLevel ClassLevel { get; set; }

        public decimal? ExamScore { get; set; }
        public decimal? TestScore { get; set; }
        public decimal? TestScore2 { get; set; }
        public decimal? Project { get; set; }
        public decimal? ClassExercise { get; set; }
        public decimal? Assessment { get; set; }
        //public decimal? TotalCA { get; set; }

        public decimal? PassMark { get; set; }

       

        [UIHint("Enum")]
        public RequiresPass RequiresPass { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public bool ShowSubject { get; set; }

        public virtual ICollection<OnlineCourseUpload> OnlineCourseUpload { get; set; }
    }
}