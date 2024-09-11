using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Models.Entities
{
    public class ContentPage
    {
        public int Id { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }
        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; }
        [Display(Name = "Show In Home")]
        public bool ShowInHome { get; set; }
        [AllowHtml]
        public string ContentHome { get; set; }

        [AllowHtml]
        public string Content { get; set; }
        [Display(Name = "Category Page Id")]
        public int CategoryPageId { get; set; }
        [Display(Name = "Category Page")]
        public CategoryPage CategoryPage { get; set; }

        [Display(Name = "Publish Page")]
        public PagePublish Publish { get; set; }

        [Display(Name = "Meta Tage")]
        public string MetaTage { get; set; }
        [Display(Name = "Meta Description")]
        public string MetaDescription { get; set; }
        [Display(Name = "Meta Keyword")]
        public string MetaKeyword { get; set; }
    }
}