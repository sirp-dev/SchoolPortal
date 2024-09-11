using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Models.Entities
{
    public class Response
    {
        public int Id { get; set; }
        [AllowHtml]
        public string Message { get; set; }
        public string RepliedBy { get; set; }
        public DateTime Date { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }
    }
}