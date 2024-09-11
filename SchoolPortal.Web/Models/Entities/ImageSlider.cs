using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{
    public class ImageSlider
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        [Display(Name = "Slider Alt")]
        public string SliderAlt { get; set; }

        [Display(Name = "Text One")]
        public string TextOne { get; set; }

        [Display(Name = "Text Two")]
        public string TextTwo { get; set; }

        [Display(Name = "Text Three")]
        public string TextThree { get; set; }

        [Display(Name = "Current Slider")]
        public bool CurrentSlider { get; set; }
    }
}