using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class LocalGovs
    {
       
        public int Id { get; set; }
        public string LGAName { get; set; }

        public int? StatesId { get; set; }
        public States States { get; set; }

       // public string StateCode { get; set; }


        public static List<LocalGovs> GetLgas()
        {
            var db = new ApplicationDbContext();
            return db.LocalGovs.ToList();

        }
    }
}