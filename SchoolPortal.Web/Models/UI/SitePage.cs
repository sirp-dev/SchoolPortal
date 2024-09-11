using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Models.UI
{
    public class SitePage
    {
        public int Id { get; set; }
        [AllowHtml]
        public string Content { get; set; }
        public string PageLink { get; set; }
        public string Title { get; set; }
        public string TitleLink { get; set; }
        public bool Show { get; set; }
        public bool MainPage { get; set; }
        public bool SubPage { get; set; }
        public bool IsNews { get; set; }
        [AllowHtml]
        public string PreviewContent { get; set; }
        public bool FooterPage { get; set; }
        public int SortOrder { get; set; }

        public  int SitePageCategoryId { get; set; }
        public SitePageCategory SitePageCategory { get; set; }
    }
}