using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class Quote
    {
        public int Id { get; set; }
        public DateTime? DateDisplayed { get; set; }
        public string QuoteOfTheDay { get; set; }


    }
}