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

namespace SchoolPortal.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class SessionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ISessionService _sessionService = new SessionService();
        private IAccomodationService _accomodationService = new AccomodationService();


        public SessionsController()
        {

        }
        public SessionsController(SessionService sessionService,AccomodationService accomodationService)
        {
            _sessionService = sessionService;
            _accomodationService = accomodationService;
        }
        // GET: Admin/Sessions
        public async Task<ActionResult> Index()
        {
            var item = await _sessionService.List();
            return View(item.OrderByDescending(x => x.SessionYear));
        }

        /// <summary>
        /// next session
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 

        ///next session
        ///
        public async Task<ActionResult> NextTerm()
        {
            bool check = await _sessionService.NextSession();
            if (check == false)
            {
                TempData["error"] = "Session Unavailable";
            }
            await _accomodationService.RefreshHostel();
            return RedirectToAction("Index");
        }


        ///previous session
        ///

        public async Task<ActionResult> PreviousTerm()
        {
            bool check = await _sessionService.PreviousSession();
            if (check == false)
            {
                TempData["error"] = "Session Unavailable";
            }
            return RedirectToAction("Index");
        }
        // GET: Admin/Sessions/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var session = await _sessionService.Get(id);
            return View(session);
        }

        // GET: Admin/Sessions/Create
        public ActionResult NewSession()
        {
            return View();
        }

        // POST: Admin/Sessions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> NewSession(Session session)
        {
            if (ModelState.IsValid)
            {
                if (session.SessionYear.ToLower().Contains("term") || session.SessionYear.ToLower().Contains("first") || session.SessionYear.ToLower().Contains("second") || session.SessionYear.ToLower().Contains("third"))
                {
                    TempData["error"] = "Invalid session year format. e.g \"2018/2019\"";
                    return View(session);
                }
                if (session.SessionYear.Length > 9 || session.SessionYear.Length < 9)
                {
                    TempData["error"] = "Invalid session year format. e.g \"2018/2019\"";
                    return View(session);
                }
                var check = await db.Sessions.Where(x => x.SessionYear.Contains(session.SessionYear)).ToListAsync();
                if (check.Count == 0)
                {


                    await _sessionService.Create(session);
                    TempData["success"] = "Added Successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = "Session ALready Exist";
                }
                return View(session);

            }
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            return View(session);
        }

        // GET: Admin/Sessions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var session = await _sessionService.Get(id);
            if (session == null)
            {
                return HttpNotFound();
            }
            return View(session);
        }

        // POST: Admin/Sessions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Session session)
        {
            if (ModelState.IsValid)
            {
                await _sessionService.Edit(session);
                return RedirectToAction("Index");
            }
            return View(session);
        }

        // GET: Admin/Sessions/Edit/5
        public async Task<ActionResult> SuperEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var session = await _sessionService.Get(id);
            if (session == null)
            {
                return HttpNotFound();
            }
            return View(session);
        }

        // POST: Admin/Sessions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> SuperEdit(Session session)
        {
            if (ModelState.IsValid)
            {
                await _sessionService.Edit(session);
                return RedirectToAction("Index");
            }
            return View(session);
        }

        // GET: Admin/Sessions/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var session = await _sessionService.Get(id);
            if (session == null)
            {
                return HttpNotFound();
            }
            return View(session);
        }

        // POST: Admin/Sessions/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _sessionService.Delete(id);
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
