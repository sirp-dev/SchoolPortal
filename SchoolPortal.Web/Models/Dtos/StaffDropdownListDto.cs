using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos
{
    public class StaffDropdownListDto
    {
        public int StaffId { get; set; }
        public string FullName { get; set; }
        public string UserId { get; set; }
    }
}