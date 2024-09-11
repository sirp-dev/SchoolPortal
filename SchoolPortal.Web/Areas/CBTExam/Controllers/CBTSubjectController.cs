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
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using SchoolPortal.Web.Models.Dtos.Api;
using SchoolPortal.Web.Areas.Service;

namespace SchoolPortal.Web.Areas.CBTExam.Controllers
{
    public class CBTSubjectController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private HttpClient client = new HttpClient();

        public CBTSubjectController()
        {
            //client.BaseAddress = new Uri("http://localhost:58920/");
            //client.BaseAddress = new Uri("http://cbttest.iskools.com/");
            //client.BaseAddress = new Uri("http://cbt.iskools.com/");
            var baseUrl = db.Settings.FirstOrDefault().CBTLink;
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public CBTSubjectController(ApplicationSignInManager signInManager, ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            SignInManager = signInManager;


        }


        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }


        // GET: CBTExam/CBTQuestion
        public async Task<ActionResult> Index(string unixconverify, string xgink, string role)
        {
            HttpResponseMessage response = client.GetAsync("/api/ExamSubjectApi/GetAllSubject?unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            var response2 = response.Content.ReadAsStringAsync().Result;
            List<CBTSubjectDto> data = JsonConvert.DeserializeObject<List<CBTSubjectDto>>(response2);
            ViewBag.data = data;
            //List<SubjectModel> data = response.Content.ReadAsAsync<List<SubjectModel>>().Result;
            return View(data);

        }


        public async Task<ActionResult> Details(int? id, string unixconverify, string xgink, string role)
        {
            ViewBag.xgink = xgink;
            ViewBag.unixconverify = unixconverify;
            ViewBag.role = role;

            HttpResponseMessage response = client.GetAsync("/api/ExamSubjectApi/GetSubjectById?id=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            CBTSubjectDto data = response.Content.ReadAsAsync<CBTSubjectDto>().Result;
            return View(data);


        }

        public async Task<ActionResult> Create(string unixconverify, string xgink, string role, int? qId)
        {
            ViewBag.xgink = xgink;
            ViewBag.unixconverify = unixconverify;
            ViewBag.role = role;

            HttpResponseMessage response = client.GetAsync("/api/ExamClassApi/ClassList?unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            List<ClassModel> data = response.Content.ReadAsAsync<List<ClassModel>>().Result;
            ViewBag.classId = new SelectList(data.OrderBy(x => x.Name), "Id", "Name");
            //ViewBag.data = ViewBag.classId.Id;
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Create(SubjectModel subject, string unixconverify, string xgink, string role, int? qId, int? classId)
        {
            if (ModelState.IsValid)
            {
                classId = subject.ClassModelId;

                HttpResponseMessage response = client.PostAsJsonAsync("/api/ExamSubjectApi/AddSubject?unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role + "&classId=" + classId, subject).Result;

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Result = "Data Is Successfully Saved!";
                    return RedirectToAction("Index", "CBTSubject", new { unixconverify = unixconverify, xgink = xgink, role = role });
                }


            }
            else
            {
                ViewBag.Result = "Error! Please try with valid data.";
                return View(subject);
            }
            //HttpResponseMessage response2 = client.GetAsync("api/ExamClassApi/ClassList?unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            //List<ClassModel> data = response2.Content.ReadAsAsync<List<ClassModel>>().Result;
            //ViewBag.classId = new SelectList(data.OrderBy(x => x.Name), "Id", "Name", subject.ClassModelId);

            return View(subject);
        }



        [HttpGet]
        public async Task<ActionResult> Edit(int? id, string unixconverify, string xgink, string role)
        {
            ViewBag.xgink = xgink;
            ViewBag.unixconverify = unixconverify;
            ViewBag.role = role;

            HttpResponseMessage response = client.GetAsync("/api/ExamSubjectApi/GetSubjectById?id=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            SubjectModel data = response.Content.ReadAsAsync<SubjectModel>().Result;
            ViewBag.data = data;
            return View(data);


        }


        [HttpPost]
        public async Task<ActionResult> Edit(SubjectModel obj, int? id, string unixconverify, string xgink, string role)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = client.PutAsJsonAsync("/api/ExamSubjectApi/EditSubject?id=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role, obj).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Subject modified successfully!";
                    return RedirectToAction("Index", "CBTSubject", new { unixconverify = unixconverify, xgink = xgink, role = role });

                }

            }
            else
            {
                return View(obj);
            }

            return View(obj);

        }


        public async Task<ActionResult> Delete(int? id, string unixconverify, string xgink, string role)
        {
            ViewBag.xgink = xgink;
            ViewBag.unixconverify = unixconverify;
            ViewBag.role = role;
            ViewBag.Id = id;

            HttpResponseMessage response = client.GetAsync("/api/ExamSubjectApi/GetSubjectById?id=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            CBTSubjectDto data = response.Content.ReadAsAsync<CBTSubjectDto>().Result;
            return View(data);

        }

        [HttpPost]
        public async Task<ActionResult> Remove(int? id, string unixconverify, string xgink, string role)
        {
            //ViewBag.xgink = xgink;
            //ViewBag.unixconverify = unixconverify;
            //ViewBag.role = role;

            HttpResponseMessage response = client.DeleteAsync("/api/ExamSubjectApi/DeleteSubject?id=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Question deleted successfully!";
                return RedirectToAction("Index", "CBTSubject", new { unixconverify = unixconverify, xgink = xgink, role = role });
            }

            return View();
        }


        public JsonResult SubjectList(int id)
        {
            string unixconverify = User.Identity.Name;
            string xgink = GeneralService.PortalLink();
            string role = "superadmin";


            HttpResponseMessage response = client.GetAsync("/api/ExamSubjectApi/SubjectListByClassId?classId=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            var response2 = response.Content.ReadAsStringAsync().Result;
            List<SubjectModel> data = JsonConvert.DeserializeObject<List<SubjectModel>>(response2);
            //var stateId = db.States.FirstOrDefault(x => x.StateName == Id).Id;
            //var local = from s in db.LocalGovs
            //            where s.StatesId == stateId
            //            select s;

            return Json(new SelectList(data.ToArray(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

}
