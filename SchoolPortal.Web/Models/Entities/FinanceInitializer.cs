using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class FinanceInitializer
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public FinanceInitializer()
        {

            var set = db.Settings.FirstOrDefault();
            var setname = set.SchoolInitials;

            this.InvoiceNumber = DateTime.UtcNow.Date.Year.ToString() +
                DateTime.UtcNow.Date.Month.ToString() +
                DateTime.UtcNow.Date.Day.ToString() + Guid.NewGuid().ToString().Substring(0, 4).ToUpper() + "INV" + "-" + setname;

            //this.ReferenceId = DateTime.UtcNow.Date.Year.ToString() +
            //    DateTime.UtcNow.Date.Month.ToString() +
            //    DateTime.UtcNow.Date.Day.ToString() + Guid.NewGuid().ToString().Substring(0, 4).ToUpper() + "REF" + "-" + setname;
        }
        public int Id { get; set; }
        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; }
        public int IncomeId { get; set; }
        public Income Income { get; set; }
        public string InvoiceNumber { get; set; }
        public int PaymentAmountId { get; set; }
        //public PaymentAmount PaymentAmount { get; set; }
        public int SessionId { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public int Percent { get; set; }
        public DateTime TransactionDate { get; set; }
        public string UniqueId { get; set; }
        public TransactionStatus TransactionStatus { get; set; }

        [Display(Name = "Reference Id")]
        public string ReferenceId { get; set; }

        [Display(Name = "Finance Source")]
        public FinanceSource FinanceSource { get; set; }

        [Display(Name = "Finance Type")]
        public FinanceType FinanceType { get; set; }
        public int PaymentTypeId { get; set; }
        public bool Payall { get; set; }

    }
}