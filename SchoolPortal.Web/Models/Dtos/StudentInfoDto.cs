using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos
{
    public class StudentInfoDto
    {
        public int Id { get; set; }
        public string userid { get; set; }
        public string Disability { get; set; }

        [Display(Name = "Emergency Contact")]
        public string EmergencyContact { get; set; }
        [Display(Name = "Student Reg Number")]
        public string StudentRegNumber { get; set; }

        [Display(Name = "About Me")]
        public string AboutMe { get; set; }
        [Display(Name = "Favourite Food")]
        public string FavouriteFood { get; set; }
        [Display(Name = "Books You Like")]
        public string BooksYouLike { get; set; }
        [Display(Name = "Movies You Like")]
        public string MoviesYouLike { get; set; }
        [Display(Name = "TV Shows You Like")]
        public string TVShowsYouLike { get; set; }
        [Display(Name = "Your Likes")]
        public string YourLikes { get; set; }
        [Display(Name = "Your DisLikes")]
        public string YourDisLikes { get; set; }
        [Display(Name = "Favourite Colour")]
        public string FavouriteColour { get; set; }

        [Display(Name = "Last Primary School Attended")]
        public string LastPrimarySchoolAttended { get; set; }
        [Display(Name = "Parent Guardian Name")]
        public string ParentGuardianName { get; set; }
        [Display(Name = "Parent Guardian Address")]
        public string ParentGuardianAddress { get; set; }
        [Display(Name = "Parent Guardian Phone Number")]
        public string ParentGuardianPhoneNumber { get; set; }
        [Display(Name = "Parent Guardian Occupation")]
        public string ParentGuardianOccupation { get; set; }
        [Display(Name = "Full Name")]
        public string Fullname { get; set; }
        [Display(Name = "Surname")]
        public string Surname { get; set; }
        [Display(Name = "First Name")]
        public string Firstname { get; set; }
        [Display(Name = "Other Name")]
        public string Othername { get; set; }

        public string Email { get; set; }
        
        public string Username { get; set; }
        [Display(Name = "Date Of Birth")]
        public DateTime? DateOfBirth { get; set; }
        [Display(Name = "Date Registered")]
        public DateTime DateRegistered { get; set; }
        public EntityStatus Status { get; set; }
        [Display(Name = "Data Status Changed")]
        public DateTime? DataStatusChanged { get; set; }

        public string Religion { get; set; }
        public string Gender { get; set; }
        [Display(Name = "Contact Address")]
        public string ContactAddress { get; set; }
        public string City { get; set; }

        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Display(Name = "Local Government Area")]
        public string LocalGov { get; set; }

        [Display(Name = "State Of Origin")]
        public string StateOfOrigin { get; set; }

        public string Nationality { get; set; }
        [Display(Name = "Registered By")]
        public string RegisteredBy { get; set; }
        public int ImageId { get; set; }
        public byte[] Image { get; set; }
    }
}