using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Entities;
using SchoolPortal.Web.Models.ResultArchive;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SchoolPortal.Web.Areas.SuperUser.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class ResultArchiveController : Controller
    {
        #region services


        private ApplicationDbContext db = new ApplicationDbContext();
        private ISessionService _sessionService = new SessionService();
        private IClassLevelService _classlevelService = new ClassLevelService();
        private IPublishResultService _publishService = new PublishResultService();
        private IEnrollmentService _enrolService = new EnrollmentService();
        private IResultArchiveService _resultService = new ResultArchiveService();
        private IStudentProfileService _studentService = new StudentProfileService();
        private ISettingService _settingService = new SettingService();
        private IEnrolledSubjectArchiveService _enrolledService = new EnrolledSubjectArchiveService();


        public ResultArchiveController()
        {

        }
        public ResultArchiveController(
            SessionService sessionService,
            ClassLevelService classLevelService,
            PublishResultService publishService,
            ResultArchiveService resultService,
            StudentProfileService studentService,
            SettingService settingService,
            EnrolledSubjectArchiveService enrolledService,
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

        // GET: SuperUser/ResultArchive
        public async Task<ActionResult> Index()
        {
            var archive = await db.ArchiveResults.Include(x => x.ClassLevel).Include(x => x.Session).Where(x => x.Session.ArchiveStatus == ArchiveStatus.Archived).OrderBy(x => x.ClassLevel.ClassName).ToListAsync();
            return View(archive);
        }

        public async Task<ActionResult> ArchiveResult()
        {
            var classlevel = await _classlevelService.ClassLevelList();
            ViewBag.ClassLevelId = new SelectList(classlevel, "Id", "ClassLevelName");

            var session = await _sessionService.GetAllSession();
            ViewBag.sessionId = new SelectList(session, "Id", "FullSession");

            var classes = await db.ClassLevels.Include(x => x.Session).OrderBy(x => x.ClassName).ToListAsync();
            return View(classes);
        }

        [HttpPost]
        public async Task<ActionResult> ArchiveResult(int sessId, int classId)
        {
            int countStudentCount = 0;
            int countUpdateCount = 0;
            var check = await db.ArchiveResults.FirstOrDefaultAsync(x => x.ClassLevelId == classId && x.SessionId == sessId);

            if (check != null)
            {
                TempData["error"] = "Result has been Archived already.";
                return RedirectToAction("ArchiveResult");
            }
            else
            {

                var sett = db.Settings.FirstOrDefault();
                var sessionlist = await _sessionService.GetAllSession();

                //School Fees Archive
                var schFees = db.SchoolFees.Include(x => x.Session).Where(x => x.SessionId == sessId).ToList();
                var feecheck2 = db.SchoolFeesArchive.Where(x => x.SessionId == sessId).FirstOrDefault();
                if (feecheck2 == null && schFees != null)
                {
                    foreach (var fee in schFees)
                    {
                        SchoolFeesArchive sch = new SchoolFeesArchive();
                        sch.Amount = fee.Amount;
                        sch.AmountDue = fee.AmountDue;
                        sch.Category = fee.Category;
                        sch.Discount = fee.Discount;
                        sch.SessionId = fee.SessionId;
                        sch.SchoolFeesId = fee.Id;
                        db.SchoolFeesArchive.Add(sch);
                        await db.SaveChangesAsync();
                    }
                }


                //Principal Archive
                var principals = db.Sessions.Where(x => x.Id == sessId).FirstOrDefault();
                var principals2 = db.PrincipalArchives.Where(x => x.SessionId == sessId).FirstOrDefault();
                if (principals2 == null && principals != null)
                {

                    PrincipalArchive principal = new PrincipalArchive();
                    principal.PrincipalName = principals.SchoolPrincipal;
                    principal.SessionId = sessId;
                    db.PrincipalArchives.Add(principal);
                    await db.SaveChangesAsync();

                }



                //News Letter Archive

                var newsLettes = db.NewsLetters.Include(x => x.Session).Where(x => x.SessionId == sessId).FirstOrDefault();
                var newsLettes2 = db.NewsLetterArchive.Where(x => x.SessionId == sessId).FirstOrDefault();
                if (newsLettes2 == null && newsLettes != null)
                {
                    NewsLetterArchive newsArchive = new NewsLetterArchive();
                    newsArchive.GenRemark = newsLettes.GenRemark;
                    newsArchive.NextTResumptionDate = newsLettes.NextTResumptionDate;
                    newsArchive.PTAFee = newsLettes.PTAFee;
                    newsArchive.PTAMeetingDate = newsLettes.PTAMeetingDate;
                    newsArchive.SessionId = sessId;
                    db.NewsLetterArchive.Add(newsArchive);
                    await db.SaveChangesAsync();
                }

                var enrolledStudents = await _resultService.StudentsBySessIdAndByClassIdArchive(sessId, classId);
                //var batchenro = enrolledStudents.Where(x => x.AverageScore > 0);
                //System.Random randomInteger = new System.Random();
                // int genNumber = randomInteger.Next(10000);
                foreach (var studentresult in enrolledStudents)
                {
                    var studentId = await db.StudentProfiles.FirstOrDefaultAsync(x => x.StudentRegNumber == studentresult.StudentRegNumber);

                    var checkDebtProfile = await _resultService.GetDefaulterByProfileId(studentId.Id);


                    if (checkDebtProfile == null)
                    {
                        var enrollment = await _resultService.GetEnrollmentBySessIdStudentProfileId(sessId, studentId.Id);
                        //var subjects = db.EnrolledSubjects.Include(x => x.Enrollments).Include(x => x.Subject).Include(x => x.Enrollments.ClassLevel).Where(x => x.EnrollmentId == enrollment.Id && x.IsOffered == true && x.TotalScore != null).ToList();
                        var subjects = db.EnrolledSubjects.Include(x => x.Enrollments).Include(x => x.Subject).Include(x => x.Enrollments.ClassLevel).Where(x => x.EnrollmentId == enrollment.Id && x.IsOffered).ToList();

                        var affectivedomains = db.AffectiveDomains.Where(x => x.EnrolmentId == enrollment.Id).ToList();
                        foreach (var affectivedomain in affectivedomains)
                        {
                            AffectiveDomainArchive domain1 = new AffectiveDomainArchive();
                            domain1.EnrolmentId = affectivedomain.EnrolmentId;
                            domain1.Attentiveness = affectivedomain.Attentiveness;
                            domain1.Honesty = affectivedomain.Honesty;
                            domain1.Neatness = affectivedomain.Neatness;
                            domain1.Punctuality = affectivedomain.Punctuality;
                            domain1.Relationship = affectivedomain.Punctuality;
                            db.AffectiveDomainArchive.Add(domain1);
                            await db.SaveChangesAsync();
                        }

                        var recognitivedomains = db.RecognitiveDomains.Where(x => x.EnrolmentId == enrollment.Id).ToList();
                        foreach (var recognitivedomain in recognitivedomains)
                        {
                            RecognitiveDomainArchive domain2 = new RecognitiveDomainArchive();
                            domain2.EnrolmentId = recognitivedomain.EnrolmentId;
                            domain2.Analyzing = recognitivedomain.Analyzing;
                            domain2.Application = recognitivedomain.Application;
                            domain2.Creativity = recognitivedomain.Creativity;
                            domain2.Evaluation = recognitivedomain.Evaluation;
                            domain2.Rememberance = recognitivedomain.Rememberance;
                            domain2.Understanding = recognitivedomain.Understanding;
                            db.RecognitiveDomainArchive.Add(domain2);
                            await db.SaveChangesAsync();
                        }

                        var psychomotordomains = db.PsychomotorDomains.Where(x => x.EnrolmentId == enrollment.Id).ToList();
                        foreach (var psychomotordomain in psychomotordomains)
                        {
                            PsychomotorDomainArchive domain3 = new PsychomotorDomainArchive();
                            domain3.EnrolmentId = psychomotordomain.EnrolmentId;
                            domain3.Club = psychomotordomain.Club;
                            domain3.Drawing = psychomotordomain.Drawing;
                            domain3.Handwriting = psychomotordomain.Handwriting;
                            domain3.Hobbies = psychomotordomain.Hobbies;
                            domain3.Painting = psychomotordomain.Painting;
                            domain3.Speech = psychomotordomain.Speech;
                            domain3.Sports = psychomotordomain.Sports;
                            db.PsychomotorDomainArchive.Add(domain3);
                            await db.SaveChangesAsync();

                        }

                        foreach (var subject in subjects)
                        {
                            EnrolledSubjectArchive archive = new EnrolledSubjectArchive();
                            archive.EnrollmentId = subject.EnrollmentId;
                            archive.ExamScore = subject.ExamScore;
                            archive.IsOffered = subject.IsOffered;
                            archive.SubjectName = subject.Subject.SubjectName;
                            archive.TestScore = subject.TestScore;
                            archive.TotalScore = subject.TotalScore;
                            archive.GradingOption = subject.GradingOption;
                            db.EnrolledSubjectArchive.Add(archive);
                            await db.SaveChangesAsync();

                            countUpdateCount++;
                        }

                        var enr = db.Enrollments.Include(x => x.ClassLevel)
                           .Include(x => x.EnrolledSubjectArchive)
                           .Include(x => x.EnrolledSubjects).Include(x => x.Session).FirstOrDefault(x => x.Id == studentresult.EnrollmentId);
                        if (enr != null)
                        {
                            enr.ArchiveAverageScore = studentresult.AverageScore;
                            db.Entry(enr).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                        }

                    }
                    countStudentCount++;
                }
                var findarchive = db.ArchiveResults.Where(x => x.SessionId == sessId && x.ClassLevelId == classId).FirstOrDefault();

                if (findarchive == null)
                {
                    ArchiveResult archiveResult = new ArchiveResult();
                    archiveResult.ClassLevelId = classId;
                    archiveResult.SessionId = sessId;
                    db.ArchiveResults.Add(archiveResult);
                    await db.SaveChangesAsync();
                }

                //Update Archived Session Status
                var findsession = db.Sessions.Where(x => x.Id == sessId).FirstOrDefault();
                if (findsession != null)
                {
                    findsession.ArchiveStatus = ArchiveStatus.Archived;
                    db.Entry(findsession).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }


                TempData["success"] = countStudentCount + " " + "Results with" +" "+ countUpdateCount + " " + "Subjects added to Archive  successfully";
                return RedirectToAction("ArchiveResult");
            }
        }


        [HttpPost]
        public async Task<ActionResult> DeleteArchiveResult(int sessId, int classId)
        {
            int countDeleteCount = 0;
            int countStudentCount = 0;

            var check = await db.ArchiveResults.Include(x => x.ClassLevel).Include(x => x.Session).Where(x => x.ClassLevelId == classId && x.SessionId == sessId).FirstOrDefaultAsync();
            if (check == null)
            {
                TempData["error"] = "Result does not exists in Archive.";
                return RedirectToAction("Index");
            }
            else
            {

                var sett = db.Settings.FirstOrDefault();
                var sessionlist = await _sessionService.GetAllSession();

                var enrolledStudents = await _resultService.StudentsBySessIdAndByClassIdArchive(sessId, classId);
                //var batchenro = enrolledStudents.Where(x => x.AverageScore > 0);

                foreach (var studentresult in enrolledStudents)
                {
                    var studentId = await db.StudentProfiles.FirstOrDefaultAsync(x => x.StudentRegNumber == studentresult.StudentRegNumber);

                    var checkDebtProfile = await _resultService.GetDefaulterByProfileId(studentId.Id);


                    if (checkDebtProfile == null)
                    {
                        var enrollment = await _resultService.GetEnrollmentBySessIdStudentProfileId(sessId, studentId.Id);
                        //var subjects = db.EnrolledSubjects.Include(x => x.Enrollments).Include(x => x.Subject).Include(x => x.Enrollments.ClassLevel).Where(x => x.EnrollmentId == enrollment.Id && x.IsOffered == true && x.TotalScore != null).ToList();

                        var subjectarchive = db.EnrolledSubjectArchive.Include(x => x.Enrollments).Where(x => x.EnrollmentId == enrollment.Id).ToList();
                        if (subjectarchive != null)
                        {
                            foreach (var remove in subjectarchive)
                            {
                                db.EnrolledSubjectArchive.Remove(remove);
                                await db.SaveChangesAsync();

                                countDeleteCount++;
                            }



                        }

                        var enr = db.Enrollments.Include(x => x.ClassLevel)
                            .Include(x => x.EnrolledSubjectArchive)
                            .Include(x => x.EnrolledSubjects).Include(x => x.Session).FirstOrDefault(x => x.Id == studentresult.EnrollmentId);
                        if (enr != null)
                        {
                            enr.ArchiveAverageScore = studentresult.AverageScore;
                            db.Entry(enr).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                        }



                    }
                    countStudentCount++;
                }
                var findarchive = db.ArchiveResults.Include(x => x.ClassLevel).Include(x => x.Session).Where(x => x.ClassLevelId == classId && x.SessionId == sessId).FirstOrDefault();

                if (findarchive != null)
                {
                    db.ArchiveResults.Remove(findarchive);
                    await db.SaveChangesAsync();
                }

                //Update Archived Session Status
                var findarchive2 = db.ArchiveResults.Include(x => x.ClassLevel).Include(x => x.Session).Where(x =>x.SessionId == sessId).FirstOrDefault();
                var findsession = db.Sessions.Where(x => x.Id == sessId).FirstOrDefault();
                if (findarchive2 == null)
                {
                    findsession.ArchiveStatus = ArchiveStatus.NotArchived;
                    db.Entry(findsession).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }

                TempData["success"] = countStudentCount + " " + "Results with" +" "+ countDeleteCount + " "+ "Subjects deleted from Archive successfully";
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public async Task<ActionResult> UpdateArchiveResult(int sessId, int classId)
        {
            int countUpdateCount = 0;
            int countAddedCount = 0;
            int countStudentCount = 0;

            var check = await db.ArchiveResults.Include(x => x.ClassLevel).Include(x => x.Session).Where(x => x.ClassLevelId == classId && x.SessionId == sessId).FirstOrDefaultAsync();
            if (check == null)
            {
                TempData["error"] = "Result does not exists in Archive.";
                return RedirectToAction("Index");
            }
            else
            {

                var sett = db.Settings.FirstOrDefault();
                var sessionlist = await _sessionService.GetAllSession();

                var enrolledStudents = await _resultService.StudentsBySessIdAndByClassIdArchive(sessId, classId);
                var batchenro = enrolledStudents.Where(x => x.AverageScore > 0);

                foreach (var studentresult in batchenro)
                {
                    var studentId = await db.StudentProfiles.FirstOrDefaultAsync(x => x.StudentRegNumber == studentresult.StudentRegNumber);

                    var checkDebtProfile = await _resultService.GetDefaulterByProfileId(studentId.Id);


                    if (checkDebtProfile == null)
                    {
                        var enrollment = await _resultService.GetEnrollmentBySessIdStudentProfileId(sessId, studentId.Id);
                        //var subjects = db.EnrolledSubjects.Include(x => x.Enrollments).Include(x => x.Subject).Include(x => x.Enrollments.ClassLevel).Where(x => x.EnrollmentId == enrollment.Id && x.IsOffered == true && x.TotalScore != null).ToList();
                        var subjects = db.EnrolledSubjects.Include(x => x.Enrollments).Include(x => x.Subject).Include(x => x.Enrollments.ClassLevel).Where(x => x.EnrollmentId == enrollment.Id && x.IsOffered).ToList();


                        foreach (var subject in subjects)
                        {
                            var subjectarchive = db.EnrolledSubjectArchive.Include(x => x.Enrollments).FirstOrDefault(x => x.EnrollmentSubjectId == subject.Id);
                            if (subjectarchive != null)
                            {


                                subjectarchive.EnrollmentId = subject.EnrollmentId;
                                subjectarchive.ExamScore = subject.ExamScore;
                                subjectarchive.IsOffered = subject.IsOffered;
                                subjectarchive.SubjectName = subject.Subject.SubjectName;
                                subjectarchive.TestScore = subject.TestScore;
                                subjectarchive.TotalScore = subject.TotalScore;
                                subjectarchive.GradingOption = subject.GradingOption;
                                subjectarchive.EnrollmentSubjectId = subject.Id;

                                db.Entry(subjectarchive).State = EntityState.Modified;
                                await db.SaveChangesAsync();

                                countUpdateCount++;
                            }
                            else if (subjectarchive == null)
                            {
                                var affectivedomains = db.AffectiveDomains.Where(x => x.EnrolmentId == enrollment.Id).ToList();
                                foreach (var affectivedomain in affectivedomains)
                                {
                                    AffectiveDomainArchive domain1 = new AffectiveDomainArchive();
                                    domain1.EnrolmentId = affectivedomain.EnrolmentId;
                                    domain1.Attentiveness = affectivedomain.Attentiveness;
                                    domain1.Honesty = affectivedomain.Honesty;
                                    domain1.Neatness = affectivedomain.Neatness;
                                    domain1.Punctuality = affectivedomain.Punctuality;
                                    domain1.Relationship = affectivedomain.Punctuality;
                                    db.AffectiveDomainArchive.Add(domain1);
                                    await db.SaveChangesAsync();
                                }

                                var recognitivedomains = db.RecognitiveDomains.Where(x => x.EnrolmentId == enrollment.Id).ToList();
                                foreach (var recognitivedomain in recognitivedomains)
                                {
                                    RecognitiveDomainArchive domain2 = new RecognitiveDomainArchive();
                                    domain2.EnrolmentId = recognitivedomain.EnrolmentId;
                                    domain2.Analyzing = recognitivedomain.Analyzing;
                                    domain2.Application = recognitivedomain.Application;
                                    domain2.Creativity = recognitivedomain.Creativity;
                                    domain2.Evaluation = recognitivedomain.Evaluation;
                                    domain2.Rememberance = recognitivedomain.Rememberance;
                                    domain2.Understanding = recognitivedomain.Understanding;
                                    db.RecognitiveDomainArchive.Add(domain2);
                                    await db.SaveChangesAsync();
                                }

                                var psychomotordomains = db.PsychomotorDomains.Where(x => x.EnrolmentId == enrollment.Id).ToList();
                                foreach (var psychomotordomain in psychomotordomains)
                                {
                                    PsychomotorDomainArchive domain3 = new PsychomotorDomainArchive();
                                    domain3.EnrolmentId = psychomotordomain.EnrolmentId;
                                    domain3.Club = psychomotordomain.Club;
                                    domain3.Drawing = psychomotordomain.Drawing;
                                    domain3.Handwriting = psychomotordomain.Handwriting;
                                    domain3.Hobbies = psychomotordomain.Hobbies;
                                    domain3.Painting = psychomotordomain.Painting;
                                    domain3.Speech = psychomotordomain.Speech;
                                    domain3.Sports = psychomotordomain.Sports;
                                    db.PsychomotorDomainArchive.Add(domain3);
                                    await db.SaveChangesAsync();

                                }

                                EnrolledSubjectArchive archive = new EnrolledSubjectArchive();
                                archive.EnrollmentId = subject.EnrollmentId;
                                archive.ExamScore = subject.ExamScore;
                                archive.IsOffered = subject.IsOffered;
                                archive.SubjectName = subject.Subject.SubjectName;
                                archive.TestScore = subject.TestScore;
                                archive.TotalScore = subject.TotalScore;
                                archive.GradingOption = subject.GradingOption;
                                archive.EnrollmentSubjectId = subject.Id;
                                db.EnrolledSubjectArchive.Add(archive);
                                await db.SaveChangesAsync();

                                countAddedCount++;
                            }

                        }
                        var enr = db.Enrollments.Include(x => x.ClassLevel)
                            .Include(x => x.EnrolledSubjectArchive)
                            .Include(x => x.EnrolledSubjects).Include(x => x.Session).FirstOrDefault(x => x.Id == studentresult.EnrollmentId);
                        if (enr != null)
                        {
                            enr.ArchiveAverageScore = studentresult.AverageScore;
                            db.Entry(enr).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                        }

                    }
                    countStudentCount++;
                }
                var findarchive = db.ArchiveResults.Include(x => x.ClassLevel).Include(x => x.Session).Where(x => x.ClassLevelId == classId && x.SessionId == sessId).FirstOrDefault();

                if (findarchive != null)
                {
                    findarchive.ClassLevelId = classId;
                    findarchive.SessionId = sessId;

                    db.Entry(findarchive).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }

                //Update Archived Session Status
                var findsession = db.Sessions.Where(x => x.Id == sessId).FirstOrDefault();
                if (findsession != null)
                {
                    findsession.ArchiveStatus = ArchiveStatus.Archived;
                    db.Entry(findsession).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }

                TempData["success"] = countStudentCount +" "+ "Results with" +" "+ countUpdateCount + " " + "Subjects updated successfully" + " " + "and" + " " + countAddedCount + " " + "Results Subject was added to the Archive";
                return RedirectToAction("Index");
            }
        }

    }
}