using SchoolPortal.Web.Areas.Data.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using SchoolPortal.Web.Models;
using System.Data.Entity;
using SchoolPortal.Web.Models.Entities;
using SchoolPortal.Web.Models.Dtos;
using SchoolPortal.Web.Models.ResultArchive;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace SchoolPortal.Web.Areas.Data.Services
{
    public class ResultArchiveService : IResultArchiveService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ResultArchiveService()
        {

        }

        public ResultArchiveService(ApplicationUserManager userManager,
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

        public async Task<List<EnrolledSubjectArchive>> EnrolledSubjectForEnrollment(int enrollmentId)
        {
            var items = await db.EnrolledSubjectArchive.Include(x => x.Enrollments.StudentProfile.user).Include(x => x.Enrollments).Include(x => x.Enrollments.ClassLevel).Where(x => x.EnrollmentId == enrollmentId && x.SubjectName != null).OrderBy(x => x.SubjectName).ThenBy(x => x.Id).ToListAsync();
            return items;
        }

        public async Task<ClassLevel> GetClassByClassId(int? id)
        {
            var userLevel = await db.ClassLevels.Include(x => x.Subjects).FirstOrDefaultAsync(x => x.Id == id);
            return userLevel;
        }

        public async Task<EnrolledSubjectArchive> GetSubjectByEnrolledSubId(int? id)
        {
            var subject = await db.EnrolledSubjectArchive.FirstOrDefaultAsync(x => x.Id == id);
            return subject;
        }

        public async Task<List<Enrollment>> Position(int sessionId, int? classLevelId)
        {
            var positions = await db.Enrollments.Include(x => x.StudentProfile).Where(s => s.SessionId == sessionId && s.ClassLevelId == classLevelId).OrderByDescending(x => x.AverageScore).ToListAsync();
            return positions;
        }

        public async Task PublishResult(int id, int classId)
        {
            PublishResult model = new PublishResult();
            model.PublishedDate = DateTime.Now;
            model.ClassLevelId = classId;
            model.SessionId = id;
            db.PublishResults.Add(model);
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if(userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Published Results";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
            
        }

        /// <summary>
        /// cumm
        /// </summary>
        /// <param name="sessId"></param>
        /// <param name="classId"></param>
        /// <returns></returns>
        /// 

        ///result reconcilliation
        ///

        public async Task<string> CumulativeResultReconciliationByClassId(int sessId, int classId)
        {
            var currentYear = db.Sessions.FirstOrDefault(f => f.Id == sessId);
            string year = currentYear.SessionYear;
            //Get Session IDs for each term
            //var firstTerm = db.Sessions.FirstOrDefault(x => x.SessionYear == year && x.Term == "First");
            //var secondTerm = db.Sessions.FirstOrDefault(x => x.SessionYear == year && x.Term == "Second");
            //var thirdTerm = db.Sessions.FirstOrDefault(x => x.SessionYear == year && x.Term == "Third");


            var EnrollmentbyclassSession = db.Enrollments.Include(v => v.Session).Include(x => x.ClassLevel).Where(f => f.ClassLevelId == classId && f.Session.SessionYear == year).ToList();

            foreach (var eachenrolled in EnrollmentbyclassSession)
            {

                var mainenrol = db.Enrollments.Include(v => v.Session).Include(x => x.ClassLevel).Where(f => f.StudentProfileId == eachenrolled.StudentProfileId && f.Session.SessionYear == year).ToList();

                var firsttermtotalscore = mainenrol.FirstOrDefault(x => x.StudentProfileId == eachenrolled.StudentProfileId && x.Session.Term.ToLower() == "first");
                var secondtermtotalscore = mainenrol.FirstOrDefault(x => x.StudentProfileId == eachenrolled.StudentProfileId && x.Session.Term.ToLower() == "second");
                var thirdtermtotalscore = mainenrol.FirstOrDefault(x => x.StudentProfileId == eachenrolled.StudentProfileId && x.Session.Term.ToLower() == "third");

                //if second and third term are empty. use first term as cumulative
                //if (secondtermtotalscore.AverageScore == null && thirdtermtotalscore.AverageScore == null)
                //{

                //    firsttermtotalscore.CummulativeAverageScore = firsttermtotalscore.AverageScore;
                //}
                //if
                try
                {


                    if (thirdtermtotalscore.AverageScore == null)
                    {
                        decimal? fst = 0;
                        if (firsttermtotalscore.AverageScore != null)
                        {
                            fst = firsttermtotalscore.AverageScore;
                        }
                        decimal? snd = 0;
                        if (secondtermtotalscore.AverageScore != null)
                        {
                            snd = secondtermtotalscore.AverageScore;
                        }
                        secondtermtotalscore.CummulativeAverageScore = (fst + snd) / 2;
                    }

                    db.Entry(secondtermtotalscore).State = EntityState.Modified;
                    await db.SaveChangesAsync();


                    //Add Tracking
                    var userId = HttpContext.Current.User.Identity.GetUserId();
                    if(userId != null)
                    {
                        var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                        Tracker tracker = new Tracker();
                        tracker.UserId = userId;
                        tracker.UserName = user.UserName;
                        tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                        tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                        tracker.Note = tracker.FullName + " " + "Cumulative Result Reconciliation";
                        //db.Trackers.Add(tracker);
                        await db.SaveChangesAsync();
                    }
                   
                }
                catch (Exception c)
                {

                }

                //if (secondtermtotalscore.AverageScore == null)
                //{
                //    decimal? fst = 0;
                //    if (firsttermtotalscore.AverageScore != null)
                //    {
                //        fst = firsttermtotalscore.AverageScore;
                //    }
                //    decimal? thd = 0;
                //    if (thirdtermtotalscore.AverageScore != null)
                //    {
                //        thd = thirdtermtotalscore.AverageScore;
                //    }
                //    thirdtermtotalscore.CummulativeAverageScore = fst + thd;
                //}

                //if (firsttermtotalscore.AverageScore == null)
                //{

                //    secondtermtotalscore.CummulativeAverageScore = firsttermtotalscore.AverageScore + secondtermtotalscore.AverageScore;
                //}



            }

            //List<EnrolledSubject> enrolledSubjectsFirst = new List<EnrolledSubject>();
            //List<EnrolledSubject> enrolledSubjectsSecond = new List<EnrolledSubject>();
            //List<EnrolledSubject> enrolledSubjectsThird = new List<EnrolledSubject>();
            //string firsttermerror = "";
            //string secondtermerror = "";
            //if (firstEnrollment != null)
            //{
            //    enrolledSubjectsFirst = db.EnrolledSubjects.Include(x => x.Subject).Include(x => x.Enrollments).Where(x => x.EnrollmentId == firstEnrollment.Id && x.IsOffered == true).OrderBy(x => x.Subject.SubjectName).ThenBy(x => x.Id).ToList();


            //}
            //else
            //{
            //    firsttermerror = "Empty";
            //}

            //if (secondEnrollment != null)
            //{
            //    enrolledSubjectsSecond = db.EnrolledSubjects.Include(x => x.Subject).Include(x => x.Enrollments).Where(x => x.EnrollmentId == secondEnrollment.Id && x.IsOffered == true).OrderBy(x => x.Subject.SubjectName).ThenBy(x => x.Id).ToList();

            //}
            //else
            //{
            //    secondtermerror = "Empty";
            //}
            //enrolledSubjectsThird = db.EnrolledSubjects.Include(x => x.Subject).Include(x => x.Enrollments).Where(x => x.EnrollmentId == thirdEnrollment.Id && x.IsOffered == true).OrderBy(x => x.Subject.SubjectName).ThenBy(x => x.Id).ToList();


            //if (thirdEnrollment.CummulativeAverageScore == null)
            //{
            //    decimal calulatedAverage = 0;
            //    decimal? cumTotal = 0;
            //    if (firstEnrollment.AverageScore == null && secondEnrollment.AverageScore == null)
            //    {
            //        cumTotal = thirdEnrollment.AverageScore;
            //        if (cumTotal == null)
            //        {
            //            calulatedAverage = 0.00m;
            //        }
            //        else
            //        {
            //            calulatedAverage = cumTotal.Value;
            //        }

            //    }
            //    else if (firstEnrollment.AverageScore == null)
            //    {
            //        cumTotal = secondEnrollment.AverageScore + thirdEnrollment.AverageScore;
            //        if (cumTotal == null)
            //        {
            //            calulatedAverage = 0.00m;
            //        }
            //        else
            //        {
            //            calulatedAverage = cumTotal.Value / 2;
            //        }

            //    }
            //    else if (secondEnrollment.AverageScore == null)
            //    {
            //        cumTotal = firstEnrollment.AverageScore + thirdEnrollment.AverageScore;
            //        if (cumTotal == null)
            //        {
            //            calulatedAverage = 0.00m;
            //        }
            //        else
            //        {
            //            calulatedAverage = cumTotal.Value / 2;
            //        }

            //    }
            //    else
            //    {
            //        cumTotal = firstEnrollment.AverageScore + secondEnrollment.AverageScore + thirdEnrollment.AverageScore;
            //        if (cumTotal == null)
            //        {
            //            calulatedAverage = 0.00m;
            //        }
            //        else
            //        {
            //            calulatedAverage = cumTotal.Value / 3;
            //        }

            //    }

            //    thirdEnrollment.CummulativeAverageScore = calulatedAverage;
            //    db.SaveChanges();
            //}

            //var cummulativePositions = db.Enrollments.Include(x => x.StudentProfile.user).Where(s => s.SessionId == getuserenrollment.SessionId && s.ClassLevelId == getuserenrollment.ClassLevelId).OrderByDescending(x => x.CummulativeAverageScore).ThenBy(x => x.StudentProfile.user.Surname);
            //// int totalStudents = positions.Count();
            //int cummulativePosition = 0;
            //decimal? cumAvg = 0;
            //int cumPosition = 0;
            //string cumAverage = "";
            //if (subjectCount != 0)
            //{
            //    foreach (var p in cummulativePositions)
            //    {
            //        cummulativePosition = cummulativePosition + 1;
            //        if (p.Id == id)
            //        {

            //            cumAvg = p.CummulativeAverageScore.Value;
            //            goto outloop;
            //        }
            //    }

            //outloop: cumPosition = cummulativePosition;
            //    decimal cummulativeAverage = cumAvg.Value;
            //    string dispaly = cummulativeAverage.ToString("0.00");
            //    cumAverage = dispaly;



            //    output = new PrintThirdTermDto
            //    {

            //        Average = useraveragescore,
            //        Position = userposition,
            //        ClassName = getuserenrollment.ClassLevel.ClassName,
            //        TotalStudent = totalStudents,
            //        SessionTerm = currentsessiomterm,
            //        SchoolName = setting.SchoolName,
            //        ResultGrade = grade,
            //        ResultRemark = remark,
            //        RegNumber = getuserprofile.StudentRegNumber,
            //        Fullname = getuserprofile.user.Surname + " " + getuserprofile.user.FirstName + " " + getuserprofile.user.OtherName,
            //        SubjectList = subjectlist.ToList(),
            //        studentImage = abuser,
            //        SchoolLogo = absession,
            //        SchoolStamp = stpp,
            //        GradingDetails = grdsystem.ToList(),
            //        RecognitiveDomain = recognitivedomain,
            //        PsychomotorDomain = psychomotordomain,
            //        AffectiveDomain = affectivedomain,
            //        SchoolAccount = SchoolAcct,
            //        Address = setting.SchoolAddress,
            //        headteacher = getenrollmentsession.SchoolPrincipal,
            //        Email = setting.ContactEmail,
            //        Phone = setting.ContactPhoneNumber,
            //        Website = setting.WebsiteLink,
            //        NewsLetter = newsletter,

            //        TotalScore = totalsc,
            //        OverallScore = overall,
            //        PromotionStatus = promote,
            //        CummulativeAverage = cumAverage,
            //        CummulativePosition = cumPosition,
            //        CummulativeSessionTerm = resultTitle,
            //        CummulativeFirstScore = enrolledSubjectsFirst.ToList(),
            //        CummulativeSecondScore = enrolledSubjectsSecond.ToList(),
            //        CummulativeThirdScore = enrolledSubjectsThird.ToList(),
            //        checkFirthTerm = firsttermerror,
            //        checkSecondTerm = secondtermerror,
            //        EnrollmentRemark = getuserenrollment.EnrollmentRemark1,
            //        EnrollmentRemark2 = getuserenrollment.EnrollmentRemark2,
            //        SchoolFees = schoolfee,
            //        AverageFirthTerm = firstEnrollment.AverageScore,
            //        AverageSecondTerm = secondEnrollment.AverageScore,
            //        AverageThirdTerm = thirdEnrollment.AverageScore,
            //        StudentId = getuserprofile.Id,
            //        IsEngMath = setting.PromotionByMathsAndEng,
            //        cumshowPosOnClassResult = getuserenrollment.ClassLevel.ShowPositionOnClassResult,
            //        cumshowSchAccountOnResult = setting.ShowAccctOnResult,
            //        cumshowSchFeeOnResult = setting.ShowFeesOnResult




            //    };



            //  }
            return "true";
        }

        public async Task<List<EnrolledStudentsByClassArchiveDto>> CumulativeStudentsBySessIdAndByClassId(int sessId, int classId)
        {
            var session = db.Sessions.FirstOrDefault(x => x.Id == sessId);
            var sessions = db.Sessions.Where(x => x.SessionYear == session.SessionYear);

            var fterm = db.Sessions.FirstOrDefault(x => x.Term.ToLower() == "first" && x.SessionYear == session.SessionYear);
            var sterm = db.Sessions.FirstOrDefault(x => x.Term.ToLower() == "second" && x.SessionYear == session.SessionYear);
            var tterm = db.Sessions.FirstOrDefault(x => x.Term.ToLower() == "third" && x.SessionYear == session.SessionYear);

            var enrolledStudents = db.Enrollments.Include(x => x.Session).OrderBy(x => x.StudentProfile.user.Surname).Include(x => x.EnrolledSubjectArchive).Include(x => x.StudentProfile);
            var checkenrolledStudents = db.Enrollments.Include(x => x.Session).OrderBy(x => x.StudentProfile.user.Surname).Include(x => x.EnrolledSubjectArchive).Include(x => x.StudentProfile).Where(x => x.SessionId == sessId && x.ClassLevelId == classId).ToList();
            var noresult = checkenrolledStudents.Where(x => x.AverageScore == null).ToList();
            var withresult = checkenrolledStudents.Where(x => x.AverageScore != null).ToList();
            if (noresult.Count() > withresult.Count())
            {
                //use second term

                enrolledStudents = enrolledStudents.Where(s => s.ClassLevelId == classId && s.SessionId == sterm.Id && s.EnrolledSubjectArchive.Count() > 0);
            }
            else
            {


                enrolledStudents = enrolledStudents.Where(s => s.ClassLevelId == classId && s.SessionId == tterm.Id && s.EnrolledSubjectArchive.Count() > 0);
            }

            var c = enrolledStudents.Count();
            var output = enrolledStudents.OrderByDescending(x => x.CummulativeAverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber).Select(x => new EnrolledStudentsByClassArchiveDto
            {
                Id = x.Id,
                StudentRegNumber = x.StudentProfile.StudentRegNumber,
                AverageScore = x.AverageScore,
                SubjectCount = x.EnrolledSubjectArchive.Where(d => d.TotalScore > 0).Count(),
                StudentName = x.StudentProfile.user.Surname + " " + x.StudentProfile.user.FirstName + " " + x.StudentProfile.user.OtherName,
                EnrollmentId = x.Id,
                StudentId = x.StudentProfile.Id,
                SessionId = x.SessionId,
                SessionYear = x.Session.SessionYear,
                CummulativeAverageScore = x.CummulativeAverageScore


            });
            return await output.ToListAsync();

        }


        public async Task<List<EnrolledStudentsByClassDto>> StudentsBySessIdAndByClassIdArchive(int sessId, int classId)
        {
            var enrolledStudents = db.Enrollments.Include(x => x.Session).OrderBy(x => x.StudentProfile.user.Surname).Include(x => x.EnrolledSubjects).Include(x => x.StudentProfile).Where(s => s.ClassLevelId == classId && s.SessionId == sessId && s.EnrolledSubjects.Count() > 0);
            var c = enrolledStudents.Count();
            var output = enrolledStudents.OrderByDescending(x => x.AverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber).Select(x => new EnrolledStudentsByClassDto
            {
                Id = x.Id,
                StudentRegNumber = x.StudentProfile.StudentRegNumber,
                AverageScore = x.AverageScore,
                SubjectCount = x.EnrolledSubjects.Where(d => d.TotalScore > 0).Count(),
                StudentName = x.StudentProfile.user.Surname + " " + x.StudentProfile.user.FirstName + " " + x.StudentProfile.user.OtherName,
                EnrollmentId = x.Id,
                StudentId = x.StudentProfile.Id,
                SessionId = x.SessionId,
                SessionYear = x.Session.SessionYear,
                CummulativeAverageScore = x.CummulativeAverageScore


            });
            return await output.ToListAsync();

        }


        public async Task<List<EnrolledStudentsByClassArchiveDto>> StudentsBySessIdAndByClassId(int sessId, int classId)
        {
            var enrolledStudents = db.Enrollments.Include(x => x.Session).OrderBy(x => x.StudentProfile.user.Surname).Include(x => x.EnrolledSubjectArchive).Include(x => x.StudentProfile).Where(s => s.ClassLevelId == classId && s.SessionId == sessId && s.EnrolledSubjectArchive.Count() > 0);
            var c = enrolledStudents.Count();
            var output = enrolledStudents.OrderByDescending(x => x.AverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber).Select(x => new EnrolledStudentsByClassArchiveDto
            {
                Id = x.Id,
                StudentRegNumber = x.StudentProfile.StudentRegNumber,
                AverageScore = x.AverageScore,
                SubjectCount = x.EnrolledSubjectArchive.Where(d => d.TotalScore > 0).Count(),
                StudentName = x.StudentProfile.user.Surname + " " + x.StudentProfile.user.FirstName + " " + x.StudentProfile.user.OtherName,
                EnrollmentId = x.Id,
                StudentId = x.StudentProfile.Id,
                SessionId = x.SessionId,
                SessionYear = x.Session.SessionYear,
                CummulativeAverageScore = x.CummulativeAverageScore


            });
            return await output.ToListAsync();

        }

        public async Task<List<EnrolledStudentsByClassArchiveDto>> StudentsByByClassId(int classId)
        {
            var session = db.Sessions.OrderByDescending(x => x.Id);
            if (session != null)
            {
                var currentSession = session.FirstOrDefault(x => x.Status == SessionStatus.Current);
                var enrolledStudents = db.Enrollments.OrderBy(x => x.StudentProfile.user.Surname).Include(x => x.Session).Include(x => x.StudentProfile).Where(s => s.ClassLevelId == classId && s.SessionId == currentSession.Id);

                var output = enrolledStudents.OrderByDescending(x => x.AverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber).Select(x => new EnrolledStudentsByClassArchiveDto
                {
                    Id = x.Id,
                    StudentId = x.StudentProfileId,
                    StudentRegNumber = x.StudentProfile.StudentRegNumber,
                    AverageScore = x.AverageScore,
                    SubjectCount = x.EnrolledSubjectArchive.Count(),
                    StudentName = x.StudentProfile.user.Surname + " " + x.StudentProfile.user.FirstName + " " + x.StudentProfile.user.OtherName,
                    CummulativeAverageScore = x.CummulativeAverageScore,
                    Session = x.Session.SessionYear + "-" + x.Session.Term

                });
                return await output.ToListAsync();
            }

            return null;
        }

        public async Task<decimal?> SumEnrolledSubjectTotalScore(int enrollmentId)
        {
            //var total = await db.EnrolledSubjects.Include(x => x.Subject).Where(x => x.EnrollmentId == enrollmentId && x.TotalScore != 0).SumAsync(x => x.TotalScore);
            var total = await db.EnrolledSubjectArchive.Where(x => x.EnrollmentId == enrollmentId && x.IsOffered == true).SumAsync(x => x.TotalScore);
            return total;
        }

        public async Task<int> TotalEnrolledSubjectCount(int enrollmentId)
        {
            int Count = await db.EnrolledSubjectArchive.Where(x => x.EnrollmentId == enrollmentId && x.IsOffered == true).CountAsync();
            return Count;
        }

        public async Task UnpublishResult(int id, int classId)
        {
            var publishId = await db.PublishResults.FirstOrDefaultAsync(x => x.SessionId == id && x.ClassLevelId == classId);
            PublishResult publishResult = await db.PublishResults.FindAsync(publishId.Id);

            db.PublishResults.Remove(publishResult);
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if(userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "Unpublished student results";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task UpdateResult(int id)
        {
            try
            {

                var total = await db.EnrolledSubjectArchive.Include(x => x.Enrollments).Where(x => x.EnrollmentId == id && x.TotalScore != 0).SumAsync(x => x.TotalScore);
                //check for 
                var setting = await db.Settings.FirstOrDefaultAsync();
                var classSubject = await db.Enrollments.Include(x => x.ClassLevel).Include(x => x.EnrolledSubjectArchive).FirstOrDefaultAsync(x => x.Id == id);
                int subjectCount = await db.EnrolledSubjectArchive.Include(x => x.Enrollments).Where(x => x.EnrollmentId == id && x.IsOffered == true).CountAsync();


                decimal? sum = total / subjectCount;
                var student = await db.Enrollments.FirstOrDefaultAsync(e => e.Id == id);

                student.AverageScore = sum.Value;


                var currentEnrollment = await db.Enrollments.FirstOrDefaultAsync(x => x.Id == id);
                var currentYear = await db.Sessions.FirstOrDefaultAsync(f => f.Id == currentEnrollment.SessionId);
                string year = currentYear.SessionYear;

                if (currentYear.Term == "Third")
                {


                    //Get Session IDs for each term
                    var firstTerm = await db.Sessions.FirstOrDefaultAsync(x => x.SessionYear == year && x.Term == "First");
                    var secondTerm = await db.Sessions.FirstOrDefaultAsync(x => x.SessionYear == year && x.Term == "Second");
                    var thirdTerm = await db.Sessions.FirstOrDefaultAsync(x => x.SessionYear == year && x.Term == "Third");

                    var currentStudent = currentEnrollment.StudentProfileId;

                    //Get enrollment for each term
                    var firstEnrollment = await db.Enrollments.FirstOrDefaultAsync(f => f.StudentProfileId == currentStudent && f.SessionId == firstTerm.Id);
                    var secondEnrollment = await db.Enrollments.FirstOrDefaultAsync(f => f.StudentProfileId == currentStudent && f.SessionId == secondTerm.Id);
                    var thirdEnrollment = await db.Enrollments.FirstOrDefaultAsync(f => f.StudentProfileId == currentStudent && f.SessionId == thirdTerm.Id);

                    ////Get Total Scores for each term\\\\\\\\\\\\\\\\\\\\\


                    //Total for First Term
                    if ((firstEnrollment == null || firstEnrollment.AverageScore == 0 || firstEnrollment.AverageScore == null) && (secondEnrollment == null || secondEnrollment.AverageScore == 0 || secondEnrollment.AverageScore == null))
                    {
                        // var averageForFirstTerm = firstEnrollment.AverageScore;
                        //var averageForSecondTerm = secondEnrollment.AverageScore;

                        var averageForThirdTerm = sum.Value;


                        decimal? totalScore = averageForThirdTerm;

                        decimal? cummulativeAverage = totalScore;
                        student.CummulativeAverageScore = cummulativeAverage;
                    }
                    //Total for second term
                    else if ((firstEnrollment == null || firstEnrollment.AverageScore == 0 || firstEnrollment.AverageScore == null) && secondEnrollment != null)
                    {
                        var averageForThirdTerm = sum.Value;
                        var averageForSecondTerm = secondEnrollment.AverageScore;

                        decimal? totalScore = averageForThirdTerm + averageForSecondTerm;
                        decimal? cummulativeAverage = totalScore / 2;
                        student.CummulativeAverageScore = cummulativeAverage;
                    }

                    else if (firstEnrollment != null && (secondEnrollment == null || secondEnrollment.AverageScore == 0 || secondEnrollment.AverageScore == null))
                    {
                        var averageForThirdTerm = sum.Value;
                        var averageForFirstTerm = firstEnrollment.AverageScore;

                        decimal? totalScore = averageForThirdTerm + averageForFirstTerm;

                        decimal? cummulativeAverage = totalScore / 2;
                        student.CummulativeAverageScore = cummulativeAverage;
                    }
                    else
                    {
                        decimal? averageForFirstTerm = 0;
                        decimal? averageForSecondTerm = 0;
                        int terms = 1;
                        var averageForThirdTerm = sum.Value;
                        if (firstEnrollment.AverageScore != null || firstEnrollment.AverageScore != 0)
                        {
                            terms = terms + 1;
                            averageForFirstTerm = firstEnrollment.AverageScore;
                        }


                        if (secondEnrollment.AverageScore != null || secondEnrollment.AverageScore != 0)
                        {
                            terms = terms + 1;
                            averageForSecondTerm = secondEnrollment.AverageScore;
                        }


                        //////Cummulative Average
                        decimal? totalScore = averageForFirstTerm + averageForSecondTerm + averageForThirdTerm;

                        decimal? cummulativeAverage = totalScore / terms;
                        student.CummulativeAverageScore = cummulativeAverage;
                    }

                }


                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();

                //Add Tracking
                var userId = HttpContext.Current.User.Identity.GetUserId();
                if(userId != null)
                {
                    var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                    Tracker tracker = new Tracker();
                    tracker.UserId = userId;
                    tracker.UserName = user.UserName;
                    tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                    tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                    tracker.Note = tracker.FullName + " " + "Updated class results";
                    //db.Trackers.Add(tracker);
                    await db.SaveChangesAsync();
                }
               
            }
            catch (Exception e)
            {

            }
            // return RedirectToAction("EditResult", new { id = id });
        }

        public async Task<StudentProfile> GetUserByRegNumber(string regnumber)
        {
            var student = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.StudentRegNumber.ToLower().Contains(regnumber.ToLower()));

            //var student = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.StudentRegNumber == regnumber);
            return student;
        }

        public async Task<Session> GetSessionBySessionId(int sessId)
        {
            var sess = await db.Sessions.FirstOrDefaultAsync(x => x.Id == sessId);
            return sess;
        }

        public async Task<Enrollment> GetEnrollmentBySessIdStudentProfileId(int sessId, int ProfileId)
        {
            var item = await db.Enrollments.Include(x => x.ClassLevel).FirstOrDefaultAsync(s => s.SessionId == sessId && s.StudentProfileId == ProfileId);
            return item;
        }

        public async Task<PinCodeModel> PinCodeByPinNumberAndSerialNumber(string pin, string serialnumber)
        {
            var pinInfo = await db.PinCodeModels.FirstOrDefaultAsync(w => w.PinNumber.Contains(pin) && w.SerialNumber.Contains(serialnumber));
            return pinInfo;
        }

        public async Task<PublishResult> PublishResultBySessIdAndClassId(int sessId, int? classId)
        {
            var item = await db.PublishResults.FirstOrDefaultAsync(d => d.SessionId == sessId && d.ClassLevelId == classId);
            return item;
        }

        public async Task<Defaulter> GetDefaulterByProfileId(int profileId)
        {
            var def = await db.Defaulters.Include(d => d.StudentProfile).FirstOrDefaultAsync(x => x.ProfileId == profileId);
            return def;
        }

        public async Task Update()
        {
            await db.SaveChangesAsync();

            //Add Tracking
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if(userId != null)
            {
                var user = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                Tracker tracker = new Tracker();
                tracker.UserId = userId;
                tracker.UserName = user.UserName;
                tracker.FullName = user.Surname + " " + user.FirstName + " " + user.OtherName;
                tracker.ActionDate = DateTime.UtcNow.AddHours(1);
                tracker.Note = tracker.FullName + " " + "updated student results";
                //db.Trackers.Add(tracker);
                await db.SaveChangesAsync();
            }
           
        }

        public async Task<int> SubjectCount(int? enrolId)
        {
            var item = await db.EnrolledSubjectArchive.Include(x => x.Enrollments).Where(x => x.EnrollmentId == enrolId && x.IsOffered == true).CountAsync();
            return item;
        }

        public async Task<StudentProfile> GetUserByUserId(string userId)
        {
            var user = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.UserId == userId);
            return user;
        }
    }
}