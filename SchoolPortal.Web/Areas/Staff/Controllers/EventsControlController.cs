using Microsoft.AspNet.Identity;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Areas.Staff.Controllers
{
    public class EventsControlController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Staff/EventsControl
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetmyEvents()
        {
            var userid = User.Identity.GetUserId();
            var events = db.Events.Where(x => x.UserId == userid || x.GeneralEvent == true).ToList();
            return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        [AllowAnonymous]
        public JsonResult LgaList(string Id)
        {
            var stateId = db.States.FirstOrDefault(x => x.StateName == Id).Id;
            var local = from s in db.LocalGovs
                        where s.StatesId == stateId
                        select s;

            return Json(new SelectList(local.ToArray(), "LGAName", "LGAName"), JsonRequestBehavior.AllowGet);
        }

        //jeson to fetch subjects by class
        public JsonResult SubjectByClass(int Id)
        {
            var uId = User.Identity.GetUserId();
            var formTeacher = db.ClassLevels.Include(x=>x.User).FirstOrDefault(x => x.UserId == uId && x.Id == Id && x.ShowClass == true);
            var classid = db.ClassLevels.FirstOrDefault(x => x.Id == Id).Id;
            var local = from s in db.Subjects
                        where s.ClassLevelId == classid
                        select s;

            if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
            {
                local = local.Where(x=>x.ShowSubject == true).OrderBy(x => x.SubjectName);
            }
            else if (formTeacher != null)
            {
                local = local.Include(x=>x.User).Where(x => x.ClassLevelId == Id && x.ShowSubject == true).OrderBy(x => x.SubjectName);
            }
            else
            {
                local = local.Include(x => x.User).Where(x => x.UserId == uId && x.ShowSubject == true).OrderBy(x => x.SubjectName);
            }
            return Json(new SelectList(local.ToArray(), "Id", "SubjectName"), JsonRequestBehavior.AllowGet);
        }

        //event
        #region
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AddEventp(string subject, string description, DateTime start, DateTime end, string color, bool general, bool fday)
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
            return RedirectToAction("Index", "Panel" ,new { area = "Staff" });
        }
        #endregion
    }
}