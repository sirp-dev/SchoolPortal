using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class ClassModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Display(Name = "School Link")]
        public string SchoolLink { get; set; }

        public int? SchoolClassId { get; set; }

        public ICollection<SubjectModel> SubjectModel { get; set; }
    }
}