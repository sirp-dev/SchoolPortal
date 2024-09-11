using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos
{
    public class EnrolledStudentsByClassArchiveDto
    {
        public int Id { get; set; }

       public string StudentRegNumber { get; set; }
       public string StudentName { get; set; }
       public int StudentId { get; set; }


        public decimal? AverageScore { get; set; }
        public decimal? TotalAverage { get; set; }

        public decimal? CummulativeAverageScore { get; set; }
        
        public int SubjectCount { get; set; }
        public int? SubjectId { get; set; }
        public string Subject { get; set; }

        [Display(Name = " Test Score:")]
        //[Range(0, 30)]
        public decimal? TestScore { get; set; }

        [Display(Name = "Exam Score:")]
        //[Range(0, 70)]
        public decimal? ExamScore { get; set; }

        [Display(Name = "2nd Test Score:")]
        public decimal? TestScore2 { get; set; }

        [Display(Name = "Project:")]
        public decimal? Project { get; set; }

        [Display(Name = "Class Exercise:")]
        public decimal? ClassExercise { get; set; }

        [Display(Name = "Assessment:")]
        public decimal? Assessment { get; set; }

        [Display(Name = "Total CA:")]
        public decimal? TotalCA { get; set; }


        [Display(Name = "Total Score:")]
        public decimal? TotalScore { get; set; }

        public int EnrollmentId { get; set; }
        public Enrollment Enrollments { get; set; }

        public string Session { get; set; }
        public string SessionYear { get; set; }
        public int SessionId { get; set; }

    }
}