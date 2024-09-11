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
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Areas.Data.IServices;

namespace SchoolPortal.Web.Areas.CBTExam.Controllers
{
    public class CBTQuestionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ISessionService _sessionService = new SessionService();
        private IClassLevelService _classService = new ClassLevelService();
        private HttpClient client = new HttpClient();

        public CBTQuestionController()
        {
            //client.BaseAddress = new Uri("http://localhost:58920/");
            //client.BaseAddress = new Uri("http://cbttest.iskools.com/");
            //client.BaseAddress = new Uri("http://cbt.iskools.com/");
            var baseUrl = db.Settings.FirstOrDefault().CBTLink;
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public CBTQuestionController(ApplicationSignInManager signInManager, ApplicationUserManager userManager, ApplicationRoleManager roleManager, SessionService sessionService, ClassLevelService classService)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            RoleManager = roleManager;
            SignInManager = signInManager;
            _sessionService = sessionService;
            _classService = classService;


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
            HttpResponseMessage response = client.GetAsync("/api/ExamQuestionApi/GetAllQuestion?unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            var response2 = response.Content.ReadAsStringAsync().Result;
            List<QuestionModel> data = JsonConvert.DeserializeObject<List<QuestionModel>>(response2);
            //List<QuestionModel> data = response.Content.ReadAsAsync<List<QuestionModel>>().Result;
            ViewBag.data = data;
            //foreach(var i in ViewBag.question)
            //{
            //    ViewBag.sub = i.Subject;
            //}


            return View(data);

        }


        public async Task<ActionResult> Details(int? id, string unixconverify, string xgink, string role)
        {
            ViewBag.xgink = xgink;
            ViewBag.unixconverify = unixconverify;
            ViewBag.role = role;

            HttpResponseMessage response = client.GetAsync("/api/ExamQuestionApi/GetQuestionById?id=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            QuestionModel data = response.Content.ReadAsAsync<QuestionModel>().Result;
            ViewBag.Detail = data;



            HttpResponseMessage response2 = client.GetAsync("/api/ExamOptionApi/GetQuestionOptionById?qId=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            var response3 = response2.Content.ReadAsStringAsync().Result;
            List<OptionModel> data2 = JsonConvert.DeserializeObject<List<OptionModel>>(response3);
            ViewBag.data2 = data2;

            return View(data);

           
        }

        public async Task<ActionResult> Create(string unixconverify, string xgink, string role, int? qId, int? classId)
        {
            ViewBag.xgink = xgink;
            ViewBag.unixconverify = unixconverify;
            ViewBag.role = role;
            ViewBag.classi = classId;

            var session = await _sessionService.GetCurrentSession();
            ViewBag.Session = session;

            var term = await _sessionService.GetCurrentSessionTerm();
            ViewBag.Term = term;

            HttpResponseMessage response = client.GetAsync("/api/ExamSubjectApi/SubjectList?unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            List<SubjectModel> data = response.Content.ReadAsAsync<List<SubjectModel>>().Result;
            ViewBag.subject = new SelectList(data.OrderBy(x => x.Name), "Id", "Name");

            //var classi = await _classService.ClassLevelList();
            //ViewBag.classi = new SelectList(classi.OrderBy(x => x.ClassLevelName), "ClassLevelName", "ClassLevelName");
            
            HttpResponseMessage response2 = client.GetAsync("/api/ExamClassApi/ClassList?unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            List<ClassModel> data2 = response2.Content.ReadAsAsync<List<ClassModel>>().Result;
            ViewBag.classi = new SelectList(data2.OrderBy(x => x.Name), "Id", "Name");

            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Create(QuestionModel quest, string unixconverify, string xgink, string role, int? qId, string classId)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = client.PostAsJsonAsync("/api/ExamQuestionApi/AddQuestion?unixconverify=" + unixconverify +"&xgink="+ xgink + "&role=" + role +"&classId=" + quest.ClassName, quest).Result;

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Result = "Data Is Successfully Saved!";

                    var QuesResponse2 = response.Content.ReadAsStringAsync().Result;
                    QuestionModel data1 = JsonConvert.DeserializeObject<QuestionModel>(QuesResponse2);

                    //HttpResponseMessage response2 = client.GetAsync("api/ExamQuestionApi/GetCurrentQuestionById?qId=" + data1.Id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
                    //var QuesResponse = response2.Content.ReadAsStringAsync().Result;

                    //QuestionModel data = JsonConvert.DeserializeObject<QuestionModel>(QuesResponse);
                    ViewBag.qId = data1.Id;

                    return RedirectToAction("Create", "CBTOption", new { qId = ViewBag.qId, unixconverify = unixconverify, xgink = xgink, role = role });
                }


            }
            else
            {
                TempData["error"] = "Error! Please try with valid data.";
                return RedirectToAction("Create", "CBTQuestion", new {unixconverify = unixconverify, xgink = xgink, role = role });
                //return View(quest);
            }
            return View(quest);
        }



        [HttpGet]
        public async Task<ActionResult> Edit (int? id, string unixconverify, string xgink, string role)
        {
            ViewBag.xgink = xgink;
            ViewBag.unixconverify = unixconverify;
            ViewBag.role = role;

            HttpResponseMessage response = client.GetAsync("/api/ExamQuestionApi/GetQuestionById?id=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            QuestionModel data = response.Content.ReadAsAsync<QuestionModel>().Result;
            ViewBag.question = data;
            return View(data);

        }


        [HttpPost]
        public async Task<ActionResult> Edit(QuestionModel obj, int? id,string unixconverify,string xgink,string role)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = client.PutAsJsonAsync("/api/ExamQuestionApi/EditQuestion?id=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role, obj).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Question modified successfully!";
                    return RedirectToAction("Index", "CBTQuestion", new { unixconverify = unixconverify, xgink = xgink, role = role });

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

            HttpResponseMessage response = client.GetAsync("/api/ExamQuestionApi/GetQuestionById?id=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            QuestionModel data = response.Content.ReadAsAsync<QuestionModel>().Result;
            return View(data);

        }

        [HttpPost]
        public async Task<ActionResult> Remove(int? id, string unixconverify, string xgink, string role)
        {
            //ViewBag.xgink = xgink;
            //ViewBag.unixconverify = unixconverify;
            //ViewBag.role = role;

            HttpResponseMessage response = client.DeleteAsync("/api/ExamQuestionApi/DeleteQuestion?id=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Question deleted successfully!";
                return RedirectToAction("Index", "CBTQuestion", new { unixconverify = unixconverify, xgink = xgink, role = role });
            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> RemoveOption(int? id,int? qId, string unixconverify, string xgink, string role)
        {
            //ViewBag.xgink = xgink;
            //ViewBag.unixconverify = unixconverify;
            //ViewBag.role = role;
            ViewBag.qId = qId;
            HttpResponseMessage response = client.DeleteAsync("/api/ExamQuestionApi/DeleteQuestion?id=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Question deleted successfully!";
                return RedirectToAction("Index", "CBTQuestion", new { qId = ViewBag.qId,unixconverify = unixconverify, xgink = xgink, role = role });
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
