//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Data.Entity;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;
//using System.Web.Http.Description;
//using System.Web;
//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.AspNet.Identity;
//using System.Threading.Tasks;
//using System.Text.RegularExpressions;
//using System.Text;
//using System.Web.Mvc;
//using SchoolPortal.Web.Models;
//using SchoolPortal.Web.Models.Entities;
//using SchoolPortal.Web.Models.Dtos.Api;
//using Newtonsoft.Json;
//using System.IO;
//using System.Net.Http.Headers;

//namespace SchoolPortal.Web.Controllers
//{
//    public class CBTQuestionController : Controller
//    {
//        public ApplicationDbContext db = new ApplicationDbContext();
//        private ApplicationSignInManager _signInManager;
//        string Baseurl ="http://localhost:58920";

//        public CBTQuestionController()
//        {
//        }


//        public CBTQuestionController(ApplicationSignInManager signInManager, ApplicationUserManager userManager, ApplicationRoleManager roleManager)
//        {
//            UserManager = userManager;
//            RoleManager = roleManager;
//            SignInManager = signInManager;
//        }


//        public ApplicationSignInManager SignInManager
//        {
//            get
//            {
//                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
//            }
//            private set
//            {
//                _signInManager = value;
//            }
//        }

//        private ApplicationUserManager _userManager;
//        public ApplicationUserManager UserManager
//        {
//            get
//            {
//                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
//            }
//            set
//            {
//                _userManager = value;
//            }
//        }

//        private ApplicationRoleManager _roleManager;
//        public ApplicationRoleManager RoleManager
//        {
//            get
//            {
//                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
//            }
//            private set
//            {
//                _roleManager = value;
//            }
//        }

     
//        // GET: CBTQuestion
//        public async Task<ActionResult> Index(string xgink, string unixconverify, string role)
//        {
//            //Hosted web API REST Service base url  
           
//            string starturl = "http://";
//            string schlink = starturl + xgink;
//            var quest = await db.QuestionModels.Include(x => x.SubjectModel).Where(x => x.SchoolLink == schlink).ToListAsync();
//            if (!String.IsNullOrEmpty(unixconverify))
//            {
//                var user = SignInManager.UserManager.FindByNameAsync(unixconverify);
//                if (user != null)
//                {
//                    try
//                    {


//                        var aname = User.Identity.Name;
//                        if (role == "staff" || role == "admin" || role == "superadmin" && schlink != null)
//                        {

//                            using (var client = new HttpClient())
//                            {
//                                //Passing service base url  
//                                client.BaseAddress = new Uri(Baseurl);

//                                client.DefaultRequestHeaders.Clear();
//                                //Define request data format  
//                                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

//                                //Sending request to find web api REST service resource GetAllQuestion using HttpClient  
//                                //HttpResponseMessage Res = await client.GetAsync("api/ExamQuestionApi/GetAllQuestion");
//                                HttpResponseMessage Res = await client.GetAsync("api/ExamQuestionApi/GetAllQuestion?xgink=" + xgink + "&unixconverify=" + unixconverify + "&role=" + role);

//                                //Checking the response is successful or not which is sent using HttpClient  
//                                if (Res.IsSuccessStatusCode)
//                                {
//                                    //Storing the response details recieved from web api   
//                                    var QuesResponse = Res.Content.ReadAsStringAsync().Result;

//                                    //Deserializing the response recieved from web api and storing into the Question list  
//                                    quest = JsonConvert.DeserializeObject<List<QuestionModel>>(QuesResponse);

//                                }
//                                //returning the question list to view  

//                            }
//                        }

//                    }
//                    catch (WebException webex)
//                    {

//                    }


//            }


//        }

//            return View(quest);

//        }

//        public async Task<ActionResult> AddQuestion(string xgink, string unixconverify, string role, int? qId)
//        {
//            //Hosted web API REST Service base url  

//            string starturl = "http://";
//            string schlink = starturl + xgink;
//            var question = await db.QuestionModels.Include(x => x.SubjectModel).FirstOrDefaultAsync(x => x.SchoolLink == schlink);
//            if (!String.IsNullOrEmpty(unixconverify))
//            {
//                var user = SignInManager.UserManager.FindByNameAsync(unixconverify);
//                if (user != null)
//                {
//                    try
//                    {


//                        var aname = User.Identity.Name;
//                        if (role == "staff" || role == "admin" || role == "superadmin" && schlink != null)
//                        {

//                            using (var client = new HttpClient())
//                            {
//                                //Passing service base url  
//                                client.BaseAddress = new Uri(Baseurl);

//                                client.DefaultRequestHeaders.Clear();
//                                //Define request data format  
//                                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

//                                //Sending request to find web api REST service resource GetAllQuestion using HttpClient  
//                                //HttpResponseMessage Res = await client.GetAsync("api/ExamQuestionApi/GetAllQuestion");
//                                HttpResponseMessage Res = await client.PostAsJsonAsync("api/ExamQuestionApi/AddQuestion?xgink=" + xgink + "&unixconverify=" + unixconverify + "&role=" + role, question);
//                                //Checking the response is successful or not which is sent using HttpClient  
//                                if (Res.IsSuccessStatusCode)
//                                {
//                                    //Storing the response details recieved from web api   
//                                    var QuesResponse = Res.Content.ReadAsStringAsync().Result;
                                    

//                                    //Deserializing the response recieved from web api and storing into the Question list  
//                                    question = JsonConvert.DeserializeObject<QuestionModel>(QuesResponse);

//                                }
//                                //returning the employee list to view  
//                                return RedirectToAction("AddOption", "CBTOption", new { qId = question.Id, xgink = xgink, unixconverify = unixconverify, role = role });
//                            }
//                        }

//                    }
//                    catch (WebException webex)
//                    {

//                    }


//                }


//            }

//            return View(question);

//        }

//        public async Task<ActionResult> EditQuestion(string xgink, string unixconverify, string role, int? id)
//        {
//            //Hosted web API REST Service base url  

//            string starturl = "http://";
//            string schlink = starturl + xgink;
//            var question = await db.QuestionModels.Include(x => x.SubjectModel).FirstOrDefaultAsync(x => x.SchoolLink == schlink);
//            if (!String.IsNullOrEmpty(unixconverify))
//            {
//                var user = SignInManager.UserManager.FindByNameAsync(unixconverify);
//                if (user != null)
//                {
//                    try
//                    {


//                        var aname = User.Identity.Name;
//                        if (role == "staff" || role == "admin" || role == "superadmin" && schlink != null)
//                        {

//                            using (var client = new HttpClient())
//                            {
//                                //Passing service base url  
//                                client.BaseAddress = new Uri(Baseurl);

//                                client.DefaultRequestHeaders.Clear();
//                                //Define request data format  
//                                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

//                                //Sending request to find web api REST service resource GetAllQuestion using HttpClient  
//                                //HttpResponseMessage Res = await client.GetAsync("api/ExamQuestionApi/GetAllQuestion");
//                                HttpResponseMessage Res = await client.PutAsJsonAsync("api/ExamQuestionApi/EditQuestion?id="+id+"xgink=" + xgink + "&unixconverify=" + unixconverify + "&role=" + role, question);
//                                //Checking the response is successful or not which is sent using HttpClient  
//                                if (Res.IsSuccessStatusCode)
//                                {
//                                    //Storing the response details recieved from web api   
//                                    var QuesResponse = Res.Content.ReadAsStringAsync().Result;


//                                    //Deserializing the response recieved from web api and storing into the Question list  
//                                    question = JsonConvert.DeserializeObject<QuestionModel>(QuesResponse);

//                                }
//                                //returning the question list to view  
//                            }
//                        }

//                    }
//                    catch (WebException webex)
//                    {

//                    }


//                }


//            }

//            return View(question);

//        }

//        public async Task<ActionResult> DeleteQuestion(string xgink, string unixconverify, string role, int? id)
//        {
//            //Hosted web API REST Service base url  

//            string starturl = "http://";
//            string schlink = starturl + xgink;
//            var question = await db.QuestionModels.Include(x => x.SubjectModel).FirstOrDefaultAsync(x => x.Id == id);
//            if (!String.IsNullOrEmpty(unixconverify))
//            {
//                var user = SignInManager.UserManager.FindByNameAsync(unixconverify);
//                if (user != null)
//                {
//                    try
//                    {


//                        var aname = User.Identity.Name;
//                        if (role == "staff" || role == "admin" || role == "superadmin" && schlink != null)
//                        {

//                            using (var client = new HttpClient())
//                            {
//                                //Passing service base url  
//                                client.BaseAddress = new Uri(Baseurl);

//                                client.DefaultRequestHeaders.Clear();
//                                //Define request data format  
//                                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

//                                //Sending request to find web api REST service resource GetAllQuestion using HttpClient  
//                                //HttpResponseMessage Res = await client.GetAsync("api/ExamQuestionApi/GetAllQuestion");
//                                HttpResponseMessage Res = await client.DeleteAsync("api/ExamQuestionApi/DeleteQuestion?id=" + id + "xgink=" + xgink + "&unixconverify=" + unixconverify + "&role=" + role);
//                                //Checking the response is successful or not which is sent using HttpClient  
//                                if (Res.IsSuccessStatusCode)
//                                {
//                                    //Storing the response details recieved from web api   
//                                    var QuesResponse = Res.Content.ReadAsStringAsync().Result;


//                                    //Deserializing the response recieved from web api and storing into the Question list  
//                                    question = JsonConvert.DeserializeObject<QuestionModel>(QuesResponse);

//                                }
//                                //returning the question list to view  
//                            }
//                        }

//                    }
//                    catch (WebException webex)
//                    {

//                    }


//                }


//            }

//            return View(question);

//        }
//    }
//}
    
