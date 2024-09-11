using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class PinCodeModel
    {
        public int Id { get; set; }

        [Display(Name = "Pin Number")]
        public string PinNumber { get; set; }

        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; }

        public int Usage { get; set; }

        public int? EnrollmentId { get; set; }

        [Display(Name = "Batch Number")]
        public string BatchNumber { get; set; }

        [Display(Name = "Student Pin")]
        public string StudentPin { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Session Id")]
        public int? SessionId { get; set; }


        public Session Session { get; set; }

    }
}