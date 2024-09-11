using SchoolPortal.Web.Areas.Service;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Dtos.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Text;
using RestSharp;
using Newtonsoft.Json;
using SchoolPortal.Web.Models.Dtos.Zoom;

namespace SchoolPortal.Web.Controllers
{
    [RoutePrefix("api/liveclass")]
    public class LiveClassApiController : ApiController
    {
        public ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public LiveClassApiController()
        {
        }

        public LiveClassApiController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;

        }


        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        //  [HttpGet]
        //[ResponseType(typeof(ApiSchoolInfoDto))]
        //[Route("api/schoolinfo")]
        public IHttpActionResult GetUsers()
        {
            try
            {
                List<User> userdata = new List<User>();
                var contents = TokenManager.GetUsers();
                    //userdata = JsonConvert.DeserializeObject<List<ZoomUserDto>>(response.Content);
                    Rootobject datalist = JsonConvert.DeserializeObject<Rootobject>(contents);
                    userdata = datalist.users.ToList();
                   

        }
            catch(Exception c) { }
            return null;
        }


        public IHttpActionResult GetUsersMeetings()
        {
            try
            {
                var token = TokenManager.GenerateToken();
                ///v2/accounts/128273138/users/{userId}/meetings
                /////https://api.zoom.us/v2/users
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient("https://api.zoom.us/v2/users/exwhyzeetech@gmail.com/meetings"); var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "Bearer" + token);

                request.AddQueryParameter("type", "scheduled");
                request.AddQueryParameter("page_size", "30");
                request.AddQueryParameter("page_number", "1");
                IRestResponse response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    var contents = response.Content.ToString();
                    return Ok(contents);
                }

            }
            catch (Exception c) { }
            return null;
        }


    }
}
