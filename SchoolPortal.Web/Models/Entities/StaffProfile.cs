using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class StaffProfile
    {
        public int Id { get; set; }
      
        public ApplicationUser user { get; set; }
        public string UserId { get; set; }

        public string Disability { get; set; }

        public string EmergencyContact { get; set; }
        public string MaritalStatus { get; set; }
      


        public string AboutMe { get; set; }

        public string FavouriteFood { get; set; }

        public string BooksYouLike { get; set; }

        public string MoviesYouLike { get; set; }

        public string TVShowsYouLike { get; set; }

        public string YourLikes { get; set; }

        public string YourDisLikes { get; set; }

        public string FavouriteColour { get; set; }

        public string StaffRegistrationId { get; set; }

        public DateTime DateOfAppointment { get; set; }

        public string PostHeld { get; set; }

        public int ImageId { get; set; }


        public ICollection<Qualification> Qualifications { get; set; }
    }
}