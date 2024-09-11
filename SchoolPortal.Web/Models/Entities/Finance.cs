using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class Finance
    {
       

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        [Display(Name = "Admin Note")]

        public string AdminNote { get; set; }
        public string Description { get; set; }

        [Display(Name = "Finance Type")]
        public FinanceType FinanceType { get; set; }

        public decimal Amount { get; set; }
        public string TellerNumber { get; set; }
        public decimal Balance { get; set; }
        public string UniqueIdCheck { get; set; }

        [Display(Name = "Reference Id")]
        public string ReferenceId { get; set; }

        [Display(Name = "Finance Source")]
        public FinanceSource FinanceSource { get; set; }

        public TransactionStatus TransactionStatus { get; set; }

        public AllocationStatus AllocationStatus { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; }
        
        public int? EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; }

        public int? SessionId { get; set; }
        public Session Session { get; set; }

        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }

        [Display(Name = "Approved By")]
        public string ApprovedById { get; set; }
        public int PaymentTypeId { get; set; }
        public int IncomeId { get; set; }
        public ApplicationUser ApprovedBy { get; set; }

        public bool Payall { get; set; }

        public bool Skip { get; set; }

    }
}