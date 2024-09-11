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

namespace SchoolPortal.Web.Areas.CBTExam.Controllers
{
    public class CBTClassController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private HttpClient client = new HttpClient();

        public CBTClassController()
        {

            //client.BaseAddress = new Uri("http://localhost:58920/");
            //client.BaseAddress = new Uri("http://cbttest.iskools.com/");
            //client.BaseAddress = new Uri("http://cbt.iskools.com/");
            var baseUrl = db.Settings.FirstOrDefault().CBTLink;
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public CBTClassController(ApplicationSignInManager signInManager, ApplicationUserManager userManager, ApplicationRoleManager roleManager)
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
            HttpResponseMessage response = client.GetAsync("/api/ExamClassApi/GetAllClass?unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            var response2 = response.Content.ReadAsStringAsync().Result;
            List<ClassModel> data = JsonConvert.DeserializeObject<List<ClassModel>>(response2);
            ViewBag.data = data;
            return View(data);

        }


        public async Task<ActionResult> Details(int? id, string unixconverify, string xgink, string role)
        {
            ViewBag.xgink = xgink;
            ViewBag.unixconverify = unixconverify;
            ViewBag.role = role;

            HttpResponseMessage response = client.GetAsync("/api/ExamClassApi/GetClassById?id=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            ClassModel data = response.Content.ReadAsAsync<ClassModel>().Result;

            HttpResponseMessage response2 = client.GetAsync("/api/ExamSubjectApi/SubjectListByClassId?classId=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            var response3 = response2.Content.ReadAsStringAsync().Result;
            List<CBTSubjectDto> data2 = JsonConvert.DeserializeObject<List<CBTSubjectDto>>(response3);
            ViewBag.data = data2;


            return View(data);


        }

        public async Task<ActionResult> Create(string unixconverify, string xgink, string role)
        {
            ViewBag.xgink = xgink;
            ViewBag.unixconverify = unixconverify;
            ViewBag.role = role;

            //HttpResponseMessage response = client.GetAsync("api/ExamSubjectApi/SubjectList?unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            //List<SubjectModel> data = response.Content.ReadAsAsync<List<SubjectModel>>().Result;
            //ViewBag.schlink = new SelectList(data.OrderBy(x => x.Name), "Id", "Name");
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Create(ClassModel examClass, string unixconverify, string xgink, string role, int? qId)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = client.PostAsJsonAsync("/api/ExamClassApi/AddClass?unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role, examClass).Result;

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Result = "Data Is Successfully Saved!";
                    return RedirectToAction("Index", "CBTClass", new { unixconverify = unixconverify, xgink = xgink, role = role });
                }


            }
            else
            {
                ViewBag.Result = "Error! Please try with valid data.";
                return View(examClass);
            }
            return View(examClass);
        }



        [HttpGet]
        public async Task<ActionResult> Edit(int? id, string unixconverify, string xgink, string role)
        {
            ViewBag.xgink = xgink;
            ViewBag.unixconverify = unixconverify;
            ViewBag.role = role;

            HttpResponseMessage response = client.GetAsync("/api/ExamClassApi/GetClassById?id=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            ClassModel data = response.Content.ReadAsAsync<ClassModel>().Result;
            return View(data);
        }


        [HttpPost]
        public async Task<ActionResult> Edit(ClassModel obj, int? id, string unixconverify, string xgink, string role)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = client.PutAsJsonAsync("/api/ExamClassApi/EditClass?id=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role, obj).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Class modified successfully!";
                    return RedirectToAction("Index", "CBTClass", new { unixconverify = unixconverify, xgink = xgink, role = role });

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

            HttpResponseMessage response = client.GetAsync("/api/ExamClassApi/GetClassById?id=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            ClassModel data = response.Content.ReadAsAsync<ClassModel>().Result;
            return View(data);

        }

        [HttpPost]
        public async Task<ActionResult> Remove(int? id, string unixconverify, string xgink, string role)
        {
            //ViewBag.xgink = xgink;
            //ViewBag.unixconverify = unixconverify;
            //ViewBag.role = role;

            HttpResponseMessage response = client.DeleteAsync("/api/ExamClassApi/DeleteClass?id=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Class deleted successfully!";
                return RedirectToAction("Index", "CBTClass", new { unixconverify = unixconverify, xgink = xgink, role = role });
            }

            return View();
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
