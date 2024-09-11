using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace SchoolPortal.Web.Models
{
    public class HelperClass
    {

        public async static Task<string> SendSMS(string messages, string phonenumber)
        {
            messages = messages.Replace("0", "O");
            string response = "";
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    var getApi = "http://account.kudisms.net/api/?username=ponwuka123@gmail.com&password=sms@123&message=@@message@@&sender=@@sender@@&mobiles=@@recipient@@";
                    string apiSending = getApi.Replace("@@sender@@", "School Payment").Replace("@@recipient@@", HttpUtility.UrlEncode(phonenumber)).Replace("@@message@@", HttpUtility.UrlEncode(messages));

                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiSending);
                    httpWebRequest.Method = "GET";
                    httpWebRequest.ContentType = "application/json";

                    //getting the respounce from the request
                    HttpWebResponse httpWebResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                    Stream responseStream = httpWebResponse.GetResponseStream();
                    StreamReader streamReader = new StreamReader(responseStream);
                    response = await streamReader.ReadToEndAsync();

                }
                catch (Exception c)
                {

                }

                if (response.ToUpper().Contains("OK") || response.ToUpper().Contains("1701"))
                {

                }

                // response = "ok";
                return response;
            }
        }

        //mail
        public async static Task<string> SendMail(string mailaddress, string message)
        {

            string response = "";
            using (var db = new ApplicationDbContext())
            {
                try
                {

                    MailMessage mail = new MailMessage();

                    //set the addresses 
                    mail.From = new MailAddress("noreply@dbenco.com"); //IMPORTANT: This must be same as your smtp authentication address.

                    //
                    //set the content Server.MapPath("~/status.txt")
                    string AppPath = HttpContext.Current.Request.PhysicalApplicationPath;
                    //StreamReader sr = new StreamReader(AppPath + "../Views/Account/HtmlPage1.html");
                    StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/Data/Success.html"));

                    mail.Body = sr.ReadToEnd();
                    sr.Close();

                    MailDefinition md = new MailDefinition();
                    md.From = "noreply@dbenco.com";
                    md.IsBodyHtml = true;
                    md.Subject = "School Portal Payment";

                    //
                    ListDictionary replacements = new ListDictionary();
                    
                    replacements.Add("{activate}", message);
                    replacements.Add("{smalltitle}", "Payment");

                    mail = md.CreateMailMessage(mailaddress, replacements, mail.Body, new System.Web.UI.Control());



                    //send the message 
                    SmtpClient smtp = new SmtpClient("mail.dbenco.com");

                    //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
                    NetworkCredential Credentials = new NetworkCredential("noreply@dbenco.com", "admin@123");
                    smtp.Credentials = Credentials;
                    smtp.Send(mail);
                }
                catch (Exception ex)
                {

                    //TempData["mssg"] = "Mail not Sent. Try Again.";
                }
                // response = "ok";
                return response;
            }
        }

    }
}