using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Dtos;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;
using Excel = Microsoft.Office.Interop.Excel;
//using LinqToExcel;

namespace SchoolPortal.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class UserManagersController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        private IClassLevelService _classlevelService = new ClassLevelService();
        private IEnrollmentService _enrollmentService = new EnrollmentService();
        private IUserManagerService _userService = new UserManagerService();
        private IImageService _imageService = new ImageService();
        private IStudentProfileService _studentService = new StudentProfileService();
        private ISessionService _sessionService = new SessionService();
        private ApplicationSignInManager _signInManager;
        private IStaffService _staffProfileService = new StaffService();


        public UserManagersController()
        {

        }
        public UserManagersController(UserManagerService userService, ApplicationSignInManager signInManager, ApplicationUserManager userManager, ApplicationRoleManager roleManager, ClassLevelService classLevelService, EnrollmentService enrollmentService, ImageService imageService, StudentProfileService studentProfile, SessionService sessionService, IStaffService staffProfileService)
        {
            _userService = userService;
            UserManager = userManager;
            _classlevelService = classLevelService;
            RoleManager = roleManager;
            _imageService = imageService;
            _studentService = studentProfile;
            _sessionService = sessionService;
            _enrollmentService = enrollmentService;
            SignInManager = signInManager;
            _staffProfileService = staffProfileService;
        }
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

        [HttpPost]
        public async Task<ActionResult> UpdateSurname(string stdid, string surname)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == stdid);
            user.Surname = surname;
            db.Entry(user).State = EntityState.Modified;
            await db.SaveChangesAsync();

            //Add Tracking
            var userId2 = User.Identity.GetUserId();
            var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
            Tracker tracker = new Tracker();
            tracker.UserId = userId2;
            tracker.UserName = user2.UserName;
            tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
            tracker.ActionDate = DateTime.UtcNow.AddHours(1);
            tracker.Note = tracker.FullName + " " + "Updated User Surname";
            //db.Trackers.Add(tracker);
            await db.SaveChangesAsync();
            return new EmptyResult();

        }
        // GET: Admin/UserManagers
        public async Task<ActionResult> Index(string searchString, string currentFilter, int? page)
        {

            var user1 = User.Identity.GetUserId();

            if (await UserManager.IsInRoleAsync(user1, "SuperAdmin"))
            {
                ViewBag.Roles = RoleManager.Roles.ToList();
            }
            else
            {
                ViewBag.Roles = RoleManager.Roles.Where(x => x.Name != "SuperAdmin" && x.Name != "Developer" && x.Name != "SuperAdmin2").ToList();
            }
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var items = await _userService.AllUsers(searchString, currentFilter, page);
            ViewBag.countall = items.Count();
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));

        }

        public async Task<ActionResult> MoveToGraduate()
        {
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel.OrderBy(x => x.ClassLevelName), "Id", "ClassLevelName");

            var session = await _sessionService.GetAllSession();
            var css = await db.Sessions.FirstOrDefaultAsync(x=>x.Status == SessionStatus.Current);
            ViewBag.sessionId = new SelectList(session.Where(x=>x.Year != css.SessionYear).OrderBy(x => x.FullSession), "Id", "FullSession");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> MoveToGraduate(int sessId, int classId)
        {
            int count = 0;
           var classdata = await db.ClassLevels.FirstOrDefaultAsync(x=>x.Id == classId);
            if(classdata != null)
            {
                var enrol = await db.Enrollments.Include(x=>x.StudentProfile.user).Where(x=>x.ClassLevelId == classdata.Id && x.SessionId == sessId).Where(x=>x.StudentProfile.user.Status != EntityStatus.Graduate).ToListAsync();
                foreach(var item in enrol)
                {
                    var iuser = await UserManager.FindByIdAsync(item.StudentProfile.UserId);
                    iuser.Status = EntityStatus.Graduate;
                    await UserManager.UpdateAsync(iuser);
                    count++;
                }
            }

            TempData["success"] = count +" Added to Graduate";
            return RedirectToAction("Graduates");
        }

        public async Task<ActionResult> Graduates(string searchString, string currentFilter, int? page)
        {

            var user1 = User.Identity.GetUserId();

            if (await UserManager.IsInRoleAsync(user1, "SuperAdmin"))
            {
                ViewBag.Roles = RoleManager.Roles.ToList();
            }
            else
            {
                ViewBag.Roles = RoleManager.Roles.Where(x => x.Name != "SuperAdmin" && x.Name != "Developer").ToList();
            }
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var items = await _userService.GraduatedUsers(searchString, currentFilter, page);
            ViewBag.countall = items.Count();
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));

        }
        public async Task<ActionResult> DropOut(string searchString, string currentFilter, int? page)
        {

            var user1 = User.Identity.GetUserId();

            if (await UserManager.IsInRoleAsync(user1, "SuperAdmin"))
            {
                ViewBag.Roles = RoleManager.Roles.ToList();
            }
            else
            {
                ViewBag.Roles = RoleManager.Roles.Where(x => x.Name != "SuperAdmin" && x.Name != "Developer").ToList();
            }
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var items = await _userService.DropoutUsers(searchString, currentFilter, page);
            ViewBag.countall = items.Count();
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));

        }

        public async Task<ActionResult> Active(string searchString, string currentFilter, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var items = await _userService.Active(searchString, currentFilter, page);
            ViewBag.countA = items.Count();
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));

        }

        public async Task<ActionResult> Expelled(string searchString, string currentFilter, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var items = await _userService.Expelled(searchString, currentFilter, page);
            ViewBag.countE = items.Count();
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));

        }

        public async Task<ActionResult> Withdrawn(string searchString, string currentFilter, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var items = await _userService.Withdrawn(searchString, currentFilter, page);
            ViewBag.countW = items.Count();
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));

        }

        public async Task<ActionResult> Archived(string searchString, string currentFilter, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var items = await _userService.Archived(searchString, currentFilter, page);
            ViewBag.countAr = items.Count();
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));

        }

        public async Task<ActionResult> Suspended(string searchString, string currentFilter, int? page)
        {

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var items = await _userService.Suspeneded(searchString, currentFilter, page);
            ViewBag.countS = items.Count();
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));

        }

        public async Task<ActionResult> AllStaff(string searchString, string currentFilter, int? page)
        {
            ViewBag.Roles = RoleManager.Roles.Where(x => x.Name != "SuperAdmin").ToList();


            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var items = await _userService.ListStaff(searchString, currentFilter, page);
            ViewBag.countallS = items.Count();
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));

        }
        public async Task<ActionResult> NonActiveStaff(string searchString, string currentFilter, int? page)
        {
            ViewBag.Roles = RoleManager.Roles.Where(x => x.Name != "SuperAdmin").ToList();


            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var items = await _userService.ListNonActiveStaff(searchString, currentFilter, page);
            ViewBag.countallS = items.Count();
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));

        }

        public async Task<ActionResult> EditStaff(int? id)
        {
            ViewBag.StateName = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await _staffProfileService.Get(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditStaff(StaffInfoDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _staffProfileService.Edit(model);
                    TempData["success"] = "Update Successful.";
                    return RedirectToAction("Staff");
                }
                catch (Exception e)
                {
                    TempData["error"] = "Update Unsuccessful, (" + e.ToString() + ")";
                }

            }
            return View(model);
        }

        public async Task<ActionResult> Staff(string searchString, string currentFilter, int? page)
        {

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var items = await _userService.ListStaff(searchString, currentFilter, page);
            ViewBag.countStaff = items.Count();
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));

        }

        public async Task<ActionResult> AllStudents(string searchString, string currentFilter, int? page)
        {
            ViewBag.Roles = RoleManager.Roles.Where(x => x.Name != "SuperAdmin").ToList();
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var items = await _userService.ListStudent(searchString, currentFilter, page);
            ViewBag.countallSt = items.Count();
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));

        }

        public async Task<ActionResult> Students(string searchString, string currentFilter, int? page)
        {

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var items = await _userService.ListStudent(searchString, currentFilter, page);
            ViewBag.count = items.Count();
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));

        }
        static string ConvertStringArrayToString(string[] array)
        {
            // Concatenate all the elements into a StringBuilder.
            StringBuilder builder = new StringBuilder();
            foreach (string value in array)
            {
                builder.Append(value);
                builder.Append('.');
            }
            return builder.ToString();
        }

        // GET: Admin/Sessions/Create
        public ActionResult NewStaff()
        {
            ViewBag.StateName = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName");
            return View();
        }

        // POST: Admin/Sessions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> NewStaff(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string succed;
                succed = await _userService.NewStaff(model);
                if (succed == "true")
                {
                    TempData["success"] = "Staff with username <i> " + model.Username + "</i> Added Successfully.";
                    return RedirectToAction("NewStaff");
                }
                else
                {


                    TempData["error1"] = succed;
                }

            }
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            TempData["error"] = "Creation of new staff not successful";
            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName", model.StateOfOrigin);
            return View(model);
        }

        public async Task<ActionResult> NewStudent()
        {
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel, "Id", "ClassLevelName");
            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName");
            return View();
        }

        // POST: Admin/Sessions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]

        public async Task<ActionResult> NewStudent(RegisterViewModel model, HttpPostedFileBase upload, int classId, string LastSchoolAttended, string ParentName, string ParentAddress, string ParentPhone, string ParentOccupation)
        {
            var ee = "";
            if (ModelState.IsValid)
            {



                try
                {
                    string succed;

                    succed = await _userService.NewStudent(model, LastSchoolAttended, ParentName, ParentAddress, ParentPhone, ParentOccupation);
                    if (succed == "true")
                    {
                        var Imageid = await _imageService.Create(upload);
                        var user = await UserManager.FindByNameAsync(model.Username);
                        // var user = await _userService.GetUserByUserEmail(model.Email);
                        var student = await _studentService.GetStudentByUserId(user.Id);

                        //profile pic upload
                        await _studentService.UpdateImageId(student.Id, Imageid);

                        //enrolment
                        await _enrollmentService.EnrollStudent(classId, student.Id);
                        var classLevel = await _classlevelService.Get(classId);
                        TempData["success"] = "Student with username <i> " + model.Username + "</i> Added Successfully to " + classLevel.ClassName + " Class";
                        return RedirectToAction("NewStudent");
                    }
                    else
                    {
                        TempData["error1"] = succed;
                    }


                }
                catch (Exception e)
                {
                    ee = e.ToString();
                }

            }
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            TempData["error"] = "Creation of new student not successful" + ee;
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel, "Id", "ClassLevelName");
            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName", model.StateOfOrigin);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> UserToRole(string rolename, string userId, bool? ischecked)
        {
            if (ischecked.HasValue && ischecked.Value)
            {
                await _userService.AddUserToRole(userId, rolename);

            }
            else
            {
                await _userService.RemoveUserToRole(userId, rolename);
            }

            return RedirectToAction("Index");
        }

        //
        // GET: /Roles/
        public ActionResult Roles()
        {
            return View(RoleManager.Roles.Where(x => x.Name != "SuperAdmin"));
        }

        //
        // GET: /Roles/Details/5
        public async Task<ActionResult> RoleDetails(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            // Get the list of Users in this Role
            var users = new List<ApplicationUser>();

            // Get the list of Users in this Role
            var usersitem = await _userService.Users();
            foreach (var user in usersitem)
            {
                if (await _userService.IsUsersinRole(user.Id, role.Name))
                {
                    users.Add(user);
                }
            }

            ViewBag.Users = users;
            ViewBag.UserCount = users.Count();
            return View(role);
        }

        //
        // GET: /Roles/Create
        public ActionResult CreateRole()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        public async Task<ActionResult> CreateRole(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole(roleViewModel.Name);
                var roleresult = await RoleManager.CreateAsync(role);
                if (!roleresult.Succeeded)
                {
                    ModelState.AddModelError("", roleresult.Errors.First());

                    //Add Tracking
                    var userId2 = User.Identity.GetUserId();
                    var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Created a role";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();

                    return View();
                }
                return RedirectToAction("Roles");
            }
            return View();
        }
        public async Task<ActionResult> ReconsileRegNumbers()
        {
            
            return View();
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]

        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ReconsileRegNumbers(string data)
        {
            var setting = db.Settings.OrderByDescending(x => x.Id).First();
            var students = db.StudentProfiles.AsEnumerable();
            foreach(var item in students)
            {
                string numberid = item.Id.ToString("D6");
                item.StudentRegNumber = setting.SchoolInitials + "/" + numberid;
                db.Entry(item).State = EntityState.Modified;
                 
            }
            await db.SaveChangesAsync();
            return View();
        }
        //
        // GET: /Roles/Edit/Admin
        public async Task<ActionResult> EditRole(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            RoleViewModel roleModel = new RoleViewModel { Id = role.Id, Name = role.Name };
            return View(roleModel);
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]

        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditRole(RoleViewModel roleModel)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleManager.FindByIdAsync(roleModel.Id);
                role.Name = roleModel.Name;
                await RoleManager.UpdateAsync(role);

                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Updated a role";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();

                return RedirectToAction("Roles");
            }
            return View();
        }

        //
        // GET: /Roles/Delete/5
        public async Task<ActionResult> DeleteRole(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        //
        // POST: /Roles/Delete/5
        [HttpPost, ActionName("DeleteRole")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id, string deleteUser)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var role = await RoleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return HttpNotFound();
                }
                IdentityResult result;
                if (deleteUser != null)
                {
                    result = await RoleManager.DeleteAsync(role);

                    //Add Tracking
                    var userId2 = User.Identity.GetUserId();
                    var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Deleted a role";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
                else
                {
                    result = await RoleManager.DeleteAsync(role);

                    //Add Tracking
                    var userId2 = User.Identity.GetUserId();
                    var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Deleted a role";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Roles");
            }
            return View();
        }

        public JsonResult LgaList(string Id)
        {
            var stateId = db.States.FirstOrDefault(x => x.StateName == Id).Id;
            var local = from s in db.LocalGovs
                        where s.StatesId == stateId
                        select s;

            return Json(new SelectList(local.ToArray(), "LGAName", "LGAName"), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// change student status via expelled ....
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public async Task<ActionResult> ChangeStudentStatus(string id)
        {
            var user = await _userService.GetUserByUserId(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> ChangeStudentStatus(ApplicationUser model)
        {
            try
            {
                if (model.Status == EntityStatus.Archived)
                {

                    model.Status = EntityStatus.Archived;
                    model.DataStatusChanged = DateTime.UtcNow.AddHours(1);
                    bool suceed = await _userService.UpdateUser(model);
                    if (suceed != true)

                    {
                        TempData["error"] = "change not successful";
                        return View(model);
                    }

                }
                else if (model.Status == EntityStatus.Suspeneded)
                {
                    model.Status = EntityStatus.Suspeneded;
                    model.DataStatusChanged = DateTime.UtcNow.AddHours(1);
                    bool suceed = await _userService.UpdateUser(model);
                    if (suceed != true)

                    {
                        TempData["error"] = "change not successful";
                        return View(model);
                    }
                }
                else if (model.Status == EntityStatus.Expelled)
                {
                    model.Status = EntityStatus.Expelled;
                    model.DataStatusChanged = DateTime.UtcNow.AddHours(1);
                    bool suceed = await _userService.UpdateUser(model);
                    if (suceed != true)

                    {
                        TempData["error"] = "change not successful";
                        return View(model);
                    }
                }
                else if (model.Status == EntityStatus.Withdrawn)
                {
                    model.Status = EntityStatus.Withdrawn;
                    model.DataStatusChanged = DateTime.UtcNow.AddHours(1);
                    bool suceed = await _userService.UpdateUser(model);
                    if (suceed != true)

                    {
                        TempData["error"] = "change not successful";
                        return View(model);
                    }
                }
                else if (model.Status == EntityStatus.Active)
                {
                    model.Status = EntityStatus.Active;
                    model.DataStatusChanged = DateTime.UtcNow.AddHours(1);
                    bool suceed = await _userService.UpdateUser(model);
                    if (suceed != true)

                    {
                        TempData["error"] = "change not successful";
                        return View(model);
                    }
                }
                else
                {
                    TempData["error"] = "You did not change the status";
                    return View(model);
                }

                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Updated Student Status";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();

                TempData["success"] = "Status changed Successfully";
                return RedirectToAction("Students");
            }
            catch (Exception e)
            {
                TempData["error"] = "An error occured. Try Again";

            }
            return View(model);


        }

        ////end

        /// <summary>
        /// change staff status via expelled ....
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public async Task<ActionResult> ChangeStaffStatus(string id)
        {
            var user = await _userService.GetUserByUserId(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        //[HttpPost]
        ////public async Task<ActionResult> ChangeStaffStatus(ApplicationUser model Status)
        //public async Task<ActionResult> ChangeStaffStatus(int Status, string Id)
        //{
        //    var model = await db.Users.FirstOrDefaultAsync(x => x.Id == Id);

        //    try
        //    {
        //        if (Status == 4)
        //        {

        //            model.Status = EntityStatus.Active;
        //            model.DataStatusChanged = DateTime.UtcNow.AddHours(1);
        //            bool suceed = await _userService.UpdateUser(model);
        //            if (suceed != true)

        //            {
        //                TempData["error"] = "change not successful";
        //                return RedirectToAction("ChangeStaffStatus", new { id = model.Id });
        //            }

        //        }
        //        else if (Status == 5)
        //        {
        //            model.Status = EntityStatus.Suspeneded;
        //            model.DataStatusChanged = DateTime.UtcNow.AddHours(1);
        //            bool suceed = await _userService.UpdateUser(model);
        //            if (suceed != true)

        //            {
        //                TempData["error"] = "change not successful";
        //                return View(model);
        //            }
        //        }
        //        else if (Status == 2)
        //        {
        //            model.Status = EntityStatus.Expelled;
        //            model.DataStatusChanged = DateTime.UtcNow.AddHours(1);
        //            bool suceed = await _userService.UpdateUser(model);
        //            if (suceed != true)

        //            {
        //                TempData["error"] = "change not successful";
        //                return RedirectToAction("ChangeStaffStatus", new { id = model.Id });
        //            }
        //        }
        //        else if (Status == 3)
        //        {
        //            model.Status = EntityStatus.Withdrawn;
        //            model.DataStatusChanged = DateTime.UtcNow.AddHours(1);
        //            bool suceed = await _userService.UpdateUser(model);
        //            if (suceed != true)

        //            {
        //                TempData["error"] = "change not successful";
        //                return RedirectToAction("ChangeStaffStatus", new { id = model.Id });
        //            }
        //        }
        //        else if (Status == 1)
        //        {
        //            model.Status = EntityStatus.Active;
        //            model.DataStatusChanged = DateTime.UtcNow.AddHours(1);

        //            bool suceed = await _userService.UpdateUser(model);
        //            if (suceed != true)

        //            {
        //                TempData["error"] = "change not successful";
        //                return RedirectToAction("ChangeStaffStatus", new { id = model.Id });
        //            }
        //        }
        //        else if (mStatus == 6)
        //        {
        //            model.Status = EntityStatus.NotActive;
        //            model.DataStatusChanged = DateTime.UtcNow.AddHours(1);
        //            bool suceed = await _userService.UpdateUser(model);
        //            if (suceed != true)

        //            {
        //                TempData["error"] = "change not successful";
        //                return RedirectToAction("ChangeStaffStatus", new { id = model.Id });
        //            }
        //        }
        //        else
        //        {
        //            TempData["error"] = "You did not change the status";
        //            return View(model);
        //        }

        //        TempData["success"] = "Status changed Successfully";
        //        return RedirectToAction("Staff");
        //    }
        //    catch (Exception e)
        //    {
        //        TempData["error"] = "An error occured. Try Again";

        //    }
        //    return RedirectToAction("ChangeStaffStatus", new { id = model.Id });


        //}

        //end


        public async Task<ActionResult> ChangeStatus(string id)
        {
            var user = await _userService.GetUserByUserId(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> ChangeStatus(int Status, string Id)
        {
            var model = await db.Users.FirstOrDefaultAsync(x => x.Id == Id);

            try
            {
                if (Status == 4)
                {

                    model.Status = EntityStatus.Active;
                    model.DataStatusChanged = DateTime.UtcNow.AddHours(1);
                    db.Entry(model).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("ChangeStatus", new { id = model.Id });

                }
                else if (Status == 5)
                {
                    model.Status = EntityStatus.Suspeneded;
                    model.DataStatusChanged = DateTime.UtcNow.AddHours(1);
                    db.Entry(model).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("ChangeStatus", new { id = model.Id });
                }
                else if (Status == 2)
                {
                    model.Status = EntityStatus.Expelled;
                    model.DataStatusChanged = DateTime.UtcNow.AddHours(1);
                    db.Entry(model).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("ChangeStatus", new { id = model.Id });
                }
                else if (Status == 3)
                {
                    model.Status = EntityStatus.Withdrawn;
                    model.DataStatusChanged = DateTime.UtcNow.AddHours(1);
                    db.Entry(model).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("ChangeStatus", new { id = model.Id });
                }
                else if (Status == 1)
                {
                    model.Status = EntityStatus.Active;
                    model.DataStatusChanged = DateTime.UtcNow.AddHours(1);
                    db.Entry(model).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("ChangeStatus", new { id = model.Id });

                }
                else if (Status == 6)
                {
                    model.Status = EntityStatus.NotActive;
                    model.DataStatusChanged = DateTime.UtcNow.AddHours(1);
                    db.Entry(model).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("ChangeStatus", new { id = model.Id });
                }
                else
                {
                    TempData["error"] = "You did not change the status";
                    return View(model);
                }

                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Updated User Status";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();

                TempData["success"] = "Status changed Successfully";
                return RedirectToAction("Staff");
            }
            catch (Exception e)
            {
                TempData["error"] = "An error occured. Try Again";

            }
            return RedirectToAction("ChangeStatus", new { id = model.Id });

        }


        public async Task<ActionResult> Edit(string id, string ReturnUrl)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.StateName = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName");
            var user = await _userService.GetUserByUserId(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReturnUrl = ReturnUrl;
            return View(user);
        }

        // POST: Admin/Settings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ApplicationUser model, string ReturnUrl, string reg)
        {
            if (model.Id != null)
            {
                try
                {
                    bool check = await _userService.UpdateUser(model);

                    var stprofile = await db.StudentProfiles.FirstOrDefaultAsync(x => x.UserId == model.Id);
                    //stprofile.StudentRegNumber = reg;
                    //db.Entry(stprofile).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    if (check == true)
                    {
                        //Add Tracking
                        var userId2 = User.Identity.GetUserId();
                        var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                        Tracker tracker = new Tracker();
                        tracker.UserId = userId2;
                        tracker.UserName = user2.UserName;
                        tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                        tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                        tracker.Note = tracker.FullName + " " + "Updated Student Profile";
                        //db.Trackers.Add(tracker);
                        await db.SaveChangesAsync();

                        TempData["success"] = "User Updated Successfully";
                        if (ReturnUrl == null)
                        {
                            return RedirectToAction("Edit", new { id = model.Id });
                        }
                        else
                        {
                            Redirect(ReturnUrl);
                        }

                    }
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        }
                    }
                }


            }
            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName", model.StateOfOrigin);
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            TempData["error"] = "Update not successful";
            ViewBag.ReturnUrl = ReturnUrl;
            return View(model);
        }



        //edit from doc

        public async Task<ActionResult> EditInfo(string id, string ReturnUrl)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.StateName = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName");
            var user = await _userService.GetUserByUserId(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReturnUrl = ReturnUrl;
            return View(user);
        }

        // POST: Admin/Settings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditInfo(ApplicationUser model, string ReturnUrl)
        {
            if (model.Id != null)
            {
                try
                {
                    bool check = await _userService.UpdateUser(model);
                    if (check == true)
                    {
                        TempData["success"] = "User Updated Successfully";
                        if (ReturnUrl == null)
                        {
                            return RedirectToAction("Index", "Doc", new { area = "Content" });
                        }
                        else
                        {
                            Redirect(ReturnUrl);
                        }

                    }
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        }
                    }
                }


            }
            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName", model.StateOfOrigin);
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            TempData["error"] = "Update not successful";
            ViewBag.ReturnUrl = ReturnUrl;
            return View(model);
        }




        public async Task<ActionResult> Islocked(string UserId)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == UserId);
            if (user.IsLocked == true)
            {
                user.IsLocked = false;
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();

                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Unlocked User Profile";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
            else
            {
                user.IsLocked = true;
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();

                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Locked User Profile";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }


        #region


        public async Task<ActionResult> New()
        {
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel, "Id", "ClassLevelName");
            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName");
            return View();
        }

        // POST: Admin/Sessions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]

        public async Task<ActionResult> New(RegisterViewModel model, HttpPostedFileBase upload, int classId, string LastSchoolAttended, string ParentName, string ParentAddress, string ParentPhone, string ParentOccupation)
        {
            var ee = "";
            if (ModelState.IsValid)
            {



                try
                {
                    string succed;

                    succed = await _userService.NewStudent(model, LastSchoolAttended, ParentName, ParentAddress, ParentPhone, ParentOccupation);
                    if (succed == "true")
                    {
                        var Imageid = await _imageService.Create(upload);
                        var user = await UserManager.FindByNameAsync(model.Username);
                        // var user = await _userService.GetUserByUserEmail(model.Email);
                        var student = await _studentService.GetStudentByUserId(user.Id);

                        //profile pic upload
                        await _studentService.UpdateImageId(student.Id, Imageid);

                        //enrolment
                        await _enrollmentService.EnrollStudent(classId, student.Id);
                        var classLevel = await _classlevelService.Get(classId);
                        TempData["success"] = "Student with username <i> " + model.Username + "</i> Added Successfully to " + classLevel.ClassName + " Class";
                        return RedirectToAction("New");
                    }
                    else
                    {
                        TempData["error1"] = succed;
                    }


                }
                catch (Exception e)
                {
                    ee = e.ToString();
                }

            }
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            TempData["error"] = "Creation of new student not successful" + ee;
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel, "Id", "ClassLevelName");
            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName", model.StateOfOrigin);
            return View(model);
        }





        public async Task<ActionResult> BatchReg()
        {
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel, "Id", "ClassLevelName");
            //ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName");
            return View();
        }

        // POST: Admin/UserManagers/BatchReg
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        public async Task<ActionResult> BatchReg(RegisterViewModel model, string LastSchoolAttended, int classId, string ParentName, string ParentAddress, string ParentPhone, string ParentOccupation, HttpPostedFileBase excelfile)
        {
            string succed;
            string ee = "";
            string path = "";
            string nameswitheerror = "";
            int sn = 1;
            if (excelfile == null || excelfile.ContentLength == 0)
            {
                TempData["error"] = "Please select an excel file";
                return RedirectToAction("BatchReg");

            }
            else
            {
                if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx"))
                {
                    path = Server.MapPath("~/ExcelUpload/" + excelfile.FileName);
                    if (System.IO.File.Exists(path))
                    {
                        TempData["error"] = "Excel file with same name exist try renaming the excel file";
                        return RedirectToAction("BatchReg");
                    }

                    //System.IO.File.Delete(path);

                    excelfile.SaveAs(path);

                    //Read Data From Excel file
                    Excel.Application application = new Excel.Application();
                    Excel.Workbook workbook = application.Workbooks.Open(path);
                    Excel.Worksheet worksheet = workbook.ActiveSheet;
                    Excel.Range range = worksheet.UsedRange;
                    List<RegisterViewModel> listUser = new List<RegisterViewModel>();

                    try
                    {
                        for (int row = 2; row <= range.Rows.Count; row++)
                        {
                            string userCheck = ((Excel.Range)range.Cells[row, 1]).Text;
                            var userexist = await UserManager.FindByNameAsync(userCheck);
                            if (userexist != null)
                            {
                                nameswitheerror = nameswitheerror + "(" + sn++ + ")" + " " + userCheck + " Username already exist! <br/>";

                            }
                            else
                            {



                                RegisterViewModel user = new RegisterViewModel();

                                user.Username = ((Excel.Range)range.Cells[row, 1]).Text;
                                user.Surname = ((Excel.Range)range.Cells[row, 2]).Text;
                                user.FirstName = ((Excel.Range)range.Cells[row, 3]).Text;
                                user.OtherName = ((Excel.Range)range.Cells[row, 4]).Text;
                                user.Email = ((Excel.Range)range.Cells[row, 5]).Text;

                                //listUser.Add(user);

                                //ViewBag.listUser = listUser.ToList();

                                string UserName = user.Username;
                                string SurName = user.Surname;
                                string FirstName = user.FirstName;
                                string OtherName = user.OtherName;
                                string Email = user.Email;



                                model.Username = UserName;
                                model.Password = "123456";
                                model.ConfirmPassword = "123456";
                                model.Surname = SurName;
                                model.FirstName = FirstName;
                                model.OtherName = OtherName;
                                model.Email = Email;


                                //Create User Account


                                succed = await _userService.BatchReg(model, LastSchoolAttended, ParentName, ParentAddress, ParentPhone, ParentOccupation);

                                if (succed == "true")
                                {
                                    var user1 = await UserManager.FindByNameAsync(model.Username);
                                    var student = await _studentService.GetStudentByUserId(user1.Id);

                                    //enrolment
                                    await _enrollmentService.EnrollStudent(classId, student.Id);
                                    var classLevel = await _classlevelService.Get(classId);
                                }
                                else
                                {

                                    TempData["error"] = succed;

                                }
                            }
                        }
                    }

                    catch (Exception e)
                    {
                        ee = e.ToString();
                    }
                    //Add Tracking
                    var userId2 = User.Identity.GetUserId();
                    var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Upload Batch Result Excel Registration";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                    TempData["msg"] = "Uploaded successfully.";
                    TempData["batcherror"] = nameswitheerror;
                    return RedirectToAction("BatchReg");


                }
                else
                {

                    TempData["error"] = "file type incorrect";

                }

                var allErrors = ModelState.Values.SelectMany(v => v.Errors);
                TempData["error1"] = ee;

                var classlevel = await _classlevelService.ClassLevelList();
                var classstudent = await _classlevelService.Students(classId);
                var sessId = await _sessionService.GetCurrentSessionId();
                var enrollStudent = _enrollmentService.EnrolledStudentBySessionClassId(sessId, classId);
                ViewBag.enrollStudent = enrollStudent;
                ViewBag.ClassStudent = classstudent.ToList();
                ViewBag.ClassLevelId = new SelectList(classlevel, "Id", "ClassLevelName");
                //ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName", model.StateOfOrigin);
                return View(model);

            }
        }



        public static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            string value = cell.CellValue.InnerXml;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }

            else
            {
                return value;
            }



        }





        //[HttpPost]
        //public async Task<ActionResult> BatchReg(RegisterViewModel model, HttpPostedFileBase upload, int classId, string LastSchoolAttended, string ParentName, string ParentAddress, string ParentPhone, string ParentOccupation)
        //{

        //    string data = "";
        //    if (upload != null)
        //    {
        //        if (upload.ContentType == "application/vnd.ms-excel" || upload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        //        {
        //            string filename = upload.FileName;

        //            if (filename.EndsWith(".xlsx"))
        //            {
        //                string targetpath = Server.MapPath("~/ExcelUpload/");
        //                upload.SaveAs(targetpath + filename);
        //                string pathToExcelFile = targetpath + filename;

        //                string sheetName = "Sheet1";

        //                var excelFile = new ExcelQueryFactory(pathToExcelFile);
        //                var empDetails = from a in excelFile.Worksheet<RegisterViewModel>(sheetName) select a;
        //                foreach (var a in empDetails)
        //                {
        //                    if (a.Username != null)
        //                    {
        //                        await _userService.NewStudent(model, LastSchoolAttended, ParentName, ParentAddress, ParentPhone, ParentOccupation);
        //                        var user = await UserManager.FindByNameAsync(model.Username);
        //                        // var user = await _userService.GetUserByUserEmail(model.Email);
        //                        var student = await _studentService.GetStudentByUserId(user.Id);

        //                        //enrolment
        //                        await _enrollmentService.EnrollStudent(classId, student.Id);
        //                        var classLevel = await _classlevelService.Get(classId);
        //                    }

        //                    else
        //                    {
        //                        data = a.Username + "Some fields are null, Please check your excel sheet";
        //                        ViewBag.Message = data;
        //                        return RedirectToAction("BatchUpload");
        //                    }

        //                }
        //            }

        //            else
        //            {
        //                data = "This file is not valid format";
        //                ViewBag.Message = data;
        //            }
        //            return RedirectToAction("BatchUpload");
        //        }
        //        else
        //        {

        //            data = "Only Excel file format is allowed";

        //            ViewBag.Message = data;
        //            return RedirectToAction("BatchUpload");

        //        }

        //    }
        //    else
        //    {

        //        if (upload == null)
        //        {
        //            data = "Please choose Excel file";
        //        }

        //        ViewBag.Message = data;
        //        return RedirectToAction("BatchUpload");
        //    }
        //}






        //public async Task<ActionResult> New()
        //{
        //    var classlevel = await _classlevelService.ClassLevelList();
        //    ViewBag.ClassLevelId = new SelectList(classlevel, "Id", "ClassLevelName");
        //    ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName");
        //    return View();
        //}

        //// POST: Admin/Sessions/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        ////[ValidateAntiForgeryToken]

        //public async Task<ActionResult> New(RegisterViewModel model, int classId, string LastSchoolAttended)
        //{

        //    var ee = "";
        //    if (ModelState.IsValid)
        //    {



        //        try
        //        {
        //            string succed;

        //            succed = await _userService.New(model, LastSchoolAttended);
        //            if (succed == "true")
        //            {

        //                var user = await UserManager.FindByNameAsync(model.Username);
        //                // var user = await _userService.GetUserByUserEmail(model.Email);
        //                var student = await _studentService.GetStudentByUserId(user.Id);


        //                //enrolment
        //                await _enrollmentService.EnrollStudent(classId, student.Id);
        //                var classLevel = await _classlevelService.Get(classId);
        //                TempData["success"] = "Student with username <i> " + model.Username + "</i> Added Successfully to " + classLevel.ClassName + " Class";
        //                return RedirectToAction("New");
        //            }
        //            else
        //            {
        //                TempData["error1"] = succed;
        //            }


        //        }
        //        catch (Exception e)
        //        {
        //            ee = e.ToString();
        //        }

        //    }
        //    var allErrors = ModelState.Values.SelectMany(v => v.Errors);
        //    TempData["error"] = "Creation of new student not successful" + ee;
        //    var classlevel = await _classlevelService.ClassLevelList();
        //    ViewBag.ClassLevelId = new SelectList(classlevel, "Id", "ClassLevelName");
        //    ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName", model.StateOfOrigin);
        //    return View(model);
        //}






        #endregion

        #region delete 

        // GET: 
        public async Task<ActionResult> Delete(string id, string msg)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var enrolledStudent = await db.Enrollments.Include(x => x.ClassLevel).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.EnrolledSubjects).Where(x => x.StudentProfile.user.Id == id).ToListAsync();
            ViewBag.id = id;

            if (enrolledStudent == null)
            {
                return HttpNotFound();
            }
            ViewBag.r = msg;
            string allabout = "";
            string allabout2 = "";
            var infouser = UserManager.FindById(id);
            var subs = db.Subjects.Include(x => x.ClassLevel).Where(x => x.UserId == id).OrderBy(x => x.SubjectName).ToList();

            foreach (var i in subs.OrderByDescending(x => x.ClassLevel.ClassName))
            {
                allabout = allabout + i.SubjectName + " /" + i.ClassLevel.ClassName + "====subjects===<br/>";
            }

            var classes = db.ClassLevels.Where(x => x.UserId == id).OrderBy(x => x.ClassName).ToList();

            foreach (var i in classes)
            {
                allabout2 = allabout2 + i.ClassName + "====CLasses===<br/>";
            }

            ViewBag.name = infouser.Surname + " " + infouser.FirstName + " " + infouser.OtherName + " (" + infouser.UserName + ") ";
            ViewBag.i1 = allabout;
            ViewBag.i11 = allabout2;

            return View(enrolledStudent);
        }

        // POST:

        public async Task<ActionResult> XremoveDelete(string id)
        {
            string r = "";
            try
            {
                var enrolledStudent = await db.Enrollments.Include(x => x.ClassLevel).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.EnrolledSubjects).Where(x => x.StudentProfile.user.Id == id).ToListAsync();
                try
                {
                    foreach (var i in enrolledStudent)
                    {


                        var sub = db.EnrolledSubjects.Where(x => x.EnrollmentId == i.Id);
                        foreach (var a in sub)
                        {
                            db.EnrolledSubjects.Remove(a);
                        }

                        var attend = db.AttendanceDetails.Where(x => x.StudentId == i.StudentProfileId).ToList();
                        foreach (var att in attend)
                        {
                            db.AttendanceDetails.Remove(att);
                        }
                        var af1 = db.AffectiveDomains.Where(x => x.EnrolmentId == i.Id).ToList();
                        foreach (var att in af1)
                        {
                            db.AffectiveDomains.Remove(att);
                        }
                        var af2 = db.AssignmentAnswers.Where(x => x.UserId == i.StudentProfile.user.Id).ToList();
                        foreach (var att in af2)
                        {
                            db.AssignmentAnswers.Remove(att);
                        }
                        var af3 = db.Attendances.Where(x => x.UserId == i.StudentProfile.user.Id).ToList();
                        foreach (var att in af3)
                        {
                            db.Attendances.Remove(att);
                        }
                        var af4 = db.AttendanceDetails.Where(x => x.UserId == i.StudentProfile.user.Id).ToList();
                        foreach (var att in af4)
                        {
                            db.AttendanceDetails.Remove(att);
                        }

                        var af5 = db.BatchResults.Where(x => x.EnrollmentId == i.Id).ToList();
                        foreach (var att in af5)
                        {
                            db.BatchResults.Remove(att);
                        }

                        var af6 = db.PsychomotorDomains.Where(x => x.EnrolmentId == i.Id).ToList();
                        foreach (var att in af6)
                        {
                            db.PsychomotorDomains.Remove(att);
                        }

                        var af7 = db.RecognitiveDomains.Where(x => x.EnrolmentId == i.Id).ToList();
                        foreach (var att in af7)
                        {
                            db.RecognitiveDomains.Remove(att);
                        }

                        var af8 = db.SchoolFees.Where(x => x.EnrolmentId == i.Id).ToList();
                        foreach (var att in af8)
                        {
                            db.SchoolFees.Remove(att);
                        }

                        var profile3 = await db.Defaulters.FirstOrDefaultAsync(x => x.ProfileId == i.StudentProfileId);
                        if (profile3 != null)
                        {
                            db.Defaulters.Remove(profile3);

                        }

                        db.Enrollments.Remove(i);
                        db.SaveChanges();

                    }

                    //Add Tracking
                    var userId2 = User.Identity.GetUserId();
                    var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Removed Student from erollment";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                    TempData["a8"] = "Removed from enrollment";
                }
                catch (Exception c)
                {

                }
                var profile = await db.StaffProfiles.FirstOrDefaultAsync(x => x.UserId == id);

                if (profile != null)
                {
                    db.StaffProfiles.Remove(profile);
                    TempData["a6"] = "Removed from table staff profile";
                    db.SaveChanges();

                    //Add Tracking
                    var userId2 = User.Identity.GetUserId();
                    var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Remove Staff Profile";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }


                var profile2 = await db.StudentProfiles.FirstOrDefaultAsync(x => x.UserId == id);
                if (profile2 != null)
                {
                    db.StudentProfiles.Remove(profile2);
                    db.SaveChanges();
                    TempData["a5"] = "Removed from table student profile";

                    //Add Tracking
                    var userId2 = User.Identity.GetUserId();
                    var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Remove student profile";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }

                db.SaveChanges();
                try
                {
                    var result = await UserManager.RemoveFromRoleAsync(id, "Student");
                    if (result.Succeeded)
                    {
                        TempData["a4"] = "Removed from role student";
                    }
                }
                catch (Exception f) { }

                try
                {
                    var result2 = await UserManager.RemoveFromRoleAsync(id, "Staff");
                    if (result2.Succeeded)
                    {
                        TempData["a3"] = "Removed from role staff";
                    }
                }
                catch (Exception f) { }

                ////
                ///
                var adminrole = RoleManager.FindByName("Admin");
                var user = UserManager.Users.FirstOrDefault(x => x.Roles.Select(c => c.RoleId).Contains(adminrole.Id)).Id;
                var subs = db.Subjects.Include(x => x.ClassLevel).Where(x => x.UserId == id).OrderBy(x => x.SubjectName).ToList();
                try
                {


                    foreach (var i in subs.OrderByDescending(x => x.ClassLevel.ClassName))
                    {
                        i.UserId = user;
                        db.Entry(i).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                    }
                    //Add Tracking
                    var userId2 = User.Identity.GetUserId();
                    var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Removed Staff from assigned subject";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                    TempData["a1"] = "Removed from asigned Subjects";
                }
                catch (Exception d) { }
                var classes = db.ClassLevels.Where(x => x.UserId == id).OrderBy(x => x.ClassName).ToList();
                try
                {
                    foreach (var i in classes)
                    {
                        i.UserId = user;
                        db.Entry(i).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                    //Add Tracking
                    var userId2 = User.Identity.GetUserId();
                    var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Removed staff from assigned class";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                    TempData["a2"] = "Removed from asigned classes";
                }
                catch (Exception d) { }
                var userentity = await UserManager.FindByIdAsync(id);
                UserManager.Delete(userentity);

                //Add Tracking
                var userId3 = User.Identity.GetUserId();
                var user3 = UserManager.Users.Where(x => x.Id == userId3).FirstOrDefault();
                Tracker tracker2 = new Tracker();
                tracker2.UserId = userId3;
                tracker2.UserName = user3.UserName;
                tracker2.FullName = user3.Surname + " " + user3.FirstName + " " + user3.OtherName;
                tracker2.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker2.Note = tracker2.FullName + " " + "Deleted User";
                db.Trackers.Add(tracker2);
                await db.SaveChangesAsync();
                TempData["a7"] = "Removed from table user";




                r = "Deleted successfull";
                //remove user from profile role user app
                return RedirectToAction("Success", new { msg = r });
            }
            catch (Exception c)
            {




            }
            r = "unable to delete";
            return RedirectToAction("Delete", new { id = id, msg = r });
        }


        public ActionResult Success(string msg)
        {
            ViewBag.m = msg;
            return View();
        }
        #endregion

        public async Task<ActionResult> RemoveWithScore(int id = 0)
        {
            //  var enrolledStudent = null;
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == Models.Entities.SessionStatus.Current);

            var enrolledStudent = await db.Enrollments.Include(x => x.ClassLevel).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.EnrolledSubjects).FirstOrDefaultAsync(x => x.Id == id);
            string name = enrolledStudent.StudentProfile.user.Surname + " " + enrolledStudent.StudentProfile.user.FirstName + " " + enrolledStudent.StudentProfile.user.OtherName;
            string classs = enrolledStudent.ClassLevel.ClassName;
            try
            {

                var sub = db.EnrolledSubjects.Where(x => x.EnrollmentId == enrolledStudent.Id);
                foreach (var a in sub)
                {
                    db.EnrolledSubjects.Remove(a);
                }

                var attend = db.AttendanceDetails.Where(x => x.StudentId == enrolledStudent.StudentProfileId).ToList();
                foreach (var att in attend)
                {
                    db.AttendanceDetails.Remove(att);
                }
                db.Enrollments.Remove(enrolledStudent);
                db.SaveChanges();

                //Add Tracking
                var userId2 = User.Identity.GetUserId();
                var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user2.UserName;
                tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Removed student from enrollment";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
                TempData["success"] = name + " has successfully been removed from " + classs + " Class";



                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["error"] = "Removal was not successfull. Please try again.";

                return RedirectToAction("Index");
            }



        }

        #region off staff Registration

        // GET: Admin/Sessions/Create
        [AllowAnonymous]
        public ActionResult StaffRegistration()
        {
            var sett = db.Settings.FirstOrDefault().SchoolName;
            ViewBag.schname = sett;
            ViewBag.StateName = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName");
            return View();
        }

        // POST: Admin/Sessions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> StaffRegistration(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string succed;
                succed = await _userService.NewStaff(model);
                if (succed == "true")
                {
                    var user = UserManager.FindByName(model.Username);
                    TempData["success"] = "Staff with username <i> " + model.Username + "</i> Added Successfully.";
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    if (await SignInManager.UserManager.IsInRoleAsync(user.Id, "Staff"))
                    {
                        //Add Tracking
                        var userId2 = User.Identity.GetUserId();
                        var user2 = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                        Tracker tracker = new Tracker();
                        tracker.UserId = userId2;
                        tracker.UserName = user2.UserName;
                        tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                        tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                        tracker.Note = tracker.FullName + " " + "Registered as a staff";
                        //db.Trackers.Add(tracker);
                        await db.SaveChangesAsync();

                        return RedirectToAction("Index", "Panel", new { area = "Staff" });
                    }
                    else
                    {
                        TempData["error"] = "Try Again or Contact Your Administration";
                        return RedirectToAction("Login", "Account", new { area = "" });
                    }
                }
                else
                {


                    TempData["error1"] = succed;
                }

            }
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            TempData["error"] = "Creation of new staff not successful";
            ViewBag.StateOfOrigin = new SelectList(db.States.OrderBy(x => x.StateName), "StateName", "StateName", model.StateOfOrigin);
            return View(model);
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