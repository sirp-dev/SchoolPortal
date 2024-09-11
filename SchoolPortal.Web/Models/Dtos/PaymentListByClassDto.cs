using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos
{
    public class PaymentListByClassDto
    {
        public string Description { get; set; }
        public string Title {get;set;}
        public int Id {get; set; }
        public decimal? Amount { get; set; }
    }
}