using Newtonsoft.Json;
using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Areas.Service;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Dtos.Api;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Areas.CBTExam.Controllers
{
    public class CBTExamResultManagerController : Controller
    {
        #region services


        private ApplicationDbContext db = new ApplicationDbContext();
        private ISessionService _sessionService = new SessionService();
        private IClassLevelService _classlevelService = new ClassLevelService();
        private IPublishResultService _publishService = new PublishResultService();
        private IEnrollmentService _enrolService = new EnrollmentService();
        private IResultService _resultService = new ResultService();
        private IStudentProfileService _studentService = new StudentProfileService();
        private ISettingService _settingService = new SettingService();
        private IEnrolledSubjectService _enrolledService = new EnrolledSubjectService();
        private HttpClient client = new HttpClient();


        public CBTExamResultManagerController()
        {
            //client.BaseAddress = new Uri("http://localhost:58920/");
            //client.BaseAddress = new Uri("http://cbttest.iskools.com/");
            //client.BaseAddress = new Uri("http://cbt.iskools.com/");
            var baseUrl = db.Settings.FirstOrDefault().CBTLink;
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public CBTExamResultManagerController(
            SessionService sessionService,
            ClassLevelService classLevelService,
            PublishResultService publishService,
            ResultService resultService,
            StudentProfileService studentService,
            SettingService settingService,
            EnrolledSubjectService enrolledService,
            EnrollmentService enrolService
            )
        {
            _sessionService = sessionService;
            _classlevelService = classLevelService;
            _publishService = publishService;
            _enrolService = enrolService;
            _resultService = resultService;
            _studentService = studentService;
            _settingService = settingService;
            _enrolledService = enrolledService;
        }
        #endregion
        // GET: CBTExam/CBTResultManager
        public async Task<ActionResult> Index()
        {
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel, "Id", "ClassLevelName");

            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            ViewBag.sessionId = session.Id;
            ViewBag.session = session.SessionYear + " - " + session.Term + " Term";
            return View();
        }

        public async Task<ActionResult> OtherTerm()
        {
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel, "Id", "ClassLevelName");

            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session, "Id", "FullSession");
            return View();
        }

        public async Task<ActionResult> Students(int sessId, int classId)
        {
            var sett = db.Settings.FirstOrDefault();
            ViewBag.sett = sett;

            await _enrolService.UpdateEnrollmentStatus(classId);
            var enrolledStudents = await _resultService.StudentsBySessIdAndByClassId(sessId, classId);
            if (enrolledStudents.Count() <= 0)
            {
                TempData["msg"] = "HttpNotFound";
                return RedirectToAction("Index");
            }



            var setting = db.Settings.FirstOrDefault();
            ViewBag.setting = setting;

            var classResult = await _classlevelService.Get(classId);
            var showPosOnClassResult = classResult.ShowPositionOnClassResult;
            ViewBag.showPosOnClassResult = showPosOnClassResult;

            //UpdateComment recognitive domain
            foreach (var abcd in enrolledStudents)
            {
                var studentsss = await db.StudentProfiles.FirstOrDefaultAsync(x => x.StudentRegNumber == abcd.StudentRegNumber);
                var checki = await db.RecognitiveDomains.FirstOrDefaultAsync(x => x.EnrolmentId == abcd.EnrollmentId);
                if (checki == null)
                {
                    RecognitiveDomain recognitive = new RecognitiveDomain();
                    recognitive.EnrolmentId = abcd.EnrollmentId;
                    db.RecognitiveDomains.Add(recognitive);
                    await db.SaveChangesAsync();
                }


            }

            //UpdateComment psychomotor domain
            foreach (var abcd in enrolledStudents)
            {
                var studentsss = await db.StudentProfiles.FirstOrDefaultAsync(x => x.StudentRegNumber == abcd.StudentRegNumber);
                var checki = await db.PsychomotorDomains.FirstOrDefaultAsync(x => x.EnrolmentId == abcd.EnrollmentId);
                if (checki == null)
                {
                    PsychomotorDomain psychomotor = new PsychomotorDomain();
                    psychomotor.EnrolmentId = abcd.EnrollmentId;
                    db.PsychomotorDomains.Add(psychomotor);
                    await db.SaveChangesAsync();
                }


            }


            //UpdateComment School Fees
            //foreach (var abcd in enrolledStudents)
            //{
            //    var studentsss = await db.StudentProfiles.FirstOrDefaultAsync(x => x.StudentRegNumber == abcd.StudentRegNumber);
            //    var checki = await db.SchoolFees.FirstOrDefaultAsync(x => x.EnrolmentId == abcd.EnrollmentId);
            //    if (checki == null)
            //    {
            //        SchoolFees fee = new SchoolFees();
            //        fee.EnrolmentId = abcd.EnrollmentId;
            //        db.SchoolFees.Add(fee);
            //        await db.SaveChangesAsync();
            //    }


            //}


            //UpdateComment affective domain
            foreach (var abcd in enrolledStudents)
            {
                var studentsss = await db.StudentProfiles.FirstOrDefaultAsync(x => x.StudentRegNumber == abcd.StudentRegNumber);
                var checki = await db.AffectiveDomains.FirstOrDefaultAsync(x => x.EnrolmentId == abcd.EnrollmentId);
                if (checki == null)
                {
                    AffectiveDomain affective = new AffectiveDomain();
                    affective.EnrolmentId = abcd.EnrollmentId;
                    db.AffectiveDomains.Add(affective);
                    await db.SaveChangesAsync();
                }


            }
            var check = await _publishService.CheckPublishResult(sessId, classId);
            if (check == true)
            {
                ViewBag.Check = "Published";
            }
            else
            {
                ViewBag.Check = "Not Published";
            }

            var classlevel = await _classlevelService.Get(classId);
            var session = await _sessionService.Get(sessId);
            ViewBag.Id = sessId;
            ViewBag.ClassId = classId;
            ViewBag.sessionId = session.Id;
            ViewBag.Term = session.Term.ToLower();
            ViewBag.Class = classlevel.ClassName;
            ViewBag.ClassSubjectCount = classlevel.Subjects.Count();
            var stinfo = classlevel.UserId;
            var staff = await db.Users.FirstOrDefaultAsync(x => x.Id == stinfo);
            ViewBag.formteacher = staff.Surname + " " + staff.FirstName + " " + staff.OtherName;
            ViewBag.Session = session.SessionYear + " - " + session.Term + " Term";
            foreach (var name in enrolledStudents)
            {
                if (name.StudentName.Substring(0, 2).Contains("\t"))
                {
                    name.StudentName = name.StudentName.Remove(0, 2);
                }
                if (name.StudentName.Substring(0, 2).Contains(" "))
                {
                    name.StudentName = name.StudentName.TrimStart();
                }
            }
            return View(enrolledStudents);
        }


        [HttpPost]
        public async Task<ActionResult> UpdateScore(int? sessionId, int? classId)
        {
            if (sessionId != null && classId != null)
            {
                var session = await db.Sessions.FirstOrDefaultAsync(x => x.Id == sessionId);
                var sessId = session.Id;
                var sessionYear = session.SessionYear;
                var sessionTerm = session.Term;
                string unixconverify = User.Identity.Name;
                string xgink = GeneralService.PortalLink();
                string role = "admin";
                string nameswitheerror = "";
                int sn = 0;
                double examscore = 0;
                double testscore = 0;

                var enrolled = await db.Enrollments.Include(x => x.Session).Include(x => x.ClassLevel).Where(x => x.ClassLevelId == classId && x.Session.Id == sessId && x.Session.SessionYear == sessionYear && x.Session.Term == sessionTerm).ToListAsync();
                int countSus = 0;
                foreach (var item in enrolled)
                {
                    HttpResponseMessage response = client.GetAsync("/api/ExamResultApi/GetExamByClassSession?unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role + "&schoolClassId=" + classId + "&session=" + sessionYear + "&term=" + sessionTerm).Result;
                    var response2 = response.Content.ReadAsStringAsync().Result;
                    List<CBTSettingModelDto> data = JsonConvert.DeserializeObject<List<CBTSettingModelDto>>(response2);
                    if (data != null)
                    {
                        foreach (var sett in data)
                        {
                            var cid = db.Enrollments.Include(x => x.ClassLevel).FirstOrDefault(x => x.Id == item.Id);
                            var std = db.StudentProfiles.Include(x => x.user).FirstOrDefault(x => x.Id == cid.StudentProfileId);
                            HttpResponseMessage response3 = client.GetAsync("/api/ExamResultApi/ExamBySettingSessionReNumber?unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role + "&regNumber=" + std.StudentRegNumber + "&session=" + sessionYear + "&term=" + sessionTerm + "&settingId=" + sett.Id).Result;
                            var response4 = response3.Content.ReadAsStringAsync().Result;
                            List<CBTExaminationDto> data2 = JsonConvert.DeserializeObject<List<CBTExaminationDto>>(response4);

                            if (data2 != null)
                            {
                                foreach (var exam in data2)
                                {
                                    HttpResponseMessage response5 = client.GetAsync("api/ExamResultApi/GetExamSubjectResultForStudent?unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role + "&settingId=" + sett.Id + "&examId=" + exam.Id).Result;
                                    var response6 = response5.Content.ReadAsStringAsync().Result;
                                    List<CBTExaminationSubjectDto> data3 = JsonConvert.DeserializeObject<List<CBTExaminationSubjectDto>>(response6);

                                    if (data3 != null)
                                    {
                                        foreach (var sub in data3)
                                        {
                                            try
                                            {
                                                if (sub.Score != 0)
                                                {
                                                    var setting = await db.ClassLevels.FirstOrDefaultAsync(x => x.Id == sub.SchoolClassId);
                                                    var subject = await _resultService.GetSubjectByEnrolledSubId(sub.SchoolSubjectId);
                                                    var classlevel = await _resultService.GetClassByClassId(sub.SchoolClassId);
                                                    var enrolsubject = db.EnrolledSubjects.Include(x => x.Enrollments).Include(x => x.Subject).FirstOrDefault(x => x.SubjectId == subject.Id && x.EnrollmentId == item.Id && x.Enrollments.Session.SessionYear == sessionYear && x.Enrollments.Session.Term == sessionTerm);
                                                    if (enrolsubject.ExamScore > setting.ExamScore || enrolsubject.TestScore > setting.AccessmentScore)
                                                    {
                                                        if (sett.ExamMode == Mode.Exam)
                                                        {
                                                            examscore = sub.Score;
                                                        }
                                                        else if (sett.ExamMode == Mode.Test)
                                                        {
                                                            testscore = sub.Score;
                                                        }



                                                        nameswitheerror = nameswitheerror + "(" + sn++ + ")" + cid.StudentProfile.StudentRegNumber + ", has exam " + examscore + " and C.A " + testscore + " that are out of range /// <br/>";

                                                        ViewBag.sessId = sessId;

                                                        TempData["error"] = nameswitheerror;

                                                        return Content("<script language='javascript' type='text/javascript'>alert('Test Score or Exam Score not in Range');</script>");
                                                    }
                                                    else
                                                    {
                                                        string userLevel = classlevel.ClassName;

                                                        if (enrolsubject.GradingOption == GradingOption.NONE)
                                                        {
                                                            if (userLevel.Substring(0, 3).Contains("SSS"))
                                                            {
                                                                enrolsubject.GradingOption = GradingOption.SSS;
                                                            }
                                                            else if (userLevel.Substring(0, 3).Contains("JSS"))
                                                            {
                                                                enrolsubject.GradingOption = GradingOption.JSS;
                                                            }
                                                            else if (userLevel.Substring(0, 3).Contains("NUR"))
                                                            {
                                                                enrolsubject.GradingOption = GradingOption.NUR;
                                                            }
                                                            else if (userLevel.Substring(0, 3).Contains("PRI"))
                                                            {
                                                                enrolsubject.GradingOption = GradingOption.PRI;
                                                            }
                                                            else if (userLevel.Substring(0, 3).Contains("PRE"))
                                                            {
                                                                enrolsubject.GradingOption = GradingOption.PRE;
                                                            }
                                                            else if (userLevel.Substring(0, 2).Contains("PG"))
                                                            {
                                                                enrolsubject.GradingOption = GradingOption.PG;
                                                            }
                                                        }

                                                        if (sett.ExamMode == Mode.Exam)
                                                        {
                                                            enrolsubject.ExamScore = Convert.ToDecimal(sub.Score);
                                                            decimal? totalScore = enrolsubject.TestScore + enrolsubject.ExamScore;
                                                            enrolsubject.TotalScore = totalScore;
                                                            enrolsubject.IsOffered = true;
                                                            db.Entry(enrolsubject).State = EntityState.Modified;
                                                            await db.SaveChangesAsync();
                                                        }
                                                        else if (sett.ExamMode == Mode.Test)
                                                        {
                                                            enrolsubject.TestScore = Convert.ToDecimal(sub.Score);
                                                            db.Entry(enrolsubject).State = EntityState.Modified;
                                                            decimal? totalScore = enrolsubject.TestScore + enrolsubject.ExamScore;
                                                            enrolsubject.TotalScore = totalScore;
                                                            enrolsubject.IsOffered = true;

                                                            await db.SaveChangesAsync();
                                                        }

                                                        await _resultService.UpdateResult(cid.Id);
                                                        countSus++;

                                                    }

                                                }

                                            }
                                            catch (Exception e)
                                            {

                                            }

                                        }



                                    }

                                }
                            }

                        }
                    }
                    else
                    {
                        TempData["error"] = "There is no CBT Score so therefore result failed to update";
                    }

                  
                }

                TempData["success"] = countSus + " Update was successful Out Of " + enrolled.Count();

                //TempData["success"] = "CBT Result has been updated to student school result";



            }
            else
            {
                TempData["error"] = "Student  failed to update if the error persist contact the Administrator!";
            }
            return RedirectToAction("Students", new { sessId = sessionId, classId = classId });
        }


        public async Task<ActionResult> StudentExam(string regNumber,string session,string term)
        {
            string unixconverify = "Admin";
            string xgink = GeneralService.PortalLink();
            string role = "admin";
            ViewBag.xgink = xgink;
            ViewBag.unixconverify = unixconverify;
            ViewBag.role = role;

            HttpResponseMessage response = client.GetAsync("/api/ExamBoardApi/GetStudentExamByRegNumber?regNumber=" + regNumber + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role + "&session=" + session + "&term=" + term).Result;
            var response2 = response.Content.ReadAsStringAsync().Result;
            List<CBTExaminationDto> data = JsonConvert.DeserializeObject<List<CBTExaminationDto>>(response2);
            ViewBag.data = data;
            return View(data);
        }

        public async Task<ActionResult> StudentExamScore(int? examId, string unixconverify, string xgink, string role)
        {
            ViewBag.xgink = xgink;
            ViewBag.unixconverify = unixconverify;
            ViewBag.role = role;

            HttpResponseMessage response = client.GetAsync("/api/ExamBoardApi/GetStudentExamScore?examId=" + examId + "&unixconverify=" + unixconverify + "&xgink=" + xgink + "&role=" + role).Result;
            var response2 = response.Content.ReadAsStringAsync().Result;
            List<CBTExaminationSubjectDto> data = JsonConvert.DeserializeObject<List<CBTExaminationSubjectDto>>(response2);
            ViewBag.data = data;
            return View(data);
        }
    }
}