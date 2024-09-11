using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Areas.Service;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Dtos.Api;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Areas.CBTExam.Controllers
{
    public class CBTExamBoardController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private HttpClient client = new HttpClient();
        private ISessionService _sessionService = new SessionService();
        private IResultService _resultService = new ResultService();

        public CBTExamBoardController()
        { 
            //client.BaseAddress = new Uri("http://localhost:58920/");
            //client.BaseAddress = new Uri("http://cbttest.iskools.com/");
            //client.BaseAddress = new Uri("http://cbt.iskools.com/");
            var baseUrl = db.Settings.FirstOrDefault().CBTLink;
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public CBTExamBoardController(ResultService resultService, SessionService sessionService, ApplicationSignInManager signInManager, ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            SignInManager = signInManager;
            _sessionService = sessionService;
            _resultService = resultService;


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

        // GET: CBTExam/CBTExamBoard
        public async Task<ActionResult> Index(string unixconverify, string xgink, string role)
        {
            HttpResponseMessage response = client.GetAsync("/api/ExamBoardApi/GetScheduleExam?unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            var response2 = response.Content.ReadAsStringAsync().Result;
            List<CBTSettingModelDto> data = JsonConvert.DeserializeObject<List<CBTSettingModelDto>>(response2);
            ViewBag.data = data;
            //List<SubjectModel> data = response.Content.ReadAsAsync<List<SubjectModel>>().Result;
            return View(data);

        }


        public async Task<ActionResult> Students(int? settingId, string unixconverify, string xgink, string role)
        {
            ViewBag.xgink = xgink;
            ViewBag.unixconverify = unixconverify;
            ViewBag.role = role;

            HttpResponseMessage response = client.GetAsync("/api/ExamBoardApi/GetStudentExamBySettingId?settingId=" + settingId + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            var response2 = response.Content.ReadAsStringAsync().Result;
            List<CBTExaminationDto> data = JsonConvert.DeserializeObject<List<CBTExaminationDto>>(response2);
            ViewBag.data = data;
            return View(data);
        }

        public async Task<ActionResult> StudentScore(int? examId, string unixconverify, string xgink, string role)
        {
            ViewBag.xgink = xgink;
            ViewBag.unixconverify = unixconverify;
            ViewBag.role = role;

            HttpResponseMessage response = client.GetAsync("/api/ExamBoardApi/GetStudentExamScore?examId=" + examId + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            var response2 = response.Content.ReadAsStringAsync().Result;
            List<CBTExaminationSubjectDto> data = JsonConvert.DeserializeObject<List<CBTExaminationSubjectDto>>(response2);
            ViewBag.data = data;
            return View(data);
        }

     
        public async Task<ActionResult> Create(string unixconverify, string xgink, string role, string className)
        {
            ViewBag.xgink = xgink;
            ViewBag.unixconverify = unixconverify;
            ViewBag.role = role;

            var session = await _sessionService.GetCurrentSession();
            ViewBag.Session = session;

            var term = await _sessionService.GetCurrentSessionTerm();
            ViewBag.Term = term;

            HttpResponseMessage response = client.GetAsync("/api/ExamClassApi/ClassList?unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            List<ClassModel> data = response.Content.ReadAsAsync<List<ClassModel>>().Result;
            ViewBag.className = new SelectList(data.OrderBy(x => x.Name), "Name", "Name");
            //ViewBag.data = ViewBag.classId.Id;
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Create(SettingModel exam, string unixconverify, string xgink, string role, string className, int? settingId)
        {
            if (ModelState.IsValid)
            {
                className = exam.ClassName;

                HttpResponseMessage response = client.PostAsJsonAsync("/api/ExamBoardApi/ExamSetting?unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role + "&className=" + className, exam).Result;

                if (response.IsSuccessStatusCode)
                {
                    var QuesResponse2 = response.Content.ReadAsStringAsync().Result;
                    SettingModel data1 = JsonConvert.DeserializeObject<SettingModel>(QuesResponse2);
                    ViewBag.Result = "Data Is Successfully Saved!";
                    return RedirectToAction("SubjectSetting", "CBTExamBoard", new { unixconverify = unixconverify, xgink = xgink, role = role, settingId = data1.Id });
                }


            }
            else
            {
                ViewBag.Result = "Error! Please try with valid data.";
                return View(exam);
            }
            //HttpResponseMessage response2 = client.GetAsync("api/ExamClassApi/ClassList?unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            //List<ClassModel> data = response2.Content.ReadAsAsync<List<ClassModel>>().Result;
            //ViewBag.classId = new SelectList(data.OrderBy(x => x.Name), "Id", "Name", subject.ClassModelId);

            return View(exam);
        }



        public async Task<ActionResult> SubjectSetting(string unixconverify, string xgink, string role, int? settingId)
        {
            ViewBag.xgink = xgink;
            ViewBag.unixconverify = unixconverify;
            ViewBag.role = role;
            ViewBag.settingId = settingId;


            //Get the Exam Setting By Id
            HttpResponseMessage response = client.GetAsync("/api/ExamBoardApi/GetSettingById?id=" + settingId + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;

            var response2 = response.Content.ReadAsStringAsync().Result;
            SettingModel data = JsonConvert.DeserializeObject<SettingModel>(response2);

            //Get Class Subject by Class Name

            HttpResponseMessage response5 = client.GetAsync("/api/ExamBoardApi/GetExamSubjectWithSettingId?settingId=" + settingId + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;

            var response6 = response5.Content.ReadAsStringAsync().Result;
            List<CBTExaminationSubjectDto> data3 = JsonConvert.DeserializeObject<List<CBTExaminationSubjectDto>>(response6);

            ViewBag.AddedSubject = data3;


            HttpResponseMessage response3 = client.GetAsync("/api/ExamSubjectApi/SubjectListByClassName?className=" + data.ClassName + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role + "&settingId=" + settingId).Result;

            var response4 = response3.Content.ReadAsStringAsync().Result;
            List<CBTSubjectDto> data2 = JsonConvert.DeserializeObject<List<CBTSubjectDto>>(response4);
            ViewBag.subject = new SelectList(data2, "Id", "Name");

            return View();


        }

        [HttpPost]
        public async Task<ActionResult> SubjectSetting(ExaminationSubject exam, string unixconverify, string xgink, string role, int? settingId, int? subId)
        {
            if (ModelState.IsValid)
            {

                HttpResponseMessage response = client.PostAsJsonAsync("/api/ExamBoardApi/SubjectSetting?unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role + "&subId=" + subId + "&settingId=" + settingId, exam).Result;

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Result = "Data Is Successfully Saved!";
                    var response1 = response.Content.ReadAsStringAsync().Result;
                    SettingModel data = JsonConvert.DeserializeObject<SettingModel>(response1);

                    HttpResponseMessage response2 = client.GetAsync("/api/ExamSubjectApi/SubjectListByClassName?className=" + data.ClassName + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role + "&settingId=" + settingId).Result;
                    var response3 = response2.Content.ReadAsStringAsync().Result;
                    List<CBTSubjectDto> data2 = JsonConvert.DeserializeObject<List<CBTSubjectDto>>(response3);
                    ViewBag.subject = new SelectList(data2, "Id", "Name");

                    return RedirectToAction("SubjectSetting", "CBTExamBoard", new { unixconverify = unixconverify, xgink = xgink, role = role, settingId = settingId });
                }


            }
            else
            {
                ViewBag.Result = "Error! Please try with valid data.";
                return View(exam);
            }


            return View(exam);
        }

        [HttpPost]
        public async Task<ActionResult> RemoveSubject(int? id, int? settingId, string unixconverify, string xgink, string role)
        {
            ViewBag.xgink = xgink;
            ViewBag.unixconverify = unixconverify;
            ViewBag.role = role;
            ViewBag.settingId = settingId;
            ViewBag.id = id;

            HttpResponseMessage response = client.DeleteAsync("/api/ExamBoardApiApi/DeleteSubject?id=" + id + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Subject deleted successfully!";
                return RedirectToAction("SubjectSetting", "CBTExamBoard", new { unixconverify = unixconverify, xgink = xgink, role = role, settingId = settingId, });
            }

            return View();
        }

        public async Task<ActionResult> DeleteExam(int? settingId, string unixconverify, string xgink, string role)
        {
            unixconverify = User.Identity.Name;
            xgink = GeneralService.PortalLink();
            role = "admin";
            ViewBag.settingId = settingId;

            HttpResponseMessage response = client.DeleteAsync("/api/ExamBoardApi/DeleteExam?settingId=" + settingId + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Exam deleted successfully!";
                return RedirectToAction("Index", "CBTExamBoard", new { unixconverify = unixconverify, xgink = xgink, role = role });
            }

            return View();
        }


       
    }
}