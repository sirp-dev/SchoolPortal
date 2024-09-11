using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentProfilesController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        private IStudentProfileService _studentProfileService = new StudentProfileService();


        public StudentProfilesController()
        {

        }
        public StudentProfilesController(StudentProfileService studentProfileService)
        {
            _studentProfileService = studentProfileService;
        }

        // GET: Admin/StudentProfile
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public async Task<ActionResult> Profile(string id)
        {
            var myid = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.UserId == id);
            return RedirectToAction("StudentProfile", new { id = myid.Id });
        }
        [AllowAnonymous]
        public async Task<ActionResult> StudentProfile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var student = await _studentProfileService.Get(id);
            var currentclass = await _studentProfileService.StudentCurrentClass(id);
            ViewBag.currentclass = currentclass;
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
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