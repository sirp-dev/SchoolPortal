using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class SettingModel
    {
        public SettingModel()
        {
            SubjectCount = 0;
        }

        public int Id { get; set; }
        public string ClassName { get; set; }

        [Display(Name = "Exam Mode")]
        [EnumDataType(typeof(Mode))]
        public Mode ExamMode { get; set; }


        [Display(Name = "Duration in Minutes")]
        public int? Duration { get; set; }

        public DateTime? TimerDate { get; set; }

        [Display(Name = "Subject Count")]
        public int SubjectCount { get; set; }

        [Display(Name = "Shuffle Question")]
        public bool ShuffleQuestion { get; set; }

        [Display(Name = "Shuffle Option")]
        public bool ShuffleOption { get; set; }

        public string SchoolLink { get; set; }

        [Display(Name = "School Session")]
        public string Session { get; set; }

        public string Term { get; set; }

        public int? SchoolClassId { get; set; }
    }
}