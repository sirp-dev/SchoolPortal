using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Mvc;
using SchoolPortal.Web.Areas.Service;
using SchoolPortal.Web.Models.Dtos;
using SchoolPortal.Web.Models.ResultArchive;

namespace SchoolPortal.Web.Areas.Service
{
    public class GeneralService
    {
        public static bool IsUserInRole(string userId, string role)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            if (manager.IsInRole(userId, role))
            {
                return true;
            }

            return false;
        }


        public static List<Subject> SubjectName(int cId)
        {
            List<Subject> subname = new List<Subject>();
            using (var db = new ApplicationDbContext())
            {
                subname = db.Subjects.Include(x=>x.ClassLevel).Where(x => x.ClassLevelId == cId && x.ShowSubject == true && x.ClassLevel.ShowClass == true).OrderBy(x => x.SubjectName).ToList();

            }
            return subname;

        }

        public static List<EnrolledSubjectArchive> SubjectArchiveName(int cId)
        {
            List<EnrolledSubjectArchive> subname = new List<EnrolledSubjectArchive>();
            using (var db = new ApplicationDbContext())
            {
                var enr = db.Enrollments.Include(x => x.EnrolledSubjectArchive).FirstOrDefault(x => x.ClassLevelId == cId);
                subname = db.EnrolledSubjectArchive.Include(x => x.Enrollments).Where(x => x.Enrollments.ClassLevelId == enr.ClassLevelId && x.IsOffered == true).OrderBy(x => x.SubjectName).ToList();

            }
            return subname;

        }
        public static List<Enrollment> StudentlistByTerm(int cId, int sid)
        {
            List<Enrollment> students = new List<Enrollment>();
            using (var db = new ApplicationDbContext())
            {
                int session = db.Sessions.FirstOrDefault(x => x.Id == sid).Id;
                students = db.Enrollments.Include(x => x.User).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.ClassLevel).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.ClassLevelId == cId && x.SessionId == session).OrderBy(x => x.User.Surname).ToList();

            }
            return students;

        }//
        public static List<Enrollment> Studentlist(int cId)
        {
            List<Enrollment> students = new List<Enrollment>();
            using (var db = new ApplicationDbContext())
            {
                int session = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current).Id;
                students = db.Enrollments.Include(x => x.User).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.ClassLevel).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x=>x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.ClassLevelId == cId && x.SessionId == session).OrderBy(x => x.User.Surname).ToList();

            }
            return students;

        }//

        public static List<Enrollment> StudentlistBySession(int cId, int sId)
        {
            List<Enrollment> students = new List<Enrollment>();
            using (var db = new ApplicationDbContext())
            {
                int session =sId;
                students = db.Enrollments.Include(x => x.User).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.ClassLevel).Where(x => x.ClassLevelId == cId && x.SessionId == session).OrderBy(x => x.User.Surname).ToList();

            }
            return students;

        }//

        public static int StudentlistBySessionCount(int cId, int sId)
        {
            List<Enrollment> students = new List<Enrollment>();
            using (var db = new ApplicationDbContext())
            {
                int session = sId;
                students = db.Enrollments.Include(x => x.User).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.ClassLevel).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.ClassLevelId == cId && x.SessionId == session).OrderBy(x => x.User.Surname).ToList();

            }
            return students.Count();

        }//


        //student by sessio term
        public static List<Enrollment> StudentlistByTermCount(int cId, int sessionId)
        {
            List<Enrollment> students = new List<Enrollment>();
            using (var db = new ApplicationDbContext())
            {

                students = db.Enrollments.Include(x => x.User).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.ClassLevel).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.ClassLevelId == cId && x.SessionId == sessionId).OrderBy(x => x.User.Surname).ToList();

            }
            return students;

        }//

        public static int ResultCount(int cId)
        {
            int students = 0;
            using (var db = new ApplicationDbContext())
            {
                int session = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current).Id;
                students = db.Enrollments.Include(x => x.StudentProfile.user).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.ClassLevelId == cId && x.SessionId == session && x.AverageScore != null).Count();

            }
            return students;

        }

        public static bool ZoomEnable()
        {
            bool data = false;
            using (var db = new ApplicationDbContext())
            {
                data = db.Settings.FirstOrDefault().EnableZoom;

            }
            return data;

        }
        public static bool MultipleZoomEnable()
        {
            bool data = false;
            using (var db = new ApplicationDbContext())
            {
                data = db.Settings.FirstOrDefault().EnableMultipleZoom;

            }
            return data;

        }

        public static bool CBTEnable()
        {
            bool data = false;
            using (var db = new ApplicationDbContext())
            {
                data = db.Settings.FirstOrDefault().EnableCBT;

            }
            return data;

        }


        public static bool IsNewsletterPageContentEnable()
        {
            bool data = false;
            using (var db = new ApplicationDbContext())
            {
                data = db.Settings.FirstOrDefault().ShowNewsletterPage;

            }
            return data;

        }

        public static string PaymentAmountStatus(int id)
        {
            string data = "";
            //using (var db = new ApplicationDbContext())
            //{
            //    IQueryable<Enrollment> enrolment = from s in db.Enrollments
            //                                       .Include(x=>x.ClassLevel.PaymentAmounts)
            //                                       .Include(x=>x.Finances)
            //     .Where(x => x.ClassName.Contains("SSS"))
            //                                        select s;
            //}
            return data;

        }

        public static bool CheckPrintedStaus(string stPin, string pin)
        {
            bool data = false;
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    var pinUsage = db.PinCodeModels.FirstOrDefault(x => x.StudentPin == stPin && x.PinNumber == pin);
                    if (pinUsage.EnrollmentId == null || pinUsage.Usage <= 0)
                    {
                        data = true;
                    }
                    else if (pinUsage.EnrollmentId != null || pinUsage.Usage <= 0)
                    {
                        data = db.Enrollments.Include(x => x.StudentProfile).FirstOrDefault(x => x.Id == pinUsage.EnrollmentId).Printed;
                    }
                    else if (pinUsage.Usage <= 0)
                    {
                        data = true;
                    }
                    else
                    {
                        data = false;
                    }
                }
                catch (Exception) { data = false; }
            }
            return data;

        }

        public static string FullName(string Uid)
        {
            string name = "";
            using (var db = new ApplicationDbContext())
            {
                var u = db.Users.FirstOrDefault(x => x.Id == Uid);
                name = u.Surname + " " + u.FirstName + " (" + u.UserName + ")";
            }
            return name;

        }

        public static string FullNameByRegNumber(string number)
        {


            string name = "";
            using (var db = new ApplicationDbContext())
            {
                if (number.ToLower().Contains("Staff"))
                {
                    var profile = db.StaffProfiles.Include(x => x.user).FirstOrDefault(x => x.StaffRegistrationId == number);
                    name = profile.user.Surname + " " + profile.user.FirstName + " (" + profile.user.UserName + ")";
                }
                else
                {
                    var profile = db.StudentProfiles.Include(x => x.user).FirstOrDefault(x => x.StudentRegNumber == number);
                    name = profile.user.Surname + " " + profile.user.FirstName + " (" + profile.user.UserName + ")";
                }
            }
            return name;

        }

        public static List<Enrollment> StudentlistForASession(int cId)
        {
            List<Enrollment> students = new List<Enrollment>();
            using (var db = new ApplicationDbContext())
            {
                students = db.Enrollments.Include(x => x.User).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.ClassLevel).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.ClassLevelId == cId).OrderBy(x => x.Id).ToList();

            }
            return students;

        }
        public static List<Enrollment> StudentlistOrderById(int cId)
        {
            List<Enrollment> students = new List<Enrollment>();
            using (var db = new ApplicationDbContext())
            {
                int session = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current).Id;
                students = db.Enrollments.Include(x => x.User).Include(x => x.StudentProfile).Include(x => x.StudentProfile.user).Include(x => x.ClassLevel).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.ClassLevelId == cId && x.SessionId == session).OrderBy(x => x.Id).ToList();

            }
            return students;

        }
        public static int SubjectCount(int cId)
        {
            int subc = 0;
            using (var db = new ApplicationDbContext())
            {
                subc = db.Subjects.Include(x=>x.ClassLevel).Where(x => x.ClassLevelId == cId && x.ShowSubject == true && x.ClassLevel.ShowClass == true).OrderBy(x => x.SubjectName).Count();

            }
            return subc;

        }

        public static int SubjectArchiveCount(int cId)
        {
            int subc = 0;
            using (var db = new ApplicationDbContext())
            {
                subc = db.EnrolledSubjectArchive.Include(x => x.Enrollments).Where(x => x.Enrollments.ClassLevelId == cId && x.IsOffered == true).OrderBy(x => x.SubjectName).Count();

            }
            return subc;

        }

        public static int SubjectCountPerStudent(int enroId)
        {
            int subjectCount = 0;
            using (var db = new ApplicationDbContext())
            {

                subjectCount = db.EnrolledSubjects.Where(x => x.EnrollmentId == enroId && x.IsOffered == true).Count();



            }
            return subjectCount;

        }

        public static int SubjectArchiveCountPerStudent(int enroId, int sessId)
        {
            int subjectCount = 0;
            using (var db = new ApplicationDbContext())
            {

                subjectCount = db.EnrolledSubjectArchive.Include(x => x.Enrollments).Where(x => x.EnrollmentId == enroId && x.Enrollments.SessionId == sessId && x.IsOffered).Count();



            }
            return subjectCount;

        }

        public static IEnumerable<EnrolledSubject> SubjectScores(int enrId)
        {
            IEnumerable<EnrolledSubject> subname = new List<EnrolledSubject>();
            using (var db = new ApplicationDbContext())
            {
                subname = db.EnrolledSubjects.Include(x => x.Subject).Include(x => x.Enrollments).AsNoTracking().Where(x => x.EnrollmentId == enrId && x.Subject.ShowSubject == true).OrderBy(x => x.Subject.SubjectName).AsEnumerable();

            }
            return subname;

        }

        public static List<EnrolledSubjectArchive> SubjectArchiveScores(int enrId)
        {
            List<EnrolledSubjectArchive> subname = new List<EnrolledSubjectArchive>();
            using (var db = new ApplicationDbContext())
            {
                subname = db.EnrolledSubjectArchive.Include(x => x.Enrollments).Where(x => x.EnrollmentId == enrId).OrderBy(x => x.SubjectName).ToList();

            }
            return subname;

        }

        //cumulatuive total score

        public static List<decimal?> CumTotalScoresBysession(int enrId, string sessionyear, int subid)
        {
            List<decimal?> subname = new List<decimal?>();
            using (var db = new ApplicationDbContext())
            {
                var session = db.Sessions.Where(x => x.SessionYear == sessionyear).OrderBy(x => x.Term).ToList();
                foreach (var data in session)
                {
                    var profile = db.Enrollments.Include(x => x.StudentProfile).FirstOrDefault(x => x.Id == enrId);
                    var enrol = db.Enrollments.Include(x => x.EnrolledSubjects).Include(x => x.Session).FirstOrDefault(x => x.StudentProfileId == profile.StudentProfileId && x.SessionId == data.Id);
                    try
                    {
                        var subinfo = db.EnrolledSubjects.Include(x => x.Subject).Include(x => x.Enrollments).Include(x => x.Enrollments.Session).FirstOrDefault(x => x.EnrollmentId == enrol.Id && x.SubjectId == subid && x.IsOffered == true);
                        if (subinfo == null)
                        {
                            subname.Add(0);
                        }
                        else
                        {
                            subname.Add(subinfo.TotalScore);
                        }

                    }
                    catch (Exception s)
                    {
                        subname.Add(0);
                    }
                    //subname.Add(enrol.AverageScore);



                }
            }
            return subname;

        }


        public static List<decimal?> CumTotalArchiveScoresBysession(int enrId, string sessionyear, int subid)
        {
            List<decimal?> subname = new List<decimal?>();
            using (var db = new ApplicationDbContext())
            {
                var session = db.Sessions.Where(x => x.SessionYear == sessionyear).OrderBy(x => x.Term).ToList();
                foreach (var data in session)
                {
                    var profile = db.Enrollments.Include(x => x.StudentProfile).FirstOrDefault(x => x.Id == enrId);
                    var enrol = db.Enrollments.Include(x => x.EnrolledSubjectArchive).Include(x => x.Session).FirstOrDefault(x => x.StudentProfileId == profile.StudentProfileId && x.SessionId == data.Id);
                    try
                    {
                        var subinfo = db.EnrolledSubjectArchive.Include(x => x.Enrollments).Include(x => x.Enrollments.Session).FirstOrDefault(x => x.EnrollmentId == enrol.Id && x.Id == subid && x.IsOffered ==true);
                        //subname.Add(subinfo.TotalScore);
                        if (subinfo == null)
                        {
                            subname.Add(0);
                        }
                        else
                        {
                            subname.Add(subinfo.TotalScore);
                        }
                    }
                    catch (Exception s)
                    {
                        subname.Add(0);
                    }
                    //subname.Add(enrol.AverageScore);



                }
            }
            return subname;

        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="enrId"></param>
        /// <returns></returns>
        /// 

        public static string FirstTermCum(int enrId, string sessionyear)
        {
            string subname = "0";
            using (var db = new ApplicationDbContext())
            {
                var session = db.Sessions.FirstOrDefault(x => x.SessionYear == sessionyear && x.Term.ToLower() == "first");

                var profile = db.Enrollments.Include(x => x.StudentProfile).FirstOrDefault(x => x.Id == enrId);
                var enrol = db.Enrollments.Include(x => x.EnrolledSubjects).Include(x => x.Session).FirstOrDefault(x => x.StudentProfileId == profile.StudentProfileId && x.SessionId == session.Id);
                try
                {
                    if(enrol == null)
                    {
                        subname.ToString();
                    }
                    else
                    {
                        subname = enrol.AverageScore.ToString();
                    }
          
                }
                catch (Exception s)
                {

                }
                if (subname == "0")
                {
                    subname = "-";
                }
                if (String.IsNullOrEmpty(subname))
                {
                    subname = "-";
                }

            }
            return subname;

        }

        public static string SecondTermCum(int enrId, string sessionyear)
        {
            string subname = "0";
            using (var db = new ApplicationDbContext())
            {
                var session = db.Sessions.FirstOrDefault(x => x.SessionYear == sessionyear && x.Term.ToLower() == "second");

                var profile = db.Enrollments.Include(x => x.StudentProfile).FirstOrDefault(x => x.Id == enrId);
                var enrol = db.Enrollments.Include(x => x.EnrolledSubjects).Include(x => x.Session).FirstOrDefault(x => x.StudentProfileId == profile.StudentProfileId && x.SessionId == session.Id);
                try
                {
                    if (enrol == null)
                    {
                        subname.ToString();
                    }
                    else
                    {
                        subname = enrol.AverageScore.ToString();
                    }
                    //subname = enrol.AverageScore.ToString();
                }
                catch (Exception s)
                {

                }
                if (subname == "0")
                {
                    subname = "-";
                }
                if (String.IsNullOrEmpty(subname))
                {
                    subname = "-";
                }
            }
            return subname;

        }

        public static string ThirdTermCum(int enrId, string sessionyear)
        {
            string subname = "0";
            using (var db = new ApplicationDbContext())
            {
                var session = db.Sessions.FirstOrDefault(x => x.SessionYear == sessionyear && x.Term.ToLower() == "third");

                var profile = db.Enrollments.Include(x => x.StudentProfile).FirstOrDefault(x => x.Id == enrId);
                var enrol = db.Enrollments.Include(x => x.EnrolledSubjects).Include(x => x.Session).FirstOrDefault(x => x.StudentProfileId == profile.StudentProfileId && x.SessionId == session.Id);
                try
                {
                    if (enrol == null)
                    {
                        subname.ToString();
                    }
                    else
                    {
                        subname = enrol.AverageScore.ToString();
                    }
                    //subname = enrol.AverageScore.ToString();
                }
                catch (Exception s)
                {

                }
                if (subname == "0")
                {
                    subname = "-";
                }
                if (String.IsNullOrEmpty(subname))
                {
                    subname = "-";
                }

            }
            return subname;

        }
        ////
        ///
        ///


        public static decimal? TotalScore(int enrId)
        {
            decimal? no = 0;
            using (var db = new ApplicationDbContext())
            {
                no = db.EnrolledSubjects.Include(x => x.Subject).Include(x => x.Enrollments).Where(x => x.EnrollmentId == enrId && x.IsOffered ==true).OrderBy(x => x.Subject.SubjectName).Sum(x => x.TotalScore);

            }
            return no;

        }


        public static decimal? TotalCAScore(int sessionId)
        {
            decimal? no = 0;
            using (var db = new ApplicationDbContext())
            {
                no = db.EnrolledSubjects.Include(x => x.Subject).Include(x => x.Enrollments).Where(x => x.Enrollments.SessionId == sessionId && x.IsOffered ==true).OrderBy(x => x.Subject.SubjectName).Sum(x => x.TotalCA);
                var setting = db.Settings.FirstOrDefault();
                if(setting.EnableTestScore == false && setting.EnableTestScore2 == false)
                {
                    no = 0;
                }
            }
            
            
            return no;

        }


        public static decimal? TotalStudentCAScore(int enrolId)
        {
            decimal? no = 0;
            using (var db = new ApplicationDbContext())
            {
                no = db.EnrolledSubjects.Include(x => x.Subject).Include(x => x.Enrollments).Where(x => x.EnrollmentId == enrolId && x.IsOffered ==true).OrderBy(x => x.Subject.SubjectName).Sum(x => x.TotalCA);
                var setting = db.Settings.FirstOrDefault();
                if (setting.EnableTestScore == false && setting.EnableTestScore2 == false)
                {
                    no = 0;
                }
            }
            return no;

        }



        public static string FirstTermArchiveCum(int enrId, string sessionyear)
        {
            string subname = "0";
            using (var db = new ApplicationDbContext())
            {
                var session = db.Sessions.FirstOrDefault(x => x.SessionYear == sessionyear && x.Term.ToLower() == "first");

                var profile = db.Enrollments.Include(x => x.StudentProfile).FirstOrDefault(x => x.Id == enrId);
                var enrol = db.Enrollments.Include(x => x.EnrolledSubjectArchive).Include(x => x.Session).FirstOrDefault(x => x.StudentProfileId == profile.StudentProfileId && x.SessionId == session.Id);
                try
                {
                    if (enrol == null)
                    {
                        subname.ToString();
                    }
                    else
                    {
                        subname = enrol.AverageScore.ToString();
                    }
                    //subname = enrol.AverageScore.ToString();
                }
                catch (Exception s)
                {

                }
                if (subname == "0")
                {
                    subname = "-";
                }
                if (String.IsNullOrEmpty(subname))
                {
                    subname = "-";
                }

            }
            return subname;

        }

        public static string SecondTermArchiveCum(int enrId, string sessionyear)
        {
            string subname = "0";
            using (var db = new ApplicationDbContext())
            {
                var session = db.Sessions.FirstOrDefault(x => x.SessionYear == sessionyear && x.Term.ToLower() == "second");

                var profile = db.Enrollments.Include(x => x.StudentProfile).FirstOrDefault(x => x.Id == enrId);
                var enrol = db.Enrollments.Include(x => x.EnrolledSubjectArchive).Include(x => x.Session).FirstOrDefault(x => x.StudentProfileId == profile.StudentProfileId && x.SessionId == session.Id);
                try
                {
                    if (enrol == null)
                    {
                        subname.ToString();
                    }
                    else
                    {
                        subname = enrol.AverageScore.ToString();
                    }
                    //subname = enrol.AverageScore.ToString();
                }
                catch (Exception s)
                {

                }
                if (subname == "0")
                {
                    subname = "-";
                }
                if (String.IsNullOrEmpty(subname))
                {
                    subname = "-";
                }
            }
            return subname;

        }

        public static string ThirdTermArchiveCum(int enrId, string sessionyear)
        {
            string subname = "0";
            using (var db = new ApplicationDbContext())
            {
                var session = db.Sessions.FirstOrDefault(x => x.SessionYear == sessionyear && x.Term.ToLower() == "third");

                var profile = db.Enrollments.Include(x => x.StudentProfile).FirstOrDefault(x => x.Id == enrId);
                var enrol = db.Enrollments.Include(x => x.EnrolledSubjectArchive).Include(x => x.Session).FirstOrDefault(x => x.StudentProfileId == profile.StudentProfileId && x.SessionId == session.Id);
                try
                {
                    if (enrol == null)
                    {
                        subname.ToString();
                    }
                    else
                    {
                        subname = enrol.AverageScore.ToString();
                    }
                    //subname = enrol.AverageScore.ToString();
                }
                catch (Exception s)
                {

                }
                if (subname == "0")
                {
                    subname = "-";
                }
                if (String.IsNullOrEmpty(subname))
                {
                    subname = "-";
                }

            }
            return subname;

        }
        ////
        ///
        ///


        public static decimal? TotalArchiveScore(int enrId)
        {
            decimal? no = 0;
            using (var db = new ApplicationDbContext())
            {
                no = db.EnrolledSubjectArchive.Include(x => x.Enrollments).Where(x => x.EnrollmentId == enrId && x.IsOffered ==true).OrderBy(x => x.SubjectName).Sum(x => x.TotalScore);

            }
            return no;

        }

        public static bool IsBatchresult()
        {
            bool batch = false;
            using (var db = new ApplicationDbContext())
            {
                var checkBatch = db.Settings.FirstOrDefault().EnableBatchResultPrinting;
                if (checkBatch == true)
                {
                    batch = true;
                }
                else
                {
                    batch = false;
                }

            }
            return batch;
        }


        public static bool IsFinanceEnabled()
        {
            bool finance = false;
            using (var db = new ApplicationDbContext())
            {
                var checkFinance = db.Settings.FirstOrDefault().EnableFinance;
                if (checkFinance == true)
                {
                    finance = true;
                }
                else
                {
                    finance = false;
                }

            }
            return finance;
        }

        public static bool IsHostelEnabled()
        {
            bool hostel = false;
            using (var db = new ApplicationDbContext())
            {
                var checkHostel = db.Settings.FirstOrDefault().EnableHostel;
                if (checkHostel == true)
                {
                    hostel = true;
                }
                else
                {
                    hostel = false;
                }

            }
            return hostel;
        }


        //Print Option One
        public static bool IsPrintOutOne()
        {
            bool print = false;
            using (var db = new ApplicationDbContext())
            {
                var checkPrint = db.Settings.FirstOrDefault().PrintOutOption;
                if (checkPrint == PrintOutOption.PrintOutOne)
                {
                    print = true;
                }
                else
                {
                    print = false;
                }

            }
            return print;
        }


        //Print Option Two
        public static bool IsPrintOutTwo()
        {
            bool print = false;
            using (var db = new ApplicationDbContext())
            {
                var checkPrint = db.Settings.FirstOrDefault().PrintOutOption;
                if (checkPrint == PrintOutOption.PrintOutTwo)
                {
                    print = true;
                }
                else
                {
                    print = false;
                }

            }
            return print;
        }


        public static bool IsPrintOutThree()
        {
            bool print = false;
            using (var db = new ApplicationDbContext())
            {
                var checkPrint = db.Settings.FirstOrDefault().PrintOutOption;
                if (checkPrint == PrintOutOption.PrintOutThree)
                {
                    print = true;
                }
                else
                {
                    print = false;
                }

            }
            return print;
        }

        //public string UserRole()
        //{
        //    string a = "";
        //    string r;
        //    string userid = HttpContext.Current.User.Identity.GetUserId();
        //    var user = UserManager.FindById(userid).Roles;
        //   foreach(var ss in user)
        //    {
        //        using (var db = new ApplicationDbContext())
        //        {
        //            var roId = ss.RoleId;
        //            var dROl = db.Roles.FirstOrDefault(x => x.Id == roId);
        //             r = dROl.Name;
        //        }
        //           a = a + "," + r;
        //    }

        //    return a;
        //}

        //public static string UserRolesName

        public static string Currentsession()
        {
            string i = "None";
            using (var db = new ApplicationDbContext())
            {
                var currentSession = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);

                if (currentSession != null)
                {
                    i = currentSession.SessionYear + " " + currentSession.Term;
                }
            };
            return i;
        }

        public static string SchoolIcon()
        {
            string i;
            using (var db = new ApplicationDbContext())
            {
                var item = db.Settings.FirstOrDefault();

                var img = db.ImageModel.FirstOrDefault(x => x.Id == item.ImageId);

                i = img.ImageContent.ToString();

            };
            return i;
        }

        public static string SchoolName()
        {
            string i = "None";
            using (var db = new ApplicationDbContext())
            {
                var currentSession = db.Settings.FirstOrDefault();

                if (currentSession != null)
                {
                    i = currentSession.SchoolName;
                }
            };
            return i;
        }



        public static string Analytics()
        {
            string i = "None";
            using (var db = new ApplicationDbContext())
            {
                var currentSession = db.Settings.FirstOrDefault();

                if (currentSession != null)
                {
                    i = currentSession.GoogleAnalytics;
                }
            };
            return i;
        }

        public static string Quote()
        {
            string i = "None";
            using (var db = new ApplicationDbContext())
            {
                var q = db.Quotes.FirstOrDefault();

                if (q != null)
                {
                    i = q.QuoteOfTheDay;
                }
            };
            return i;
        }

        public static string WebsiteLink()
        {
            string i = "None";
            using (var db = new ApplicationDbContext())
            {
                var q = db.Settings.FirstOrDefault();

                if (q != null)
                {
                    i = q.WebsiteLink;
                }
            };
            return i;
        }

        public static string PortalLink()
        {
            string i = "None";
            using (var db = new ApplicationDbContext())
            {
                var q = db.Settings.FirstOrDefault();

                if (q != null)
                {
                    i = q.PortalLink;
                }
            };
            return i;
        }


        public static string FullNameByEnrolId(int? id)
        {
            string i = "None";
            using (var db = new ApplicationDbContext())
            {

                var abc = db.StudentProfiles.Include(x => x.user).FirstOrDefault(x => x.Id == id);

                if (abc != null)
                {
                    i = abc.user.Surname + " " + abc.user.FirstName + " " + abc.user.OtherName;
                }
            };
            return i;
        }

        public static string RegNumByEnrolId(int? id)
        {
            string i = "None";
            using (var db = new ApplicationDbContext())
            {

                var abc = db.StudentProfiles.Include(x => x.user).FirstOrDefault(x => x.Id == id);

                if (abc != null)
                {
                    i = abc.StudentRegNumber;
                }
            };
            return i;
        }

        public static string RegNumByUserId(string userId)
        {
            string i = "None";
            using (var db = new ApplicationDbContext())
            {
                var abc = db.StudentProfiles.Include(x => x.user).FirstOrDefault(x => x.UserId == userId);

                if (abc != null)
                {
                    i = abc.StudentRegNumber;
                }
            };
            return i;
        }

        public static bool HasTakenAttendance(int? StudentId, int? EnrollmentId)
        {
            bool i = false;
            using (var db = new ApplicationDbContext())
            {
                var item = db.AttendanceDetails.FirstOrDefault(x => x.StudentId == StudentId && x.EnrollmentId == EnrollmentId);

                if (item != null)
                {
                    i = item.IsPresent;
                }
            };

            return i;
        }

        public static string StudentorPupil()
        {
            string i = "Student";
            using (var db = new ApplicationDbContext())
            {
                var abc = db.Settings.FirstOrDefault();

                if (abc.IsPrimaryNursery == true)
                {
                    i = "Pupil";
                }
            };
            return i;
        }
        public static string EnrolmentStatus(int classId, int sessionid, int studentid)
        {
            string i = "false";
            using (var db = new ApplicationDbContext())
            {
                var abc = db.Enrollments.Include(x=>x.ClassLevel).FirstOrDefault(x=>x.SessionId == sessionid && x.StudentProfileId == studentid);

                if (abc != null)
                {
                    i = "Enrolled to " + abc.ClassLevel.ClassName;
                }
            };
            return i;
        }
        public static string HeadteacherOrPrincipal()
        {
            string i = "Principal";
            using (var db = new ApplicationDbContext())
            {

                var abc = db.Settings.FirstOrDefault();

                if (abc.IsPrimaryNursery == true)
                {
                    i = "Head Teacher";
                }
            };
            return i;
        }

        public static string CheckPublish(int sessid, int cid)
        {
            string i = "true";
            using (var db = new ApplicationDbContext())
            {

                var abc = db.PublishResults.Include(x => x.ClassLevel).Include(x => x.Session).FirstOrDefault(x => x.SessionId == sessid && x.ClassLevelId == cid);

                if (abc == null)
                {
                    i = "false";
                }
            };
            return i;
        }

        #region

        public static List<int> ClassList(int cId)
        {

            List<int> c = new List<int>();
            using (var db = new ApplicationDbContext())
            {
                var session = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current).SessionYear;
                var sessionss = db.Sessions.Where(x => x.SessionYear == session).OrderBy(x => x.Id).ToList();
                foreach (var i in sessionss)
                {
                    try
                    {
                        var enro = db.Enrollments.Include(x => x.Session).Include(x => x.ClassLevel).Where(x => x.SessionId == i.Id && x.ClassLevelId == cId).Count();
                        c.Add(enro);
                    }
                    catch (Exception r)
                    {
                        c.Add(0);
                    }

                }

            }
            return c;

        }

        #region promotion for pass and fail that return count and data
        public static int PromotionPass(int cId)
        {

            int c = 0;
            using (var db = new ApplicationDbContext())
            {
                var session = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
                var settings = db.Settings.FirstOrDefault();
                var currntclass = db.ClassLevels.FirstOrDefault(x => x.Id == cId);
                if (settings.PromotionByMathsAndEng == true)
                {
                    int count = 0;
                    var enro = db.Enrollments.Include(x => x.Session).Include(x => x.ClassLevel).Where(x => x.Session.Id == session.Id && x.ClassLevelId == cId);
                    foreach (var t in enro)
                    {
                        string fail = PrintService.PromotionSubject(t.StudentProfileId);
                        if (fail == "(PROMOTED)")
                        {
                            count++;
                        }
                    }
                    c = count;

                }
                else//use passmark
                {
                    try
                    {
                        int count = 0;
                        var enro = db.Enrollments.Include(x => x.Session).Include(x => x.ClassLevel).Where(x => x.Session.Id == session.Id && x.ClassLevelId == cId);
                        count = enro.Where(x => x.CummulativeAverageScore >= currntclass.Passmark).Count();

                        //foreach (var t in enro)
                        //{
                        //    if (t >= settings.Passmark)
                        //    {
                        //        count++;
                        //    }
                        //}
                        c = count;
                    }
                    catch (Exception r)
                    {

                    }
                }

            }
            return c;

        }

        public static int PromotionFail(int cId)
        {

            int c = 0;
            using (var db = new ApplicationDbContext())
            {
                var session = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
                var settings = db.Settings.FirstOrDefault();
                var currntclass = db.ClassLevels.FirstOrDefault(x => x.Id == cId);
                if (settings.PromotionByMathsAndEng == true)
                {

                    int count = 0;
                    var enro = db.Enrollments.Include(x => x.Session).Include(x => x.ClassLevel).Where(x => x.Session.Id == session.Id && x.ClassLevelId == cId);
                    foreach (var t in enro)
                    {
                        string fail = PrintService.PromotionSubject(t.StudentProfileId);
                        if (fail == "(NOT PROMOTED)")
                        {
                            count++;
                        }
                    }

                    c = count;
                }
                else//use passmark
                {
                    try
                    {
                        int count = 0;
                        var enro = db.Enrollments.Include(x => x.Session).Include(x => x.ClassLevel).Where(x => x.Session.Id == session.Id && x.ClassLevelId == cId);
                        count = enro.Where(x => x.CummulativeAverageScore < currntclass.Passmark).Count();

                        //foreach (var t in enro)
                        //{
                        //    if (t >= settings.Passmark)
                        //    {
                        //        count++;
                        //    }
                        //}
                        c = count;
                    }
                    catch (Exception r)
                    {

                    }
                }

            }
            return c;

        }


        //pass that return data

        public static List<StudentProfile> PromotionPassData(int cId)
        {
            List<StudentProfile> c = new List<StudentProfile>();


            using (var db = new ApplicationDbContext())
            {
                var session = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
                var settings = db.Settings.FirstOrDefault();
                var currntclass = db.ClassLevels.FirstOrDefault(x => x.Id == cId);
                if (settings.PromotionByMathsAndEng == true)
                {
                    int count = 0;
                    var enro = db.Enrollments.Include(x => x.StudentProfile).Include(x => x.Session).Include(x => x.ClassLevel).Where(x => x.Session.Id == session.Id && x.ClassLevelId == cId);
                    foreach (var t in enro)
                    {
                        string fail = PrintService.PromotionSubject(t.StudentProfileId);
                        if (fail == "(PROMOTED)")
                        {
                            count++;
                            c.Add(t.StudentProfile);
                        }
                    }


                }
                else//use passmark
                {
                    try
                    {
                        int count = 0;
                        var enro = db.Enrollments.Include(x => x.Session).Include(x => x.ClassLevel).Where(x => x.Session.Id == session.Id && x.ClassLevelId == cId);
                        var en = enro.Where(x => x.CummulativeAverageScore >= currntclass.Passmark).ToList();
                        foreach (var i in en)
                        {
                            c.Add(i.StudentProfile);
                        }

                    }
                    catch (Exception r)
                    {

                    }
                }

            }
            return c;

        }

        public static List<StudentProfile> PromotionFailData(int cId)
        {

            List<StudentProfile> c = new List<StudentProfile>();


            using (var db = new ApplicationDbContext())
            {
                var session = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
                var settings = db.Settings.FirstOrDefault();
                var currntclass = db.ClassLevels.FirstOrDefault(x => x.Id == cId);
                if (settings.PromotionByMathsAndEng == true)
                {

                    int count = 0;
                    var enro = db.Enrollments.Include(x => x.StudentProfile).Include(x => x.Session).Include(x => x.ClassLevel).Where(x => x.Session.Id == session.Id && x.ClassLevelId == cId);
                    foreach (var t in enro)
                    {
                        string fail = PrintService.PromotionSubject(t.StudentProfileId);
                        if (fail == "(NOT PROMOTED)")
                        {
                            count++;
                            c.Add(t.StudentProfile);
                        }
                    }


                }
                else//use passmark
                {
                    try
                    {
                        int count = 0;
                        var enro = db.Enrollments.Include(x => x.Session).Include(x => x.ClassLevel).Where(x => x.Session.Id == session.Id && x.ClassLevelId == cId);
                        var en = enro.Where(x => x.CummulativeAverageScore >= currntclass.Passmark).ToList();
                        foreach (var i in en)
                        {
                            c.Add(i.StudentProfile);
                        }
                    }
                    catch (Exception r)
                    {

                    }
                }

            }
            return c;

        }

        public static int PromotionNull(int cId)
        {

            int c = 0;
            using (var db = new ApplicationDbContext())
            {
                var session = db.Sessions.FirstOrDefault(x => x.Status == SessionStatus.Current);
                var settings = db.Settings.FirstOrDefault();
                if (settings.PromotionByMathsAndEng == true)
                {

                    int count = 0;
                    var enro = db.Enrollments.Include(x => x.Session).Include(x => x.ClassLevel).Where(x => x.Session.Id == session.Id && x.ClassLevelId == cId);
                    foreach (var t in enro)
                    {
                        string fail = PrintService.PromotionSubject(t.StudentProfileId);
                        if (fail == "Null")
                        {
                            count++;
                        }
                    }
                    c = count;
                }
                else//use passmark
                {
                    try
                    {
                        int count = 0;
                        var enro = db.Enrollments.Include(x => x.Session).Include(x => x.ClassLevel).Where(x => x.Session.Id == session.Id && x.ClassLevelId == cId);
                        count = enro.Where(x => x.CummulativeAverageScore == null).Count();

                        //foreach (var t in enro)
                        //{
                        //    if (t >= settings.Passmark)
                        //    {
                        //        count++;
                        //    }
                        //}
                        c = count;
                    }
                    catch (Exception r)
                    {

                    }
                }

            }
            return c;

        }

        #endregion
        public static IEnumerable<SelectListItem> DropdownClasses(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                List<SelectListItem> classes = db.ClassLevels.Where(x => x.Id != id && x.ShowClass == true)
                    .OrderBy(n => n.ClassName)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.Id.ToString(),
                            Text = n.ClassName
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "choose a class"
                };
                classes.Insert(0, countrytip);
                return new SelectList(classes, "Value", "Text");
            }
        }
        #endregion

        #region staff list

        public static string StaffClassList(string userid)
        {
            string firstTable = "<table><tr><td>Subject</td><td>Class</td></tr>";
            string endTable = "</table>";
            string c = "";
            using (var db = new ApplicationDbContext())
            {
                var staff = db.StaffProfiles.Include(x => x.user).FirstOrDefault(x => x.UserId == userid);
                var classes = db.ClassLevels.Where(x => x.UserId == userid && x.ShowClass == true).OrderBy(x => x.ClassName).ToList();
                foreach (var i in classes)
                {
                    try
                    {

                        c = c + "<tr><td>" + i.ClassName + "</td></tr>";

                    }
                    catch (Exception r)
                    {

                    }

                }

            }
            // string allitem = firstTable + c + endTable;
            if (string.IsNullOrEmpty(c))
            {
                c = "<tr><td> None </td></tr>";
            }
            return c;

        }

        public static string StaffSubjectList(string userid)
        {
            string firstTable = "<table><tr><td>Subject</td><td>Class</td></tr>";
            string endTable = "</table>";
            string c = "";
            using (var db = new ApplicationDbContext())
            {
                var staff = db.StaffProfiles.Include(x => x.user).FirstOrDefault(x => x.UserId == userid);
                var sub = db.Subjects.Include(x => x.ClassLevel).Where(x => x.UserId == userid && x.ShowSubject == true && x.ClassLevel.ShowClass == true).OrderBy(x => x.SubjectName).ToList();
                foreach (var i in sub.OrderByDescending(x => x.ClassLevel.ClassName))
                {
                    try
                    {
                        c = c + "<tr><td>" + i.SubjectName + " <strong>in</strong> " + i.ClassLevel.ClassName + "</td></tr>";

                    }
                    catch (Exception r)
                    {

                    }

                }

            }
            //string allitem = firstTable + c + endTable;
            if (string.IsNullOrEmpty(c))
            {
                c = "<tr><td> None </td></tr>";
            }
            return c;

        }

        #endregion
        ///
        public static string ResultBySessionTerm(int id)
        {
            string output = "";
            using (var db = new ApplicationDbContext())
            {
                var enr = db.Enrollments.Include(x => x.StudentProfile.user).Where(x => x.StudentProfile.user.Status == EntityStatus.Active).Where(x => x.StudentProfileId == id).ToList();
                foreach (var i in enr)
                {
                    var abc = "";
                    try
                    {
                        abc = i.Session.SessionYear + "-" + i.Session.Term + "-Av:" + i.AverageScore + "-cu:" + i.CummulativeAverageScore + "<br/>";

                    }
                    catch (Exception v)
                    {

                    }
                    output = String.Join(", ", abc);
                }
            }
            return output;
        }

        public static string MasterListRemark(int id)
        {
            string output = "";
            using (var db = new ApplicationDbContext())
            {
                var enr = db.Enrollments.Include(x => x.ClassLevel).Include(x => x.StudentProfile).FirstOrDefault(x => x.Id == id);
                if (enr.CummulativeAverageScore == enr.ClassLevel.Passmark)
                {
                    output = "Pass";
                }
                else if (enr.CummulativeAverageScore > enr.ClassLevel.Passmark)
                {
                    output = "Pass";
                }
                else
                {
                    output = "Fail";
                }



                //if (enr.CummulativeAverageScore < enr.ClassLevel.Passmark)
                //{
                //    output = "FAIL";
                //}

                //else if (enr.CummulativeAverageScore > enr.ClassLevel.Passmark)
                //{
                //    output = "PASS";
                //}

                //else if (enr.CummulativeAverageScore == enr.ClassLevel.Passmark)
                //{
                //    output = "PASS";
                //}


                //else if (enr.CummulativeAverageScore <= 49.00m && enr.CummulativeAverageScore >= 40.00m)
                //{
                //    output = "PASS";
                //}

                //else if (enr.CummulativeAverageScore <= 49.99m && enr.CummulativeAverageScore >= 40.99m)
                //{
                //    output = "PASS";
                //}

                //else if (enr.CummulativeAverageScore <= 59.00m && enr.CummulativeAverageScore >= 50.00m)
                //{
                //    output = "PASS";
                //}
                //else if (enr.CummulativeAverageScore <= 59.99m && enr.CummulativeAverageScore >= 55.99m)
                //{
                //    output = "PASS";
                //}
                //else if (enr.CummulativeAverageScore <= 64.00m && enr.CummulativeAverageScore >= 60.00m)
                //{
                //    output = "GOOD";
                //}
                //else if (enr.CummulativeAverageScore <= 64.99m && enr.CummulativeAverageScore >= 60.99m)
                //{
                //    output = "GOOD";
                //}
                //else if (enr.CummulativeAverageScore <= 69.00m && enr.CummulativeAverageScore >= 65.00m)
                //{
                //    output = "GOOD";
                //}
                //else if (enr.CummulativeAverageScore <= 69.99m && enr.CummulativeAverageScore >= 65.99m)
                //{
                //    output = "GOOD";
                //}
                //else if (enr.CummulativeAverageScore <= 73.00m && enr.CummulativeAverageScore >= 70.00m)
                //{
                //    output = "VERY GOOD";
                //}
                //else if (enr.CummulativeAverageScore <= 73.99m && enr.CummulativeAverageScore >= 70.99m)
                //{
                //    output = "VERY GOOD";
                //}
                //else if (enr.CummulativeAverageScore <= 100 && enr.CummulativeAverageScore >= 74.00m)
                //{
                //    output = "EXCELLENT";

                //}
                //else if (enr.CummulativeAverageScore <= 100 && enr.CummulativeAverageScore >= 74.99m)
                //{
                //    output = "EXCELLENT";

                //}



            }
            return output;
        }


        public static string MasterListRemark2(int id)
        {
            string output = "";
            using (var db = new ApplicationDbContext())
            {
                var enr = db.Enrollments.Include(x => x.ClassLevel).Include(x => x.StudentProfile).FirstOrDefault(x => x.Id == id);
                if (enr.AverageScore == enr.ClassLevel.Passmark)
                {
                    output = "Pass";
                }
                else if (enr.AverageScore > enr.ClassLevel.Passmark)
                {
                    output = "Pass";
                }
                else
                {
                    output = "Fail";
                }



            }
            return output;
        }

        public static int CheckCatch()
        {
            int i = 0;
            using (var db = new ApplicationDbContext())
            {

                var abc = db.Trackers.Count();
                i = abc;
            };
            return i;
        }


        public static bool CheckModal()
        {
            bool i = true;
            using (var db = new ApplicationDbContext())
            {

                var abc = db.Notifications.FirstOrDefault().ShowModal;

                if (abc == false)
                {
                    i = false;
                }
            };
            return i;
        }

        public static bool CheckMarque()
        {
            bool i = true;
            using (var db = new ApplicationDbContext())
            {

                var abc = db.Notifications.FirstOrDefault().ShowMarque;

                if (abc == false)
                {
                    i = false;
                }
            };
            return i;
        }

        public static string NotificationTitle()
        {
            string i = "";
            using (var db = new ApplicationDbContext())
            {

                var abc = db.Notifications.FirstOrDefault();

                if (abc == null)
                {
                    i = "";
                }
                else
                {
                    i = abc.Title;
                }
            };
            return i;
        }

        public static string NotificationMessage()
        {
            string i = "";
            using (var db = new ApplicationDbContext())
            {

                var abc = db.Notifications.FirstOrDefault();

                if (abc == null)
                {
                    i = "";
                }
                else
                {
                    i = abc.Message;
                }
            };
            return i;
        }

        public static bool EnabledTestScore()
        {
            bool i = true;
            using (var db = new ApplicationDbContext())
            {

                var abc = db.Settings.FirstOrDefault().EnableTestScore;

                if (abc == false)
                {
                    i = false;
                }
            };
            return i;
        }

        public static bool EnabledTestScore2()
        {
            bool i = true;
            using (var db = new ApplicationDbContext())
            {

                var abc = db.Settings.FirstOrDefault().EnableTestScore2;

                if (abc == false)
                {
                    i = false;
                }
            };
            return i;
        }

        public static bool EnabledExamScore()
        {
            bool i = true;
            using (var db = new ApplicationDbContext())
            {

                var abc = db.Settings.FirstOrDefault().EnableExamScore;

                if (abc == false)
                {
                    i = false;
                }
            };
            return i;
        }

        public static bool EnabledProjectScore()
        {
            bool i = true;
            using (var db = new ApplicationDbContext())
            {

                var abc = db.Settings.FirstOrDefault().EnableProject;

                if (abc == false)
                {
                    i = false;
                }
            };
            return i;
        }

        public static bool EnabledClassExerciseScore()
        {
            bool i = true;
            using (var db = new ApplicationDbContext())
            {

                var abc = db.Settings.FirstOrDefault().EnableClassExercise;

                if (abc == false)
                {
                    i = false;
                }
            };
            return i;
        }

        public static bool EnabledAssessmentScore()
        {
            bool i = true;
            using (var db = new ApplicationDbContext())
            {

                var abc = db.Settings.FirstOrDefault().EnableAssessment;

                if (abc == false)
                {
                    i = false;
                }
            };
            return i;
        }
    }
}