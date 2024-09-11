using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos
{
    public class DefaulterDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string Fullname { get; set; }
    }
}