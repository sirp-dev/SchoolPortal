using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Models.UI
{
    public class SiteSetting
    {
        public int Id { get; set; }

        public bool Show { get; set; }

        public string EmailOne { get; set; }
        public string EmailTwo { get; set; }
        public string EmailThree { get; set; }

        public string PhoneOne { get; set; }
        public string PhoneTwo { get; set; }
        public string PhoneThree { get; set; }

        public string AddressOne { get; set; }
        public string AddressTwo { get; set; }
        public string AddressThree { get; set; }

        public string Host { get; set; }
        public string PX { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
    }
}