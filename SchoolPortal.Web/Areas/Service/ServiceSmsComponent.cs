using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace SchoolPortal.Web.Areas.Service
{
    public class ServiceSmsComponent
    {
        public string SendSMS(string senderUserName, string senderPassword, string senderId, string recipients, string message, string sendOption, string scheduleDate)
        {
            // byte[] buffer = Encoding.Default.GetBytes(message);
            string urlString = "http://xyzsms.com/api/ApiXyzSms/ComposeMessage?username="+senderUserName+"&password="+senderPassword+"&recipients="+recipients+"&senderId="+senderId+"&smsmessage="+message+"&smssendoption="+sendOption;
            //  string urlString = "http://www.xyzsms.com/components/com_spc/smsapi.php?username=" + senderUserName + "&password=" + senderPassword + "&sender=" + senderId + "&recipient=" + recipient + "&message=" + message;
            string response = "";
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(urlString);
                httpWebRequest.Method = "GET";
                httpWebRequest.ContentType = "application/json";

                //getting the respounce from the request
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                response = streamReader.ReadToEnd();
            }catch(Exception d)
            {

            }
            return response;
        }

        public void SendSMSArray(string senderUserName, string senderPassword, string senderId, Array recipient, string message)
        {
            byte[] buffer = Encoding.Default.GetBytes(message);
            string urlString = "http://www.xyzsms.com/components/com_spc/smsapi.php?username=" + senderUserName + "&password=" + senderPassword + "&sender=" + senderId + "&recipient=" + recipient + "&message=" + message;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(urlString);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = buffer.Length;

            Stream postData = req.GetRequestStream();
            postData.Write(buffer, 0, buffer.Length);
            postData.Close();
            HttpWebResponse myRes = (HttpWebResponse)req.GetResponse();
            Stream Response = myRes.GetResponseStream();
            StreamReader _Response = new StreamReader(Response);
            string result = _Response.ReadToEnd();
            _Response.Close();



        }
    }
}