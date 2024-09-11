using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class StudentProfile
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser user { get; set; }
        public string Disability { get; set; }

        public string EmergencyContact { get; set; }
        public string StudentRegNumber { get; set; }


        public string AboutMe { get; set; }

        public string FavouriteFood { get; set; }

        public string BooksYouLike { get; set; }

        public string MoviesYouLike { get; set; }

        public string TVShowsYouLike { get; set; }

        public string YourLikes { get; set; }

        public string YourDisLikes { get; set; }

        public string FavouriteColour { get; set; }
       

        public string LastPrimarySchoolAttended { get; set; }

        public string ParentGuardianName { get; set; }

        public string ParentGuardianAddress { get; set; }

        public string ParentGuardianPhoneNumber { get; set; }

        public string ParentGuardianOccupation { get; set; }

        public int ImageId { get; set; }
        public bool IsUpdated { get; set; }

        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }
        public bool Graduate { get; set; }
    }
}