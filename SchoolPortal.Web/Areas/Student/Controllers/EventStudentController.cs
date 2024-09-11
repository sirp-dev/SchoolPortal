using Microsoft.AspNet.Identity;
using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Areas.Service;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Dtos;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Areas.Student.Controllers
{
    public class EventStudentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IResultService _resultService = new ResultService();
     

        public EventStudentController()
        {

        }
        public EventStudentController(
            ResultService resultService
           
            )
        {
         
            _resultService = resultService;
        
        }
        // GET: Student/EventStudent
        public ActionResult Index()
        {
            return View();
        }

        //event
        #region

        public JsonResult GetEvents()
        {
            var userid = User.Identity.GetUserId();
            var events = db.Events.Where(x => x.UserId == userid || x.GeneralEvent == true).ToList();
            return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AddEventpa(string subject, string description, DateTime start, DateTime end, string color, bool general, bool fday)
        {
            try
            {
                string u = User.Identity.GetUserId();
                Event m = new Event();
                m.Subject = subject;
                m.DIscription = description;
                m.Start = start;
                m.End = end;
                m.Color = color;
                m.GeneralEvent = general;
                m.IsFullDay = fday;
                m.UserId = u;
                db.Events.Add(m);
                db.SaveChanges();
                TempData["success"] = "Event Added";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Something Went Wrong. Try again.";
            }
            return RedirectToAction("Index", "Panel", new { area = "Student" });
        }
        #endregion

        public async Task<ActionResult> CheckRegNumber(CheckResultViewModelDto models)
        {
            var student = db.StudentProfiles.FirstOrDefault(x => x.StudentRegNumber.ToLower().Contains(models.StudentPIN.ToLower()));


            string fullname = GeneralService.StudentorPupil() + " not found or wrong registration number";
            var userProfile = await _resultService.GetUserByRegNumber(models.StudentPIN);
            if (userProfile != null)
            {
                string name = "Fullname: " + userProfile.user.Surname + " " + userProfile.user.FirstName + " " + userProfile.user.OtherName;
                fullname = name;
            }
            return Json(fullname, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> CheckDetails(CheckResultViewModelDto2 models)
        {
            var student = db.StudentProfiles.FirstOrDefault(x => x.Id == models.StudentProfileId);


            string fullname = GeneralService.StudentorPupil() + " not found or wrong user detail";
            var userProfile = await _resultService.GetUserByRegNumber(student.StudentRegNumber);
            if (userProfile != null)
            {
                string name = "Fullname: " + userProfile.user.Surname + " " + userProfile.user.FirstName + " " + userProfile.user.OtherName;
                fullname = name;
            }
            return Json(fullname, JsonRequestBehavior.AllowGet);
        }
    }
}