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
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Net.Mail;
using System.IO;

namespace SchoolPortal.Web.Areas.Admin.Controllers
{

    public class DataUserRequestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IClassLevelService _classlevelService = new ClassLevelService();
        private IUserManagerService _userService = new UserManagerService();
        private IImageService _imageService = new ImageService();
        private IStudentProfileService _studentService = new StudentProfileService();
        private ISessionService _sessionService = new SessionService();
        private ApplicationSignInManager _signInManager;
        private IEnrollmentService _enrollmentService = new EnrollmentService();



        public DataUserRequestsController()
        {

        }
        public DataUserRequestsController(EnrollmentService enrollmentService, UserManagerService userService, ApplicationSignInManager signInManager, ApplicationUserManager userManager, ApplicationRoleManager roleManager, ClassLevelService classLevelService, ImageService imageService, StudentProfileService studentProfile, SessionService sessionService)
        {
            UserManager = userManager;
            _userService = userService;
            _classlevelService = classLevelService;
            _imageService = imageService;
            _studentService = studentProfile;
            _sessionService = sessionService;
            _enrollmentService = enrollmentService;
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

        // GET: Admin/DataUserRequests
        public async Task<ActionResult> Index()
        {
            return View(await db.DataUserRequests.ToListAsync());
        }


        public async Task<ActionResult> ResetAndSendText(int id, string uid)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == uid);
            var datafetch = await db.DataUserRequests.FirstOrDefaultAsync(x => x.Id == id);
            var sett = await db.Settings.FirstOrDefaultAsync();
            // await  _userManager.AddPasswordAsync(userId, newPassword);
            var removePassword = UserManager.RemovePassword(user.Id);
            if (removePassword.Succeeded)
            {
                //Removed Password Success
                var AddPassword = UserManager.AddPassword(user.Id, "123456");
                if (AddPassword.Succeeded)
                {
                    //var userm = _userManager.Users.FirstOrDefault(x => x.Id == userId);
                    TempData["password"] = "Password Resset Successful.";

                    string mass = "";
                    try
                    {

                        MailMessage mail = new MailMessage();

                        //set the addresses 
                        mail.From = new MailAddress("learnonline@iskools.com"); //IMPORTANT: This must be same as your smtp authentication address.
                        mail.To.Add(datafetch.Email);
                        mail.To.Add("iskoolsportal@gmail.com");

                        //set the content 

                        mail.Subject = " Login for " + sett.SchoolName;

                        mass = "Hello " + user.Surname + " " + user.FirstName + " " + user.OtherName + ", your login details is Username: " + user.UserName + " and password: 123456 Click " + sett.PortalLink + " to login. Thank you and stay safe";

                        mail.Body = mass;
                        //send the message 
                        SmtpClient smtp = new SmtpClient("mail.iskools.com");

                        //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
                        NetworkCredential Credentials = new NetworkCredential("learnonline@iskools.com", "Exwhyzee@123");
                        smtp.Credentials = Credentials;
                        smtp.Send(mail);

                    }
                    catch (Exception ex)
                    {


                    }

                    try
                    {
                        string urlString = "http://xyzsms.com/api/ApiXyzSms/ComposeMessage?username=onwuka1&password=nation&recipients=08165680904," + datafetch.ParentsPhoneNumber + "," + datafetch.StudentsPhoneNumber + "&senderId=ISKOOLS&smsmessage=" + mass + "&smssendoption=SendNow";
                        //  string urlString = "http://www.xyzsms.com/components/com_spc/smsapi.php?username=" + senderUserName + "&password=" + senderPassword + "&sender=" + senderId + "&recipient=" + recipient + "&message=" + message;
                        string response = "";
                        try
                        {
                            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(urlString);
                            httpWebRequest.Method = "GET";
                            httpWebRequest.ContentType = "application/json";

                            //getting the respounce from the request
                            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                            Stream responseStream = httpWebResponse.GetResponseStream();
                            StreamReader streamReader = new StreamReader(responseStream);
                            response = streamReader.ReadToEnd();
                        }
                        catch (Exception d)
                        {

                        }
                    }
                    catch (Exception c) { }

                    //Add Tracking
                    var userId = User.Identity.GetUserId();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId;
                    tracker.UserName = user.UserName;
                    tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Requested password reset";
                    //db.Trackers.Add(tracker);
                    db.SaveChangesAsync();

                    return RedirectToAction("Details", new { id = id });
                }
            }
            TempData["error"] = "Password Resset Successful.";

            return RedirectToAction("Details", new { id = id });
        }

        // GET: Admin/DataUserRequests/Details/5
        public async Task<ActionResult> Details(int? id, string searchString)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataUserRequest dataUserRequest = await db.DataUserRequests.FindAsync(id);
            try
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    var items = await _enrollmentService.EnrolledStudents(searchString, "", 1);
                    ViewBag.userreturn = items;
                }
            }
            catch (Exception c) { }


            if (dataUserRequest == null)
            {
                return HttpNotFound();
            }
            return View(dataUserRequest);
        }

        // GET: Admin/DataUserRequests/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/DataUserRequests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,FullName,DateOfBirth,ParentName,StudentsPhoneNumber,ParentsPhoneNumber,ParentsOccupation,ClassName,FormTeacher")] DataUserRequest dataUserRequest)
        {
            if (ModelState.IsValid)
            {
                db.DataUserRequests.Add(dataUserRequest);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(dataUserRequest);
        }

        // GET: Admin/DataUserRequests/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataUserRequest dataUserRequest = await db.DataUserRequests.FindAsync(id);
            if (dataUserRequest == null)
            {
                return HttpNotFound();
            }
            return View(dataUserRequest);
        }

        // POST: Admin/DataUserRequests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,FullName,DateOfBirth,ParentName,StudentsPhoneNumber,ParentsPhoneNumber,ParentsOccupation,ClassName,FormTeacher")] DataUserRequest dataUserRequest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dataUserRequest).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(dataUserRequest);
        }

        // GET: Admin/DataUserRequests/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataUserRequest dataUserRequest = await db.DataUserRequests.FindAsync(id);
            if (dataUserRequest == null)
            {
                return HttpNotFound();
            }
            return View(dataUserRequest);
        }

        // POST: Admin/DataUserRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            DataUserRequest dataUserRequest = await db.DataUserRequests.FindAsync(id);
            db.DataUserRequests.Remove(dataUserRequest);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
