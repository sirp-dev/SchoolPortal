using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.Web.Models.Entities
{
    public class Property
    {
        public int Id { get; set; }

        [Display(Name = "Unit per Sms")]
        public decimal? UnitsPerSms { get; set; }

        [Display(Name = "Unit in Account")]
        public decimal? UnitsInAccount { get; set; }

        [Display(Name = "Sms Username")]
        public string SmsUsername { get; set; }

        [Display(Name = "Sms Password")]
        public string SmsPassword { get; set; }

     
    }
}