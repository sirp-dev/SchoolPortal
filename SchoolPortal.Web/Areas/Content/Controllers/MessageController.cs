using Newtonsoft.Json;
using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Controllers;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Dtos.Zoom;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Areas.Content.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IMessageService _messageService = new MessageService();
        private IClassLevelService _classLevelService = new ClassLevelService();


        public MessageController()
        {

        }
        public MessageController(
            MessageService messageService,
            ClassLevelService classLevelService)
        {
            _messageService = messageService;
            _classLevelService = classLevelService;
        }
        // GET: Content/Message
        public async Task<ActionResult> Index()
        {
            var classlevel = await _classLevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel, "Id", "ClassLevelName");
            return View();
        }

        public async Task<ActionResult> SendSMS(string Target, string ClassSend)
        {
            try
            {
                var numbers = await _messageService.GetPhoneNumbers(Target, ClassSend);
                TempData["numbers"] = numbers;
                return RedirectToAction("ComposeSms");
            }
            catch (Exception c)
            {
                TempData["error"] = "try again";
                return View();
            }

        }


        public async Task<ActionResult> SendToAllParents()
        {
            string numbers = await _messageService.AllParentInContact();
            TempData["numbers"] = numbers;
            return RedirectToAction("ComposeSms");
        }


        public async Task<ActionResult> SendSMSLiveClass(long id, string Target, string ClassSend)
        {
            var sett = await db.Settings.FirstOrDefaultAsync();
            var liveclass = await db.OnlineZooms.Include(x => x.Session).Include(x => x.ClassLevel).Include(x => x.Subject).Include(x => x.User).FirstOrDefaultAsync(x => x.MeetingId == id);
            var numbers = "08165680904,";
            try
            {
                 numbers = await _messageService.GetPhoneNumbers(Target, ClassSend);
                

            }
            catch (Exception c)
            {
               
            }
            string messages = "Log onto your dashboard to join the live class on "+liveclass.ClassLevel.ClassName+ "("+liveclass.Subject.SubjectName + ") on " + liveclass.ClassDate + " by " + liveclass.ClassTime + " Follow the link http://" + sett.PortalLink + " to start";
            try
            {
                string response = await _messageService.SendSms("ISKOOLS", numbers, messages, "SendNow", "");

            }
            catch (Exception c) { }
            return RedirectToAction("LiveClassList", "Panel", new { area = "Staff" });
        }


        public async Task<ActionResult> ComposeSms()
        {

            int count = 0;
            try
            {
                TempData["SmsNumbers"] = TempData["numbers"];
                string numberscount = TempData["numbers"].ToString();

                count = NumberCount(numberscount);
            }
            catch (Exception c)
            {

            }
            TempData["countnub"] = count;
            return View();
        }

        public static int NumberCount(string Numbers)
        {
            string n = Numbers.Replace("\r\n", ",");
            IList<string> numbers = n.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
            numbers = numbers.Distinct().ToList();
            return numbers.Count();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ComposeSms(string SenderId, string Recipients, string Message, string SendOption, string ScheduleDate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string response = await _messageService.SendSms(SenderId, Recipients, Message, SendOption, ScheduleDate);
                    TempData["Succes"] = "Message Sent";
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    TempData["Error"] = "Unable to send. Please try again or contact the administrator.";
                    return RedirectToAction("Index");
                }

            }
            TempData["Error"] = "Something went wrong? Please try again or contact the administrator.";
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> MessageHistory()
        {
            var items = await _messageService.MessageHistory();
            return View(items);
        }

        public async Task<ActionResult> MessageDetails(int id)
        {
            var item = await _messageService.GetMessage(id);
            return View(item);
        }
    }
}