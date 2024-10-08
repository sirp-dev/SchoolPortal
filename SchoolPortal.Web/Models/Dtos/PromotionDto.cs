﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos
{
    public class PromotionDto
    {
        public int ProfileId { get; set; }
        public string UserName { get; set; }
        public string ClassName { get; set; }
        public string FullName { get; set; }
        public string StudentRegNumber { get; set; }
        public int EnrollmentId { get; set; }
    }
}