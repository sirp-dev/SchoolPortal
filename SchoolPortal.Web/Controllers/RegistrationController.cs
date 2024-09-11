using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Entities;

namespace SchoolPortal.Web.Controllers
{
    public class RegistrationController : Controller
    {
            private ApplicationDbContext db = new ApplicationDbContext();
           
            // GET: Registration
            public ActionResult Index()
        {
            var sett = db.Settings.FirstOrDefault();
            if (sett.AdmissionPinOption == AdmissionPinOption.UsedPin)
            {
                return RedirectToAction("Index","Apply",new { area= "Admission"});
            }
            else if (sett.AdmissionPinOption == AdmissionPinOption.NoPin)
            {
                return RedirectToAction("Index2", "Apply", new { area = "Admission" });


            }
            return View();
        }

    }
}