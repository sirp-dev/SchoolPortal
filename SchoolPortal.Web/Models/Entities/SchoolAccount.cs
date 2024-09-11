using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class SchoolAccount
    {
        public int Id { get; set; }

        [Display(Name = "ACCOUNT PURPOSE")]
        public string AcctPurpose { get; set; }

        [Display(Name = "BANK NAME")]
        public string BankName { get; set; }

        [Display(Name = "ACCOUNT NAME")]
        public string AcctName { get; set; }

        [Display(Name = "ACCOUNT NUMBER")]
        public string AcctNumber { get; set; }


    }
}