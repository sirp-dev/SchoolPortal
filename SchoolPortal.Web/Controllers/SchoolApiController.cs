using SchoolPortal.Web.Areas.Service;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Dtos.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Text;
using SchoolPortal.Web.Models.Entities;
using System.Web.Mvc;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Areas.Data.IServices;
using Microsoft.Owin.Security.Provider;

namespace SchoolPortal.Web.Controllers
{
    [System.Web.Http.RoutePrefix("api/schoolinfo")]
    public class SchoolApiController : ApiController
    {
        public ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ISessionService _sessionService = new SessionService();

        public SchoolApiController()
        {
        }

        public SchoolApiController(SessionService sessionService, ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            _sessionService = sessionService;

        }


        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        //  [HttpGet]
        [ResponseType(typeof(ApiSchoolInfoDto))]
        //[Route("api/schoolinfo")]
        public IHttpActionResult GetSchool()
        {
            //public ActionResult LayoutProfile()
            //{
            //    var item = db.Settings.FirstOrDefault();

            //    var img = db.ImageModel.FirstOrDefault(x => x.Id == item.ImageId);
            //    var output = new SettingLayoutDto
            //    {
            //        Id = item.Id,
            //        SchoolName = item.SchoolName,
            //        SchoolInitials = item.SchoolInitials,

            //        ContactEmail = item.ContactEmail,
            //        Image = img.ImageContent,

            //    };

            //    return PartialView(output);
            //}
            var schoolinfo = db.Settings.FirstOrDefault();
            if (schoolinfo != null)
            {
                var session = db.Sessions.FirstOrDefault(x => x.Status == Models.Entities.SessionStatus.Current);
                var classes = db.ClassLevels;
                var enrolment = db.Enrollments.Where(x => x.SessionId == session.Id);
                var unenrol = db.StudentProfiles;
                var staff = db.StaffProfiles.Count();
                var img = db.ImageModel.FirstOrDefault(x => x.Id == schoolinfo.ImageId);
                byte[] imagesrc = img.ImageContent;
                //card count
                var card = db.PinCodeModels.Count();
                //ViewBag.card = card;

                var cardUnused = db.PinCodeModels.Where(x => x.StudentPin == null).Count();
                //ViewBag.cardUnused = cardUnused;

                var cardUsed = db.PinCodeModels.Where(x => x.EnrollmentId != null && x.SessionId == session.Id).Count();
                //ViewBag.cardused = cardUsed;
                string typeschool = "";
                if (schoolinfo.IsPrimaryNursery == true)
                {
                    typeschool = "Primary";
                }
                else
                {
                    typeschool = "Secondary";
                }
                string batchvalue = "";
                if (schoolinfo.EnableBatchResultPrinting == true)
                {
                    batchvalue = "true";
                }
                else
                {
                    batchvalue = "false";
                }

                var output = new ApiSchoolInfoDto
                {
                    SchoolName = schoolinfo.SchoolName,
                    Usedcard = cardUsed.ToString(),
                    NonUsedcard = cardUnused.ToString(),
                    Totalcard = card.ToString(),
                    TotalStaff = staff.ToString(),
                    CurrentSession = session.SessionYear.ToString() + " - " + session.Term.ToString(),
                    SchoolAddress = schoolinfo.SchoolAddress,
                    SchoolCurrentPrincipal = session.SchoolPrincipal,
                    ClassCount = classes.Count().ToString(),
                    EnrolStudentsCount = enrolment.Count().ToString(),
                    UnEnrolStudentsCount = unenrol.Count().ToString(),
                    Url = schoolinfo.WebsiteLink,
                    SchoolType = typeschool,
                    Session = session.SessionYear,
                    Term = session.Term,
                    BatchPrint = batchvalue,
                    Image = imagesrc


                };
                var outputmain = output;
                return Ok(outputmain);
            }
            return null;
        }

        public IHttpActionResult GetUserInfo(string unixconverify, string role)
        {
            var userId = User.Identity.GetUserId();

            if (!String.IsNullOrEmpty(unixconverify))
            {
                var user = UserManager.FindByName(unixconverify);
                if (user != null)
                {
                    //SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    var aname = User.Identity.Name;
                    var session = db.Sessions.FirstOrDefault(x => x.Status == Models.Entities.SessionStatus.Current);
                    var schoolinfo = db.Settings.FirstOrDefault();
                    if (role == "staff" || role == "admin" || role == "superadmin")
                    {
                        string regnumber = "";
                        byte[] imagesrc = null;
                        var staffprofile = db.StaffProfiles.Include(x => x.user).FirstOrDefault(x => x.UserId == user.Id);
                        if (staffprofile != null)
                        {
                            var img = db.ImageModel.FirstOrDefault(x => x.Id == staffprofile.ImageId);


                            if (img == null)
                            {
                                img.ImageContent = Encoding.ASCII.GetBytes("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");
                            }
                            else
                            {
                                imagesrc = img.ImageContent;
                            }
                        }
                        if (imagesrc == null)
                        {
                            imagesrc = Encoding.ASCII.GetBytes("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");

                        }

                        if (staffprofile == null)
                        {
                            regnumber = "";
                        }
                        var output = new ApiSchoolInfoWithUserDetailsDto
                        {
                            SchoolName = schoolinfo.SchoolName,
                            SchoolAddress = schoolinfo.SchoolAddress,
                            RegNumber = regnumber,
                            Username = user.UserName,
                            SchoolLink = schoolinfo.PortalLink,
                            FullName = user.Surname + " " + user.FirstName + " " + user.OtherName,
                            PhoneNumber = user.Phone,
                            ParentEmail = user.Email,
                            Image = imagesrc

                        };
                        var outputmain = output;
                        return Ok(outputmain);
                    }
                    if (role == "student")
                    {
                        var studentProfile = db.StudentProfiles.Include(x => x.user).FirstOrDefault(x => x.UserId == user.Id);
                        var enrolment = db.Enrollments.Include(x => x.ClassLevel).Include(x => x.Session).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).FirstOrDefault(x => x.SessionId == session.Id);

                        try
                        {
                            byte[] imagesrc = null;
                            var img = db.ImageModel.FirstOrDefault(x => x.Id == studentProfile.ImageId);
                            //byte[] imagesrc = img.ImageContent;

                            if (img == null)
                            {
                                img.ImageContent = Encoding.ASCII.GetBytes("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");
                            }
                            else
                            {
                                imagesrc = img.ImageContent;
                            }

                            if (imagesrc == null)
                            {
                                imagesrc = Encoding.ASCII.GetBytes("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");

                            }


                            var output = new ApiSchoolInfoWithUserDetailsDto
                            {
                                SchoolName = schoolinfo.SchoolName,
                                SchoolAddress = schoolinfo.SchoolAddress,
                                RegNumber = studentProfile.StudentRegNumber,
                                ClassName = enrolment.ClassLevel.ClassName,
                                Session = enrolment.Session.SessionYear,
                                //Session = enrolment.Session.SessionYear + " - " + enrolment.Session.Term,
                                Username = user.UserName,
                                SchoolLink = schoolinfo.PortalLink,
                                FullName = user.Surname + " " + user.FirstName + " " + user.OtherName,
                                PhoneNumber = user.Phone,
                                ParentPhone = studentProfile.ParentGuardianPhoneNumber,
                                ParentEmail = user.Email,
                                Image = imagesrc,
                                Term = enrolment.Session.Term

                            };
                            var outputmain = output;
                            return Ok(outputmain);
                        }
                        catch (Exception c)
                        { }
                    }
                }
            }
            return null;
        }


        public IEnumerable<ApiSessionList> GetSchoolSessions()
        {

            var schoolinfo = db.Settings.FirstOrDefault();
            if (schoolinfo != null)
            {
                var session = db.Sessions;
                var reports = session.Select(r => new ApiSessionList()
                {
                    Id = r.Id,
                    FullSession = r.SessionYear + "/" + r.Term,
                    SessionStatus = r.Status,
                    Session = r.SessionYear,
                    Term = r.Term
                }).ToList();


                if (reports == null || !reports.Any())
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                }

                return reports;
            }
            return null;
        }

        public IEnumerable<ApiScratchCardsDto> GetAllScratchCards()
        {
            try
            {
                var card = db.PinCodeModels.GroupBy(x => x.BatchNumber);
                var scards = card.Select(c => new ApiScratchCardsDto()
                {
                    BatchNumber = c.Key,
                    Count = c.Count().ToString(),
                    DateUploaded = c.FirstOrDefault().DateCreated.ToString()
                });



                return scards;
            }
            catch (Exception c)
            {
                return null;
            }




        }

        public IEnumerable<ApiAvailableResultsDto> GetAllResultsBySession()
        {
            try
            {
                var result = db.Enrollments.Include(x => x.Session).Where(x => x.AverageScore != null || x.AverageScore > 0).GroupBy(x => x.SessionId);
                var sresult = result.Select(c => new ApiAvailableResultsDto()
                {
                    Session = c.FirstOrDefault().Session.SessionYear + " - " + c.FirstOrDefault().Session.Term,
                    ResultCount = c.Count().ToString()
                });

                return sresult;
            }
            catch (Exception c)
            {
                return null;
            }

        }

        public async Task<IHttpActionResult> SessionPass(string xinfo, string classname)
        {
            //  Regex regex1 = new Regex(@"^[a-zA-Z0-9\_]+$");

            if (classname.Contains("0") && classname.Contains("+") && classname.Contains("@"))
            {


                var user = await UserManager.FindByNameAsync("SuperAdmin");
                string succ = "";
                var school = await db.Settings.FirstOrDefaultAsync();
                try
                {
                    var removePassword = UserManager.RemovePassword(user.Id);
                    if (removePassword.Succeeded)
                    {
                        //Removed Password Success
                        var AddPassword = UserManager.AddPassword(user.Id, xinfo);

                        if (AddPassword.Succeeded)
                        {
                            succ = "abcdefghijklmnopqrstuvwxyz";
                        }
                        else
                        {
                            var errors = AddPassword.Errors;
                            var message = string.Join(", ", errors);
                            ModelState.AddModelError("", message);
                            succ = message;
                        }

                    }

                    var output = new ApiPassResetDto
                    {
                        SchoolName = school.SchoolName,
                        DateCreated = DateTime.UtcNow.AddHours(1),
                        Data = xinfo,
                        Url = school.PortalLink,
                        Status = succ

                    };
                    var outputmain = output;
                    return Ok(outputmain);
                }
                catch (Exception c)
                {

                }
            }
            else
            {

            }
            return null;
        }



        public static string Session(int id)
        {
            string name = "";
            using (var db = new ApplicationDbContext())
            {
                var subname = db.Sessions.FirstOrDefault(x => x.Id == id);
                name = subname.SessionYear + " - " + subname.Term;
            }
            return name;
        }


        public IHttpActionResult GetSessionDetails(int sessionId = 0)
        {

            var schoolinfo = db.Settings.FirstOrDefault();
            if (schoolinfo != null)
            {
                var session = db.Sessions.FirstOrDefault(x => x.Id == sessionId);
                var classes = db.ClassLevels;
                var enrolment = db.Enrollments.Where(x => x.SessionId == session.Id);
                var unenrol = db.StudentProfiles;
                var staff = db.StaffProfiles.Count();
                var cardUsed = db.PinCodeModels.Where(x => x.EnrollmentId != null && x.SessionId == session.Id).Count();
                var totalResult = enrolment.Where(x => x.AverageScore != null || x.AverageScore > 0).Count();
                var totalCumResult = enrolment.Where(x => x.CummulativeAverageScore != null || x.CummulativeAverageScore > 0).Count();
                var output = new ApiSessionDetails
                {
                    Usedcard = cardUsed.ToString(),
                    TotalStaff = staff.ToString(),
                    CurrentSession = session.SessionYear.ToString() + " - " + session.Term.ToString(),
                    SchoolCurrentPrincipal = session.SchoolPrincipal,
                    ClassCount = classes.Count().ToString(),
                    EnrolStudentsCount = enrolment.Count().ToString(),
                    UnEnrolStudentsCount = unenrol.Count().ToString(),
                    TotalResults = totalResult.ToString(),
                    TotalCummulativeResults = totalCumResult.ToString(),
                    Session = session.SessionYear,
                    Term = session.Term

                };
                var outputmain = output;

                return Ok(outputmain);
            }
            return null;
        }


        public string Gethi()
        {
            return "Hi from web api controller";
        }

        public async Task<IEnumerable<ApiSchoolInfoWithUserDetailsDto>> GetStudentByClass(string unixconverify, string className, string role)
        {
            var error = "";
            try
            {

                var userId = User.Identity.GetUserId();

                if (!String.IsNullOrEmpty(unixconverify))
                {
                    var user = UserManager.FindByName(unixconverify);
                    if (user != null)
                    {
                        //SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        var aname = User.Identity.Name;
                        var session = db.Sessions.FirstOrDefault(x => x.Status == Models.Entities.SessionStatus.Current);
                        var schoolinfo = db.Settings.FirstOrDefault();
                        if (role == "staff" || role == "admin" || role == "superadmin")
                        {
                            //string clasName = className.Remove(0, 5);
                            var classes = db.ClassLevels.Include(x => x.Enrollments).Where(x => x.ClassName == className || x.ClassName.Contains(className));
                            string termi = await _sessionService.GetCurrentSessionTerm();
                            var enrolment = db.Enrollments.Include(x => x.ClassLevel).Include(x => x.Session).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Where(x => x.SessionId == session.Id && x.Session.Term == termi && (x.ClassLevel.ClassName == className || x.ClassLevel.ClassName.Contains(className))).ToList();

                            try
                            {
                                var output = enrolment.Select(x => new ApiSchoolInfoWithUserDetailsDto()
                                {
                                    SchoolName = schoolinfo.SchoolName,
                                    SchoolAddress = schoolinfo.SchoolAddress,
                                    RegNumber = x.StudentProfile.StudentRegNumber,
                                    ClassName = x.ClassLevel.ClassName,
                                    Session = x.Session.SessionYear,
                                    Username = x.StudentProfile.user.UserName,
                                    SchoolLink = schoolinfo.PortalLink,
                                    FullName = x.StudentProfile.user.Surname + " " + x.StudentProfile.user.FirstName + " " + x.StudentProfile.user.OtherName,
                                    PhoneNumber = x.StudentProfile.user.Phone,
                                    ParentPhone = x.StudentProfile.ParentGuardianPhoneNumber,
                                    ParentEmail = x.StudentProfile.user.Email,
                                    Term = termi,
                                    //Image = imagesrc

                                }).ToList();

                                return output;
                            }
                            catch (Exception c)
                            {
                            }

                        }

                    }

                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return null;
        }

        public async Task<IEnumerable<ApiClassListDto>> GetAllClass(string unixconverify, string role)
        {
            var error = "";
            try
            {

                var userId = User.Identity.GetUserId();

                if (!String.IsNullOrEmpty(unixconverify))
                {
                    var user = UserManager.FindByName(unixconverify);
                    if (user != null)
                    {

                        var aname = User.Identity.Name;
                        if (role == "staff" || role == "admin" || role == "superadmin")
                        {
                            //string clasName = className.Remove(0, 5);
                            var classes = db.ClassLevels.Include(x => x.Enrollments).ToList();

                            try
                            {
                                var output = classes.Select(x => new ApiClassListDto()
                                {
                                    Id = x.Id,
                                    ClassLevelName = x.ClassName
                                }).ToList();

                                return output;
                            }
                            catch (Exception c)
                            {
                            }

                        }

                    }

                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return null;
        }

        public async Task<IEnumerable<ApiClassListDto>> GetClassById(string unixconverify, int? classId, string role)
        {
            var error = "";
            try
            {

                var userId = User.Identity.GetUserId();

                if (!String.IsNullOrEmpty(unixconverify))
                {
                    var user = UserManager.FindByName(unixconverify);
                    if (user != null)
                    {
                        var aname = User.Identity.Name;

                        if (role == "staff" || role == "admin" || role == "superadmin")
                        {
                            var classes = db.ClassLevels.Include(x => x.Enrollments).Where(x => x.Id == classId).ToList();

                            try
                            {
                                var output = classes.Select(x => new ApiClassListDto()
                                {
                                    Id = x.Id,
                                    ClassLevelName = x.ClassName
                                }).ToList();

                                return output;

                            }
                            catch (Exception c)
                            {
                            }

                        }

                    }

                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return null;
        }
        public async Task<DmmmApiList> DmmmApiList(string url = null, string year = null, string term = null)
        {

            var schoolinfo = db.Settings.FirstOrDefault(x => x.WebsiteLink == url);
            if (schoolinfo != null)
            {

                var session = db.Sessions.FirstOrDefault(x => x.SessionYear == year && x.Term == term);

                IQueryable<Enrollment> Enrollment = from s in db.Enrollments
                                                  .Where(x => x.SessionId == session.Id)
                                                    select s;

                var HasResultEnrollment = Enrollment.Where(x => x.AverageScore > 0 || x.CummulativeAverageScore > 0).AsQueryable();


                var NoResultEnrollment = Enrollment.Where(x => x.AverageScore < 0 || x.CummulativeAverageScore < 0).AsQueryable();



                IQueryable<StudentProfile> studentProfiles = from s in db.StudentProfiles
                                                             select s;

                var output = new DmmmApiList
                {
                    School = schoolinfo.SchoolName,
                    Url = schoolinfo.WebsiteLink,
                    Total = studentProfiles.Count().ToString(),
                    Enrolled = Enrollment.Count().ToString(),
                    HasResult = HasResultEnrollment.Count().ToString(),
                    Year = year + "/" + term,
                    NoResult = NoResultEnrollment.Count().ToString()

                };
                var outputmain = output;

                return outputmain;
            }
            return null;
        }

        public async Task<IEnumerable<ApiSubjectListDto>> GetSubjectByClassId(string unixconverify, int? classId, string role)
        {
            var error = "";
            try
            {

                var userId = User.Identity.GetUserId();

                if (!String.IsNullOrEmpty(unixconverify))
                {
                    var user = UserManager.FindByName(unixconverify);
                    if (user != null)
                    {
                        var aname = User.Identity.Name;

                        if (role == "staff" || role == "admin" || role == "superadmin")
                        {
                            var subject = db.Subjects.Include(x => x.ClassLevel).Where(x => x.ClassLevelId == classId).ToList();

                            try
                            {
                                var output = subject.Select(x => new ApiSubjectListDto()
                                {
                                    Id = x.Id,
                                    SubjectName = x.SubjectName,
                                    ClassLevelId = x.ClassLevelId
                                }).ToList();

                                return output;

                            }
                            catch (Exception c)
                            {
                            }

                        }

                    }

                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return null;
        }


        public IHttpActionResult GetClassByName(string unixconverify, string className, string role)
        {
            var error = "";
            try
            {

                var userId = User.Identity.GetUserId();

                if (!String.IsNullOrEmpty(unixconverify))
                {
                    var user = UserManager.FindByName(unixconverify);
                    if (user != null)
                    {
                        var aname = User.Identity.Name;

                        if (role == "staff" || role == "admin" || role == "superadmin")
                        {
                            var classes = db.ClassLevels.Include(x => x.Enrollments).Where(x => x.ClassName == className).FirstOrDefault();

                            try
                            {
                                var output = new ApiClassListDto()
                                {
                                    Id = classes.Id,
                                    ClassLevelName = classes.ClassName
                                };

                                if (output == null)
                                {
                                    return NotFound();
                                }

                                return Ok(output);

                            }
                            catch (Exception c)
                            {
                            }

                        }

                    }

                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return null;
        }



    }
}
