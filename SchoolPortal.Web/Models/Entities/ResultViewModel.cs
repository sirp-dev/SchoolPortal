using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class ResultViewModel
    {
        [Required(ErrorMessage="PIN Number is a required field")]
        [Display(Name="PIN Number")]
        public string PinNumber { get; set; }

        [Required(ErrorMessage = "Serial Number is a required field")]
        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; }
    }
}