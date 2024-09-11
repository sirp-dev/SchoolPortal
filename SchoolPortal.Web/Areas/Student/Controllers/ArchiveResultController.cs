using Microsoft.AspNet.Identity.Owin;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using SchoolPortal.Web.Models.Dtos;
using SchoolPortal.Web.Areas.Service;
using SchoolPortal.Web.Models.Entities;

namespace SchoolPortal.Web.Areas.Student.Controllers
{
    public class ArchiveResultController : Controller
    {
        #region services
        private ApplicationDbContext db = new ApplicationDbContext();
        private IStaffService _staffProfileService = new StaffService();
        private IClassLevelService _classLevelService = new ClassLevelService();
        private IResultArchiveService _resultService = new ResultArchiveService();
        private IEnrollmentService _enrollmentService = new EnrollmentService();
        private IStudentProfileService _studentProfileService = new StudentProfileService();
        private ISubjectService _subjectService = new SubjectService();
        private ISessionService _sessionService = new SessionService();
        private IEnrolledSubjectArchiveService _enrolledSubjectService = new EnrolledSubjectArchiveService();
        private ISettingService _settingService = new SettingService();
        private IPublishResultService _publishResultService = new PublishResultService();
        private IImageService _imageService = new ImageService();
        private IUserManagerService _userManagerService = new UserManagerService();
        private IPostService _postService = new PostService();
        private IAssignmentService _assignmentService = new AssignmentService();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;


        public ArchiveResultController()
        {

        }
        public ArchiveResultController(StaffService staffProfileService,
            ClassLevelService classLevelService,
            PostService postService,
            ResultArchiveService resultService,
            EnrollmentService enrollmentService,
            StudentProfileService studentProfileService,
            SubjectService subjectService,
            SessionService sessionService,
            EnrolledSubjectArchiveService enrolledSubjectService,
            SettingService settingService,
            PublishResultService publishResultService,
            ImageService imageService,
            UserManagerService userManagerService,
            AssignmentService assignmentService, ApplicationUserManager userManager, ApplicationSignInManager signInManager
            )
        {
            _userManagerService = userManagerService;
            _imageService = imageService;
            _staffProfileService = staffProfileService;
            _classLevelService = classLevelService;
            _postService = postService;
            _resultService = resultService;
            _enrollmentService = enrollmentService;
            _studentProfileService = studentProfileService;
            _subjectService = subjectService;
            _sessionService = sessionService;
            _enrolledSubjectService = enrolledSubjectService;
            _settingService = settingService;
            _publishResultService = publishResultService;
            _assignmentService = assignmentService;
            UserManager = userManager;
            SignInManager = signInManager;
        }

        #endregion

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

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }



        // GET: Student/ArchiveResult
        public ActionResult Index()
        {
            return View();
        }


        #region Result
        [AllowAnonymous]
        //public async Task<ActionResult> Result(string skip)
        public async Task<ActionResult> Result()
        {
            var session = await _sessionService.GetAllSession();

            ViewBag.sessionId = new SelectList(session, "Id", "FullSession");
            //ViewBag.skip = skip;
            var sett = db.Settings.FirstOrDefault();
            ViewBag.name = sett.SchoolName;
            if (User.IsInRole("Student"))
            {
                var user = await _resultService.GetUserByUserId(User.Identity.GetUserId());
                ViewBag.regnumber = user.StudentRegNumber;
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //public async Task<ActionResult> Result(CheckResultViewModelDto model, string skip)
        public async Task<ActionResult> Result(CheckResultViewModelDto model)
        {
            var sessionlist = await _sessionService.GetAllSession();
            TempData["validator"] = "asdflkjhgqwerpoiu";

            if (ModelState.IsValid)
            {
                try
                {


                    var userProfile = await _resultService.GetUserByRegNumber(model.StudentPIN);
                    if (userProfile == null)
                    {
                        ViewBag.Error = GeneralService.StudentorPupil() + " Wrong Registration Number.";
                        ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);

                        return View(model);
                    }
                    ///check if profile is complete
                    //
                    //if (skip == null)
                    //{
                    //    var usercompleteprofile = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.StudentRegNumber == userProfile.StudentRegNumber);
                    //    if (usercompleteprofile.IsUpdated == false)
                    //    {
                    //        if (string.IsNullOrEmpty(usercompleteprofile.AboutMe) || string.IsNullOrEmpty(usercompleteprofile.ParentGuardianOccupation) || string.IsNullOrEmpty(usercompleteprofile.user.Gender) || string.IsNullOrEmpty(usercompleteprofile.user.StateOfOrigin) || string.IsNullOrEmpty(usercompleteprofile.user.LocalGov))
                    //        {
                    //            return RedirectToAction("UpdateProfile", new { id = usercompleteprofile.Id });
                    //        }
                    //    }
                    //}

                    var session = await _resultService.GetSessionBySessionId(model.SessionId);
                    var enrollment = await _resultService.GetEnrollmentBySessIdStudentProfileId(model.SessionId, userProfile.Id);
                    ///
                     //Check debt profile
                    if (enrollment == null)
                    {
                        ViewBag.Error =
                            "You Were Not Enrolled In This Session/Term. Contact Your Form Teacher";
                        ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                        return View(model);
                    }
                    var pinCode = await _resultService.PinCodeByPinNumberAndSerialNumber(model.PinNumber, model.SerialNumber);
                    var checkResultStatus = await _resultService.PublishResultBySessIdAndClassId(enrollment.SessionId, enrollment.ClassLevelId);
                    var checkDebtProfile = await _resultService.GetDefaulterByProfileId(userProfile.Id);
                    //Check debt profile
                    if (checkDebtProfile != null)
                    {
                        ViewBag.Error =
                            "You can not check your result. You may still be indebted to the school. Please Contact the Rector for further details.";
                        ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                        return View(model);
                    }

                    //check if Student PIN is correct
                    if (userProfile == null)
                    {
                        ViewBag.Error = "The " + GeneralService.StudentorPupil() + "Registration Number does not exist. Please try again.";
                        ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                        return View(model);
                    }
                    if (checkResultStatus == null)
                    {
                        ViewBag.Error = "Your Result is not yet ready. Please try again later.";
                        ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                        return View(model);
                    }

                    if (enrollment == null)
                    {
                        ViewBag.Error = "You don't have a Result for the Session/Term you chose.";
                        ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                        return View(model);
                    }
                    var sett = db.Settings.FirstOrDefault();

                    //check if PIN Number exists
                    if (model.PinNumber == "000000000000" && model.SerialNumber == "000000")
                    {
                        if (sett.PrintOutOption == PrintOutOption.PrintOutOne)
                        {
                            if (sett.ShowCumulativeResultForThirdTerm == false)
                            {
                                TempData["Verified"] = "True";
                                return RedirectToAction("PrintResult", new { id = enrollment.Id });
                            }
                            else
                            {
                                if (!session.Term.Contains("Third"))
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                }
                                else
                                {
                                    return RedirectToAction("PrintThirdResult",
                                        new { id = enrollment.Id, sess = session.Id });
                                }
                            }
                        }
                        else if (sett.PrintOutOption == PrintOutOption.PrintOutTwo)
                        {

                            if (sett.ShowCumulativeResultForThirdTerm == false)
                            {
                                TempData["Verified"] = "True";
                                return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                            }
                            else
                            {
                                if (!session.Term.Contains("Third"))
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                }
                                else
                                {
                                    return RedirectToAction("PrintThirdResult2",
                                        new { id = enrollment.Id, sess = session.Id });
                                }
                            }
                        }
                        else
                        {
                            if (sett.ShowCumulativeResultForThirdTerm == false)
                            {
                                TempData["Verified"] = "True";
                                return RedirectToAction("PrintResult", new { id = enrollment.Id });
                            }
                            else
                            {
                                if (!session.Term.Contains("Third"))
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                }
                                else
                                {
                                    return RedirectToAction("PrintThirdResult",
                                        new { id = enrollment.Id, sess = session.Id });
                                }
                            }
                        }

                    }
                    if (pinCode == null)
                    {
                        ViewBag.Error =
                            "The PIN does not exist. Please check the PIN Number and Serial Number and then try again.";
                        ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                        return View(model);
                    }
                    else if (pinCode != null)
                    {

                        //Check if it has been used by another user
                        if (pinCode.StudentPin != userProfile.StudentRegNumber && pinCode.StudentPin != null)
                        {
                            ViewBag.Error = "The PIN has been used by another user.";
                            ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);

                            return View(model);
                        }
                        //Check If it has been used by you for another term
                        else if (pinCode.StudentPin == userProfile.StudentRegNumber && pinCode.EnrollmentId == enrollment.Id && pinCode.EnrollmentId != null && sett.PinValidOption == PinValidOption.Termly)
                        {

                            if (session != null)
                            {
                                //if (pinCode.EnrollmentId == enrollment.Id)
                                //{
                                //    ViewBag.Error = "The PIN has been used by you for another term.";
                                //    ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                                //    return View(model);
                                //}
                                if (pinCode.EnrollmentId == enrollment.Id && sett.PinValidOption == PinValidOption.Termly && pinCode.Usage <= 0)
                                {
                                    ViewBag.Error = "The PIN is no longer active. You have used it in another term.";
                                    ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                                    return View(model);
                                }
                                else
                                {
                                    pinCode.Usage = pinCode.Usage - 1;
                                    await _resultService.Update();

                                    if (sett.PrintOutOption == PrintOutOption.PrintOutOne)
                                    {
                                        if (sett.ShowCumulativeResultForThirdTerm == false)
                                        {
                                            TempData["Verified"] = "True";
                                            return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                        }
                                        else
                                        {
                                            if (!session.Term.Contains("Third"))
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                return RedirectToAction("PrintThirdResult",
                                                    new { id = enrollment.Id, sess = session.Id });
                                            }
                                        }
                                    }
                                    else if (sett.PrintOutOption == PrintOutOption.PrintOutTwo)
                                    {

                                        if (sett.ShowCumulativeResultForThirdTerm == false)
                                        {
                                            TempData["Verified"] = "True";
                                            return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                        }
                                        else
                                        {
                                            if (!session.Term.Contains("Third"))
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                return RedirectToAction("PrintThirdResult2",
                                                    new { id = enrollment.Id, sess = session.Id });
                                            }
                                        }
                                    }

                                }
                            }
                        }

                        //Check if it has been used by you for another session
                        else if (pinCode.StudentPin == userProfile.StudentRegNumber && pinCode.EnrollmentId == enrollment.Id && pinCode.EnrollmentId != null && sett.PinValidOption == PinValidOption.Yearly)
                        {

                            if (session != null)
                            {
                                //if (session.SessionYear != selectedYear.SessionYear)
                                //if (pinCode.EnrollmentId == enrollment.Id)
                                //{
                                //    ViewBag.Error = "The PIN has been used by you for another Session.";
                                //    ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                                //    return View(model);
                                //}
                                if (pinCode.EnrollmentId == enrollment.Id && pinCode.Usage <= 0 && sett.PinValidOption == PinValidOption.Yearly)
                                {
                                    ViewBag.Error = "The PIN is no longer active. You have used it 5 times in another Session.";
                                    ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                                    return View(model);
                                }
                                else
                                {
                                    pinCode.Usage = pinCode.Usage - 1;
                                    await _resultService.Update();

                                    if (sett.PrintOutOption == PrintOutOption.PrintOutOne)
                                    {
                                        if (sett.ShowCumulativeResultForThirdTerm == false)
                                        {
                                            TempData["Verified"] = "True";
                                            return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                        }
                                        else
                                        {
                                            if (!session.Term.Contains("Third"))
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                return RedirectToAction("PrintThirdResult",
                                                    new { id = enrollment.Id, sess = session.Id });
                                            }
                                        }
                                    }
                                    else if (sett.PrintOutOption == PrintOutOption.PrintOutTwo)
                                    {

                                        if (sett.ShowCumulativeResultForThirdTerm == false)
                                        {
                                            TempData["Verified"] = "True";
                                            return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                        }
                                        else
                                        {
                                            if (!session.Term.Contains("Third"))
                                            {
                                                TempData["Verified"] = "True";
                                                return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                            }
                                            else
                                            {
                                                return RedirectToAction("PrintThirdResult2",
                                                    new { id = enrollment.Id, sess = session.Id });
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        else
                        {
                            if (pinCode.StudentPin == null)
                            {
                                pinCode.StudentPin = userProfile.StudentRegNumber;
                                pinCode.EnrollmentId = enrollment.Id;
                                pinCode.SessionId = enrollment.SessionId;
                            }
                            else if (pinCode.EnrollmentId == enrollment.Id && pinCode.Usage <= 0)
                            {
                                ViewBag.Error = "The PIN is no longer active. You have used it 2 times.";
                                ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
                                return View(model);
                            }
                            pinCode.Usage = pinCode.Usage - 1;
                            await _resultService.Update();
                            if (sett.PrintOutOption == PrintOutOption.PrintOutOne)
                            {
                                if (sett.ShowCumulativeResultForThirdTerm == false)
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                }
                                else
                                {
                                    if (!session.Term.Contains("Third"))
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        return RedirectToAction("PrintThirdResult",
                                            new { id = enrollment.Id, sess = session.Id });
                                    }
                                }
                            }
                            else if (sett.PrintOutOption == PrintOutOption.PrintOutTwo)
                            {

                                if (sett.ShowCumulativeResultForThirdTerm == false)
                                {
                                    TempData["Verified"] = "True";
                                    return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                }
                                else
                                {
                                    if (!session.Term.Contains("Third"))
                                    {
                                        TempData["Verified"] = "True";
                                        return RedirectToAction("PrintResult2", new { id = enrollment.Id });
                                    }
                                    else
                                    {
                                        return RedirectToAction("PrintThirdResult2",
                                            new { id = enrollment.Id, sess = session.Id });
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var a = ex;
                }

            }

            ViewBag.Error = "Unable to check result, Contact your Teacher.";
            ViewBag.sessionId = new SelectList(sessionlist, "Id", "FullSession", model.SessionId);
            return View(model);
        }

        #endregion

        #region Print Result
        [AllowAnonymous]
        public async Task<ActionResult> PrintResult(Int32? id, int Id)
        {
            try
            {
                string valstring = TempData["validator"].ToString();
                if (valstring != "asdflkjhgqwerpoiu")
                {

                    return RedirectToAction("Result");
                }
            }
            catch (Exception f)
            {

                return RedirectToAction("Result");
            }


            try
            {


                int subjectCount = await _resultService.SubjectCount(id);
                if (subjectCount == 0)
                {
                    ViewBag.Sum = "No Result score has been entered";
                }

                // var result = await _resultService.PrintResult(id);
                ViewBag.id = id;
                var PositionShow = db.Settings.FirstOrDefault();
                ViewBag.showpos = PositionShow;

                //var classResult = await _classLevelService.Get(Id);
                //var showPosOnClassResult = classResult.ShowPositionOnClassResult;
                //ViewBag.showPosOnClassResult = showPosOnClassResult;
                return View();
            }
            catch (Exception ex)
            {
                var session = await _sessionService.GetAllSession();
                ViewBag.sessionId = new SelectList(session, "Id", "FullSession");
                return View("Result");
            }
        }


        [AllowAnonymous]
        public async Task<ActionResult> PrintResult2(Int32? id, int Id)
        {
            try
            {
                string valstring = TempData["validator"].ToString();
                if (valstring != "asdflkjhgqwerpoiu")
                {

                    return RedirectToAction("Result");
                }
            }
            catch (Exception f)
            {

                return RedirectToAction("Result");
            }

            try
            {


                int subjectCount = await _resultService.SubjectCount(id);
                if (subjectCount == 0)
                {
                    ViewBag.Sum = "No Result score has been entered";
                }

                // var result = await _resultService.PrintResult(id);
                ViewBag.id = id;
                var PositionShow = db.Settings.FirstOrDefault();
                ViewBag.showpos = PositionShow;

                //var classResult = await _classLevelService.Get(Id);
                //var showPosOnClassResult = classResult.ShowPositionOnClassResult;
                //ViewBag.showPosOnClassResult = showPosOnClassResult;
                return View();
            }
            catch (Exception ex)
            {
                var session = await _sessionService.GetAllSession();
                ViewBag.sessionId = new SelectList(session, "Id", "FullSession");
                return View("Result");
            }
        }


        [AllowAnonymous]
        public async Task<ActionResult> PrintResult3(Int32? id, int Id)
        {
            try
            {
                string valstring = TempData["validator"].ToString();
                if (valstring != "asdflkjhgqwerpoiu")
                {

                    return RedirectToAction("Result");
                }
            }
            catch (Exception f)
            {

                return RedirectToAction("Result");
            }

            try
            {


                int subjectCount = await _resultService.SubjectCount(id);
                if (subjectCount == 0)
                {
                    ViewBag.Sum = "No Result score has been entered";
                }

                // var result = await _resultService.PrintResult(id);
                ViewBag.id = id;
                var PositionShow = db.Settings.FirstOrDefault();
                ViewBag.showpos = PositionShow;

                //var classResult = await _classLevelService.Get(Id);
                //var showPosOnClassResult = classResult.ShowPositionOnClassResult;
                //ViewBag.showPosOnClassResult = showPosOnClassResult;
                return View();
            }
            catch (Exception ex)
            {
                var session = await _sessionService.GetAllSession();
                ViewBag.sessionId = new SelectList(session, "Id", "FullSession");
                return View("Result");
            }
        }

        #endregion



        #region Cummulative

        [AllowAnonymous]
        public async Task<ActionResult> PrintThirdResult(Int32? id, int? sess, int Id)
        {
            try
            {
                int subjectCount = await _resultService.SubjectCount(id);
                if (subjectCount == 0)
                {
                    ViewBag.Sum = "No Result score has been entered";
                }

                ViewBag.id = id;
                ViewBag.sessid = sess;

                //var classResult = await _classLevelService.Get(Id);
                //var showPosOnClassResult = classResult.ShowPositionOnClassResult;
                //ViewBag.showPosOnClassResult = showPosOnClassResult;

                return View();
            }
            catch (Exception ex)
            {
                var session = await _sessionService.GetAllSession();
                ViewBag.sessionId = new SelectList(session, "Id", "FullSession");
                return View("Result");
            }
        }




        [AllowAnonymous]
        public async Task<ActionResult> PrintThirdResult2(Int32? id, int? sess, int Id)
        {
            try
            {


                int subjectCount = await _resultService.SubjectCount(id);
                if (subjectCount == 0)
                {
                    ViewBag.Sum = "No Result score has been entered";
                }

                ViewBag.id = id;
                ViewBag.sessid = sess;

                //var classResult = await _classLevelService.Get(Id);
                //var showPosOnClassResult = classResult.ShowPositionOnClassResult;
                //ViewBag.showPosOnClassResult = showPosOnClassResult;

                return View();
            }
            catch (Exception ex)
            {
                var session = await _sessionService.GetAllSession();
                ViewBag.sessionId = new SelectList(session, "Id", "FullSession");
                return View("Result");
            }
        }


        [AllowAnonymous]
        public async Task<ActionResult> PrintThirdResult3(Int32? id, int? sess, int Id)
        {
            try
            {


                int subjectCount = await _resultService.SubjectCount(id);
                if (subjectCount == 0)
                {
                    ViewBag.Sum = "No Result score has been entered";
                }

                ViewBag.id = id;
                ViewBag.sessid = sess;

                //var classResult = await _classLevelService.Get(Id);
                //var showPosOnClassResult = classResult.ShowPositionOnClassResult;
                //ViewBag.showPosOnClassResult = showPosOnClassResult;

                return View();
            }
            catch (Exception ex)
            {
                var session = await _sessionService.GetAllSession();
                ViewBag.sessionId = new SelectList(session, "Id", "FullSession");
                return View("Result");
            }
        }


        #endregion cumulative
    }
}