using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos
{
    public class CheckResultViewModelDto2
    {
        [Required(ErrorMessage = "The Session year and Term has not been selected")]
        [Display(Name = "Session")]
        public int SessionId { get; set; }

        [Required(ErrorMessage = "PIN Number is required")]
        [Display(Name = "PIN Number")]
        [DataType(DataType.Password)]
        public string PinNumber { get; set; }

        [Required(ErrorMessage = "Serial Number is required")]
        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; }

        [Required(ErrorMessage = "Class is Required")]
        [Display(Name = "Class")]
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Student name is required")]
        [Display(Name = "Student Name")]
        public int StudentProfileId { get; set; }
    }
}