using SchoolPortal.Web.Areas.Data.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using SchoolPortal.Web.Models.Dtos;
using System.Threading.Tasks;
using SchoolPortal.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SchoolPortal.Web.Models.Entities;

namespace SchoolPortal.Web.Areas.Data.Services
{

    public class UserManagerService : IUserManagerService
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        public UserManagerService()
        {
        }

        public UserManagerService(ApplicationUserManager userManager,
            ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
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
                return _roleManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }


        public Task Delete(int? id)
        {
            throw new NotImplementedException();
        }

        //public Task Edit(NewStaffDto models)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<StaffProfile> GetStaff(int? id)
        {
            var staff = await db.StaffProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.Id == id);
            if (staff != null)
            {
                return staff;
            }
            return null;
        }

        public async Task<StudentProfile> GetStudent(int? id)
        {
            var student = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.Id == id);
            if (student != null)
            {
                return student;
            }
            return null;
        }

        public async Task<ApplicationUser> GetUserByUserId(string id)
        {
            var student = await UserManager.FindByIdAsync(id);
            if (student != null)
            {
                return student;
            }
            return null;
        }



        public async Task<string> NewStaff(RegisterViewModel model)
        {
            var setting = db.Settings.OrderByDescending(x => x.Id).First();
            var officer = HttpContext.Current.User.Identity.GetUserName();
            if (officer == "SuperAdmin")
            {
                officer = "Admin";
            }
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Surname = model.Surname,
                Email = model.Email,
                FirstName = model.FirstName,
                OtherName = model.OtherName,
                DateOfBirth = model.DateOfBirth,
                Religion = model.Religion,
                DateRegistered = DateTime.UtcNow.AddHours(1),
                Phone = model.Phone,
                ContactAddress = model.ContactAddress,
                City = model.City,
                Status = EntityStatus.Active,
                StateOfOrigin = model.StateOfOrigin,
                Nationality = model.Nationality,
                RegisteredBy = officer
            };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await UserManager.AddToRoleAsync(user.Id, "Staff");
                StaffProfile staff = new StaffProfile();
                staff.UserId = user.Id;
                staff.DateOfAppointment = DateTime.UtcNow;
                db.StaffProfiles.Add(staff);
                await db.SaveChangesAsync();

                var staffReg = await db.StaffProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                string numberid = staffReg.Id.ToString("D3");
                //staffReg.StaffRegistrationId = setting.SchoolInitials + "/STAFF/00" + staffReg.Id;
                staffReg.StaffRegistrationId = setting.SchoolInitials + "/STAFF/" + numberid;
                db.Entry(staffReg).State = EntityState.Modified;
                await db.SaveChangesAsync();

                //email verifiation

                //Add Tracking
                var userId = HttpContext.Current.User.Identity.GetUserId();
                if(userId != null)
                {
                    var user2 = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Added a new staff";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
               


                return "true";
            }
          

            var errors = result.Errors;
            var message = string.Join(", ", errors);

            return message;
        }



        public async Task<string> NewStudent(RegisterViewModel model, string LastSchoolAttended, string ParentName, string ParentAddress, string ParentPhone, string ParentOccupation)
        {
            var setting = db.Settings.OrderByDescending(x => x.Id).First();
            var officer = HttpContext.Current.User.Identity.GetUserName();
            if (officer == "SuperAdmin")
            {
                officer = "Admin";
            }
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                Surname = model.Surname,
                FirstName = model.FirstName,
                OtherName = model.OtherName,
                DateOfBirth = model.DateOfBirth,
                Religion = model.Religion,
                Phone = model.Phone,
                DateRegistered = DateTime.UtcNow.AddHours(1),
                ContactAddress = model.ContactAddress,
                City = model.City,
                Status = EntityStatus.Active,
                StateOfOrigin = model.StateOfOrigin,
                Nationality = model.Nationality,
                RegisteredBy = officer
            };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await UserManager.AddToRoleAsync(user.Id, "Student");

                StudentProfile student = new StudentProfile();
                student.UserId = user.Id;
                student.LastPrimarySchoolAttended = LastSchoolAttended;
                student.ParentGuardianName = ParentName;
                student.ParentGuardianAddress = ParentAddress;
                student.ParentGuardianPhoneNumber = ParentPhone;
                student.ParentGuardianOccupation = ParentOccupation;
                db.StudentProfiles.Add(student);
                await db.SaveChangesAsync();

                var studentReg = await db.StudentProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                string numberid = studentReg.Id.ToString("D6");
                studentReg.StudentRegNumber = setting.SchoolInitials + "/" + numberid;
                db.Entry(studentReg).State = EntityState.Modified;
                await db.SaveChangesAsync();

                //Add Tracking
                var userId = HttpContext.Current.User.Identity.GetUserId();
                if(userId != null)
                {
                    var user2 = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Added a new student";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
               

                return "true";
            }

            string error = string.Join(" ", result.Errors);
            return error;
        }

        /// <summary>
        /// short user registration
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="currentFilter"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        /// 

        public async Task<string> New(RegisterViewModel model, string LastSchoolAttended)
        {
            var setting = db.Settings.OrderByDescending(x => x.Id).First();
            var officer = HttpContext.Current.User.Identity.GetUserName();
            if (officer == "SuperAdmin")
            {
                officer = "Admin";
            }
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = setting.ContactEmail,
                Surname = model.Surname,
                FirstName = model.FirstName,
                OtherName = model.OtherName,
                DateOfBirth = DateTime.UtcNow,
                Religion = model.Religion,
                Phone = model.Phone,
                DateRegistered = DateTime.UtcNow.AddHours(1),
                ContactAddress = model.ContactAddress,
                City = model.City,
                Status = EntityStatus.Active,
                StateOfOrigin = model.StateOfOrigin,
                Nationality = model.Nationality,
                RegisteredBy = officer
            };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await UserManager.AddToRoleAsync(user.Id, "Student");

                StudentProfile student = new StudentProfile();
                student.UserId = user.Id;
                student.LastPrimarySchoolAttended = LastSchoolAttended;

                db.StudentProfiles.Add(student);
                await db.SaveChangesAsync();

                var studentReg = await db.StudentProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                string numberid = studentReg.Id.ToString("D6");
                studentReg.StudentRegNumber = setting.SchoolInitials + "/" + numberid;
                db.Entry(studentReg).State = EntityState.Modified;
                await db.SaveChangesAsync();

                //Add Tracking
                var userId = HttpContext.Current.User.Identity.GetUserId();
                if(userId != null)
                {
                    var user2 = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Added a new student";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
               

                return "true";
            }

            string error = string.Join(" ", result.Errors);
            return error;
        }


        public async Task<string> BatchReg(RegisterViewModel model, string LastSchoolAttended, string ParentName, string ParentAddress, string ParentPhone, string ParentOccupation)
        {
            var setting = db.Settings.OrderByDescending(x => x.Id).First();
            var officer = HttpContext.Current.User.Identity.GetUserName();
            if (officer == "SuperAdmin")
            {
                officer = "Admin";
            }
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                Surname = model.Surname,
                FirstName = model.FirstName,
                OtherName = model.OtherName,
                DateOfBirth = model.DateOfBirth,
                Religion = model.Religion,
                Phone = model.Phone,
                DateRegistered = DateTime.UtcNow.AddHours(1),
                ContactAddress = model.ContactAddress,
                City = model.City,
                Status = EntityStatus.Active,
                StateOfOrigin = model.StateOfOrigin,
                Nationality = model.Nationality,
                RegisteredBy = officer
            };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await UserManager.AddToRoleAsync(user.Id, "Student");

                StudentProfile student = new StudentProfile();
                student.UserId = user.Id;
                student.LastPrimarySchoolAttended = LastSchoolAttended;
                student.ParentGuardianName = ParentName;
                student.ParentGuardianAddress = ParentAddress;
                student.ParentGuardianPhoneNumber = ParentPhone;
                student.ParentGuardianOccupation = ParentOccupation;
                db.StudentProfiles.Add(student);
                await db.SaveChangesAsync();

                var studentReg = await db.StudentProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                string numberid = studentReg.Id.ToString("D6");
                studentReg.StudentRegNumber = setting.SchoolInitials + "/" + numberid;
                db.Entry(studentReg).State = EntityState.Modified;
                await db.SaveChangesAsync();

                //Add Tracking
                var userId = HttpContext.Current.User.Identity.GetUserId();
                if(userId != null)
                {
                    var user2 = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId;
                    tracker.UserName = user2.UserName;
                    tracker.FullName = user2.Surname + " " + user2.FirstName + " " + user2.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Added a new student";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
               
                return "true";
            }

            string error = string.Join(" ", result.Errors);
            return error;
        }


        public async Task<List<StudentProfile>> ListStudent(string searchString, string currentFilter, int? page)
        {
            var list = db.StudentProfiles.Include(x => x.user).Where(x => x.user.Status == EntityStatus.Active);
            if (!String.IsNullOrEmpty(searchString))
            {
                if (CountString(searchString) > 1)
                {
                    string[] searchStringCollection = searchString.Split(' ');

                    foreach (var item in searchStringCollection)
                    {
                        list = list.Where(s => s.user.Surname.ToUpper().Contains(item.ToUpper()) || s.user.FirstName.ToUpper().Contains(item.ToUpper())
                                                               || s.user.OtherName.ToUpper().Contains(item.ToUpper()) || s.StudentRegNumber.ToUpper().Contains(item.ToUpper()) || s.user.UserName.ToUpper().Contains(item.ToUpper()));
                    }
                }
                else
                {
                    list = list.Where(s => s.user.Surname.ToUpper().Contains(searchString.ToUpper()) || s.user.FirstName.ToUpper().Contains(searchString.ToUpper())
                                                               || s.user.OtherName.ToUpper().Contains(searchString.ToUpper()) || s.StudentRegNumber.ToUpper().Contains(searchString.ToUpper()) || s.user.UserName.ToUpper().Contains(searchString.ToUpper()));
                }

            }
            return await list.ToListAsync();
        }

        public async Task<List<StaffProfile>> ListStaff(string searchString, string currentFilter, int? page)
        {
            var list = db.StaffProfiles.Include(x => x.user).Where(x => x.user.Status == EntityStatus.Active);
            if (!String.IsNullOrEmpty(searchString))
            {
                if (CountString(searchString) > 1)
                {
                    string[] searchStringCollection = searchString.Split(' ');

                    foreach (var item in searchStringCollection)
                    {
                        list = list.Where(s => s.user.Surname.ToUpper().Contains(item.ToUpper()) || s.user.FirstName.ToUpper().Contains(item.ToUpper())
                                                               || s.user.OtherName.ToUpper().Contains(item.ToUpper()) || s.StaffRegistrationId.ToUpper().Contains(item.ToUpper()) || s.user.UserName.ToUpper().Contains(item.ToUpper()));
                    }
                }
                else
                {
                    list = list.Where(s => s.user.Surname.ToUpper().Contains(searchString.ToUpper()) || s.user.FirstName.ToUpper().Contains(searchString.ToUpper())
                                                               || s.user.OtherName.ToUpper().Contains(searchString.ToUpper()) || s.StaffRegistrationId.ToUpper().Contains(searchString.ToUpper()) || s.user.UserName.ToUpper().Contains(searchString.ToUpper()));
                }

            }
            return await list.ToListAsync();
        }
        public async Task<List<StaffProfile>> ListNonActiveStaff(string searchString, string currentFilter, int? page)
        {
            var list = db.StaffProfiles.Include(x => x.user).Where(x => x.user.Status == EntityStatus.NotActive);
            if (!String.IsNullOrEmpty(searchString))
            {
                if (CountString(searchString) > 1)
                {
                    string[] searchStringCollection = searchString.Split(' ');

                    foreach (var item in searchStringCollection)
                    {
                        list = list.Where(s => s.user.Surname.ToUpper().Contains(item.ToUpper()) || s.user.FirstName.ToUpper().Contains(item.ToUpper())
                                                               || s.user.OtherName.ToUpper().Contains(item.ToUpper()) || s.StaffRegistrationId.ToUpper().Contains(item.ToUpper()) || s.user.UserName.ToUpper().Contains(item.ToUpper()));
                    }
                }
                else
                {
                    list = list.Where(s => s.user.Surname.ToUpper().Contains(searchString.ToUpper()) || s.user.FirstName.ToUpper().Contains(searchString.ToUpper())
                                                               || s.user.OtherName.ToUpper().Contains(searchString.ToUpper()) || s.StaffRegistrationId.ToUpper().Contains(searchString.ToUpper()) || s.user.UserName.ToUpper().Contains(searchString.ToUpper()));
                }

            }
            return await list.ToListAsync();
        }

        public async Task<List<ApplicationUser>> UserAll()
        {

            //var users = UserManager.Users.Where(x => x.UserName != "SuperAdmin" && x.UserName != "Education Sec").OrderBy(x => x.UserName);
            var users = UserManager.Users.Where(x => x.UserName != "SuperAdmin" && x.Status == EntityStatus.Active).OrderBy(x => x.UserName);
            return await users.ToListAsync();
        }




        public async Task<List<ApplicationUser>> AllUsers(string searchString, string currentFilter, int? page)
        {

            //var users = UserManager.Users.Where(x => x.UserName != "SuperAdmin" && x.UserName != "Education Sec");
            var users = UserManager.Users.Where(x => x.UserName != "SuperAdmin");
            if (!String.IsNullOrEmpty(searchString))
            {
                if (CountString(searchString) > 1)
                {
                    string[] searchStringCollection = searchString.Split(' ');

                    foreach (var item in searchStringCollection)
                    {
                        users = users.Where(s => s.Surname.ToUpper().Contains(item.ToUpper()) || s.FirstName.ToUpper().Contains(item.ToUpper())
                                                               || s.OtherName.ToUpper().Contains(item.ToUpper()) || s.UserName.ToUpper().Contains(item.ToUpper()));
                    }
                }
                else
                {
                    users = users.Where(s => s.Surname.ToUpper().Contains(searchString.ToUpper()) || s.FirstName.ToUpper().Contains(searchString.ToUpper())
                                                               || s.OtherName.ToUpper().Contains(searchString.ToUpper()) || s.UserName.ToUpper().Contains(searchString.ToUpper()));
                }

            }

            return await users.OrderBy(x => x.Surname).ToListAsync();
        }

        public async Task<bool> IsUsersinRole(string userid, string role)
        {
            var users = await _userManager.IsInRoleAsync(userid, role);
            return users;
        }
        ///
        public int CountString(string searchString)
        {
            int result = 0;

            searchString = searchString.Trim();

            if (searchString == "")
                return 0;

            while (searchString.Contains("  "))
                searchString = searchString.Replace("  ", " ");

            foreach (string y in searchString.Split(' '))

                result++;


            return result;
        }


        public async Task<List<ApplicationUser>> Users()
        {
            return (await UserManager.Users.ToListAsync());
        }

        public async Task AddUserToRole(string userId, string rolename)
        {
            await UserManager.AddToRoleAsync(userId, rolename);

            //Add Tracking
            var userId2 = HttpContext.Current.User.Identity.GetUserId();
            if(userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Added a user to role";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }
        public async Task RemoveUserToRole(string userId, string rolename)
        {
            await UserManager.RemoveFromRoleAsync(userId, rolename);

            //Add Tracking
            var userId2 = HttpContext.Current.User.Identity.GetUserId();
            if(userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId2;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Removed a user from a role";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
            
        }

        public async Task<bool> UpdateUser(ApplicationUser model)
        {
            try
            {
                //IdentityResult check = await UserManager.UpdateAsync(model);
                // if (check.Succeeded)
                db.Entry(model).State = EntityState.Modified;
                await db.SaveChangesAsync();

                //Add Tracking
                var userId2 = HttpContext.Current.User.Identity.GetUserId();
                if(userId2 != null)
                {
                    var user = UserManager.Users.Where(x => x.Id == userId2).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId2;
                    tracker.UserName = user.UserName;
                    tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Updated a user";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
              
                return true;
            }
            catch (Exception c)
            {

            }

            return false;


        }

        public async Task<List<ApplicationUser>> Active(string searchString, string currentFilter, int? page)
        {
            var users = UserManager.Users.Where(x => x.Status == EntityStatus.Active && x.UserName != "SuperAdmin" && x.UserName != "Education Sec");
            if (!String.IsNullOrEmpty(searchString))
            {
                if (CountString(searchString) > 1)
                {
                    string[] searchStringCollection = searchString.Split(' ');

                    foreach (var item in searchStringCollection)
                    {
                        users = users.Where(s => s.Surname.ToUpper().Contains(item.ToUpper()) || s.FirstName.ToUpper().Contains(item.ToUpper())
                                                               || s.OtherName.ToUpper().Contains(item.ToUpper()) || s.UserName.ToUpper().Contains(item.ToUpper()));
                    }
                }
                else
                {
                    users = users.Where(s => s.Surname.ToUpper().Contains(searchString.ToUpper()) || s.FirstName.ToUpper().Contains(searchString.ToUpper())
                                                               || s.OtherName.ToUpper().Contains(searchString.ToUpper()) || s.UserName.ToUpper().Contains(searchString.ToUpper()));
                }

            }

            return await users.OrderBy(x => x.UserName).ToListAsync();
        }

        public async Task<List<ApplicationUser>> Expelled(string searchString, string currentFilter, int? page)
        {
            var users = UserManager.Users.Where(x => x.Status == EntityStatus.Expelled && x.UserName != "SuperAdmin" && x.UserName != "Education Sec");
            if (!String.IsNullOrEmpty(searchString))
            {
                if (CountString(searchString) > 1)
                {
                    string[] searchStringCollection = searchString.Split(' ');

                    foreach (var item in searchStringCollection)
                    {
                        users = users.Where(s => s.Surname.ToUpper().Contains(item.ToUpper()) || s.FirstName.ToUpper().Contains(item.ToUpper())
                                                               || s.OtherName.ToUpper().Contains(item.ToUpper()) || s.UserName.ToUpper().Contains(item.ToUpper()));
                    }
                }
                else
                {
                    users = users.Where(s => s.Surname.ToUpper().Contains(searchString.ToUpper()) || s.FirstName.ToUpper().Contains(searchString.ToUpper())
                                                               || s.OtherName.ToUpper().Contains(searchString.ToUpper()) || s.UserName.ToUpper().Contains(searchString.ToUpper()));
                }

            }

            return await users.OrderBy(x => x.UserName).ToListAsync();
        }

        public async Task<List<ApplicationUser>> Withdrawn(string searchString, string currentFilter, int? page)
        {
            var users = UserManager.Users.Where(x => x.Status == EntityStatus.Withdrawn && x.UserName != "SuperAdmin" && x.UserName != "Education Sec");
            if (!String.IsNullOrEmpty(searchString))
            {
                if (CountString(searchString) > 1)
                {
                    string[] searchStringCollection = searchString.Split(' ');

                    foreach (var item in searchStringCollection)
                    {
                        users = users.Where(s => s.Surname.ToUpper().Contains(item.ToUpper()) || s.FirstName.ToUpper().Contains(item.ToUpper())
                                                               || s.OtherName.ToUpper().Contains(item.ToUpper()) || s.UserName.ToUpper().Contains(item.ToUpper()));
                    }
                }
                else
                {
                    users = users.Where(s => s.Surname.ToUpper().Contains(searchString.ToUpper()) || s.FirstName.ToUpper().Contains(searchString.ToUpper())
                                                               || s.OtherName.ToUpper().Contains(searchString.ToUpper()) || s.UserName.ToUpper().Contains(searchString.ToUpper()));
                }

            }

            return await users.OrderBy(x => x.UserName).ToListAsync();
        }

        public async Task<List<ApplicationUser>> Archived(string searchString, string currentFilter, int? page)
        {
            var users = UserManager.Users.Where(x => x.Status == EntityStatus.Archived && x.UserName != "SuperAdmin" && x.UserName != "Education Sec");
            if (!String.IsNullOrEmpty(searchString))
            {
                if (CountString(searchString) > 1)
                {
                    string[] searchStringCollection = searchString.Split(' ');

                    foreach (var item in searchStringCollection)
                    {
                        users = users.Where(s => s.Surname.ToUpper().Contains(item.ToUpper()) || s.FirstName.ToUpper().Contains(item.ToUpper())
                                                               || s.OtherName.ToUpper().Contains(item.ToUpper()) || s.UserName.ToUpper().Contains(item.ToUpper()));
                    }
                }
                else
                {
                    users = users.Where(s => s.Surname.ToUpper().Contains(searchString.ToUpper()) || s.FirstName.ToUpper().Contains(searchString.ToUpper())
                                                               || s.OtherName.ToUpper().Contains(searchString.ToUpper()) || s.UserName.ToUpper().Contains(searchString.ToUpper()));
                }

            }

            return await users.OrderBy(x => x.UserName).ToListAsync();
        }

        public async Task<List<ApplicationUser>> Suspeneded(string searchString, string currentFilter, int? page)
        {
            var users = UserManager.Users.Where(x => x.Status == EntityStatus.Suspeneded && x.UserName != "SuperAdmin" && x.UserName != "Education Sec");
            if (!String.IsNullOrEmpty(searchString))
            {
                if (CountString(searchString) > 1)
                {
                    string[] searchStringCollection = searchString.Split(' ');

                    foreach (var item in searchStringCollection)
                    {
                        users = users.Where(s => s.Surname.ToUpper().Contains(item.ToUpper()) || s.FirstName.ToUpper().Contains(item.ToUpper())
                                                               || s.OtherName.ToUpper().Contains(item.ToUpper()) || s.UserName.ToUpper().Contains(item.ToUpper()));
                    }
                }
                else
                {
                    users = users.Where(s => s.Surname.ToUpper().Contains(searchString.ToUpper()) || s.FirstName.ToUpper().Contains(searchString.ToUpper())
                                                               || s.OtherName.ToUpper().Contains(searchString.ToUpper()) || s.UserName.ToUpper().Contains(searchString.ToUpper()));
                }

            }

            return await users.OrderBy(x => x.UserName).ToListAsync();
        }

        public async Task<ApplicationUser> GetUserByUserEmail(string email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            if (user != null)
            {
                return user;
            }
            return null;
        }

        public async Task<List<ApplicationUser>> GraduatedUsers(string searchString, string currentFilter, int? page)
        {
            var users = UserManager.Users.Where(x => x.UserName != "SuperAdmin" && x.Status == EntityStatus.Graduate);
            if (!String.IsNullOrEmpty(searchString))
            {
                if (CountString(searchString) > 1)
                {
                    string[] searchStringCollection = searchString.Split(' ');

                    foreach (var item in searchStringCollection)
                    {
                        users = users.Where(s => s.Surname.ToUpper().Contains(item.ToUpper()) || s.FirstName.ToUpper().Contains(item.ToUpper())
                                                               || s.OtherName.ToUpper().Contains(item.ToUpper()) || s.UserName.ToUpper().Contains(item.ToUpper()));
                    }
                }
                else
                {
                    users = users.Where(s => s.Surname.ToUpper().Contains(searchString.ToUpper()) || s.FirstName.ToUpper().Contains(searchString.ToUpper())
                                                               || s.OtherName.ToUpper().Contains(searchString.ToUpper()) || s.UserName.ToUpper().Contains(searchString.ToUpper()));
                }

            }

            return await users.OrderBy(x => x.Surname).ToListAsync();
        }

        public async Task<List<ApplicationUser>> DropoutUsers(string searchString, string currentFilter, int? page)
        {
            var users = UserManager.Users.Where(x => x.UserName != "SuperAdmin" && x.Status == EntityStatus.Dropout);
            if (!String.IsNullOrEmpty(searchString))
            {
                if (CountString(searchString) > 1)
                {
                    string[] searchStringCollection = searchString.Split(' ');

                    foreach (var item in searchStringCollection)
                    {
                        users = users.Where(s => s.Surname.ToUpper().Contains(item.ToUpper()) || s.FirstName.ToUpper().Contains(item.ToUpper())
                                                               || s.OtherName.ToUpper().Contains(item.ToUpper()) || s.UserName.ToUpper().Contains(item.ToUpper()));
                    }
                }
                else
                {
                    users = users.Where(s => s.Surname.ToUpper().Contains(searchString.ToUpper()) || s.FirstName.ToUpper().Contains(searchString.ToUpper())
                                                               || s.OtherName.ToUpper().Contains(searchString.ToUpper()) || s.UserName.ToUpper().Contains(searchString.ToUpper()));
                }

            }

            return await users.OrderBy(x => x.Surname).ToListAsync();
        }
    }
}