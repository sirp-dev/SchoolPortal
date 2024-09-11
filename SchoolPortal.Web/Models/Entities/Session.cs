using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.Web.Models.Entities
{
    public class Session
    {
        public int Id { get; set; }

        [Display(Name = "Term:")]
       
        public string Term { get; set; }

        [Display(Name = "School Principal:")]
        [Required(ErrorMessage = "School Principal is required")]
        public string SchoolPrincipal { get; set; }


        [Display(Name = "Session Year:")]
        [Required(ErrorMessage = "Session Year is required")]
        public string SessionYear { get; set; }

        [UIHint("Enum")]
        public SessionStatus Status { get; set; }

        [UIHint("Enum")]
        public ArchiveStatus ArchiveStatus { get; set; }
       

    }
}