using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos
{
    public class ClassLevelListDto
    {
        public int Id { get; set; }
        public string ClassLevelName { get; set; }
        public string userId { get; set; }
        public string FormTeacher { get; set; }

        public bool ShowClass { get; set; }
    }
}