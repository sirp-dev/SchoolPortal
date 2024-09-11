using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class Invoice
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public Invoice()
        {

            var set = db.Settings.FirstOrDefault();
            var setname = set.SchoolInitials;

            this.InvoiceNumber = DateTime.UtcNow.Date.Year.ToString() +
                DateTime.UtcNow.Date.Month.ToString() +
                DateTime.UtcNow.Date.Day.ToString() + Guid.NewGuid().ToString().Substring(0, 4).ToUpper() + "INV" + "-" + setname;
            this.CreatedDate = DateTime.UtcNow;
            this.Amount = 0;
           
        }

        public int Id { get; set; }
        public string ClassName { get; set; }
        public string Title { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Amount { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public PaymentStatus Status { get; set; }

        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; }
        public string TransactionReference { get; set; }
        public int? EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; }
        public int? SessionId { get; set; }
        public Session Session { get; set; }
    }
}