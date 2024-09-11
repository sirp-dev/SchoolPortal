using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class PaymentData
    {
        public int Id { get; set; }
        public int? StudentProfileId { get; set; }
        public StudentProfile StudentProfile { get; set; }
        public int SessionId { get; set; }
        public Session Session { get; set; }
        public decimal Amount { get; set; }
        [Display(Name = "Finance Source")]
        public FinanceSource FinanceSource { get; set; }

        public string UniqueId { get; set; }
        public string ApproveId { get; set; }
        public ApplicationUser ApproveUser { get; set; }
    }
}