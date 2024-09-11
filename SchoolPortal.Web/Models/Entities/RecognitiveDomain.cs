using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class RecognitiveDomain
    {

        public RecognitiveDomain()
        {
              Rememberance ="Good";
         Understanding ="Good";
         Application ="Good";
         Analyzing ="Good";
         Evaluation ="Good";
         Creativity ="Good";
    }
        public int Id { get; set; }
        public string Rememberance { get;set;}
        public string Understanding { get;set;}
        public string Application { get;set;}
        public string Analyzing { get;set;}
        public string Evaluation { get;set;}
        public string Creativity { get;set;}
        public int EnrolmentId { get; set; }
    }
}