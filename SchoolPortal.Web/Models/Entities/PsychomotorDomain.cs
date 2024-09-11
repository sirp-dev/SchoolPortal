using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class PsychomotorDomain
    {

        public PsychomotorDomain()
        {
            Drawing = "Good";
            Painting = "Good";
            Handwriting = "Good";
            Hobbies = "Good";
            Speech  = "Good";
            Sports = "Good";
            Club = "Good";
        }
        public int Id { get; set; }
        public string Drawing { get; set; }
        public string Club { get; set; }
        public string Painting { get; set; }
        public string Handwriting { get; set; }
        public string Hobbies { get; set; }
        public string Speech { get; set; }
        public string Sports { get; set; }
        public int EnrolmentId { get; set; }
    }
}
