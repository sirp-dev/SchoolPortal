using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Complain { get; set; }
        public TicketPriority Priority { get; set; }
        public DateTime Date { get; set; }
        public bool Closed { get; set; }
        public string TicketNumber { get; set; }

        public string IpAddress { get; set; }
        public string browser { get; set; }

        public ICollection<Response> Responses { get; set; }
    }
}