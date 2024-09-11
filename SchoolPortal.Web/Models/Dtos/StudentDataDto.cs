using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos
{
    public class StudentDataDto
    {
        public int Id { get; set; }

        public string RegistrationNumber { get; set; }

        [Display(Name = "Surname")]
        [Required(ErrorMessage = "Surname field is required")]
        public string Surname { get; set; }


        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name field is required")]
        public string FirstName { get; set; }

        [Display(Name = "Other Name")]
        public string OtherNames { get; set; }

        [Display(Name = "Email Address ")]
        public string EmailAddress { get; set; }


        [Display(Name = "Date of Birth")]
        [Required(ErrorMessage = "Date of Birth field is required")]
        public DateTime? DateOfBirth { get; set; }


        [Display(Name = "Last Primary School")]
        [Required(ErrorMessage = "Last Primary School field is required")]
        public string LastPrimarySchoolAttended { get; set; }


        [Required(ErrorMessage = "Religion field is required")]
        public string Religion { get; set; }


        [Display(Name = "Name of Parents/Guardian")]
        [Required(ErrorMessage = "Name of Parents/Guardian field is required")]
        public string NameOfParents { get; set; }

        [Display(Name = "Address of Parent/Guardian")]
        [Required(ErrorMessage = "Address of Parent/Guardian field is required")]
        public string ParentsAddress { get; set; }

        [Display(Name = "Phone Number of Parent/Guardian")]
        [Required(ErrorMessage = "Phone Number of Parent/Guardian field is required")]
        public string PhoneNumberOfParents { get; set; }

        [Display(Name = "Occupation of Parent/Guardian")]
        [Required(ErrorMessage = "Occupation of Parent/Guardian field is required")]
        public string OccupationOfParents { get; set; }


        [Display(Name = "Permanent Address")]
        [Required(ErrorMessage = "Permanent Address field is required")]
        public string PermanentHomeAddress { get; set; }

        [Display(Name = "Local Government of Origin")]
        //[Required(ErrorMessage = "Local Government of Origin field is required")]
        public string LocalGov { get; set; }

        [Display(Name = "State of Origin")]
        //[Required(ErrorMessage = "State of Origin field is required")]
        public string StateOfOrigin { get; set; }

        [Required(ErrorMessage = "Nationality of Origin field is required")]
        public string Nationality { get; set; }

        [Display(Name = "Examination Centre")]
        ////[Required(ErrorMessage = "Examination Centre field is required")]
        public string ExamCenter { get; set; }

        [Display(Name = "State Disability type if any")]
        public string Disability { get; set; }

        [Display(Name = "Person to contact in case of emergency (Name,Address & Phone Number)")]
        [Required(ErrorMessage = "Provide person to contact in case of emergency.")]
        public string EmergencyContact { get; set; }

        [Display(Name = "Exam Score")]
        public decimal? ExamScore { get; set; }


        public string Remarks { get; set; }

        [Display(Name = "Admission Number")]
        public string AdmissionNumber { get; set; }

        public AdmissionStatus Status { get; set; }

        [Display(Name = "Date of Admission")]
        public DateTime? DateOfAdmission { get; set; }

        [Display(Name = "Admission Officer")]
        public string AdmissionOfficer { get; set; }

        [Display(Name = "Application Date")]
        public DateTime? ApplicationDate { get; set; }

        public int ImageId { get; set; }

        public byte[] Image { get; set; }
    }
}