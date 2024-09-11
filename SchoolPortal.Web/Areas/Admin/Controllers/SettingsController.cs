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
using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNet.Identity;
using SchoolPortal.Web.Areas.Service;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity.Owin;

namespace SchoolPortal.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin,Developer")]
    public class SettingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //private PortalContext context = new PortalContext();
        private ISettingService _settingService = new SettingService();
        private IImageService _imageService = new ImageService();
        private HttpClient client = new HttpClient();


        public SettingsController()
        {
            //client.BaseAddress = new Uri("http://localhost:58920/");
            //client.BaseAddress = new Uri("http://cbttest.iskools.com/");
            client.BaseAddress = new Uri("http://cbt.iskools.com/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public SettingsController(SettingService settingService,
            ImageService imageService, ApplicationUserManager userManager)
        {
            _settingService = settingService;
            _imageService = imageService;
            UserManager = userManager;
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

        // GET: Admin/Settings
        public async Task<ActionResult> Index()
        {
            var item = await _settingService.GetSetting();
            return View(item);
        }

        // GET: Admin/Settings/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var setting = await _settingService.Get(id);
            if (setting == null)
            {
                return HttpNotFound();
            }
            return View(setting);
        }

        public async Task<ActionResult> SuperDetails()
        {

            var setting = await db.Settings.FirstOrDefaultAsync();
            if (setting == null)
            {
                return HttpNotFound();
            }
            return View(setting);
        }

        public async Task<ActionResult> AddSchoolToPortal()
        {
            var setting = await db.Settings.FirstOrDefaultAsync();
            //           var checkAbrivition = await context.Schools.FirstOrDefaultAsync(x => x.Abriviation == setting.SchoolInitials);
            //           if (checkAbrivition == null)
            //           {




            //               Schools d = new Schools();
            //               d.SchoolName = setting.SchoolName;
            //               d.Abriviation = setting.SchoolInitials;
            //               d.PortalUrl = setting.PortalLink;
            //               d.WebsiteUrl = setting.WebsiteLink;
            //               d.DateCreated = DateTime.UtcNow;
            //               context.Schools.Add(d);
            //               context.SaveChanges();
            //              TempData["msg"] = "successfully added to portal";
            //           }else if(checkAbrivition != null)
            //           {
            //               TempData["msg"] = "Data already exist";
            //           }
            //           else
            //           {
            //TempData["msg"] = "Error. try again";
            //           }


            return RedirectToAction("AddSchoolToPortalSuccess", "Settings");
        }

        public async Task<ActionResult> AddSchoolToPortalSuccess()
        {
            return View();
        }
        // GET: Admin/Settings/Create

        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Settings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Setting setting, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                //var checkAbrivition = await context.Schools.FirstOrDefaultAsync(x => x.Abriviation == setting.SchoolInitials);
                //if (checkAbrivition == null)
                //{


                var check = setting.AccessmentScore + setting.ExamScore;
                decimal number = 100;
                if (check == number)
                {
                    try
                    {
                        await _settingService.Create(setting);

                    }catch(Exception c)
                    {

                    }
                    var Imageid = await _imageService.Create(upload);
                    setting.ImageId = Imageid;
                    await _settingService.Edit(setting);

                }

                Schools d = new Schools();
                //d.SchoolName = setting.SchoolName;
                //d.Abriviation = setting.SchoolInitials;
                //d.PortalUrl = setting.PortalLink;
                //d.WebsiteUrl = setting.WebsiteLink;
                //d.DateCreated = DateTime.UtcNow;
                //context.Schools.Add(d);
                //context.SaveChanges();
                return RedirectToAction("Index", "Gradings");

            }
            TempData["error"] = "Note: exam score and assesment score must be 100 and fill other fields";
            return View(setting);
        }
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult> UpdateSchoolLogo()
        {

            var setting = await db.Settings.FirstOrDefaultAsync();
            if (setting == null)
            {
                return HttpNotFound();
            }
            return View(setting);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateSchoolLogo(int id, HttpPostedFileBase upload)
        {
            ImageModel model = await db.ImageModel.FindAsync(id);
            if (upload != null && upload.ContentLength > 0)
            {


                // Find its length and convert it to byte array
                int ContentLength = upload.ContentLength;

                // Create Byte Array
                byte[] bytImg = new byte[ContentLength];

                // Read Uploaded file in Byte Array
                upload.InputStream.Read(bytImg, 0, ContentLength);

                model.ImageContent = bytImg;
                model.ContentType = upload.ContentType;
                model.FileName = upload.FileName;

            }

            db.Entry(model).State = EntityState.Modified;
            await db.SaveChangesAsync();

            //Add Tracking
            var userId2 = User.Identity.GetUserId();
            var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
            Tracker tracker = new Tracker();
            tracker.UserId = userId2;
            tracker.UserName = user2.UserName;
            tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
            tracker.ActionDate = DateTime.UtcNow.AddHours(1);
            tracker.Note = tracker.FullName + " " + "Updated school logo";
            //db.Trackers.Add(tracker);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// //school Stam
        /// </summary>
        /// <returns></returns>
        /// 


        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult> SchoolStam()
        {

            var stam = await db.ImageModel.FirstOrDefaultAsync(x => x.FileName == "SCHOOLSTAM");
            if (stam == null)
            {
                return RedirectToAction("NewStamp");
            }
            return View(stam);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> SchoolStam(int id, HttpPostedFileBase upload)
        {
            ImageModel model = await db.ImageModel.FindAsync(id);
            if (upload != null && upload.ContentLength > 0)
            {


                // Find its length and convert it to byte array
                int ContentLength = upload.ContentLength;

                // Create Byte Array
                byte[] bytImg = new byte[ContentLength];

                // Read Uploaded file in Byte Array
                upload.InputStream.Read(bytImg, 0, ContentLength);

                model.ImageContent = bytImg;
                model.ContentType = upload.ContentType;


            }

            db.Entry(model).State = EntityState.Modified;
            await db.SaveChangesAsync();

            //Add Tracking
            var userId2 = User.Identity.GetUserId();
            var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
            Tracker tracker = new Tracker();
            tracker.UserId = userId2;
            tracker.UserName = user2.UserName;
            tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
            tracker.ActionDate = DateTime.UtcNow.AddHours(1);
            tracker.Note = tracker.FullName + " " + "Updated School Stamp";
            //db.Trackers.Add(tracker);
            await db.SaveChangesAsync();

            return RedirectToAction("SchoolStam");
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult NewStamp()
        {


            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> NewStamp(HttpPostedFileBase upload)
        {
            ImageModel model = new ImageModel();
            if (upload != null && upload.ContentLength > 0)
            {


                // Find its length and convert it to byte array
                int ContentLength = upload.ContentLength;

                // Create Byte Array
                byte[] bytImg = new byte[ContentLength];

                // Read Uploaded file in Byte Array
                upload.InputStream.Read(bytImg, 0, ContentLength);

                model.ImageContent = bytImg;
                model.ContentType = upload.ContentType;
                model.FileName = "SCHOOLSTAM";

            }

            db.ImageModel.Add(model);
            await db.SaveChangesAsync();

            //Add Tracking
            var userId2 = User.Identity.GetUserId();
            var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
            Tracker tracker = new Tracker();
            tracker.UserId = userId2;
            tracker.UserName = user2.UserName;
            tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
            tracker.ActionDate = DateTime.UtcNow.AddHours(1);
            tracker.Note = tracker.FullName + " " + "Added School Stamp";
            //db.Trackers.Add(tracker);
            await db.SaveChangesAsync();

            return RedirectToAction("SchoolStam");
        }

        // GET: Admin/Settings/Edit/5
        //[Authorize(Roles = "Developer")]
        public async Task<ActionResult> SupperSettingsEdit(string unixconverify, string xgink, string role)
        {
            unixconverify = "SuperAdmin";
            xgink = GeneralService.PortalLink();
            role = "superadmin";

            ViewBag.unixconverify = unixconverify;
            ViewBag.xgink = xgink;
            ViewBag.role = role;

            var setting = await db.Settings.FirstOrDefaultAsync();
            var paymentCallBackUrl = setting.PortalLink + "/Financial/Payment/Complete";
            ViewBag.paymentCallBackUrl = paymentCallBackUrl;
            if (setting == null)
            {
                return HttpNotFound();
            }
            return View(setting);
        }

        // POST: Admin/Settings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> SupperSettingsEdit(Setting setting, string unixconverify, string xgink, string role)
        {
            if (ModelState.IsValid)
            {
            
                await _settingService.Edit(setting);
                ClassModel activation = new ClassModel();
                if (setting.EnableCBT == true)
                {
                    HttpResponseMessage response = client.PostAsJsonAsync("api/SettingsApi/CBTActivation?unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role, activation).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }


                }
                return RedirectToAction("Index");
            }
            return View(setting);
        }

        // GET: Admin/Settings/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var setting = await _settingService.Get(id);
            if (setting == null)
            {
                return HttpNotFound();
            }
            return View(setting);
        }

        // POST: Admin/Settings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Setting setting)
        {
            if (ModelState.IsValid)
            {
                await _settingService.Edit(setting);
                return RedirectToAction("Index");
            }
            return View(setting);
        }

        // GET: Admin/Settings/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Setting setting = await db.Settings.FindAsync(id);
            if (setting == null)
            {
                return HttpNotFound();
            }
            return View(setting);
        }

        // POST: Admin/Settings/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Setting setting = await db.Settings.FindAsync(id);
            db.Settings.Remove(setting);
            await db.SaveChangesAsync();

            //Add Tracking
            var userId2 = User.Identity.GetUserId();
            var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
            Tracker tracker = new Tracker();
            tracker.UserId = userId2;
            tracker.UserName = user2.UserName;
            tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
            tracker.ActionDate = DateTime.UtcNow.AddHours(1);
            tracker.Note = tracker.FullName + " " + "Deleted Settings";
            //db.Trackers.Add(tracker);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        public ActionResult EmptyExcelFolder()
        {
            try
            {


                string[] files = Directory.GetFiles(Server.MapPath("~/ExcelUpload/"));
                foreach (string file in files)
                {
                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                    }
                }

                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Emptied Excel Sheet";
                //db.Trackers.Add(tracker);
                db.SaveChangesAsync();

                ViewBag.mssg = "Success";
            }
            catch (Exception e)
            {

            }


            return View();
        }

        public async Task<ActionResult> UpdateNewsletter()
        {
            var setting = await db.Settings.FirstOrDefaultAsync();
            if (setting == null)
            {
                return HttpNotFound();
            }
            return View(setting);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateNewsletter(Setting settingold)
        {
            if (ModelState.IsValid)
            {

                var setting = await db.Settings.FirstOrDefaultAsync();
                if (setting == null)
                {
                    return HttpNotFound();
                }
                setting.NewsletterContent = settingold.NewsletterContent;
                await _settingService.Edit(setting);
                return RedirectToAction("Newsletter", new { id = setting.Id });
            }
            return RedirectToAction("Index");
        }

        // GET: Admin/Settings/Details/5
        public async Task<ActionResult> Newsletter()
        {
            var setting = await db.Settings.FirstOrDefaultAsync();
            if (setting == null)
            {
                return HttpNotFound();
            }
            return View(setting);
        }
        [AllowAnonymous]
        public ActionResult Log()
        {
            var fileContents = System.IO.File.ReadAllText(Server.MapPath("~/App_Data/Logger.txt"));
            return Content(fileContents);
        }

        #region settings value

        public async Task<ActionResult> SettingValueIndex()
        {
            var value = await _settingService.SettingsValueList();
            return View(value);
        }

        // GET: Content/Riddles/Details/5
        public async Task<ActionResult> SettingValueDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var data = await _settingService.GetSettingValue(id);
            if (data == null)
            {
                return HttpNotFound();
            }
            return View(data);
        }

        // GET: Content/Riddles/Create
        public ActionResult SettingValueCreate()
        {
            return View();
        }

        // POST: Content/Riddles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> SettingValueCreate(SettingsValue value)
        {
            if (ModelState.IsValid)
            {
                await _settingService.CreateSettingValue(value);
                return RedirectToAction("SettingValueIndex");
            }

            return View(value);
        }

        // GET: Content/Riddles/Edit/5
        public async Task<ActionResult> SettingValueEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var data = await _settingService.GetSettingValue(id);
            if (data == null)
            {
                return HttpNotFound();
            }
            return View(data);
        }

        // POST: Content/Riddles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> SettingValueEdit(SettingsValue model)
        {
            if (ModelState.IsValid)
            {
                await _settingService.EditSettingValue(model);
                return RedirectToAction("SettingValueIndex");
            }
            return View(model);
        }

        // GET: Content/Riddles/Delete/5
        public async Task<ActionResult> SettingValueDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var data = await _settingService.GetSettingValue(id);
            if (data == null)
            {
                return HttpNotFound();
            }
            return View(data);
        }

        // POST: Content/Riddles/Delete/5
        [HttpPost, ActionName("SettingValueDelete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> SettingValueDeleteConfirmed(int id)
        {
            await _settingService.DeleteSettingValue(id);
            return RedirectToAction("SettingValueIndex");
        }


        #endregion

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
