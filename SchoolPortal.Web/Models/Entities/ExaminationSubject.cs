using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class ExaminationSubject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public double Score { get; set; }

        //public int? SubjectId { get; set; }

        [Display(Name = "Examination")]
        public int? ExaminationId { get; set; }

        [Display(Name = "School Link")]
        public string SchoolLink { get; set; }

        [Display(Name = "Settings")]
        public int? SettingModelId { get; set; }

        [Display(Name = "Subject")]
        public int? SubjectModelId { get; set; }
        public SubjectModel SubjectModel { get; set; }

        public int? SchoolSubjectId { get; set; }
        public int? SchoolClassId { get; set; }
    }
}