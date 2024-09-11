using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class Examination
    {
        public int Id { get; set; }

        [Display(Name = "User Name")]
        public string Username { get; set; }

        [Display(Name = "School Link")]
        public string SchoolLink { get; set; }

        [Display(Name = "School Name")]
        public string SchoolName { get; set; }

        [Display(Name = "School Address")]
        public string SchoolAddress { get; set; }


        [Display(Name = "School Session")]
        public string Session { get; set; }

        public string Term { get; set; }

        [Display(Name = "Registration Number")]
        public string RegNumber { get; set; }
        [Display(Name = "Class")]
        public string ClassName { get; set; }
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Parent Phone Number")]
        public string ParentPhone { get; set; }


        [Display(Name = "Parent Email")]
        public string ParentEmail { get; set; }

        [Display(Name = "Profile Image")]
        public string Image { get; set; }

        [Display(Name = "Settings")]
        public int? SettingModelId { get; set; }
        public bool IsFinished { get; set; }
        public SettingModel SettingModel { get; set; }

        //public ICollection<ExaminationSubject> ExaminationSubjects { get; set; }

        public string Result { get; set; }

        public ExamStatus Status { get; set; }

        public DateTime? TimerDate { get; set; }

        public DateTime? DateCreated { get; set; }

        [Display(Name = "Duration in Minutes")]
        public int? Duration { get; set; }
    }
}