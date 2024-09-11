using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class SettingsValue
    {
        public int Id { get; set; }
        public int ValueData { get; set; }
        public string Description { get; set; }
    }
}