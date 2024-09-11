using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class Qualification
    {
        public int Id { get; set; }

        [Required]
        [Display(Name="Name of Institution")]
        public string NameOfInstitution { get; set; }

        [Required]
         [Display(Name = "Year Obtained")]
        public string YearObtained { get; set; }

        [Required]
        [Display(Name = "Certificate Obtained")]
        public string CertificateObtained { get; set; }

       
        public int StaffId { get; set; }
        public string UserId { get; set; }
        public StaffProfile StaffProfile { get; set; }
    }
}