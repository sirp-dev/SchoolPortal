using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Web.Models.Dtos.Paystack
{
    public class PaymentInitalizationResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public SubData data { get; set; }
    }

    public class SubData
    {
        public string authorization_url { get; set; }
        public string access_code { get; set; }
        public string reference { get; set; }
    }
}
