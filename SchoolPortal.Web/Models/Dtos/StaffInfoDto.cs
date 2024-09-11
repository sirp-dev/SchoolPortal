using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos
{
    public class StaffInfoDto
    {
        public int Id { get; set; }
        public string userid { get; set; }
        public string Disability { get; set; }

        public string EmergencyContact { get; set; }
        public string MaritalStatus { get; set; }

        public string StaffRegistrationId { get; set; }

        public DateTime DateOfAppointment { get; set; }

        public string PostHeld { get; set; }


        public string AboutMe { get; set; }

        public string FavouriteFood { get; set; }

        public string BooksYouLike { get; set; }

        public string MoviesYouLike { get; set; }

        public string TVShowsYouLike { get; set; }

        public string YourLikes { get; set; }

        public string YourDisLikes { get; set; }

        public string FavouriteColour { get; set; }
        public string Fullname { get; set; }

        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Othername { get; set; }
        
        public string Email { get; set; }

        public string Username { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public DateTime DateRegistered { get; set; }
        public EntityStatus Status { get; set; }
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
        public string RegisteredBy { get; set; }
        public int ImageId { get; set; }
        public byte[] Image { get; set; }

    }
}