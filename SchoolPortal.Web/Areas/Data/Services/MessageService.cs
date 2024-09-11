using SchoolPortal.Web.Areas.Data.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Data.Entity;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Entities;
using SchoolPortal.Web.Areas.Service;
using System.Net;
using System.IO;

namespace SchoolPortal.Web.Areas.Data.Services
{
    public class MessageService : IMessageService
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        private ServiceSmsComponent smsService = new ServiceSmsComponent();


        #region Old components
        public async Task<string> AccountBalance()
        {
            var settings = await db.Settings.FirstOrDefaultAsync();

            string balance = "";

            var dataString = "username=" + settings.SmsUsername + "&password=" + settings.SmsPassword + "&balance=true";
            var url = "http://www.xyzsms.com/components/com_spc/smsapi.php?" + dataString;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "GET";
            httpWebRequest.ContentType = "application/json";

            //getting the response from the request
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream responseStream = httpWebResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream);
            string response = streamReader.ReadToEnd();
            balance = response.Trim();
            if (balance == "2905")
            {
                balance = "Wrong Credentials";
            }
            return balance;
        }

        public async Task<string> AllParentInClassContact(int[] classLevelId)
        {
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            string numbers = "";
            if (session != null)
            {
                foreach (var item in classLevelId)
                {
                    var enrolledStudents = db.Enrollments.Include(x => x.StudentProfile).Include(x => x.User).Where(c => c.Session.SessionYear == session.SessionYear && c.ClassLevelId == item && c.StudentProfile.ParentGuardianPhoneNumber != null).Select(x => x.StudentProfile.ParentGuardianPhoneNumber);

                    string[] studentNumbers = enrolledStudents.ToArray();
                    numbers = string.Join(",", studentNumbers.ToArray());

                }

            }
            return numbers;
        }

        public async Task<string> AllParentInContact()
        {
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            string numbers = "";
            if (session != null)
            {
                // var parent = db.StudentProfiles.Include(c => c.user).Where(c => c..SessionYear == session.SessionYear && c.ParentGuardianPhoneNumber != null).Select(x => x.ParentGuardianPhoneNumber);
                var enrolledStudentsParent = db.Enrollments.Include(x => x.StudentProfile).Include(x => x.User).Where(c => c.Session.SessionYear == session.SessionYear && c.StudentProfile.ParentGuardianPhoneNumber != null).Select(x => x.StudentProfile.ParentGuardianPhoneNumber);

                string[] studentNumbers = enrolledStudentsParent.ToArray();
                numbers = string.Join(",", studentNumbers.ToArray());
                return numbers;
            }


            return "";
        }

        public async Task<string> AllStaffContact()
        {
            string numbers = "";
            var staff = db.StaffProfiles.Include(c => c.user).Where(c => c.user.Phone != null).Select(x => x.user.Phone);
            string[] studentNumbers = staff.ToArray();
            numbers = string.Join(",", studentNumbers.ToArray());
            return numbers;

        }

        public async Task<string> AllStudentInClassContact(int[] classLevelId)
        {
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            string numbers = "";
            if (session != null)
            {
                foreach (var item in classLevelId)
                {
                    var enrolledStudents = db.Enrollments.Include(x => x.StudentProfile).Include(x => x.User).Where(c => c.Session.SessionYear == session.SessionYear && c.ClassLevelId == item && c.StudentProfile.user.Phone != null).Select(x => x.StudentProfile.user.Phone);

                    string[] studentNumbers = enrolledStudents.ToArray();
                    numbers = string.Join(",", studentNumbers.ToArray());

                }
                numbers += numbers;
            }
            return numbers;
        }

        public async Task<string> AllStudentsContact()
        {
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
            string numbers = "";
            if (session != null)
            {
                // var students = System.Web.Security.Roles.GetUsersInRole("student");
                var enrolledStudents = db.Enrollments.Include(x => x.StudentProfile).Include(x => x.User).Where(c => c.Session.SessionYear == session.SessionYear && c.StudentProfile.user.Phone != null).Select(x => x.StudentProfile.user.Phone);

                string[] studentNumbers = enrolledStudents.ToArray();
                numbers = string.Join(",", studentNumbers.ToArray());
                return numbers;
            }
            return "";
        }

        public async Task<SmsReport> GetMessage(int id)
        {
            var report = await db.SmsReports.FirstOrDefaultAsync(x => x.Id == id);
            return report;
        }


        public async Task<List<SmsReport>> MessageHistory()
        {
            var msg = db.SmsReports.OrderByDescending(x => x.DateSent);
            return await msg.ToListAsync();
        }

       

        public async Task<Property> SmsProperty()
        {
            var item = await db.Properties.FirstOrDefaultAsync();
            return item;
        }

        #endregion

        #region new services

        public async Task<string> SendSms(string SenderId, string Recipients, string Message, string SendOption, string ScheduleDate)
        {
            var response = "";
            var settings = await db.Settings.FirstOrDefaultAsync();
            try
            {
                 response = smsService.SendSMS(settings.SmsUsername, settings.SmsPassword, SenderId, Recipients, Message, SendOption, ScheduleDate);
            }catch(Exception c)
            {

            }
            return response;

        }
        public async Task<string> GetPhoneNumbers(string category, string ClassSend)
        {
            string numbers = "";
            try
            {


                var session = await db.Sessions.FirstOrDefaultAsync(x => x.Status == SessionStatus.Current);
               
                if (category == "SendToAllStudent")
                {
                    var studentsnumber = await db.Enrollments.Include(x => x.StudentProfile.user).Where(x => x.SessionId == session.Id).Select(x => x.StudentProfile.user.Phone).ToListAsync();

                    string[] studentNumbers = studentsnumber.ToArray();
                    numbers = string.Join(",", studentNumbers.ToArray());
                }
                else if (ClassSend == "Sendtostudent")
                {
                    int classid = Convert.ToInt32(category);
                    var studentsnumber = await db.Enrollments.Include(x => x.StudentProfile.user).Include(x => x.ClassLevel).Where(x => x.SessionId == session.Id && x.ClassLevelId == classid).Select(x => x.StudentProfile.user.Phone).ToListAsync();

                    string[] studentNumbers = studentsnumber.ToArray();
                    numbers = string.Join(",", studentNumbers.ToArray());
                }
                else if (category == "SendToStaff")
                {
                    var staff = await db.StaffProfiles.Include(x => x.user).Select(x => x.user.Phone).ToListAsync();
                    string[] staffNumbers = staff.ToArray();
                    numbers = string.Join(",", staffNumbers.ToArray());
                }
                else if (category == "SendToAllParent")
                {
                    var studentsParentnumber = await db.Enrollments.Include(x => x.StudentProfile.user).Where(x => x.SessionId == session.Id).Select(x => x.StudentProfile.ParentGuardianPhoneNumber).ToListAsync();

                    string[] parentsNumbers = studentsParentnumber.ToArray();
                    numbers = string.Join(",", parentsNumbers.ToArray());
                }
                else if (ClassSend == "SendtoParent")
                {
                    int classid = Convert.ToInt32(category);
                    var studentsnumber = await db.Enrollments.Include(x => x.StudentProfile.user).Include(x => x.ClassLevel).Where(x => x.SessionId == session.Id && x.ClassLevelId == classid).Select(x => x.StudentProfile.ParentGuardianPhoneNumber).ToListAsync();

                    string[] studentNumbers = studentsnumber.ToArray();
                    numbers = string.Join(",", studentNumbers.ToArray());
                }
                else if (category == "")
                {
                    numbers = "No contact found";
                }
            }
            catch (Exception f)
            {
                numbers = "No contact found";
            }
            return numbers;
        }


        #endregion
    }
}