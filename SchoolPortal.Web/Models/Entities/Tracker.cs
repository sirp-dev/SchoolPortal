using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class Tracker
    {
        public Tracker()
        {
            ActionDate = DateTime.UtcNow.AddHours(1);
        }
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string FullName { get; set; }

        public string UserName { get; set; }
        public string Note { get; set; }
        public DateTime ActionDate { get; set; }
    }
}