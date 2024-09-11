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
    public class CBTOptionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private HttpClient client = new HttpClient();

        public CBTOptionController()
        {
            //client.BaseAddress = new Uri("http://localhost:58920/");
            //client.BaseAddress = new Uri("http://cbttest.iskools.com/");
            //client.BaseAddress = new Uri("http://cbt.iskools.com/");
            var baseUrl = db.Settings.FirstOrDefault().CBTLink;
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public CBTOptionController(ApplicationSignInManager signInManager, ApplicationUserManager userManager, ApplicationRoleManager roleManager)
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


        // GET: CBTExam/CBTOption
        public async Task<ActionResult> Index(string unixconverify, string xgink, string role)
        {
            HttpResponseMessage response = client.GetAsync("/api/ExamOptionApi/GetAllOption?unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            var response2 = response.Content.ReadAsStringAsync().Result;
            List<OptionModel> data = JsonConvert.DeserializeObject<List<OptionModel>>(response2);
            //List<OptionModel> data = response.Content.ReadAsAsync<List<OptionModel>>().Result;
            ViewBag.data = data;
            return View(data);

        }


        public async Task<ActionResult> Details(int? id, string unixconverify, string xgink, string role)
        {
            ViewBag.xgink = xgink;
            ViewBag.unixconverify = unixconverify;
            ViewBag.role = role;

            HttpResponseMessage response = client.GetAsync("/api/ExamOptionApi/GetOptionById?id=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            OptionModel data = response.Content.ReadAsAsync<OptionModel>().Result;
            ViewBag.Detail = data;
            return View(data);


        }

        public async Task<ActionResult> Create(int? qId, string unixconverify, string xgink, string role)
        {
            ViewBag.xgink = xgink;
            ViewBag.unixconverify = unixconverify;
            ViewBag.role = role;
            ViewBag.qId = qId;


            HttpResponseMessage response = client.GetAsync("/api/ExamOptionApi/GetQuestionOptionById?qId=" + qId + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            var response2 = response.Content.ReadAsStringAsync().Result;
            List<OptionModel> data = JsonConvert.DeserializeObject<List<OptionModel>>(response2);
            //List<OptionModel> data = response.Content.ReadAsAsync<List<OptionModel>>().Result;
            ViewBag.option = data;
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Create(OptionModel option,int? qId, string unixconverify, string xgink, string role)
        {
           
            if (ModelState.IsValid)
            {

                HttpResponseMessage response = client.PostAsJsonAsync("/api/ExamOptionApi/AddOption?qId="+ qId +"&unixconverify=" + unixconverify +"&xgink=" + xgink +"&role=" + role, option).Result;

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Result = "Data Is Successfully Saved!";

                    return RedirectToAction("Create", "CBTOption", new { qId = qId, unixconverify = unixconverify, xgink = xgink, role = role });
                }

            }
            else
            {
                TempData["error"] = "Error! Please try with valid data.";
                return View(option);
            }
            return View(option);
        }



        [HttpGet]
        public async Task<ActionResult> Edit(int? id, string unixconverify, string xgink, string role)
        {
            ViewBag.xgink = xgink;
            ViewBag.unixconverify = unixconverify;
            ViewBag.role = role;

            HttpResponseMessage response = client.GetAsync("/api/ExamOptionApi/GetOptionById?id=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            OptionModel data = response.Content.ReadAsAsync<OptionModel>().Result;
            ViewBag.option = data;
            return View(data);

        }


        [HttpPost]
        public async Task<ActionResult> Edit(OptionModel obj, int? id, string unixconverify, string xgink, string role)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = client.PutAsJsonAsync("/api/ExamOptionApi/EditOption?id=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role, obj).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Customer modified successfully!";
                    return RedirectToAction("Index", "CBTOption", new { unixconverify = unixconverify, xgink = xgink, role = role });

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

            HttpResponseMessage response = client.GetAsync("/api/ExamOptionApi/GetOptionById?id=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            QuestionModel data = response.Content.ReadAsAsync<QuestionModel>().Result;
            return View(data);

        }

        [HttpPost]
        public async Task<ActionResult> Remove(int? id, string unixconverify, string xgink, string role)
        {
            //ViewBag.xgink = xgink;
            //ViewBag.unixconverify = unixconverify;
            //ViewBag.role = role;

            HttpResponseMessage response = client.DeleteAsync("/api/ExamOptionApi/DeleteOption?id=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Option deleted successfully!";
                return RedirectToAction("Index", "CBTOption", new { unixconverify = unixconverify, xgink = xgink, role = role });
            }

            return View();
        }


        [HttpPost]
        public async Task<ActionResult> RemoveOption(int? id,int? qId, string unixconverify, string xgink, string role)
        {
            ViewBag.xgink = xgink;
            ViewBag.unixconverify = unixconverify;
            ViewBag.role = role;
            ViewBag.qId = qId;
            ViewBag.id = id;

            HttpResponseMessage response = client.DeleteAsync("/api/ExamOptionApi/DeleteQuestionOption?qId=" + qId + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Option deleted successfully!";
                return RedirectToAction("Create", "CBTOption", new { qId = qId, unixconverify = unixconverify, xgink = xgink, role = role });
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
