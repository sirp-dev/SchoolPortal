using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Models.Entities
{
    public class Syllable
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [AllowHtml]
        public string Content { get; set; }
        public int? SessionId { get; set; }
        public Session Session { get; set; }
        public DateTime DateAdded { get; set; }

       
    }
}