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
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace SchoolPortal.Web.Areas.Documentaries.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class DocController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ApplicationSignInManager _signInManager;
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
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

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        // GET: Documentaries/Doc
        public async Task<ActionResult> Index()
        {
            return View();
        }

        public async Task<ActionResult> DocumentaryIndex()
        {
            return View(await db.Documentaries.ToListAsync());
        }


        public ActionResult _DocumentaryMenu()
        {
            var user1 = User.Identity.GetUserId();
            var item = db.Documentaries.ToList();
            if (UserManager.IsInRole(user1, "SuperAdmin"))
            {
                item = item.Where(x=>x.Role == "SuperAdmin").ToList();
                
            }
           else if (UserManager.IsInRole(user1, "Admin"))
            {
                item = item.Where(x => x.Role == "Admin").ToList();
            }
           else if (UserManager.IsInRole(user1, "ReadOnly"))
            {
                item = item.Where(x => x.Role == "ReadOnly").ToList();
            }
           else if (UserManager.IsInRole(user1, "Developer"))
            {
                item = item.Where(x => x.Role == "Developer").ToList();
              
            }
           else if (UserManager.IsInRole(user1, "Finance"))
            {
                item = item.Where(x => x.Role == "Finance").ToList();
             
            }
           else if (UserManager.IsInRole(user1, "Staff"))
            {
                item = item.Where(x => x.Role == "Staff").ToList();
           
            }
           else if (UserManager.IsInRole(user1, "FormTeacher"))
            {
                item = item.Where(x => x.Role == "FormTeacher").ToList();
           
            }
           else if (UserManager.IsInRole(user1, "Student"))
            {
                item = item.Where(x => x.Role == "Student").ToList();
          
            }
            else
            {
               item = item.Where(x => x.Role == "Student").ToList();
            }
            ViewBag.item = item;
            return PartialView(item);
        }


       
        // GET: Documentaries/Doc/Details/5
        public async Task<ActionResult> Details(int? id, string title)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Documentary documentary = await db.Documentaries.FindAsync(id);
            if (documentary == null)
            {
                return HttpNotFound();
            }
            return View(documentary);
        }

        // GET: Documentaries/Doc/Create
        public ActionResult Create()
        {
            ViewBag.RoleName = new SelectList(RoleManager.Roles.OrderBy(x => x.Name), "Name", "Name");
            return View();
        }

        // POST: Documentaries/Doc/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Documentary documentary, string Role)
        {
            if (ModelState.IsValid)
            {
                documentary.DateCreated = DateTime.UtcNow.AddHours(1);
                documentary.Role = Role;
                db.Documentaries.Add(documentary);
                await db.SaveChangesAsync();
                TempData["success"] = "Documentary Added Successfully";
                return RedirectToAction("DocumentaryIndex");
            }
            ViewBag.RoleName = new SelectList(RoleManager.Roles.OrderBy(x => x.Name), "Name", "Name");
            TempData["error"] = "Unable to add Documentary";
            return View(documentary);
        }

        // GET: Documentaries/Doc/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Documentary documentary = await db.Documentaries.FindAsync(id);
            if (documentary == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleName = new SelectList(RoleManager.Roles.OrderBy(x => x.Name), "Name", "Name", documentary.Role);
            return View(documentary);
        }

        // POST: Documentaries/Doc/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Documentary documentary, string Role)
        {
            if (ModelState.IsValid)
            {
                documentary.Role = Role;
                db.Entry(documentary).State = EntityState.Modified;
                await db.SaveChangesAsync();
                TempData["success"] = "Documentary Edited Successfully";
                return RedirectToAction("DocumentaryIndex");
            }
            ViewBag.RoleName = new SelectList(RoleManager.Roles.OrderBy(x => x.Name), "Name", "Name", documentary.Role);
            TempData["error"] = "Unable to Edit Documentary";
            return View(documentary);
        }

        // GET: Documentaries/Doc/Delete/5
        public async Task<ActionResult> Delete(int? id, string title)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Documentary documentary = await db.Documentaries.FindAsync(id);
            if (documentary == null)
            {
                return HttpNotFound();
            }
            return View(documentary);
        }

        // POST: Documentaries/Doc/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Documentary documentary = await db.Documentaries.FindAsync(id);
            db.Documentaries.Remove(documentary);
            await db.SaveChangesAsync();
            TempData["success"] = "Documentary Deleted Successfully";
            return RedirectToAction("DocumentaryIndex");
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
