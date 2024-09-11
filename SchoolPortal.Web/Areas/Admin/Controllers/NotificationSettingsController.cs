using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin,Developer")]
    public class NotificationSettingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private INotificationSettingService _notification = new NotificationSettingService();

        public NotificationSettingsController()
        {

        }

        public NotificationSettingsController(
            NotificationSettingService notification,
            ApplicationUserManager userManager)
        {
            _notification = notification;
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

        // GET: Admin/NotificationSettings
        public async Task<ActionResult> Index()
        {
            var item = await _notification.GetNotification();
            return View(item);
        }

        // GET: Admin/NotificationSettings/Details/5
        public async Task<ActionResult> Details(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var setting = await _notification.Get(id);
            if (setting == null)
            {
                return HttpNotFound();
            }
            return View(setting);
        }

        // GET: Admin/NotificationSettings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/NotificationSettings/Create
        [HttpPost]
        public async Task<ActionResult> Create(Notification notifications)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    await _notification.Create(notifications);
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return View();
            }
            return View(notifications);
        }

        // GET: Admin/NotificationSettings/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var setting = await _notification.Get(id);
            if (setting == null)
            {
                return HttpNotFound();
            }
            return View(setting);
        }

        // POST: Admin/NotificationSettings/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(Notification notifications)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    await _notification.Edit(notifications);
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/NotificationSettings/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification setting = await db.Notifications.FindAsync(id);
            if (setting == null)
            {
                return HttpNotFound();
            }
            return View(setting);
        }

        // POST: Admin/NotificationSettings/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int? id)
        {
            try
            {
                // TODO: Add delete logic here
                Notification setting = await db.Notifications.FindAsync(id);
                db.Notifications.Remove(setting);
                await db.SaveChangesAsync();

                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                var user2 = UserManager.Users.Where(x => x.Id == userId2 && x.Status == EntityStatus.Active).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Deleted Notification";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
