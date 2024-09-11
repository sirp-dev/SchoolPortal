using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos
{
    public class StaffDetailsDto
    {
        public int Id { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string OtherName { get; set; }
        public string Gender { get; set; }
       
        public DateTime? DateOfBirth { get; set; }

        public string Religion { get; set; }

        public string PermanentHomeAddress { get; set; }
        public string ContactAddress { get; set; }
        public string City { get; set; }

        public string LocalGov { get; set; }

        public string StateOfOrigin { get; set; }

        public string Nationality { get; set; }

        public string Disability { get; set; }

        public string EmergencyContact { get; set; }

        public string Phonenumber { get; set; }

        public string AboutMe { get; set; }

        public string FavouriteFood { get; set; }

        public string BooksYouLike { get; set; }

        public string MoviesYouLike { get; set; }

        public string TVShowsYouLike { get; set; }

        public string YourLikes { get; set; }

        public string YourDisLikes { get; set; }

        public string FavouriteColour { get; set; }

        public string StaffId { get; set; }

        public DateTime DateOfAppointment { get; set; }

        public string PostHeld { get; set; }

        public int ImageId { get; set; }


        public ICollection<Qualification> Qualifications { get; set; }
    }
}