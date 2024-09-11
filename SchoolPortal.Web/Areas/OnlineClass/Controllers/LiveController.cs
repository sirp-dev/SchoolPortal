using Newtonsoft.Json;
using SchoolPortal.Web.Controllers;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Dtos.Zoom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;

namespace SchoolPortal.Web.Areas.OnlineClass.Controllers
{
    public class LiveController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: OnlineClass/Live
        public async Task<ActionResult> Index()
        {
            try
            {
                var prof = await db.OnlineZooms.Include(x => x.Session).Include(x => x.ClassLevel).Include(x => x.Subject).Include(x => x.User).ToListAsync();

                return View(prof);
            }
            catch (Exception c)
            {

            }
            return View();
        }


        public async Task<ActionResult> DetailsApi(long id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {

                List<User> userdata = new List<User>();
                var content = TokenManager.GetAMeeting(id);

                //userdata = JsonConvert.DeserializeObject<List<ZoomUserDto>>(response.Content);
                RootobjectDetails data = JsonConvert.DeserializeObject<RootobjectDetails>(content);
                var prof = await db.OnlineZooms.Include(x => x.Session).Include(x => x.ClassLevel).Include(x => x.Subject).Include(x => x.User).FirstOrDefaultAsync(x => x.MeetingId == data.id);
                ViewBag.prof = prof;
                return View(data);
            }
            catch (Exception c)
            {

            }
            return View();
        }


        public ActionResult OnlineClassDetails()
        {
            try
            {

                List<User> userdata = new List<User>();
                var content = TokenManager.GetAMeeting(1234);
                //userdata = JsonConvert.DeserializeObject<List<ZoomUserDto>>(response.Content);
                RootobjectDetails data = JsonConvert.DeserializeObject<RootobjectDetails>(content);
                return View(data);
            }
            catch (Exception c)
            {

            }
            return View();
        }

        public ActionResult NewOnlineClass(RootobjectNewMeeting model)
        {
            try
            {

                var content = TokenManager.NewMeeting(model, "");
                RootobjectDetails data = JsonConvert.DeserializeObject<RootobjectDetails>(content);
                return View(data);
            }
            catch (Exception c)
            {

            }
            return View();
        }



        public ActionResult AllMeetingByUser()
        {
            try
            {
                List<Meeting> userdata = new List<Meeting>();
                var contents = TokenManager.GetMeetingsByUser();
                //userdata = JsonConvert.DeserializeObject<List<ZoomUserDto>>(response.Content);
                RootobjectMeetingList datalist = JsonConvert.DeserializeObject<RootobjectMeetingList>(contents);
                userdata = datalist.meetings.ToList();
                return View(userdata);
            }
            catch (Exception c)
            {

            }
            return View();
        }

    }
}