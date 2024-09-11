using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class DataUserRequest
    {
        public int Id { get; set; }
        [Display(Name = "Full Name")]

        public string FullName { get; set; }
        public string Email { get; set; }
        [Display(Name = "Date Of Birth")]

        public string DateOfBirth { get; set; }
        [Display(Name = "Parent Name")]

        public string ParentName { get; set; }
        [Display(Name = "Phone Number")]

        public string StudentsPhoneNumber { get; set; }
        [Display(Name = "Parents Phone Number")]

        public string ParentsPhoneNumber { get; set; }
        [Display(Name = "Parents Occupation")]

        public string ParentsOccupation { get; set; }
        [Display(Name = "Class")]

        public string ClassName { get; set; }
        [Display(Name = "Form Teacher or Class Teacher")]

        public string FormTeacher { get; set; }
        

        public DateTime Date { get; set; }
    }
}