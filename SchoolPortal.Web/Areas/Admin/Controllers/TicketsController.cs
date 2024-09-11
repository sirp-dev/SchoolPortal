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
using System.Net.Mail;
using System.IO;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace SchoolPortal.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public TicketsController()
        { }
        public TicketsController(
            ApplicationUserManager userManager)
        {
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

        // GET: Admin/Tickets
        public async Task<ActionResult> Index()
        {

            return View(await db.Tickets.OrderByDescending(x => x.Date).ToListAsync());
        }

        public ActionResult _TicketHistory()
        {
            var tick = db.Tickets.Include(x => x.Responses).OrderByDescending(x => x.Date).Take(8).ToList();
            return PartialView(tick);
        }

        public ActionResult _sticky()
        {
            var tick = db.Tickets;
            //ViewBag.not = tick.Include(x => x.Responses).Where(x => x.Closed == false && x.Responses.Count() > 0 && x.Responses.OrderByDescending(f=>f.Date).FirstOrDefault().RepliedBy.ToLower() != "superadmin").Count();
            //var sd = tick.Include(x => x.Responses).Where(x => x.Closed == false && x.Responses.Count() > 0).Count();
            //ViewBag.resp = tick.Include(x => x.Responses).Where(x => x.Closed == false && x.Responses.Count() > 0 && x.Responses.OrderByDescending(r=>r.Date).FirstOrDefault().RepliedBy.ToString().ToLower() == "superadmin").Count();
            //var fghjk = tick.Include(x => x.Responses).Where(x => x.Closed == false && x.Responses.Count() == 0 && x.Responses.OrderByDescending(r=>r.Date).FirstOrDefault().RepliedBy.ToString().ToLower() == "superadmin").Count();


            ViewBag.not = tick.Include(x => x.Responses).Where(x => x.Closed == false && x.Responses.OrderByDescending(f => f.Date).FirstOrDefault().RepliedBy.ToLower() != "superadmin").Count();
            ViewBag.resp = tick.Include(x => x.Responses).Where(x => x.Closed == false && x.Responses.OrderByDescending(f => f.Date).FirstOrDefault().RepliedBy.ToLower() == "superadmin").Count();
            var sd = tick.Include(x => x.Responses).Where(x => x.Closed == false && x.Responses.Count() > 0).Count();
            //   ViewBag.resp = tick.Include(x => x.Responses).Where(x => x.Closed == false && x.Responses.Count() > 0 && x.Responses.OrderByDescending(r => r.Date).FirstOrDefault().RepliedBy.ToString().ToLower() == "superadmin").Count();
            var fghjk = tick.Include(x => x.Responses).Where(x => x.Closed == false && x.Responses.Count() == 0 && x.Responses.OrderByDescending(r => r.Date).FirstOrDefault().RepliedBy.ToString().ToLower() == "superadmin").Count();

            return PartialView();
        }



        // GET: Admin/Tickets/Details/5
        public async Task<ActionResult> Close(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = await db.Tickets.Include(x => x.Responses).FirstOrDefaultAsync(x => x.Id == id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ticket.Closed = true;
            db.Entry(ticket).State = EntityState.Modified;
            await db.SaveChangesAsync();


            //Add Tracking
            var userId2 = User.Identity.GetUserId();
            if (userId2 != null)
            {
                var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Closed A Ticket";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           

            return RedirectToAction("Details", new { id = ticket.Id });
        }

        // GET: Admin/Tickets/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = await db.Tickets.Include(x => x.Responses).FirstOrDefaultAsync(x => x.Id == id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Admin/Tickets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Ticket ticket)
        {
            System.Random randomInteger = new System.Random();
            int genNumber = randomInteger.Next(100000);

            if (ModelState.IsValid)
            {
                var user = User.Identity.Name;
                HttpRequest req = System.Web.HttpContext.Current.Request;
                string browserName = req.Browser.Browser;


                var ip = Dns.GetHostAddresses(Dns.GetHostName()).First(f => f.AddressFamily == AddressFamily.InterNetwork).ToString();

                var school = await db.Settings.FirstOrDefaultAsync();
                ticket.Title = "Complaint";
                ticket.Date = DateTime.UtcNow.AddHours(1);
                ticket.TicketNumber = "#" + genNumber;
                ticket.IpAddress = ip;
                ticket.browser = browserName;
                db.Tickets.Add(ticket);
                await db.SaveChangesAsync();


                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                if (userId2 != null)
                {
                    var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Created A Ticket";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }

                try
                {
                    string message = "";

                    MailMessage mail = new MailMessage();

                    //set the addresses 
                    mail.From = new MailAddress("Iskools@exwhyzee.ng"); //IMPORTANT: This must be same as your smtp authentication address.
                    //mail.To.Add("onwukaemeka41@gmail.com");
                    //mail.To.Add("ibiznex@gmail.com");
                    //mail.To.Add("judengama@gmail.com");
                    //
                    //set the content Server.MapPath("~/status.txt")C:\VISUAL STUDIO PROJECTS\OFFICE PROJECTS\ACTIVE PROJECTS\SchoolPortal\SchoolPortal.Web\Areas\Admin\Models\HtmlPage1.html
                    string AppPath = Request.PhysicalApplicationPath;
                    //StreamReader sr = new StreamReader(AppPath + "../Views/Account/HtmlPage1.html");
                    StreamReader sr = new StreamReader(Server.MapPath("~/Areas/Admin/Models/HtmlPage1.html"));

                    mail.Body = sr.ReadToEnd();
                    sr.Close();

                    MailDefinition md = new MailDefinition();
                    md.From = "Iskools@exwhyzee.ng";
                    md.IsBodyHtml = true;
                    md.Subject = "ISkool Ticket";
                    md.CC = "onwukaemeka41@gmail.com,ponwuka123@gmail.com,iskoolsportal@gmail.com";

                    //{portal}{ticketmessage}{schoolname}
                    ListDictionary replacements = new ListDictionary();
                    replacements.Add("{schoolname}", school.SchoolName);
                    replacements.Add("{ticketmessage}", ticket.Complain);
                    replacements.Add("{portal}", school.PortalLink);


                    ////string body = "<div>Hello {name} You're from {country}. Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a></div>";
                    //string body = "<div>Hello {name} You're from {country}. Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a></div>";

                    mail = md.CreateMailMessage("onwukaemeka41@gmail.com", replacements, mail.Body, new System.Web.UI.Control());



                    //send the message 
                    //SmtpClient smtp = new SmtpClient("mail.iskools.com");
                    SmtpClient smtp = new SmtpClient("mail.exwhyzee.ng");

                    //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
                    NetworkCredential Credentials = new NetworkCredential("Iskools@exwhyzee.ng", "Admin@123");
                    smtp.Credentials = Credentials;
                    smtp.Send(mail);
                    //TempData["mssg"] = message = "Mail Sent Successfull. JK-Fulton Customer Care will get back to you soon";
                }
                catch (Exception ex)
                {

                    //TempData["mssg"] = "Mail not Sent. Try Again.";
                }


                return RedirectToAction("Index");
            }

            return View(ticket);
        }

        // GET: Admin/Tickets/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = await db.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Admin/Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                await db.SaveChangesAsync();

                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                if (userId2 != null)
                {
                    var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Edited a Ticket";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
               

                return RedirectToAction("Index");
            }
            return View(ticket);
        }

        // GET: Admin/Tickets/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = await db.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Admin/Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Ticket ticket = await db.Tickets.FindAsync(id);
            db.Tickets.Remove(ticket);
            await db.SaveChangesAsync();

            //Add Tracking
            var userId2 = User.Identity.GetUserId();
            if (userId2 != null)
            {
                var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Deleted a ticket";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
            

            return RedirectToAction("Index");
        }


        #region response region

        // GET: Admin/Tickets/Create
        public ActionResult CreateResponse(int tId)
        {
            return View();
        }

        // POST: Admin/Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateResponse(Response response, int tId)
        {
            if (ModelState.IsValid)
            {
                var user = User.Identity.Name;
                response.RepliedBy = user;
                response.TicketId = tId;
                response.Date = DateTime.UtcNow.AddHours(1);
                db.Responses.Add(response);
                await db.SaveChangesAsync();

                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                if (userId2 != null)
                {
                    var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Added a response to a ticket";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
                

                return RedirectToAction("Details", new { id = tId });
            }

            return View(response);
        }

        // GET: Admin/Tickets/Edit/5
        public async Task<ActionResult> EditResponse(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Response response = await db.Responses.FindAsync(id);
            if (response == null)
            {
                return HttpNotFound();
            }
            return View(response);
        }

        // POST: Admin/Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditResponse(Response response)
        {
            if (ModelState.IsValid)
            {
                db.Entry(response).State = EntityState.Modified;
                await db.SaveChangesAsync();

                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                if (userId2 != null)
                {
                    var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Edited Ticket Response";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
               

                return RedirectToAction("Details", new { id = response.TicketId });
            }
            return View(response);
        }

        // GET: Admin/Tickets/Delete/5
        public async Task<ActionResult> DeleteResponse(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Response response = await db.Responses.FindAsync(id);
            ViewBag.id = response.TicketId;
            if (response == null)
            {
                return HttpNotFound();
            }
            return View(response);
        }

        // POST: Admin/Tickets/Delete/5
        [HttpPost, ActionName("DeleteResponse")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteResponseConfirmed(int id, string ticketId)
        {
            int tId = Convert.ToInt32(ticketId);
            Response response = await db.Responses.FindAsync(id);
            db.Responses.Remove(response);
            await db.SaveChangesAsync();

            //Add Tracking
            var userId2 = User.Identity.GetUserId();
            if (userId2 != null)
            {
                var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Deleted Ticket Response";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
            

            return RedirectToAction("Details", new { id = tId });
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
