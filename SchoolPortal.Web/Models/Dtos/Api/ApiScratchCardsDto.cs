using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos.Api
{
    public class ApiScratchCardsDto
    {
        public string BatchNumber { get; set; }
        public string Count { get; set; }
        public string DateUploaded { get; set; }
    }
}