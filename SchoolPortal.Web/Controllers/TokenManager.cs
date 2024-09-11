using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RestSharp;
using SchoolPortal.Web.Models.Dtos.Zoom;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace SchoolPortal.Web.Controllers
{
    public class TokenManager
    {

        public static string GenerateToken()
        {

            // Token will be good for 20 minutes
            DateTime Expiry = DateTime.UtcNow.AddMinutes(20);

            string ApiKey = "jR-uAymbRaymBCsSAlJlzA";
            string ApiSecret = "ScMbSIE6sdEu0kcmeCri14CjBfamLjzHXUrV";

            int ts = (int)(Expiry - new DateTime(1970, 1, 1)).TotalSeconds;

            // Create Security key  using private key above:
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiSecret));

            // length should be >256b
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Finally create a Token
            var header = new JwtHeader(credentials);

            //Zoom Required Payload
            var payload = new JwtPayload
        {
            { "iss", ApiKey},
            { "exp", ts },
        };

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            // Token to String so you can use it in your client
            var tokenString = handler.WriteToken(secToken);

            return tokenString;
        }

        private static string apiurl = "https://api.zoom.us/v2";

        public static string GetUsers()
        {
            try
            {
                var token = TokenManager.GenerateToken();
                ///v2/accounts/128273138/users/{userId}/meetings
                /////https://api.zoom.us/v2/users
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl + "/users"); var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "Bearer" + token);

                request.AddQueryParameter("status", "active");
                request.AddQueryParameter("page_size", "300");
                request.AddQueryParameter("page_number", "1");
                IRestResponse response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    var contents = response.Content.ToString();
                    return contents;
                }

            }
            catch (Exception c) { }
            return null;
        }

        public static string NewMeeting(RootobjectNewMeeting model, string email)
        {
            try
            {
                var token = TokenManager.GenerateToken();
                ///v2/accounts/128273138/users/{userId}/meetings
                /////https://api.zoom.us/v2/users
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl + "/users/"+ email +"/meetings");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "Bearer" + token);

                var lad = new RootobjectNewMeeting
                {
                    topic = model.topic,
                    type = model.type,
                    start_time = model.start_time,
                    duration = model.duration,
                    schedule_for = "",
                    timezone = "Africa/Bangui",
                    password = model.password,
                    agenda = model.agenda,

                    recurrence = new Recurrence
                    {
                        type = "1",
                        repeat_interval = "1",
                        weekly_days = "",
                        monthly_day = "",
                        monthly_week = "",
                        monthly_week_day = "",
                        end_times = "",
                        end_date_time = "2020-06-19T14:00:00Z",
                    },
                    settings = new Settings
                    {
                        host_video = "true",
                        participant_video = "true",
                        cn_meeting = "true",
                        in_meeting = "true",
                        join_before_host = "false",
                        mute_upon_entry = "true",
                        watermark = "false",
                        use_pmi = "false",
                        approval_type = "2",
                        registration_type = "",
                        audio = "both",
                        auto_recording = "cloud",
                        enforce_login = "false",
                        enforce_login_domains = "",
                        alternative_hosts = "",
                        global_dial_in_countries = new string[] { "", ""},
                        registrants_email_notification = ""
                    }
                };

                request.AddJsonBody(lad);

                IRestResponse response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    var contents = response.Content.ToString();
                    return contents;
                }

            }
            catch (Exception c) { }
            return null;
        }


        public static string GetAMeeting(long id)
        {
            try
            {
                var token = TokenManager.GenerateToken();
                ///v2/accounts/128273138/users/{userId}/meetings
                /////https://api.zoom.us/v2/users
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl + "/meetings/"+id); var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "Bearer" + token);

                IRestResponse response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    var contents = response.Content.ToString();
                    return contents;
                }

            }
            catch (Exception c) { }
            return null;
        }

        public static string GetAMeetingRecording(string id)
        {
            try
            {
                var token = TokenManager.GenerateToken();
                ///v2/accounts/128273138/users/{userId}/meetings
                /////https://api.zoom.us/v2/users
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl + "/meetings/" + id+ "/recordings"); var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "Bearer" + token);

                IRestResponse response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    var contents = response.Content.ToString();
                    return contents;
                }

            }
            catch (Exception c) { }
            return null;
        }

        public static string GetAMeetingParticipant(long id)
        {
            try
            {
                var token = TokenManager.GenerateToken();
                ///v2/accounts/128273138/users/{userId}/meetings
                /////https://api.zoom.us/v2/users
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl + "/metrics/meetings/" + id + "/participants"); 
                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "Bearer" + token);

                request.AddQueryParameter("type", "past");
                request.AddQueryParameter("page_size", "300");

                IRestResponse response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    var contents = response.Content.ToString();
                    return contents;
                }

            }
            catch (Exception c) { }
            return null;
        }

        public static string GetMeetingsByUser()
        {
            try
            {
                var token = TokenManager.GenerateToken();
                ///v2/accounts/128273138/users/{userId}/meetings
                /////https://api.zoom.us/v2/users
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl + "/users/exwhyzeetech@gmail.com/meetings"); var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "Bearer" + token);
                request.AddQueryParameter("status", "active");
                request.AddQueryParameter("page_size", "300");
                request.AddQueryParameter("page_number", "1");
                IRestResponse response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    var contents = response.Content.ToString();
                    return contents;
                }

            }
            catch (Exception c) { }
            return null;
        }

    }
}