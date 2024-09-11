using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;
using System.Data;
using SchoolPortal.Web.Models.Entities;
using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Models.ResultArchive;

namespace SchoolPortal.Web.Areas.Service
{
    public class PrintArchiveService
    {
        private IEnrollmentService _enrollmentService = new EnrollmentService();
        public PrintArchiveService()
        {

        }
        public PrintArchiveService(
            EnrollmentService enrollmentService
            )
        {

            _enrollmentService = enrollmentService;
        }
        #region print result
        public static PrintResultArchiveDto PrintResult(Int32? id, string batchid)
        {
            using (var db = new ApplicationDbContext())
            {

                string grade = "";
                string remark = "";
                int userposition = 0;
                decimal? useraveragescore = 0;

                decimal? highestaveragescore = 0;
                decimal? lowestaveragescore = 0;
                int totalStudents = 0;
                string currentsessiomterm;
                string currentsessiomterm1;
                string currentsessiomterm2;
                var getuserenrollment = db.Enrollments.Include(p => p.ClassLevel).Include(x => x.EnrolledSubjectArchive).FirstOrDefault(x => x.Id == id);
                var getuserprofile = db.StudentProfiles.Include(c => c.user).FirstOrDefault(x => x.Id == getuserenrollment.StudentProfileId);
                var getenrollmentsession = db.Sessions.FirstOrDefault(x => x.Id == getuserenrollment.SessionId);
                currentsessiomterm = getenrollmentsession.Term.ToUpper() + " TERM " + getenrollmentsession.SessionYear + " " + "ACADEMIC SESSION";
                currentsessiomterm1 = getenrollmentsession.Term.ToUpper() + " TERM ";
                currentsessiomterm2 = getenrollmentsession.SessionYear + " " + "ACADEMIC SESSION";
                var subjectlis = db.EnrolledSubjectArchive.Include(z => z.Enrollments).Where(d => d.EnrollmentId == id && d.IsOffered == true);
                var subjectlist = db.EnrolledSubjectArchive.Include(z => z.Enrollments).Where(d => d.EnrollmentId == id && d.IsOffered == true);
                var highscore = subjectlist.OrderByDescending(x => x.TotalScore).FirstOrDefault();
                var lowerscore = subjectlist.OrderByDescending(x => x.TotalScore).FirstOrDefault();

                var outputSubjectList = subjectlist.Select(c => new SubjectListPrintoutArchiveDto()
                {
                    Id = c.Id,
                    Subject = c.SubjectName,
                    SubjectId = c.Id,
                    TestScore = c.TestScore,
                    ExamScore = c.ExamScore,
                    TotalScore = c.TotalScore,
                    HighestTotalScore = highscore.TotalScore,
                    LowestTotalScore = lowerscore.TotalScore,
                    EnrollmentId = c.EnrollmentId,
                    Enrollments = c.Enrollments,
                    IsOffered = c.IsOffered,
                    GradingOption = c.GradingOption,
                    TestScore2 = c.TestScore2,
                    Assessment = c.Assessment,
                    ClassExercise = c.ClassExercise,
                    Project = c.Project,
                    TotalCA = c.TotalCA


                });
                var outputSubjectListout = outputSubjectList;

                var total = db.EnrolledSubjectArchive.Include(x => x.Enrollments).Where(f => f.EnrollmentId == id).Sum(x => x.TotalScore);
                var setting = db.Settings.FirstOrDefault();
                int subjectCount = db.EnrolledSubjectArchive.Where(x => x.EnrollmentId == id && x.IsOffered == true).Count();
                int getuserenrollmentcount = db.Enrollments.Include(p => p.ClassLevel).Include(x => x.EnrolledSubjectArchive).Where(x => x.Id == id).Count();

                #region

                decimal? totalsc = 0;
                decimal? overall = 0;
                decimal? cutoff = getuserenrollment.ClassLevel.Passmark;

                if (subjectCount != 0)
                {
                    decimal? sum = total / subjectCount;
                    decimal averageScore = sum.Value;
                    totalsc = total;
                    overall = subjectCount * 100;
                    //if (sum <= cutoff)
                    //{
                    //    grade = "F";
                    //    remark = "Fail";
                    //}
                    //else if (sum >= cutoff && sum <= 100)
                    //{
                    //    grade = "P";
                    //    remark = "Pass";
                    //}

                    if (cutoff > sum)
                    {
                        grade = "F";
                        remark = "FAIL";
                    }


                    else if (cutoff == sum)
                    {
                        grade = "P";
                        remark = "PASS";
                    }


                    else if (sum <= 49.00m && sum >= 40.00m)
                    {
                        grade = "P";
                        remark = "PASS";
                    }

                    else if (sum <= 49.99m && sum >= 40.99m)
                    {
                        grade = "P";
                        remark = "PASS";
                    }

                    else if (sum <= 59.00m && sum >= 50.00m)
                    {
                        grade = "P";
                        remark = "PASS";
                    }
                    else if (sum <= 59.99m && sum >= 55.99m)
                    {
                        grade = "P";
                        remark = "PASS";
                    }
                    else if (sum <= 64.00m && sum >= 60.00m)
                    {
                        grade = "C";
                        remark = "GOOD";
                    }
                    else if (sum <= 64.99m && sum >= 60.99m)
                    {
                        grade = "C";
                        remark = "GOOD";
                    }
                    else if (sum <= 69.00m && sum >= 65.00m)
                    {
                        grade = "C";
                        remark = "GOOD";
                    }
                    else if (sum <= 69.99m && sum >= 65.99m)
                    {
                        grade = "C";
                        remark = "GOOD";
                    }
                    else if (sum <= 73.00m && sum >= 70.00m)
                    {
                        grade = "B";
                        remark = "VERY GOOD";
                    }
                    else if (sum <= 73.99m && sum >= 70.99m)
                    {
                        grade = "B";
                        remark = "VERY GOOD";
                    }
                    else if (sum <= 100 && sum >= 74.00m)
                    {
                        grade = "A";
                        remark = "EXCELLENT";

                    }
                    else if (sum <= 100 && sum >= 74.99m)
                    {
                        grade = "A";
                        remark = "EXCELLENT";

                    }


                    string mainAverage = averageScore.ToString("0.00");
                    var positionsh = db.Enrollments.Include(x => x.StudentProfile).Include(x => x.EnrolledSubjectArchive).Where(x => x.SessionId == getuserenrollment.SessionId && x.ClassLevelId == getuserenrollment.ClassLevelId).OrderByDescending(x => x.AverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber);
                    var positions = db.Enrollments.Include(x => x.StudentProfile).Include(x => x.EnrolledSubjectArchive).Where(x => x.SessionId == getuserenrollment.SessionId && x.ClassLevelId == getuserenrollment.ClassLevelId && x.AverageScore > 0).OrderByDescending(x => x.AverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber);
                    try
                    {
                        if (positions.OrderByDescending(x => x.AverageScore).FirstOrDefault() != null)
                        {
                            highestaveragescore = positions.OrderByDescending(x => x.AverageScore).FirstOrDefault().AverageScore;
                        }
                        else
                        {
                            highestaveragescore = 0;
                        }

                        if (positions.OrderBy(x => x.AverageScore).FirstOrDefault() != null)
                        {
                            lowestaveragescore = positions.OrderBy(x => x.AverageScore).FirstOrDefault().AverageScore;
                        }
                        else
                        {
                            lowestaveragescore = 0;
                        }
                    }
                    catch (Exception c)
                    {
                        highestaveragescore = 0;
                        lowestaveragescore = 0;
                    }
                    totalStudents = positions.Count();
                    int position = 0;
                    int mposition = 0;
                    decimal? avg = 0;
                    var GETaa = db.BatchResults.Where(x => x.BatchId == batchid).OrderByDescending(x => x.AverageScore);
                    var useposl = GETaa.Where(x => x.AverageScore == getuserenrollment.AverageScore);
                    var usepositionforall = useposl.Select(x => x.AverageScore);
                    var usepositionfor = db.BatchResults.Where(x => x.BatchId == batchid && x.AverageScore == getuserenrollment.AverageScore).OrderBy(x => x.AverageScore).ToArray();

                    if (subjectCount != 0)
                    {

                        foreach (var p in positions)
                        {


                            position = position + 1;

                            if (p.Id == id)
                            {
                                avg = p.AverageScore.Value;
                                goto outloop;
                            }
                        }

                        outloop: userposition = position;
                        useraveragescore = Math.Round(averageScore, 2);

                    }
                    if (usepositionforall.Count() > 1)
                    {
                        if (usepositionforall.Contains(getuserenrollment.AverageScore))
                        {
                            var assignposion = usepositionfor.ToArray();
                            var assignposition = usepositionfor.First();
                            var pos = Position12(assignposition.EnrollmentId);
                            userposition = pos.Position;
                        }
                    }

                }
                byte[] abuser;
                byte[] absession;
                byte[] stpp;
                var img = db.ImageModel.FirstOrDefault(x => x.Id == getuserprofile.ImageId);
                var Sch = db.Settings.FirstOrDefault();
                var Schlogo = db.ImageModel.FirstOrDefault(x => x.Id == Sch.ImageId);
                var schstamp = db.ImageModel.FirstOrDefault(x => x.FileName == "SCHOOLSTAM");

                if (Schlogo == null)
                {
                    absession = new byte[0];
                }
                else
                {
                    absession = Schlogo.ImageContent;
                }
                if (img == null)
                {
                    abuser = new byte[0];
                }
                else
                {
                    abuser = img.ImageContent;
                }
                if (schstamp == null)
                {
                    stpp = new byte[0];
                }
                else
                {
                    stpp = schstamp.ImageContent;
                }
                string promote = "";
                //grading system in result =
                var avb = db.Gradings.Include(x => x.GradingDetails).FirstOrDefault(x => x.Name == getuserenrollment.ClassLevel.ClassName.Substring(0, 3));

                var grdsystem = db.GradingDetails.Where(x => x.GradingId == avb.Id);
                var SchoolAcct = db.SchoolAccounts.ToList();
                var recognitivedomain = new RecognitiveDomainArchive();
                var recognitivedomain1 = db.RecognitiveDomainArchive.FirstOrDefault(x => x.EnrolmentId == getuserenrollment.Id);
                if (recognitivedomain1 != null)
                {
                    recognitivedomain = recognitivedomain1;
                }
                else
                {
                    if (recognitivedomain == null)
                    {
                        recognitivedomain.Rememberance = "Good";
                        recognitivedomain.Understanding = "Good";
                        recognitivedomain.Application = "Good";
                        recognitivedomain.Analyzing = "Good";
                        recognitivedomain.Evaluation = "Good";
                        recognitivedomain.Creativity = "Good";
                    }
                }


                //Psychomtor Domain

                var psychomotordomain = new PsychomotorDomainArchive();
                var psychomotordomain1 = db.PsychomotorDomainArchive.FirstOrDefault(x => x.EnrolmentId == getuserenrollment.Id);
                if (psychomotordomain != null)
                {
                    psychomotordomain = psychomotordomain1;
                }
                else
                {
                    if (psychomotordomain == null)
                    {
                        psychomotordomain.Drawing = "Good";
                        psychomotordomain.Painting = "Good";
                        psychomotordomain.Handwriting = "Good";
                        psychomotordomain.Hobbies = "Good";
                        psychomotordomain.Sports = "Good";
                        psychomotordomain.Club = "Good";
                    }
                }

                //Affective Domain

                var affectivedomain = new AffectiveDomainArchive();
                var affectivedomain1 = db.AffectiveDomainArchive.FirstOrDefault(x => x.EnrolmentId == getuserenrollment.Id);
                if (affectivedomain != null)
                {
                    affectivedomain = affectivedomain1;
                }
                else
                {
                    if (affectivedomain == null)
                    {
                        affectivedomain.Attentiveness = "Good";
                        affectivedomain.Honesty = "Good";
                        affectivedomain.Neatness = "Good";
                        affectivedomain.Punctuality = "Good";
                        affectivedomain.Relationship = "Good";

                    }
                }

                // SchoolFees
                var schoolfee = db.SchoolFeesArchive.Where(x => x.SessionId == getenrollmentsession.Id).ToList();
                //var newsletter = new NewsLetter();
                var newsletter = db.NewsLetterArchive.Where(x => x.SessionId == getenrollmentsession.Id).OrderByDescending(x => x.SessionId).Take(1).ToList();
                var principal = db.PrincipalArchives.Where(x => x.SessionId == getenrollmentsession.Id).OrderByDescending(x => x.SessionId).FirstOrDefault();
                #endregion
                //check promoted or not
                var subjectMath = subjectlist.FirstOrDefault(x => x.SubjectName.Substring(0, 6).ToUpper() == "MATHEM");
                var subjectEng = subjectlist.FirstOrDefault(x => x.SubjectName.Substring(0, 7).ToUpper() == "ENGLISH");
                // user class
                var userclass = db.ClassLevels.FirstOrDefault(x => x.Id == getuserenrollment.ClassLevelId);
                decimal? mathvalue = 0;
                decimal? engvalue = 0;
                if (subjectMath != null || subjectEng != null)
                {
                    if (subjectEng != null)
                    {
                        if (subjectEng.TotalScore != null)
                        {
                            engvalue = subjectEng.TotalScore;
                        }

                    }
                    if (subjectMath != null)
                    {
                        if (subjectMath.TotalScore != null)
                        {
                            mathvalue = subjectMath.TotalScore;
                        }
                    }
                    if (engvalue > userclass.Passmark || mathvalue > userclass.Passmark)
                    {
                        promote = "PROMOTED";
                    }
                    else
                    {
                        promote = "NOT PROMOTED";
                    }
                }

                var output = new PrintResultArchiveDto
                {

                    Average = useraveragescore,
                    HighestTotalAverage = highestaveragescore,
                    LowestTotalAverage = lowestaveragescore,
                    Position = userposition,
                    ClassName = getuserenrollment.ClassLevel.ClassName,
                    NumberInClass = getuserenrollmentcount,
                    TotalStudent = totalStudents,
                    SessionTerm = currentsessiomterm,
                    SessionTerm1 = currentsessiomterm1,
                    SessionTerm2 = currentsessiomterm2,
                    SchoolName = setting.SchoolName,
                    ResultGrade = grade,
                    ResultRemark = remark,
                    RegNumber = getuserprofile.StudentRegNumber,
                    Fullname = getuserprofile.user.Surname + " " + getuserprofile.user.FirstName + " " + getuserprofile.user.OtherName,
                    SubjectList = outputSubjectListout.ToList(),
                    studentImage = abuser,
                    SchoolLogo = absession,
                    SchoolStamp = stpp,
                    GradingDetails = grdsystem.ToList(),
                    RecognitiveDomain = recognitivedomain,
                    PsychomotorDomain = psychomotordomain,
                    AffectiveDomain = affectivedomain,
                    Address = setting.SchoolAddress,
                    headteacher = principal.PrincipalName,
                    Email = setting.ContactEmail,
                    Phone = setting.ContactPhoneNumber,
                    Website = setting.WebsiteLink,
                    TotalScore = totalsc,
                    OverallScore = overall,
                    PromotionStatus = promote,
                    EnrollmentRemark = getuserenrollment.EnrollmentRemark1,
                    EnrollmentRemark2 = getuserenrollment.EnrollmentRemark2,
                    NewsLetter = newsletter,

                    SchoolFees = schoolfee,
                    SchoolAccount = SchoolAcct,
                    showPosOnClassResult = getuserenrollment.ClassLevel.ShowPositionOnClassResult,
                    showSchAccountOnResult = setting.ShowAccctOnResult,
                    showSchFeeOnResult = setting.ShowFeesOnResult

                };

                var resu = output;
                return output;
            }
        }
        #endregion
        /// <summary>
        /// print sig
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 

        public static PrintResultArchiveDto PrintSingleResult(Int32? id)
        {
            using (var db = new ApplicationDbContext())
            {


                string grade = "";
                string remark = "";
                int userposition = 0;
                decimal? useraveragescore = 0;

                decimal? highestaveragescore = 0;
                decimal? lowestaveragescore = 0;
                int totalStudents = 0;
                string currentsessiomterm;
                string currentsessiomterm1;
                string currentsessiomterm2;

                var setting = db.Settings.FirstOrDefault();
                var getuserenrollment = db.Enrollments.Include(p => p.ClassLevel).Include(x => x.EnrolledSubjectArchive).FirstOrDefault(x => x.Id == id);
                var getuserprofile = db.StudentProfiles.Include(c => c.user).FirstOrDefault(x => x.Id == getuserenrollment.StudentProfileId);
                var getenrollmentsession = db.Sessions.FirstOrDefault(x => x.Id == getuserenrollment.SessionId);
                currentsessiomterm = getenrollmentsession.Term.ToUpper() + " TERM " + getenrollmentsession.SessionYear + " " + "ACADEMIC SESSION";
                currentsessiomterm1 = getenrollmentsession.Term.ToUpper() + " TERM ";
                currentsessiomterm2 = getenrollmentsession.SessionYear + " " + "ACADEMIC SESSION";
                var subjectlis = db.EnrolledSubjectArchive.Include(z => z.Enrollments).Where(d => d.EnrollmentId == id && d.IsOffered == true);
                var subjectlist = db.EnrolledSubjectArchive.Include(z => z.Enrollments).Where(d => d.EnrollmentId == id && d.IsOffered == true);
                var subjectlistRangeScore = db.EnrolledSubjectArchive.Include(z => z.Enrollments).Where(d => d.Enrollments.ClassLevelId == getuserenrollment.ClassLevelId && d.IsOffered == true).ToList();
                var total = db.EnrolledSubjectArchive.Where(f => f.EnrollmentId == id).Sum(x => x.TotalScore);
                int subjectCount = db.EnrolledSubjectArchive.Where(x => x.EnrollmentId == id && x.IsOffered == true).Count();
                int getuserenrollmentcount = db.Enrollments.Include(p => p.ClassLevel).Include(x => x.EnrolledSubjectArchive).Where(x => x.Id == id).Count();
                var highscore = subjectlistRangeScore.OrderByDescending(x => x.TotalScore).FirstOrDefault();
                var lowerscore = subjectlistRangeScore.OrderByDescending(x => x.TotalScore).FirstOrDefault();

                //
                var outputSubjectList = subjectlist.Select(c => new SubjectListPrintoutArchiveDto()
                {
                    Id = c.Id,
                    Subject = c.SubjectName,
                    SubjectId = c.Id,
                    TestScore = c.TestScore,
                    ExamScore = c.ExamScore,
                    TotalScore = c.TotalScore,
                    EnrollmentId = c.EnrollmentId,
                    Enrollments = c.Enrollments,
                    IsOffered = c.IsOffered,
                    GradingOption = c.GradingOption,
                    TestScore2 = c.TestScore2,
                    Assessment = c.Assessment,
                    ClassExercise = c.ClassExercise,
                    Project = c.Project,
                    TotalCA = c.TotalCA


                });
                var outputSubjectListout = outputSubjectList;



                decimal? totalsc = 0;
                decimal? overall = 0;
                decimal? cutoff = getuserenrollment.ClassLevel.Passmark;

                if (subjectCount != 0)
                {
                    decimal? sum = total / subjectCount;
                    decimal averageScore = sum.Value;
                    totalsc = total;
                    overall = subjectCount * 100;
                    //if (sum <= cutoff)
                    //{
                    //    grade = "F";
                    //    remark = "Fail";
                    //}
                    //else if (sum >= cutoff && sum <= 100)
                    //{
                    //    grade = "P";
                    //    remark = "Pass";
                    //}

                    if (cutoff > sum)
                    {
                        grade = "F";
                        remark = "FAIL";
                    }


                    else if (cutoff == sum)
                    {
                        grade = "P";
                        remark = "PASS";
                    }


                    else if (sum <= 49.00m && sum >= 40.00m)
                    {
                        grade = "P";
                        remark = "PASS";
                    }

                    else if (sum <= 49.99m && sum >= 40.99m)
                    {
                        grade = "P";
                        remark = "PASS";
                    }

                    else if (sum <= 59.00m && sum >= 50.00m)
                    {
                        grade = "P";
                        remark = "PASS";
                    }
                    else if (sum <= 59.99m && sum >= 55.99m)
                    {
                        grade = "P";
                        remark = "PASS";
                    }
                    else if (sum <= 64.00m && sum >= 60.00m)
                    {
                        grade = "C";
                        remark = "GOOD";
                    }
                    else if (sum <= 64.99m && sum >= 60.99m)
                    {
                        grade = "C";
                        remark = "GOOD";
                    }
                    else if (sum <= 69.00m && sum >= 65.00m)
                    {
                        grade = "C";
                        remark = "GOOD";
                    }
                    else if (sum <= 69.99m && sum >= 65.99m)
                    {
                        grade = "C";
                        remark = "GOOD";
                    }
                    else if (sum <= 73.00m && sum >= 70.00m)
                    {
                        grade = "B";
                        remark = "VERY GOOD";
                    }
                    else if (sum <= 73.99m && sum >= 70.99m)
                    {
                        grade = "B";
                        remark = "VERY GOOD";
                    }
                    else if (sum <= 100 && sum >= 74.00m)
                    {
                        grade = "A";
                        remark = "EXCELLENT";

                    }
                    else if (sum <= 100 && sum >= 74.99m)
                    {
                        grade = "A";
                        remark = "EXCELLENT";

                    }



                    string mainAverage = averageScore.ToString("0.00");
                    var positionsh = db.Enrollments.Include(x => x.StudentProfile).Include(x => x.EnrolledSubjectArchive).Where(x => x.SessionId == getuserenrollment.SessionId && x.ClassLevelId == getuserenrollment.ClassLevelId).OrderByDescending(x => x.AverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber);
                    var positions = db.Enrollments.Include(x => x.StudentProfile).Include(x => x.EnrolledSubjectArchive).Where(x => x.SessionId == getuserenrollment.SessionId && x.ClassLevelId == getuserenrollment.ClassLevelId && x.AverageScore > 0).OrderByDescending(x => x.AverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber);
                    try
                    {
                        if (positions.OrderByDescending(x => x.AverageScore).FirstOrDefault() != null)
                        {
                            highestaveragescore = positions.OrderByDescending(x => x.AverageScore).FirstOrDefault().AverageScore;
                        }
                        else
                        {
                            highestaveragescore = 0;
                        }

                        if (positions.OrderBy(x => x.AverageScore).FirstOrDefault() != null)
                        {
                            lowestaveragescore = positions.OrderBy(x => x.AverageScore).FirstOrDefault().AverageScore;
                        }
                        else
                        {
                            lowestaveragescore = 0;
                        }
                    }
                    catch (Exception c)
                    {
                        highestaveragescore = 0;
                        lowestaveragescore = 0;
                    }
                    totalStudents = positions.Count();
                    int position = 0;
                    int mposition = 0;
                    decimal? avg = 0;
                    //  var GETaa = db.BatchResults.Where(x => x.BatchId == batchid).OrderByDescending(x => x.AverageScore);
                    //   var useposl = GETaa.Where(x => x.AverageScore == getuserenrollment.AverageScore);
                    //var GETaa = db.Enrollments.Include(x => x.StudentProfile).Include(x => x.EnrolledSubjectArchive).Where(x => x.SessionId == getuserenrollment.SessionId && x.ClassLevelId == getuserenrollment.ClassLevelId && x.AverageScore > 0).OrderByDescending(x => x.AverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber);

                    //var usepositionforall = GETaa.Select(x => x.AverageScore);
                    //var usepositionfor = db.Enrollments.Include(x => x.StudentProfile).Include(x => x.EnrolledSubjectArchive).Where(x => x.SessionId == getuserenrollment.SessionId && x.ClassLevelId == getuserenrollment.ClassLevelId && x.AverageScore > 0).OrderByDescending(x => x.AverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber);



                    var GETaa = db.Enrollments.Include(x => x.StudentProfile).Include(x => x.EnrolledSubjectArchive).OrderBy(x => x.StudentProfile.user.Surname).Include(x => x.StudentProfile).Where(s => s.ClassLevelId == getuserenrollment.ClassLevelId && s.SessionId == getuserenrollment.SessionId && s.EnrolledSubjectArchive.Count() > 0);

                    var useposl = GETaa.Where(x => x.AverageScore == getuserenrollment.AverageScore);
                    var usepositionforall = useposl.Select(x => x.AverageScore);
                    var usepositionfor = db.Enrollments.OrderBy(x => x.StudentProfile.user.Surname).Include(x => x.StudentProfile).Where(s => s.ClassLevelId == getuserenrollment.ClassLevelId && s.SessionId == getuserenrollment.SessionId && s.EnrolledSubjectArchive.Count() > 0 && s.AverageScore == getuserenrollment.AverageScore).OrderByDescending(x => x.AverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber).ToArray();
                    var usepobbb = db.Enrollments.OrderBy(x => x.StudentProfile.user.Surname).Include(x => x.StudentProfile).Where(s => s.ClassLevelId == getuserenrollment.ClassLevelId && s.SessionId == getuserenrollment.SessionId && s.EnrolledSubjectArchive.Count() > 0 && s.AverageScore == getuserenrollment.AverageScore).OrderByDescending(x => x.AverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber).Select(x => x.StudentProfile.StudentRegNumber).ToArray();

                    if (subjectCount != 0)
                    {

                        foreach (var p in positions)
                        {


                            position = position + 1;

                            if (p.Id == id)
                            {
                                avg = p.AverageScore.Value;
                                goto outloop;
                            }
                        }

                        outloop: userposition = position;
                        useraveragescore = Math.Round(averageScore, 2);

                    }
                    if (usepositionforall.Count() > 1)
                    {
                        if (usepositionforall.Contains(getuserenrollment.AverageScore))
                        {
                            var assignposion = usepositionfor.ToArray();
                            var assignposition = usepositionfor.First();
                            var pos = Position12(assignposition.Id);
                            userposition = pos.Position;
                        }
                    }

                }
                byte[] abuser;
                byte[] absession;
                byte[] stpp;
                var img = db.ImageModel.FirstOrDefault(x => x.Id == getuserprofile.ImageId);
                var Sch = db.Settings.FirstOrDefault();
                var Schlogo = db.ImageModel.FirstOrDefault(x => x.Id == Sch.ImageId);
                var schstamp = db.ImageModel.FirstOrDefault(x => x.FileName == "SCHOOLSTAM");

                if (Schlogo == null)
                {
                    absession = new byte[0];
                }
                else
                {
                    absession = Schlogo.ImageContent;
                }
                if (img == null)
                {
                    abuser = new byte[0];
                }
                else
                {
                    abuser = img.ImageContent;
                }
                if (schstamp == null)
                {
                    stpp = new byte[0];
                }
                else
                {
                    stpp = schstamp.ImageContent;
                }


                string promote = "";
                //grading system in result =
                var avb = db.Gradings.Include(x => x.GradingDetails).FirstOrDefault(x => x.Name == getuserenrollment.ClassLevel.ClassName.Substring(0, 3));
                var grdsystem = db.GradingDetails.Where(x => x.GradingId == avb.Id);
                var SchoolAcct = db.SchoolAccounts.ToList();


                var recognitivedomain = new RecognitiveDomainArchive();
                var recognitivedomain1 = db.RecognitiveDomainArchive.FirstOrDefault(x => x.EnrolmentId == getuserenrollment.Id);
                if (recognitivedomain1 != null)
                {
                    recognitivedomain = recognitivedomain1;
                }
                else
                {
                    if (recognitivedomain == null)
                    {
                        recognitivedomain.Rememberance = "Good";
                        recognitivedomain.Understanding = "Good";
                        recognitivedomain.Application = "Good";
                        recognitivedomain.Analyzing = "Good";
                        recognitivedomain.Evaluation = "Good";
                        recognitivedomain.Creativity = "Good";
                    }
                }



                //Psychomtor Domain

                var psychomotordomain = new PsychomotorDomainArchive();
                var psychomotordomain1 = db.PsychomotorDomainArchive.FirstOrDefault(x => x.EnrolmentId == getuserenrollment.Id);
                if (psychomotordomain != null)
                {
                    psychomotordomain = psychomotordomain1;
                }
                else
                {
                    if (psychomotordomain == null)
                    {
                        psychomotordomain.Drawing = "Good";
                        psychomotordomain.Painting = "Good";
                        psychomotordomain.Handwriting = "Good";
                        psychomotordomain.Hobbies = "Good";
                        psychomotordomain.Sports = "Good";
                        psychomotordomain.Club = "Good";
                    }
                }

                //Affective Domain

                var affectivedomain = new AffectiveDomainArchive();
                var affectivedomain1 = db.AffectiveDomainArchive.FirstOrDefault(x => x.EnrolmentId == getuserenrollment.Id);
                if (affectivedomain != null)
                {
                    affectivedomain = affectivedomain1;
                }
                else
                {
                    if (affectivedomain == null)
                    {
                        affectivedomain.Attentiveness = "Good";
                        affectivedomain.Honesty = "Good";
                        affectivedomain.Neatness = "Good";
                        affectivedomain.Punctuality = "Good";
                        affectivedomain.Relationship = "Good";

                    }
                }

                // recognitived = recognitivedomain1;

                // SchoolFees
                var schoolfee = db.SchoolFeesArchive.Where(x => x.SessionId == getenrollmentsession.Id).ToList();

                var newsletter = db.NewsLetterArchive.Where(x => x.SessionId == getenrollmentsession.Id).OrderByDescending(x => x.SessionId).Take(1).ToList();
                //check promoted or not
                var subjectMath = subjectlist.FirstOrDefault(x => x.SubjectName.Substring(0, 6).ToUpper() == "MATHEM");
                var subjectEng = subjectlist.FirstOrDefault(x => x.SubjectName.Substring(0, 7).ToUpper() == "ENGLISH");
                // user class
                var userclass = db.ClassLevels.FirstOrDefault(x => x.Id == getuserenrollment.ClassLevelId);

                var principal = db.PrincipalArchives.Where(x => x.SessionId == getenrollmentsession.Id).OrderByDescending(x => x.SessionId).FirstOrDefault();
                if (setting.PromotionByMathsAndEng == true)
                {
                    if (subjectMath != null && subjectEng != null)
                    {
                        if (subjectEng.TotalScore >= userclass.Passmark && subjectMath.TotalScore >= userclass.Passmark)
                        {

                            promote = " (PROMOTED)";
                        }
                        else if (subjectEng.TotalScore >= userclass.Passmark || subjectMath.TotalScore >= userclass.Passmark && useraveragescore >= userclass.Passmark)
                        {
                            promote = " (PROMOTED)";
                        }
                        else
                        {
                            promote = " (NOT PROMOTED)";
                        }
                    }
                }


                var output = new PrintResultArchiveDto
                {

                    Average = useraveragescore,
                    HighestTotalAverage = highestaveragescore,
                    LowestTotalAverage = lowestaveragescore,
                    Position = userposition,
                    ClassName = getuserenrollment.ClassLevel.ClassName,
                    NumberInClass = getuserenrollmentcount,
                    TotalStudent = totalStudents,
                    SessionTerm = currentsessiomterm,
                    SessionTerm1 = currentsessiomterm1,
                    SessionId = getuserenrollment.SessionId,
                    SessionTerm2 = currentsessiomterm2,
                    SchoolName = setting.SchoolName,
                    ResultGrade = grade,
                    ResultRemark = remark,
                    RegNumber = getuserprofile.StudentRegNumber,
                    Fullname = getuserprofile.user.Surname + " " + getuserprofile.user.FirstName + " " + getuserprofile.user.OtherName,
                    SubjectList = outputSubjectListout.ToList(),
                    studentImage = abuser,
                    SchoolLogo = absession,
                    SchoolStamp = stpp,
                    GradingDetails = grdsystem.ToList(),
                    RecognitiveDomain = recognitivedomain,
                    PsychomotorDomain = psychomotordomain,
                    AffectiveDomain = affectivedomain,
                    Address = setting.SchoolAddress,
                    headteacher = principal.PrincipalName,
                    Email = setting.ContactEmail,
                    Phone = setting.ContactPhoneNumber,
                    Website = setting.WebsiteLink,
                    TotalScore = totalsc,
                    OverallScore = overall,
                    PromotionStatus = promote,
                    EnrollmentRemark = getuserenrollment.EnrollmentRemark1,
                    EnrollmentRemark2 = getuserenrollment.EnrollmentRemark2,
                    NewsLetter = newsletter,

                    SchoolFees = schoolfee,
                    SchoolAccount = SchoolAcct,
                    showPosOnClassResult = getuserenrollment.ClassLevel.ShowPositionOnClassResult,
                    showSchAccountOnResult = setting.ShowAccctOnResult,
                    showSchFeeOnResult = setting.ShowFeesOnResult,
                    ShowNewsletterPage = setting.ShowNewsletterPage,
                    NewsletterContent = setting.NewsletterContent
                };

                var resu = output;
                return output;

            }
        }


        public static PrintResultArchiveDto Position12(Int32? id)
        {
            if (id == 5722)
            {
                var j = "yes";
            }
            using (var db = new ApplicationDbContext())
            {
                string grade = "";
                string remark = "";
                int userposition = 0;
                decimal? useraveragescore = 0;
                decimal? highestaveragescore = 0;
                decimal? lowestaveragescore = 0;
                int totalStudents = 0;
                string currentsessiomterm;
                string currentsessiomterm1;
                string currentsessiomterm2;
                var getuserenrollment = db.Enrollments.Include(x => x.EnrolledSubjectArchive).Include(p => p.ClassLevel).FirstOrDefault(x => x.Id == id);
                var getuserprofile = db.StudentProfiles.Include(c => c.user).FirstOrDefault(x => x.Id == getuserenrollment.StudentProfileId);
                var getenrollmentsession = db.Sessions.FirstOrDefault(x => x.Id == getuserenrollment.SessionId);
                currentsessiomterm = getenrollmentsession.Term.ToUpper() + " TERM " + getenrollmentsession.SessionYear + " " + "ACADEMIC SESSION";
                currentsessiomterm1 = getenrollmentsession.Term.ToUpper() + " TERM ";
                currentsessiomterm2 = getenrollmentsession.SessionYear + " " + "ACADEMIC SESSION";
                var subjectlis = db.EnrolledSubjectArchive.Include(z => z.Enrollments).Where(d => d.EnrollmentId == id && d.IsOffered == true);
                var subjectlist = db.EnrolledSubjectArchive.Include(z => z.Enrollments).Where(d => d.EnrollmentId == id && d.IsOffered == true);
                var total = db.EnrolledSubjectArchive.Where(f => f.EnrollmentId == id).Sum(x => x.TotalScore);
                var setting = db.Settings.FirstOrDefault();
                int subjectCount = db.EnrolledSubjectArchive.Where(x => x.EnrollmentId == id && x.IsOffered == true).Count();

                var highscore = subjectlist.OrderByDescending(x => x.TotalScore).FirstOrDefault();
                var lowerscore = subjectlist.OrderBy(x => x.TotalScore).FirstOrDefault();
                //
                var outputSubjectList = subjectlist.Select(c => new SubjectListPrintoutArchiveDto()
                {
                    Id = c.Id,
                    Subject = c.SubjectName,
                    SubjectId = c.Id,
                    TestScore = c.TestScore,
                    ExamScore = c.ExamScore,
                    TotalScore = c.TotalScore,
                    HighestTotalScore = highscore.TotalScore,
                    LowestTotalScore = lowerscore.TotalScore,
                    EnrollmentId = c.EnrollmentId,
                    Enrollments = c.Enrollments,
                    IsOffered = c.IsOffered,
                    GradingOption = c.GradingOption,
                    TestScore2 = c.TestScore2,
                    Assessment = c.Assessment,
                    ClassExercise = c.ClassExercise,
                    Project = c.Project,
                    TotalCA = c.TotalCA

                });
                var outputSubjectListout = outputSubjectList;

                decimal? totalsc = 0;
                decimal? overall = 0;
                decimal? cutoff = getuserenrollment.ClassLevel.Passmark; ;

                if (subjectCount != 0)
                {
                    decimal? sum = total / subjectCount;
                    decimal averageScore = sum.Value;
                    totalsc = total;
                    overall = subjectCount * 100;
                    //if (sum <= cutoff)
                    //{
                    //    grade = "F";
                    //    remark = "Fail";
                    //}
                    //else if (sum >= cutoff && sum <= 100)
                    //{
                    //    grade = "P";
                    //    remark = "Pass";
                    //}

                    if (cutoff > sum)
                    {
                        grade = "F";
                        remark = "FAIL";
                    }


                    else if (cutoff == sum)
                    {
                        grade = "P";
                        remark = "PASS";
                    }


                    else if (sum <= 49.00m && sum >= 40.00m)
                    {
                        grade = "P";
                        remark = "PASS";
                    }

                    else if (sum <= 49.99m && sum >= 40.99m)
                    {
                        grade = "P";
                        remark = "PASS";
                    }

                    else if (sum <= 59.00m && sum >= 50.00m)
                    {
                        grade = "P";
                        remark = "PASS";
                    }
                    else if (sum <= 59.99m && sum >= 55.99m)
                    {
                        grade = "P";
                        remark = "PASS";
                    }
                    else if (sum <= 64.00m && sum >= 60.00m)
                    {
                        grade = "C";
                        remark = "GOOD";
                    }
                    else if (sum <= 64.99m && sum >= 60.99m)
                    {
                        grade = "C";
                        remark = "GOOD";
                    }
                    else if (sum <= 69.00m && sum >= 65.00m)
                    {
                        grade = "C";
                        remark = "GOOD";
                    }
                    else if (sum <= 69.99m && sum >= 65.99m)
                    {
                        grade = "C";
                        remark = "GOOD";
                    }
                    else if (sum <= 73.00m && sum >= 70.00m)
                    {
                        grade = "B";
                        remark = "VERY GOOD";
                    }
                    else if (sum <= 73.99m && sum >= 70.99m)
                    {
                        grade = "B";
                        remark = "VERY GOOD";
                    }
                    else if (sum <= 100 && sum >= 74.00m)
                    {
                        grade = "A";
                        remark = "EXCELLENT";

                    }
                    else if (sum <= 100 && sum >= 74.99m)
                    {
                        grade = "A";
                        remark = "EXCELLENT";

                    }



                    string mainAverage = averageScore.ToString("0.00");
                    var positions = db.Enrollments.Include(x => x.StudentProfile).Include(x => x.EnrolledSubjectArchive).Where(x => x.SessionId == getuserenrollment.SessionId && x.ClassLevelId == getuserenrollment.ClassLevelId && x.AverageScore > 0).OrderByDescending(x => x.AverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber);
                    totalStudents = positions.Count();
                    int position = 0;
                    decimal? avg = 0;
                    try
                    {
                        if (positions.OrderByDescending(x => x.AverageScore).FirstOrDefault() != null)
                        {
                            highestaveragescore = positions.OrderByDescending(x => x.AverageScore).FirstOrDefault().AverageScore;
                        }
                        else
                        {
                            highestaveragescore = 0;
                        }

                        if (positions.OrderBy(x => x.AverageScore).FirstOrDefault() != null)
                        {
                            lowestaveragescore = positions.OrderBy(x => x.AverageScore).FirstOrDefault().AverageScore;
                        }
                        else
                        {
                            lowestaveragescore = 0;
                        }
                    }
                    catch (Exception c)
                    {
                        highestaveragescore = 0;
                        lowestaveragescore = 0;
                    }


                    if (subjectCount != 0)
                    {
                        foreach (var p in positions)
                        {
                            position = position + 1;
                            if (p.Id == id)
                            {
                                avg = p.AverageScore.Value;
                                goto outloop;
                            }
                        }

                        outloop: userposition = position;
                        useraveragescore = Math.Round(averageScore, 2);

                    }

                }
                byte[] abuser;
                byte[] absession;
                byte[] stpp;
                var img = db.ImageModel.FirstOrDefault(x => x.Id == getuserprofile.ImageId);
                var Sch = db.Settings.FirstOrDefault();
                var Schlogo = db.ImageModel.FirstOrDefault(x => x.Id == Sch.ImageId);
                var schstamp = db.ImageModel.FirstOrDefault(x => x.FileName == "SCHOOLSTAM");

                if (Schlogo == null)
                {
                    absession = new byte[0];
                }
                else
                {
                    absession = Schlogo.ImageContent;
                }
                if (img == null)
                {
                    abuser = new byte[0];
                }
                else
                {
                    abuser = img.ImageContent;
                }
                if (schstamp == null)
                {
                    stpp = new byte[0];
                }
                else
                {
                    stpp = schstamp.ImageContent;
                }
                //grading system in result =
                var avb = db.Gradings.Include(x => x.GradingDetails).FirstOrDefault(x => x.Name == getuserenrollment.ClassLevel.ClassName.Substring(0, 3));
                var grdsystem = db.GradingDetails.Where(x => x.GradingId == avb.Id);
                var SchoolAcct = db.SchoolAccounts.ToList();
                var recognitivedomain = db.RecognitiveDomainArchive.FirstOrDefault(x => x.EnrolmentId == getuserenrollment.Id);
                var psychomotordomain = db.PsychomotorDomainArchive.FirstOrDefault(x => x.EnrolmentId == getuserenrollment.Id);
                var affectivedomain = db.AffectiveDomainArchive.FirstOrDefault(x => x.EnrolmentId == getuserenrollment.Id);
                // SchoolFees
                var schoolfee = db.SchoolFeesArchive.Where(x => x.SessionId == getenrollmentsession.Id).ToList();

                var newsletter = db.NewsLetterArchive.Where(x => x.SessionId == getenrollmentsession.Id).OrderByDescending(x => x.SessionId).Take(1).ToList();

                var principal = db.PrincipalArchives.Where(x => x.SessionId == getenrollmentsession.Id).OrderByDescending(x => x.SessionId).FirstOrDefault();
                var output = new PrintResultArchiveDto
                {

                    Average = useraveragescore,
                    HighestTotalAverage = highestaveragescore,
                    LowestTotalAverage = lowestaveragescore,
                    Position = userposition,
                    ClassName = getuserenrollment.ClassLevel.ClassName,
                    TotalStudent = totalStudents,
                    SessionTerm = currentsessiomterm,
                    SessionTerm1 = currentsessiomterm1,
                    SessionTerm2 = currentsessiomterm2,
                    SchoolName = setting.SchoolName,
                    ResultGrade = grade,
                    ResultRemark = remark,
                    RegNumber = getuserprofile.StudentRegNumber,
                    Fullname = getuserprofile.user.Surname + " " + getuserprofile.user.FirstName + " " + getuserprofile.user.OtherName,
                    RecognitiveDomain = recognitivedomain,
                    PsychomotorDomain = psychomotordomain,
                    AffectiveDomain = affectivedomain,
                    Address = setting.SchoolAddress,
                    headteacher = principal.PrincipalName,
                    Email = setting.ContactEmail,
                    Phone = setting.ContactPhoneNumber,
                    Website = setting.WebsiteLink,
                    TotalScore = totalsc,
                    OverallScore = overall,
                    NewsLetter = newsletter,

                    SchoolFees = schoolfee,
                    SchoolAccount = SchoolAcct,
                    showPosOnClassResult = getuserenrollment.ClassLevel.ShowPositionOnClassResult,
                    showSchAccountOnResult = setting.ShowAccctOnResult,
                    showSchFeeOnResult = setting.ShowFeesOnResult

                };

                var resu = output;
                return output;
            }
        }




        public static int MasterListPosition(Int32? id)
        {
            using (var db = new ApplicationDbContext())
            {
                int userposition = 0;
                decimal? useraveragescore = 0;

                decimal? highestaveragescore = 0;
                decimal? lowestaveragescore = 0;
                int totalStudents = 0;
                var getuserenrollment = db.Enrollments.Include(p => p.ClassLevel).Include(x => x.EnrolledSubjectArchive).FirstOrDefault(x => x.Id == id);
                var getuserprofile = db.StudentProfiles.Include(c => c.user).FirstOrDefault(x => x.Id == getuserenrollment.StudentProfileId);
                var subjectlist = db.EnrolledSubjectArchive.Include(z => z.Enrollments).Where(d => d.EnrollmentId == id && d.IsOffered == true);
                var total = db.EnrolledSubjectArchive.Where(f => f.EnrollmentId == id).Sum(x => x.TotalScore);
                var setting = db.Settings.FirstOrDefault();
                int subjectCount = db.EnrolledSubjectArchive.Where(x => x.EnrollmentId == id && x.IsOffered == true).Count();




                decimal? totalsc = 0;
                decimal? overall = 0;
                decimal? cutoff = getuserenrollment.ClassLevel.Passmark;

                if (subjectlist.Count() != 0)
                {
                    decimal? sum = total / subjectCount;
                    decimal averageScore = sum.Value;
                    totalsc = total;
                    overall = subjectlist.Count() * 100;

                    var positions = db.Enrollments.Include(x => x.StudentProfile).Where(x => x.SessionId == getuserenrollment.SessionId && x.ClassLevelId == getuserenrollment.ClassLevelId).OrderByDescending(x => x.AverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber);

                    totalStudents = positions.Count();
                    int position = 0;
                    int mposition = 0;
                    decimal? avg = 0;
                    //var GETaa = db.BatchResults.OrderByDescending(x => x.AverageScore);
                    var GETaa = db.Enrollments.Include(x => x.StudentProfile).OrderBy(x => x.StudentProfile.user.Surname).Include(x => x.StudentProfile).Where(s => s.ClassLevelId == getuserenrollment.ClassLevelId && s.SessionId == getuserenrollment.SessionId && s.EnrolledSubjectArchive.Count() > 0);

                    var useposl = GETaa.Where(x => x.AverageScore == getuserenrollment.AverageScore);
                    var usepositionforall = useposl.Select(x => x.AverageScore);
                    var usepositionfor = db.Enrollments.OrderBy(x => x.StudentProfile.user.Surname).Include(x => x.StudentProfile).Where(s => s.ClassLevelId == getuserenrollment.ClassLevelId && s.SessionId == getuserenrollment.SessionId && s.EnrolledSubjectArchive.Count() > 0 && s.AverageScore == getuserenrollment.AverageScore).OrderByDescending(x => x.AverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber).ToArray();
                    var usepobbb = db.Enrollments.OrderBy(x => x.StudentProfile.user.Surname).Include(x => x.StudentProfile).Where(s => s.ClassLevelId == getuserenrollment.ClassLevelId && s.SessionId == getuserenrollment.SessionId && s.EnrolledSubjectArchive.Count() > 0 && s.AverageScore == getuserenrollment.AverageScore).OrderByDescending(x => x.AverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber).Select(x => x.StudentProfile.StudentRegNumber).ToArray();
                    if (subjectlist.Count() != 0)
                    {

                        foreach (var p in positions)
                        {


                            position = position + 1;

                            if (p.Id == id)
                            {
                                if (p.AverageScore != null)
                                {
                                    avg = p.AverageScore.Value;
                                    goto outloop;
                                }



                            }
                        }

                        outloop: userposition = position;
                        useraveragescore = Math.Round(averageScore, 2);

                    }
                    if (usepositionforall.Count() > 1)
                    {
                        if (usepositionforall.Contains(getuserenrollment.AverageScore))
                        {
                            var assignposion = usepositionfor.ToArray();
                            var assignposition = usepositionfor.First();
                            var pos = Position12(assignposition.Id);
                            userposition = pos.Position;
                        }
                    }

                }

                var output = new PrintResultArchiveDto
                {

                    Average = useraveragescore,
                    Position = userposition,
                    HighestTotalAverage = highestaveragescore,
                    LowestTotalAverage = lowestaveragescore,
                };

                var resu = output;
                return output.Position;
            }
        }



        ///cummulative position
        ///
        public static int CummulativeMasterListPosition(Int32? id)
        {
            using (var db = new ApplicationDbContext())
            {
                int userposition = 0;
                decimal? useraveragescore = 0;

                decimal? highestaveragescore = 0;
                decimal? lowestaveragescore = 0;
                int totalStudents = 0;
                var getuserenrollment = db.Enrollments.Include(p => p.ClassLevel).Include(x => x.EnrolledSubjectArchive).FirstOrDefault(x => x.Id == id);
                var getuserprofile = db.StudentProfiles.Include(c => c.user).FirstOrDefault(x => x.Id == getuserenrollment.StudentProfileId);
                var subjectlist = db.EnrolledSubjectArchive.Include(z => z.Enrollments).Where(d => d.EnrollmentId == id && d.IsOffered == true);
                var total = db.EnrolledSubjectArchive.Where(f => f.EnrollmentId == id).Sum(x => x.TotalScore);
                var setting = db.Settings.FirstOrDefault();
                int subjectCount = db.EnrolledSubjectArchive.Where(x => x.EnrollmentId == id && x.IsOffered == true).Count();




                decimal? totalsc = 0;
                decimal? overall = 0;
                decimal? cutoff = getuserenrollment.ClassLevel.Passmark;

                if (subjectlist.Count() != 0)
                {
                    decimal? sum = total / subjectCount;
                    decimal cumaverageScore = sum.Value;
                    totalsc = total;
                    overall = subjectlist.Count() * 100;

                    var positions = db.Enrollments.Include(x => x.StudentProfile).Where(x => x.SessionId == getuserenrollment.SessionId && x.ClassLevelId == getuserenrollment.ClassLevelId).OrderByDescending(x => x.CummulativeAverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber);

                    totalStudents = positions.Count();
                    int position = 0;
                    int mposition = 0;
                    decimal? avg = 0;
                    //var GETaa = db.BatchResults.OrderByDescending(x => x.AverageScore);
                    var GETaa = db.Enrollments.Include(x => x.StudentProfile).OrderBy(x => x.StudentProfile.user.Surname).Include(x => x.StudentProfile).Where(s => s.ClassLevelId == getuserenrollment.ClassLevelId && s.SessionId == getuserenrollment.SessionId && s.EnrolledSubjectArchive.Count() > 0);

                    var useposl = GETaa.Where(x => x.CummulativeAverageScore == getuserenrollment.CummulativeAverageScore);
                    var usepositionforall = useposl.Select(x => x.CummulativeAverageScore);
                    var usepositionfor = db.Enrollments.OrderBy(x => x.StudentProfile.user.Surname).Include(x => x.StudentProfile).Where(s => s.ClassLevelId == getuserenrollment.ClassLevelId && s.SessionId == getuserenrollment.SessionId && s.EnrolledSubjectArchive.Count() > 0 && s.CummulativeAverageScore == getuserenrollment.CummulativeAverageScore).OrderByDescending(x => x.CummulativeAverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber).ToArray();
                    var usepobbb = db.Enrollments.OrderBy(x => x.StudentProfile.user.Surname).Include(x => x.StudentProfile).Where(s => s.ClassLevelId == getuserenrollment.ClassLevelId && s.SessionId == getuserenrollment.SessionId && s.EnrolledSubjectArchive.Count() > 0 && s.CummulativeAverageScore == getuserenrollment.CummulativeAverageScore).OrderByDescending(x => x.CummulativeAverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber).Select(x => x.StudentProfile.StudentRegNumber).ToArray();
                    if (subjectlist.Count() != 0)
                    {

                        foreach (var p in positions)
                        {


                            position = position + 1;

                            if (p.Id == id)
                            {
                                if (p.CummulativeAverageScore != null)
                                {
                                    if(p.CummulativeAverageScore != null) { 
                                    avg = p.CummulativeAverageScore.Value;
                                    }
                                    goto outloop;
                                }



                            }
                        }

                        outloop: userposition = position;
                        useraveragescore = Math.Round(cumaverageScore, 2);

                    }
                    if (usepositionforall.Count() > 1)
                    {
                        if (usepositionforall.Contains(getuserenrollment.AverageScore))
                        {
                            var assignposion = usepositionfor.ToArray();
                            var assignposition = usepositionfor.First();
                            var pos = Position12(assignposition.Id);
                            userposition = pos.Position;
                        }
                    }

                }

                var output = new PrintResultArchiveDto
                {

                    Average = useraveragescore,
                    Position = userposition,
                    HighestTotalAverage = highestaveragescore,
                    LowestTotalAverage = lowestaveragescore,
                };

                var resu = output;
                return output.Position;
            }
        }





        /////
        ///cumu
        ///

        public static PrintThirdTermArchiveDto PrintThirdTermArchiveResult(Int32? id, int sessionId)
        {
            using (var db = new ApplicationDbContext())
            {
                var currentEnrollment = db.Enrollments.Include(x => x.ClassLevel).FirstOrDefault(x => x.Id == id);
                var currentYear = db.Sessions.FirstOrDefault(f => f.Id == currentEnrollment.SessionId);
                string year = currentYear.SessionYear;
                //Get Session IDs for each term
                var firstTerm = db.Sessions.FirstOrDefault(x => x.SessionYear == year && x.Term == "First");
                var secondTerm = db.Sessions.FirstOrDefault(x => x.SessionYear == year && x.Term == "Second");
                var thirdTerm = db.Sessions.FirstOrDefault(x => x.SessionYear == year && x.Term == "Third");


                string grade = "";
                string remark = "";
                int userposition = 0;
                decimal? useraveragescore = 0;
                int totalStudents = 0;
                string currentsessiomterm;
                string currentsessiomterm1;
                string currentsessiomterm2;
                var getuserenrollment = db.Enrollments.Include(x => x.Session).Include(p => p.ClassLevel).Include(x => x.EnrolledSubjectArchive).FirstOrDefault(x => x.Id == id);
                if (getuserenrollment.EnrollmentRemark == null)
                {
                    getuserenrollment.EnrollmentRemark = "";
                }
                if (getuserenrollment.EnrollmentRemark1 == null)
                {
                    getuserenrollment.EnrollmentRemark1 = "";
                }
                if (getuserenrollment.EnrollmentRemark2 == null)
                {
                    getuserenrollment.EnrollmentRemark2 = "";
                }
                var getuserprofile = db.StudentProfiles.Include(c => c.user).FirstOrDefault(x => x.Id == getuserenrollment.StudentProfileId);
                var getenrollmentsession = db.Sessions.FirstOrDefault(x => x.Id == getuserenrollment.SessionId);
                currentsessiomterm = getenrollmentsession.Term.ToUpper() + " TERM " + getenrollmentsession.SessionYear + " " + "ACADEMIC SESSION";
                currentsessiomterm1 = getenrollmentsession.Term.ToUpper() + " TERM ";
                currentsessiomterm2 = getenrollmentsession.SessionYear + " " + "ACADEMIC SESSION";
                var subjectlis = db.EnrolledSubjectArchive.Include(z => z.Enrollments).Where(d => d.EnrollmentId == id && d.IsOffered == true);
                var subjectlist = db.EnrolledSubjectArchive.Include(z => z.Enrollments).Where(d => d.EnrollmentId == id && d.IsOffered == true);
                var total = db.EnrolledSubjectArchive.Where(f => f.EnrollmentId == id).Sum(x => x.TotalScore);
                var setting = db.Settings.FirstOrDefault();
                int subjectCount = db.EnrolledSubjectArchive.Where(x => x.EnrollmentId == id && x.IsOffered == true).Count();



                string resultTitle = "CUMMULATIVE REPORT FOR " + currentsessiomterm;

                decimal? totalsc = 0;
                decimal? overall = 0;
                decimal? cutoff = getuserenrollment.ClassLevel.Passmark;

                if (subjectCount != 0)
                {
                    decimal? sum = total / subjectCount;
                    decimal averageScore = sum.Value;
                    totalsc = total;
                    overall = subjectCount * 100;
                    //if (sum <= cutoff)
                    //{
                    //    grade = "F";
                    //    remark = "Fail";
                    //}
                    //else if (sum >= cutoff && sum <= 100)
                    //{
                    //    grade = "P";
                    //    remark = "Pass";
                    //}

                    if (cutoff > sum)
                    {
                        grade = "F";
                        remark = "FAIL";
                    }


                    else if (cutoff == sum)
                    {
                        grade = "P";
                        remark = "PASS";
                    }


                    else if (sum <= 49.00m && sum >= 40.00m)
                    {
                        grade = "P";
                        remark = "PASS";
                    }

                    else if (sum <= 49.99m && sum >= 40.99m)
                    {
                        grade = "P";
                        remark = "PASS";
                    }

                    else if (sum <= 59.00m && sum >= 50.00m)
                    {
                        grade = "P";
                        remark = "PASS";
                    }
                    else if (sum <= 59.99m && sum >= 55.99m)
                    {
                        grade = "P";
                        remark = "PASS";
                    }
                    else if (sum <= 64.00m && sum >= 60.00m)
                    {
                        grade = "C";
                        remark = "GOOD";
                    }
                    else if (sum <= 64.99m && sum >= 60.99m)
                    {
                        grade = "C";
                        remark = "GOOD";
                    }
                    else if (sum <= 69.00m && sum >= 65.00m)
                    {
                        grade = "C";
                        remark = "GOOD";
                    }
                    else if (sum <= 69.99m && sum >= 65.99m)
                    {
                        grade = "C";
                        remark = "GOOD";
                    }
                    else if (sum <= 73.00m && sum >= 70.00m)
                    {
                        grade = "B";
                        remark = "VERY GOOD";
                    }
                    else if (sum <= 73.99m && sum >= 70.99m)
                    {
                        grade = "B";
                        remark = "VERY GOOD";
                    }
                    else if (sum <= 100 && sum >= 74.00m)
                    {
                        grade = "A";
                        remark = "EXCELLENT";

                    }
                    else if (sum <= 100 && sum >= 74.99m)
                    {
                        grade = "A";
                        remark = "EXCELLENT";

                    }



                    string mainAverage = averageScore.ToString("0.00");
                    var positionsh = db.Enrollments.Include(x => x.StudentProfile).Include(x => x.EnrolledSubjectArchive).Where(x => x.SessionId == getuserenrollment.SessionId && x.ClassLevelId == getuserenrollment.ClassLevelId).OrderByDescending(x => x.AverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber);
                    var positions = db.Enrollments.Include(x => x.StudentProfile).Include(x => x.EnrolledSubjectArchive).Where(x => x.SessionId == getuserenrollment.SessionId && x.ClassLevelId == getuserenrollment.ClassLevelId && x.AverageScore > 0).OrderByDescending(x => x.AverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber);

                    totalStudents = positions.Count();
                    int position = 0;
                    int mposition = 0;
                    decimal? avg = 0;
                    //  var GETaa = db.BatchResults.Where(x => x.BatchId == batchid).OrderByDescending(x => x.AverageScore);
                    //   var useposl = GETaa.Where(x => x.AverageScore == getuserenrollment.AverageScore);
                    //var GETaa = db.Enrollments.Include(x => x.StudentProfile).Include(x => x.EnrolledSubjectArchive).Where(x => x.SessionId == getuserenrollment.SessionId && x.ClassLevelId == getuserenrollment.ClassLevelId && x.AverageScore > 0).OrderByDescending(x => x.AverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber);

                    //var usepositionforall = GETaa.Select(x => x.AverageScore);
                    //var usepositionfor = db.Enrollments.Include(x => x.StudentProfile).Include(x => x.EnrolledSubjectArchive).Where(x => x.SessionId == getuserenrollment.SessionId && x.ClassLevelId == getuserenrollment.ClassLevelId && x.AverageScore > 0).OrderByDescending(x => x.AverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber);



                    var GETaa = db.Enrollments.Include(x => x.StudentProfile).Include(x => x.EnrolledSubjectArchive).OrderBy(x => x.StudentProfile.user.Surname).Include(x => x.StudentProfile).Where(s => s.ClassLevelId == getuserenrollment.ClassLevelId && s.SessionId == getuserenrollment.SessionId && s.EnrolledSubjectArchive.Count() > 0);

                    var useposl = GETaa.Where(x => x.AverageScore == getuserenrollment.AverageScore);
                    var usepositionforall = useposl.Select(x => x.AverageScore);
                    var usepositionfor = db.Enrollments.OrderBy(x => x.StudentProfile.user.Surname).Include(x => x.StudentProfile).Where(s => s.ClassLevelId == getuserenrollment.ClassLevelId && s.SessionId == getuserenrollment.SessionId && s.EnrolledSubjectArchive.Count() > 0 && s.AverageScore == getuserenrollment.AverageScore).OrderByDescending(x => x.AverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber).ToArray();
                    var usepobbb = db.Enrollments.OrderBy(x => x.StudentProfile.user.Surname).Include(x => x.StudentProfile).Where(s => s.ClassLevelId == getuserenrollment.ClassLevelId && s.SessionId == getuserenrollment.SessionId && s.EnrolledSubjectArchive.Count() > 0 && s.AverageScore == getuserenrollment.AverageScore).OrderByDescending(x => x.AverageScore).ThenBy(x => x.StudentProfile.StudentRegNumber).Select(x => x.StudentProfile.StudentRegNumber).ToArray();

                    if (subjectCount != 0)
                    {

                        foreach (var p in positions)
                        {


                            position = position + 1;

                            if (p.Id == id)
                            {
                                avg = p.AverageScore.Value;
                                goto outloop;
                            }
                        }

                        outloop: userposition = position;
                        useraveragescore = Math.Round(averageScore, 2);

                    }
                    if (usepositionforall.Count() > 1)
                    {
                        if (usepositionforall.Contains(getuserenrollment.AverageScore))
                        {
                            var assignposion = usepositionfor.ToArray();
                            var assignposition = usepositionfor.First();
                            var pos = Position12(assignposition.Id);
                            userposition = pos.Position;
                        }
                    }

                }
                byte[] abuser;
                byte[] absession;
                byte[] stpp;
                var img = db.ImageModel.FirstOrDefault(x => x.Id == getuserprofile.ImageId);
                var Sch = db.Settings.FirstOrDefault();
                var Schlogo = db.ImageModel.FirstOrDefault(x => x.Id == Sch.ImageId);
                var schstamp = db.ImageModel.FirstOrDefault(x => x.FileName == "SCHOOLSTAM");

                if (Schlogo == null)
                {
                    absession = new byte[0];
                }
                else
                {
                    absession = Schlogo.ImageContent;
                }

                if (img == null)
                {
                    abuser = new byte[0];
                }
                else
                {
                    abuser = img.ImageContent;
                }
                abuser = new byte[0];
                if (schstamp == null)
                {
                    stpp = new byte[0];
                }
                else
                {
                    stpp = schstamp.ImageContent;
                }

                var output = new PrintThirdTermArchiveDto();
                string promote = "";
                //grading system in result =
                var avb = db.Gradings.Include(x => x.GradingDetails).FirstOrDefault(x => x.Name == getuserenrollment.ClassLevel.ClassName.Substring(0, 3));
                var grdsystem = db.GradingDetails.Where(x => x.GradingId == avb.Id);
                var SchoolAcct = db.SchoolAccounts.ToList();

                var recognitivedomain = new RecognitiveDomainArchive();
                var recognitivedomain1 = db.RecognitiveDomainArchive.FirstOrDefault(x => x.EnrolmentId == getuserenrollment.Id);
                if (recognitivedomain1 != null)
                {
                    recognitivedomain = recognitivedomain1;
                }
                else
                {
                    if (recognitivedomain == null)
                    {
                        recognitivedomain.Rememberance = "Good";
                        recognitivedomain.Understanding = "Good";
                        recognitivedomain.Application = "Good";
                        recognitivedomain.Analyzing = "Good";
                        recognitivedomain.Evaluation = "Good";
                        recognitivedomain.Creativity = "Good";
                    }
                }
                // recognitivedomain = recognitivedomain1;


                //Psychomtor Domain

                var psychomotordomain = new PsychomotorDomainArchive();
                var psychomotordomain1 = db.PsychomotorDomainArchive.FirstOrDefault(x => x.EnrolmentId == getuserenrollment.Id);
                if (psychomotordomain != null)
                {
                    psychomotordomain = psychomotordomain1;
                }
                else
                {
                    if (psychomotordomain == null)
                    {
                        psychomotordomain.Drawing = "Good";
                        psychomotordomain.Painting = "Good";
                        psychomotordomain.Handwriting = "Good";
                        psychomotordomain.Hobbies = "Good";
                        psychomotordomain.Sports = "Good";
                        psychomotordomain.Club = "Good";
                    }
                }

                //Affective Domain

                var affectivedomain = new AffectiveDomainArchive();
                var affectivedomain1 = db.AffectiveDomainArchive.FirstOrDefault(x => x.EnrolmentId == getuserenrollment.Id);
                if (affectivedomain != null)
                {
                    affectivedomain = affectivedomain1;
                }
                else
                {
                    if (affectivedomain == null)
                    {
                        affectivedomain.Attentiveness = "Good";
                        affectivedomain.Honesty = "Good";
                        affectivedomain.Neatness = "Good";
                        affectivedomain.Punctuality = "Good";
                        affectivedomain.Relationship = "Good";

                    }
                }

                // SchoolFees
                var schoolfee = db.SchoolFeesArchive.Where(x => x.SessionId == getenrollmentsession.Id).ToList();

                //News Letter
                var newsletter = db.NewsLetterArchive.Where(x => x.SessionId == getenrollmentsession.Id).OrderByDescending(x => x.SessionId).Take(1).ToList();

                var principal = db.PrincipalArchives.Where(x => x.SessionId == getenrollmentsession.Id).OrderByDescending(x => x.SessionId).FirstOrDefault();
                //Get enrollment for each term
                var firstEnrollment = db.Enrollments.FirstOrDefault(f => f.StudentProfileId == getuserenrollment.StudentProfileId && f.SessionId == firstTerm.Id);
                var secondEnrollment = db.Enrollments.FirstOrDefault(f => f.StudentProfileId == getuserenrollment.StudentProfileId && f.SessionId == secondTerm.Id);
                var thirdEnrollment = db.Enrollments.FirstOrDefault(f => f.StudentProfileId == getuserenrollment.StudentProfileId && f.SessionId == thirdTerm.Id);

                List<EnrolledSubjectArchive> EnrolledSubjectArchiveFirst = new List<EnrolledSubjectArchive>();
                List<EnrolledSubjectArchive> EnrolledSubjectArchiveSecond = new List<EnrolledSubjectArchive>();
                List<EnrolledSubjectArchive> EnrolledSubjectArchiveThird = new List<EnrolledSubjectArchive>();
                string firsttermerror = "";
                string secondtermerror = "";
                if (firstEnrollment != null)
                {
                    EnrolledSubjectArchiveFirst = db.EnrolledSubjectArchive.Include(x => x.Enrollments).Where(x => x.EnrollmentId == firstEnrollment.Id && x.IsOffered == true).OrderBy(x => x.SubjectName).ThenBy(x => x.Id).ToList();


                }
                else
                {
                    firsttermerror = "Empty";
                }

                if (secondEnrollment != null)
                {
                    EnrolledSubjectArchiveSecond = db.EnrolledSubjectArchive.Include(x => x.Enrollments).Where(x => x.EnrollmentId == secondEnrollment.Id && x.IsOffered == true).OrderBy(x => x.SubjectName).ThenBy(x => x.Id).ToList();

                }
                else
                {
                    secondtermerror = "Empty";
                }
                EnrolledSubjectArchiveThird = db.EnrolledSubjectArchive.Include(x => x.Enrollments).Where(x => x.EnrollmentId == thirdEnrollment.Id && x.IsOffered == true).OrderBy(x => x.SubjectName).ThenBy(x => x.Id).ToList();


                if (thirdEnrollment.CummulativeAverageScore == null)
                {
                    decimal calulatedAverage = 0;
                    decimal? cumTotal = 0;
                    if (firstEnrollment.AverageScore == null && secondEnrollment.AverageScore == null)
                    {
                        cumTotal = thirdEnrollment.AverageScore;
                        if (cumTotal == null)
                        {
                            calulatedAverage = 0.00m;
                        }
                        else
                        {
                            calulatedAverage = cumTotal.Value;
                        }

                    }
                    else if (firstEnrollment.AverageScore == null)
                    {
                        cumTotal = secondEnrollment.AverageScore + thirdEnrollment.AverageScore;
                        if (cumTotal == null)
                        {
                            calulatedAverage = 0.00m;
                        }
                        else
                        {
                            calulatedAverage = cumTotal.Value / 2;
                        }

                    }
                    else if (secondEnrollment.AverageScore == null)
                    {
                        cumTotal = firstEnrollment.AverageScore + thirdEnrollment.AverageScore;
                        if (cumTotal == null)
                        {
                            calulatedAverage = 0.00m;
                        }
                        else
                        {
                            calulatedAverage = cumTotal.Value / 2;
                        }

                    }
                    else
                    {
                        cumTotal = firstEnrollment.AverageScore + secondEnrollment.AverageScore + thirdEnrollment.AverageScore;
                        if (cumTotal == null)
                        {
                            calulatedAverage = 0.00m;
                        }
                        else
                        {
                            calulatedAverage = cumTotal.Value / 3;
                        }

                    }

                    thirdEnrollment.CummulativeAverageScore = calulatedAverage;
                    db.SaveChanges();
                }

                var cummulativePositions = db.Enrollments.Include(x => x.StudentProfile.user).Where(s => s.SessionId == getuserenrollment.SessionId && s.ClassLevelId == getuserenrollment.ClassLevelId).OrderByDescending(x => x.CummulativeAverageScore).ThenBy(x => x.StudentProfile.user.Surname);
                // int totalStudents = positions.Count();
                int cummulativePosition = 0;
                decimal? cumAvg = 0;
                int cumPosition = 0;
                string cumAverage = "";
                if (subjectCount != 0)
                {
                    foreach (var p in cummulativePositions)
                    {
                        cummulativePosition = cummulativePosition + 1;
                        if (p.Id == id)
                        {
                            if(p.CummulativeAverageScore != null) { 
                            cumAvg = p.CummulativeAverageScore.Value;
                            }
                            goto outloop;
                        }
                    }

                    outloop: cumPosition = cummulativePosition;
                    decimal cummulativeAverage = cumAvg.Value;
                    string dispaly = cummulativeAverage.ToString("0.00");
                    cumAverage = dispaly;

                    decimal? AfirstTerm = 0;
                    decimal? AsecTerm = 0;
                    decimal? AthirdTerm = 0;
                    if (firstEnrollment != null)
                    {
                        AfirstTerm = firstEnrollment.AverageScore;
                    }
                    if (secondEnrollment != null)
                    {
                        AsecTerm = secondEnrollment.AverageScore;
                    }
                    if (thirdEnrollment != null)
                    {
                        AthirdTerm = thirdEnrollment.AverageScore;
                    }

                    output = new PrintThirdTermArchiveDto
                    {

                        Average = useraveragescore,
                        Position = userposition,
                        ClassName = getuserenrollment.ClassLevel.ClassName,
                        TotalStudent = totalStudents,
                        SessionTerm = currentsessiomterm,
                        SchoolName = setting.SchoolName,
                        ResultGrade = grade,
                        ResultRemark = remark,
                        RegNumber = getuserprofile.StudentRegNumber,
                        Fullname = getuserprofile.user.Surname + " " + getuserprofile.user.FirstName + " " + getuserprofile.user.OtherName,
                        SubjectList = subjectlist.ToList(),
                        studentImage = abuser,
                        SchoolLogo = absession,
                        SchoolStamp = stpp,
                        GradingDetails = grdsystem.ToList(),
                        RecognitiveDomain = recognitivedomain,
                        PsychomotorDomain = psychomotordomain,
                        AffectiveDomain = affectivedomain,
                        SchoolAccount = SchoolAcct,
                        Address = setting.SchoolAddress,
                        headteacher = principal.PrincipalName,
                        Email = setting.ContactEmail,
                        Phone = setting.ContactPhoneNumber,
                        Website = setting.WebsiteLink,
                        NewsLetter = newsletter,

                        TotalScore = totalsc,
                        OverallScore = overall,
                        PromotionStatus = promote,
                        CummulativeAverage = cumAverage,
                        CummulativePosition = cumPosition,
                        CummulativeSessionTerm = resultTitle,
                        CummulativeFirstScore = EnrolledSubjectArchiveFirst.ToList(),
                        CummulativeSecondScore = EnrolledSubjectArchiveSecond.ToList(),
                        CummulativeThirdScore = EnrolledSubjectArchiveThird.ToList(),
                        checkFirthTerm = firsttermerror,
                        checkSecondTerm = secondtermerror,
                        EnrollmentRemark = getuserenrollment.EnrollmentRemark1,
                        EnrollmentRemark2 = getuserenrollment.EnrollmentRemark2,
                        SchoolFees = schoolfee,
                        AverageFirthTerm = AfirstTerm,
                        AverageSecondTerm = AsecTerm,
                        AverageThirdTerm = AthirdTerm,
                        StudentId = getuserprofile.Id,
                        IsEngMath = setting.PromotionByMathsAndEng,
                        cumshowPosOnClassResult = getuserenrollment.ClassLevel.ShowPositionOnClassResult,
                        cumshowSchAccountOnResult = setting.ShowAccctOnResult,
                        cumshowSchFeeOnResult = setting.ShowFeesOnResult




                    };



                }

                var resu = output;
                return output;

            }
        }
        public static string abcd(int id, int StudentId)
        {
            string a = AverageScoreSubject(id, StudentId);
            return a;
        }

        public static string AverageScoreSubject(int? id, int? StudentId)
        {
            string subc = "";
            using (var db = new ApplicationDbContext())
            {
                try
                {


                    var getuserenrollment = db.Enrollments.Include(x => x.Session).Include(p => p.ClassLevel).Include(x => x.EnrolledSubjectArchive).FirstOrDefault(x => x.StudentProfileId == StudentId);

                    var currentEnrollment = db.Enrollments.Include(x => x.ClassLevel).FirstOrDefault(x => x.StudentProfileId == StudentId);
                    var currentYear = db.Sessions.FirstOrDefault(f => f.Id == currentEnrollment.SessionId);
                    string year = currentYear.SessionYear;
                    var getuserprofile = db.StudentProfiles.Include(c => c.user).FirstOrDefault(x => x.Id == getuserenrollment.StudentProfileId);

                    //Get Session IDs for each term
                    var firstTerm = db.Sessions.FirstOrDefault(x => x.SessionYear == year && x.Term == "First");
                    var secondTerm = db.Sessions.FirstOrDefault(x => x.SessionYear == year && x.Term == "Second");
                    var thirdTerm = db.Sessions.FirstOrDefault(x => x.SessionYear == year && x.Term == "Third");
                    //Get enrollment for each term
                    var firstEnrollment = db.Enrollments.FirstOrDefault(f => f.StudentProfileId == StudentId && f.SessionId == firstTerm.Id);
                    var secondEnrollment = db.Enrollments.FirstOrDefault(f => f.StudentProfileId == StudentId && f.SessionId == secondTerm.Id);
                    var thirdEnrollment = db.Enrollments.FirstOrDefault(f => f.StudentProfileId == StudentId && f.SessionId == thirdTerm.Id);

                    var firsttermsubject = db.EnrolledSubjectArchive.FirstOrDefault(x => x.Id == id && x.EnrollmentId == firstEnrollment.Id);
                    var secondtermsubject = db.EnrolledSubjectArchive.FirstOrDefault(x => x.Id == id && x.EnrollmentId == secondEnrollment.Id);
                    var thirdtermsubject = db.EnrolledSubjectArchive.FirstOrDefault(x => x.Id == id && x.EnrollmentId == thirdEnrollment.Id);
                    decimal? Ave1 = 0.00m;
                    decimal? Ave2 = 0.00m;
                    decimal? Ave3 = 0.00m;

                    if (firsttermsubject != null)
                    {
                        Ave1 = firsttermsubject.TotalScore;
                    }

                    if (secondtermsubject != null)
                    {
                        Ave2 = secondtermsubject.TotalScore;
                    }

                    if (thirdtermsubject != null)
                    {
                        Ave3 = thirdtermsubject.TotalScore;
                    }
                    decimal? totalScore = 0;

                    decimal? AverageTotalScore = 0;
                    if (firsttermsubject != null && secondtermsubject != null && thirdtermsubject != null)
                    {
                        if (Ave1 > 0 && Ave2 > 0 && Ave3 > 0)
                        {
                            totalScore = Ave1 + Ave2 + Ave3;

                            AverageTotalScore = totalScore / 3;
                        }
                        else if (Ave1 == 0 && Ave2 > 0 && Ave3 > 0)
                        {
                            totalScore = Ave2 + Ave3;

                            AverageTotalScore = totalScore / 2;
                        }
                        else if (Ave1 == 0 && Ave2 == 0 && Ave3 > 0)
                        {
                            totalScore = Ave3;

                            AverageTotalScore = totalScore / 1;
                        }
                        else if (Ave1 > 0 && Ave2 == 0 && Ave3 > 0)
                        {
                            totalScore = Ave1 + Ave3;

                            AverageTotalScore = totalScore / 2;
                        }

                    }


                    AverageTotalScore = Math.Round(Convert.ToDecimal(AverageTotalScore), 2);
                    subc = AverageTotalScore.ToString();
                }
                catch (Exception e)
                {

                }
            }
            return subc;

        }

        public static string PromotionSubject(int? StudentId)
        {
            string subc = "";
            decimal? MathsAve = 0.00m;
            decimal? EngAve = 0.00m;
            string promote = "Null";
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    var enrolmnt = db.Enrollments.Include(x => x.ClassLevel).FirstOrDefault(x => x.StudentProfileId == StudentId);

                    var subjectss = db.EnrolledSubjectArchive.Where(x => x.EnrollmentId == enrolmnt.Id).ToList();
                    var subjectMath = subjectss.FirstOrDefault(x => x.SubjectName.Substring(0, 6).ToUpper() == "MATHEM");
                    var subjectEng = subjectss.FirstOrDefault(x => x.SubjectName.ToUpper().Contains("ENGLISH STUDIES") || x.SubjectName.ToUpper().Contains("ENGLISH LANGUAGE"));
                    var mathsidd = db.EnrolledSubjectArchive.FirstOrDefault(x => x.Id == subjectMath.Id);
                    var engidd = db.EnrolledSubjectArchive.FirstOrDefault(x => x.Id == subjectEng.Id);

                    foreach (var a1 in subjectss)
                    {

                        if (a1.Id == subjectMath.Id)
                        {
                            try
                            {
                                var currentEnrollment = db.Enrollments.Include(x => x.ClassLevel).FirstOrDefault(x => x.StudentProfileId == StudentId);

                                var getuserenrollment = db.Enrollments.Include(x => x.Session).Include(p => p.ClassLevel).Include(x => x.ClassLevel.Subjects).FirstOrDefault(x => x.StudentProfileId == StudentId);

                                var currentYear = db.Sessions.FirstOrDefault(f => f.Id == currentEnrollment.SessionId);
                                string year = currentYear.SessionYear;
                                var getuserprofile = db.StudentProfiles.Include(c => c.user).FirstOrDefault(x => x.Id == getuserenrollment.StudentProfileId);

                                //Get Session IDs for each term
                                var firstTerm = db.Sessions.FirstOrDefault(x => x.SessionYear == year && x.Term == "First");
                                var secondTerm = db.Sessions.FirstOrDefault(x => x.SessionYear == year && x.Term == "Second");
                                var thirdTerm = db.Sessions.FirstOrDefault(x => x.SessionYear == year && x.Term == "Third");
                                //Get enrollment for each term
                                var firstEnrollment = db.Enrollments.FirstOrDefault(f => f.StudentProfileId == StudentId && f.SessionId == firstTerm.Id);
                                var secondEnrollment = db.Enrollments.FirstOrDefault(f => f.StudentProfileId == StudentId && f.SessionId == secondTerm.Id);
                                var thirdEnrollment = db.Enrollments.FirstOrDefault(f => f.StudentProfileId == StudentId && f.SessionId == thirdTerm.Id);

                                //var firsttermsubject = db.EnrolledSubjectArchive.FirstOrDefault(x => x.Id == mathsidd.Id && x.EnrollmentId == firstEnrollment.Id);
                                //var secondtermsubject = db.EnrolledSubjectArchive.FirstOrDefault(x => x.Id == mathsidd.Id && x.EnrollmentId == secondEnrollment.Id);
                                //var thirdtermsubject = db.EnrolledSubjectArchive.FirstOrDefault(x => x.Id == mathsidd.Id && x.EnrollmentId == thirdEnrollment.Id);
                                var secondtermsubject = secondEnrollment.EnrolledSubjectArchive.FirstOrDefault(x => x.Id == subjectMath.Id && x.EnrollmentId == secondEnrollment.Id);
                                var thirdtermsubject = thirdEnrollment.EnrolledSubjectArchive.FirstOrDefault(x => x.Id == subjectMath.Id && x.EnrollmentId == thirdEnrollment.Id);
                                var firsttermsubject = firstEnrollment.EnrolledSubjectArchive.FirstOrDefault(x => x.Id == subjectMath.Id && x.EnrollmentId == firstEnrollment.Id);
                                decimal? Ave1 = 0.00m;
                                decimal? Ave2 = 0.00m;
                                decimal? Ave3 = 0.00m;
                                if (firsttermsubject != null)
                                {
                                    Ave1 = firsttermsubject.TotalScore;
                                }

                                if (secondtermsubject != null)
                                {
                                    Ave2 = secondtermsubject.TotalScore;
                                }

                                if (thirdtermsubject != null)
                                {
                                    Ave3 = thirdtermsubject.TotalScore;
                                }
                                decimal? totalScore = 0;

                                decimal? AverageTotalScore = 0;
                                if (firsttermsubject != null && secondtermsubject != null && thirdtermsubject != null)
                                {
                                    if (Ave1 > 0 && Ave2 > 0 && Ave3 > 0)
                                    {
                                        totalScore = Ave1 + Ave2 + Ave3;

                                        AverageTotalScore = totalScore / 3;
                                    }
                                    else if (Ave1 == 0 && Ave2 > 0 && Ave3 > 0)
                                    {
                                        totalScore = Ave2 + Ave3;

                                        AverageTotalScore = totalScore / 2;
                                    }
                                    else if (Ave1 == 0 && Ave2 == 0 && Ave3 > 0)
                                    {
                                        totalScore = Ave3;

                                        AverageTotalScore = totalScore / 1;
                                    }

                                }



                                AverageTotalScore = Math.Round(Convert.ToDecimal(AverageTotalScore), 2);
                                MathsAve = AverageTotalScore;
                            }
                            catch (Exception e)
                            {

                            }

                        }
                    }

                    foreach (var a2 in subjectss)
                    {

                        if (a2.Id == subjectEng.Id)
                        {
                            try
                            {
                                var getuserenrollment = db.Enrollments.Include(x => x.Session).Include(p => p.ClassLevel).Include(x => x.ClassLevel.Subjects).FirstOrDefault(x => x.StudentProfileId == StudentId);

                                var currentEnrollment = db.Enrollments.Include(x => x.ClassLevel).FirstOrDefault(x => x.StudentProfileId == StudentId);
                                var currentYear = db.Sessions.FirstOrDefault(f => f.Id == currentEnrollment.SessionId);
                                string year = currentYear.SessionYear;
                                var getuserprofile = db.StudentProfiles.Include(c => c.user).FirstOrDefault(x => x.Id == getuserenrollment.StudentProfileId);

                                //Get Session IDs for each term
                                var firstTerm = db.Sessions.FirstOrDefault(x => x.SessionYear == year && x.Term == "First");
                                var secondTerm = db.Sessions.FirstOrDefault(x => x.SessionYear == year && x.Term == "Second");
                                var thirdTerm = db.Sessions.FirstOrDefault(x => x.SessionYear == year && x.Term == "Third");
                                //Get enrollment for each term
                                var firstEnrollment = db.Enrollments.FirstOrDefault(f => f.StudentProfileId == StudentId && f.SessionId == firstTerm.Id);
                                var secondEnrollment = db.Enrollments.FirstOrDefault(f => f.StudentProfileId == StudentId && f.SessionId == secondTerm.Id);
                                var thirdEnrollment = db.Enrollments.FirstOrDefault(f => f.StudentProfileId == StudentId && f.SessionId == thirdTerm.Id);

                                //var firsttermsubject = db.EnrolledSubjectArchive.FirstOrDefault(x => x.Id == mathsidd.Id && x.EnrollmentId == firstEnrollment.Id);
                                //var secondtermsubject = db.EnrolledSubjectArchive.FirstOrDefault(x => x.Id == mathsidd.Id && x.EnrollmentId == secondEnrollment.Id);
                                //var thirdtermsubject = db.EnrolledSubjectArchive.FirstOrDefault(x => x.Id == mathsidd.Id && x.EnrollmentId == thirdEnrollment.Id);
                                var secondtermsubject = secondEnrollment.EnrolledSubjectArchive.FirstOrDefault(x => x.Id == subjectEng.Id && x.EnrollmentId == secondEnrollment.Id);
                                var thirdtermsubject = thirdEnrollment.EnrolledSubjectArchive.FirstOrDefault(x => x.Id == subjectEng.Id && x.EnrollmentId == thirdEnrollment.Id);
                                var firsttermsubject = firstEnrollment.EnrolledSubjectArchive.FirstOrDefault(x => x.Id == subjectEng.Id && x.EnrollmentId == firstEnrollment.Id);
                                decimal? Ave1 = 0.00m;
                                decimal? Ave2 = 0.00m;
                                decimal? Ave3 = 0.00m;
                                if (firsttermsubject != null)
                                {
                                    Ave1 = firsttermsubject.TotalScore;
                                }

                                if (secondtermsubject != null)
                                {
                                    Ave2 = secondtermsubject.TotalScore;
                                }

                                if (thirdtermsubject != null)
                                {
                                    Ave3 = thirdtermsubject.TotalScore;
                                }

                                if (firsttermsubject != null)
                                {
                                    Ave1 = firsttermsubject.TotalScore;
                                }

                                if (secondtermsubject != null)
                                {
                                    Ave2 = secondtermsubject.TotalScore;
                                }

                                if (thirdtermsubject != null)
                                {
                                    Ave3 = thirdtermsubject.TotalScore;
                                }
                                decimal? totalScore = 0;

                                decimal? AverageTotalScore = 0;
                                if (firsttermsubject != null && secondtermsubject != null && thirdtermsubject != null)
                                {
                                    if (Ave1 > 0 && Ave2 > 0 && Ave3 > 0)
                                    {
                                        totalScore = Ave1 + Ave2 + Ave3;

                                        AverageTotalScore = totalScore / 3;
                                    }
                                    else if (Ave1 == 0 && Ave2 > 0 && Ave3 > 0)
                                    {
                                        totalScore = Ave2 + Ave3;

                                        AverageTotalScore = totalScore / 2;
                                    }
                                    else if (Ave1 == 0 && Ave2 == 0 && Ave3 > 0)
                                    {
                                        totalScore = Ave3;

                                        AverageTotalScore = totalScore / 1;
                                    }

                                }


                                AverageTotalScore = Math.Round(Convert.ToDecimal(AverageTotalScore), 2);
                                EngAve = AverageTotalScore;
                            }
                            catch (Exception e)
                            {

                            }

                        }
                    }

                    // user class
                    var usetting = db.Settings.FirstOrDefault();
                    if (usetting.PromotionByMathsAndEng == true)
                    {
                        var getuserenrollment = db.Enrollments.Include(x => x.Session).Include(p => p.ClassLevel).Include(x => x.EnrolledSubjectArchive).FirstOrDefault(x => x.StudentProfileId == StudentId);

                        var currentEnrollment = db.Enrollments.Include(x => x.ClassLevel).FirstOrDefault(x => x.StudentProfileId == StudentId);
                        var currentYear = db.Sessions.FirstOrDefault(f => f.Id == currentEnrollment.SessionId);
                        string year = currentYear.SessionYear;
                        var getuserprofile = db.StudentProfiles.Include(c => c.user).FirstOrDefault(x => x.Id == getuserenrollment.StudentProfileId);

                        //Get Session IDs for each term
                        var firstTerm = db.Sessions.FirstOrDefault(x => x.SessionYear == year && x.Term == "First");
                        var secondTerm = db.Sessions.FirstOrDefault(x => x.SessionYear == year && x.Term == "Second");
                        var thirdTerm = db.Sessions.FirstOrDefault(x => x.SessionYear == year && x.Term == "Third");
                        //Get enrollment for each term
                        var firstEnrollment = db.Enrollments.FirstOrDefault(f => f.StudentProfileId == StudentId && f.SessionId == firstTerm.Id);
                        var secondEnrollment = db.Enrollments.FirstOrDefault(f => f.StudentProfileId == StudentId && f.SessionId == secondTerm.Id);
                        var thirdEnrollment = db.Enrollments.FirstOrDefault(f => f.StudentProfileId == StudentId && f.SessionId == thirdTerm.Id);

                        var userclass = db.ClassLevels.FirstOrDefault(x => x.Id == enrolmnt.ClassLevelId);
                        if (subjectMath != null || subjectEng != null)
                        {
                            if (usetting.Port == "0000")
                            {
                                if (thirdEnrollment.CummulativeAverageScore > usetting.Passmark)
                                {
                                    if (EngAve <= userclass.Passmark || MathsAve <= userclass.Passmark)
                                    {
                                        promote = "(PROMOTED ON TRIAL)";
                                    }
                                    else
                                    {
                                        promote = "(PROMOTED)";

                                    }
                                }
                                else
                                {
                                    promote = "(NOT PROMOTED)";
                                }

                            }
                            else
                            {


                                if (EngAve > userclass.Passmark || MathsAve > userclass.Passmark)
                                {
                                    promote = "(PROMOTED)";
                                }
                                else
                                {
                                    promote = "(NOT PROMOTED)";
                                }
                            }
                        }
                    }

                }
                catch (Exception e)
                {

                }


            }
            return promote;

        }



        public static string RemarkSubject(int id, int StudentId)
        {
            string subc = "";
            using (var db = new ApplicationDbContext())
            {

                var getuserenrollment = db.Enrollments.Include(x => x.Session).Include(p => p.ClassLevel).Include(x => x.EnrolledSubjectArchive).FirstOrDefault(x => x.StudentProfileId == StudentId);

                var currentEnrollment = db.Enrollments.Include(x => x.ClassLevel).FirstOrDefault(x => x.StudentProfileId == StudentId);
                var currentYear = db.Sessions.FirstOrDefault(f => f.Id == currentEnrollment.SessionId);
                string year = currentYear.SessionYear;
                var getuserprofile = db.StudentProfiles.Include(c => c.user).FirstOrDefault(x => x.Id == getuserenrollment.StudentProfileId);

                //Get Session IDs for each term
                var firstTerm = db.Sessions.FirstOrDefault(x => x.SessionYear == year && x.Term == "First");
                var secondTerm = db.Sessions.FirstOrDefault(x => x.SessionYear == year && x.Term == "Second");
                var thirdTerm = db.Sessions.FirstOrDefault(x => x.SessionYear == year && x.Term == "Third");
                //Get enrollment for each term
                var firstEnrollment = db.Enrollments.FirstOrDefault(f => f.StudentProfileId == StudentId && f.SessionId == firstTerm.Id);
                var secondEnrollment = db.Enrollments.FirstOrDefault(f => f.StudentProfileId == StudentId && f.SessionId == secondTerm.Id);
                var thirdEnrollment = db.Enrollments.FirstOrDefault(f => f.StudentProfileId == StudentId && f.SessionId == thirdTerm.Id);

                var firsttermsubject = db.EnrolledSubjectArchive.FirstOrDefault(x => x.Id == id && x.EnrollmentId == firstEnrollment.Id);
                var secondtermsubject = db.EnrolledSubjectArchive.FirstOrDefault(x => x.Id == id && x.EnrollmentId == secondEnrollment.Id);
                var thirdtermsubject = db.EnrolledSubjectArchive.FirstOrDefault(x => x.Id == id && x.EnrollmentId == thirdEnrollment.Id);
                decimal? Ave1 = 0;
                decimal? Ave2 = 0;
                decimal? Ave3 = 0;


                if (firsttermsubject != null)
                {
                    Ave1 = firsttermsubject.TotalScore;
                }

                if (secondtermsubject != null)
                {
                    Ave2 = secondtermsubject.TotalScore;
                }

                if (thirdtermsubject != null)
                {
                    Ave3 = thirdtermsubject.TotalScore;
                }
                decimal? totalScore = 0;

                decimal? AverageTotalScore = 0;
                if (firsttermsubject != null && secondtermsubject != null && thirdtermsubject != null)
                {
                    if (Ave1 > 0 && Ave2 > 0 && Ave3 > 0)
                    {
                        totalScore = Ave1 + Ave2 + Ave3;

                        AverageTotalScore = totalScore / 3;
                    }
                    else if (Ave1 == 0 && Ave2 > 0 && Ave3 > 0)
                    {
                        totalScore = Ave2 + Ave3;

                        AverageTotalScore = totalScore / 2;
                    }
                    else if (Ave1 == 0 && Ave2 == 0 && Ave3 > 0)
                    {
                        totalScore = Ave3;

                        AverageTotalScore = totalScore / 1;
                    }

                }

                AverageTotalScore = Math.Round(Convert.ToDecimal(AverageTotalScore), 2);

                if (AverageTotalScore <= 39.99m && AverageTotalScore >= 0.00m)
                {
                    return "FAIL";
                }
                else if (AverageTotalScore <= 49.99m && AverageTotalScore >= 40.99m)
                {
                    return "PASS";
                }
                else if (AverageTotalScore <= 59.99m && AverageTotalScore >= 50.99m)
                {
                    return "CREDIT";
                }
                else if (AverageTotalScore <= 69.99m && AverageTotalScore >= 60.99m)
                {
                    return "VERY GOOD";
                }
                else if (AverageTotalScore <= 100 && AverageTotalScore >= 70.00m)
                {
                    return "DISTINCTION";
                }

                else
                {
                    return "No Remark";
                }


            }
            return subc;

        }

        #region REMARK COMPREHENSIVE


        //                if (getuserenrollment.ClassLevel.ClassName.Contains("SSS"))
        //                {
        //                    foreach (var item in db.GradingDetails.Where(x => x.Grading.Name == GradingOption.SSS.ToString()))
        //                    {
        //                        if (AverageTotalScore == Convert.ToDecimal(0.00))
        //                        {
        //                            return "-";
        //                        }
        //                        else if (AverageTotalScore >= item.LowerLimit && AverageTotalScore <= item.UpperLimit)
        //                        {
        //                            return item.Remark;
        //                        }
        //}
        //                }
        //                else if (getuserenrollment.ClassLevel.ClassName.Contains("JSS"))
        //                {
        //                    foreach (var item in db.GradingDetails.Where(x => x.Grading.Name == GradingOption.JSS.ToString()))
        //                    {

        //                        if (AverageTotalScore == Convert.ToDecimal(0.00))
        //                        {
        //                            return "-";
        //                        }
        //                        else if (AverageTotalScore >= item.LowerLimit && AverageTotalScore <= item.UpperLimit)
        //                        {
        //                            return item.Remark;
        //                        }

        //                    }

        //                }
        //                else if (getuserenrollment.ClassLevel.ClassName.Contains("PRI"))
        //                {
        //                    foreach (var item in db.GradingDetails.Where(x => x.Grading.Name == GradingOption.PRI.ToString()))
        //                    {
        //                        if (AverageTotalScore == Convert.ToDecimal(0.00))
        //                        {
        //                            return "-";
        //                        }
        //                        else if (AverageTotalScore >= item.LowerLimit && AverageTotalScore <= item.UpperLimit)
        //                        {
        //                            return item.Remark;
        //                        }

        //                    }

        //                }
        //                else if (getuserenrollment.ClassLevel.ClassName.Contains("NUR"))
        //                {
        //                    foreach (var item in db.GradingDetails.Where(x => x.Grading.Name == GradingOption.NUR.ToString()))
        //                    {
        //                        if (AverageTotalScore == Convert.ToDecimal(0.00))
        //                        {
        //                            return "-";
        //                        }
        //                        else if (AverageTotalScore >= item.LowerLimit && AverageTotalScore <= item.UpperLimit)
        //                        {
        //                            return item.Remark;
        //                        }

        //                    }

        //                }
        //                else if (getuserenrollment.ClassLevel.ClassName.Contains("PG"))
        //                {
        //                    try
        //                    {
        //                        string vcv = GradingOption.PG.ToString();
        //var g = db.Gradings.Where(x => x.Name == GradingOption.PG.ToString());
        //var itemq = db.GradingDetails.Where(x => x.Grading.Name == GradingOption.PG.ToString());
        //int dfd = itemq.Count();

        //                        foreach (var item in db.GradingDetails.Where(x => x.Grading.Name == GradingOption.PG.ToString()))
        //                        {
        //                            if (AverageTotalScore == Convert.ToDecimal(0.00))
        //                            {
        //                                return "-";
        //                            }
        //                            else if (AverageTotalScore >= item.LowerLimit && AverageTotalScore <= item.UpperLimit)
        //                            {
        //                                return item.Remark;
        //                            }
        //                        }
        //                    }
        //                    catch (Exception c)
        //                    {

        //                    }
        //                }

        //                else if (getuserenrollment.ClassLevel.ClassName.Contains("PRE"))
        //                {
        //                    foreach (var item in db.GradingDetails.Where(x => x.Grading.Name == GradingOption.PRE.ToString()))
        //                    {
        //                        if (AverageTotalScore == Convert.ToDecimal(0.00))
        //                        {
        //                            return "-";
        //                        }
        //                        else if (AverageTotalScore >= item.LowerLimit && AverageTotalScore <= item.UpperLimit)
        //                        {
        //                            return item.Remark;
        //                        }

        //                    }

        //                }

        #endregion

        //subject range in result
        public static SubjectRangeInResultDto SubjectRangeInResult(int sessionId, int subjectId, int? classId)
        {
            using (var db = new ApplicationDbContext())
            {

                try
                {

                    var subjectlistRangeScore = db.EnrolledSubjectArchive.Include(z => z.Enrollments).Where(d => d.Enrollments.ClassLevelId == classId && d.Id == subjectId && d.IsOffered == true && d.TotalScore > 0 && d.Enrollments.SessionId == sessionId).ToList();
                    var lowerscore = subjectlistRangeScore.OrderBy(x => x.TotalScore).FirstOrDefault().TotalScore;
                    var highscore = subjectlistRangeScore.OrderByDescending(x => x.TotalScore).FirstOrDefault().TotalScore;
                    var output = new SubjectRangeInResultDto
                    {
                        HghestScoreInSubject = highscore,
                        LowestScoreInSubject = lowerscore

                    };
                    var outputmain = output;
                    return outputmain;
                }
                catch (Exception c)
                {

                    var output = new SubjectRangeInResultDto
                    {
                        HghestScoreInSubject = 0,
                        LowestScoreInSubject = 0

                    };
                    var outputmain = output;
                    return outputmain;
                }
            }
        }


        ///cumulative for 1st and 2nd term
        ///

    }
}