using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos
{
    public class ClassStudentsDto
    {
        public int ProfileId { get; set; }
        public int? ClassId { get; set; }
        public string UserName { get; set; }        
        public string FullName { get; set; }
        public string StudentRegNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public int EnrollmentId { get; set; }
        public string UserId { get; set; }
        public string Status { get;set;}
        public string Regnumber { get;set;}
        public bool IsSelected { get; set; }
    }

    public class ItemModelDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }

    public class OutComeDto
    {
        public int Id { get; set; }
        public string Result { get; set; }
        public bool Success { get; set; }
    }
    public class PromotionStudentsDto
    {
        public int ProfileId { get; set; }
        public int? ClassId { get; set; }
        public string UserName { get; set; }
        public string ClassName { get; set; }
        public string FullName { get; set; }
        public string StudentRegNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public int EnrollmentId { get; set; }
        public string UserId { get; set; }
    }
}