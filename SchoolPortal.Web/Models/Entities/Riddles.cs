using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class Riddles
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string UserAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime? DateAnswered { get; set; }
        public int TimeCountDown { get; set; }
    }
}