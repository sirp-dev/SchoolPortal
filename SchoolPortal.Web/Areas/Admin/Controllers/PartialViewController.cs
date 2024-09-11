using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Areas.Admin.Controllers
{
    public class PartialViewController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/PartialView
        public ActionResult LayoutProfile()
        {
            var item = db.Settings.FirstOrDefault();

            var img =  db.ImageModel.FirstOrDefault(x => x.Id == item.ImageId);
            var output = new SettingLayoutDto
            {
                Id = item.Id,
               SchoolName = item.SchoolName,
                SchoolInitials = item.SchoolInitials,
                
                ContactEmail = item.ContactEmail,
                Image = img.ImageContent,
               
            };

            return PartialView(output);
        }

        public ActionResult LayoutProfileTitle()
        {
            var item = db.Settings.FirstOrDefault();
            return PartialView(item);
        }

        public ActionResult LayoutSchoolName()
        {
            var item = db.Settings.FirstOrDefault().SchoolName;

            return PartialView(item);
        }

        public ActionResult SchoolIcon()
        {
            var item = db.Settings.FirstOrDefault();

            var img = db.ImageModel.FirstOrDefault(x => x.Id == item.ImageId);
            var output = new SettingLayoutDto
            {
               
                Image = img.ImageContent

            };

            return PartialView(output);
        }
    }
}