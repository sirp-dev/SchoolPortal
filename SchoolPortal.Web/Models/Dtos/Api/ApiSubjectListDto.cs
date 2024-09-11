using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos.Api
{
    public class ApiSubjectListDto
    {
        public int Id { get; set; }
        public string SubjectName { get; set; }
        public int ClassLevelId { get; set; }
    }
}