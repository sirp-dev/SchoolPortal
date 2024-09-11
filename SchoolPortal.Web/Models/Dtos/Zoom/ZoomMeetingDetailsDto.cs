using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos.Zoom
{

    public class RootobjectDetails
    {
        public string uuid { get; set; }
        public long id { get; set; }
        public string host_id { get; set; }
        public string topic { get; set; }
        public int type { get; set; }
        public string status { get; set; }
        public DateTime start_time { get; set; }
        public int duration { get; set; }
        public string timezone { get; set; }
        public string agenda { get; set; }
        public DateTime created_at { get; set; }
        public string start_url { get; set; }
        public string join_url { get; set; }
        public string password { get; set; }
        public string h323_password { get; set; }
        public string pstn_password { get; set; }
        public string encrypted_password { get; set; }
        public SettingsDetails settings { get; set; }
    }

    public class SettingsDetails
    {
        public bool host_video { get; set; }
        public bool participant_video { get; set; }
        public bool cn_meeting { get; set; }
        public bool in_meeting { get; set; }
        public bool join_before_host { get; set; }
        public bool mute_upon_entry { get; set; }
        public bool watermark { get; set; }
        public bool use_pmi { get; set; }
        public int approval_type { get; set; }
        public string audio { get; set; }
        public string auto_recording { get; set; }
        public bool enforce_login { get; set; }
        public string enforce_login_domains { get; set; }
        public string alternative_hosts { get; set; }
        public bool close_registration { get; set; }
        public bool registrants_confirmation_email { get; set; }
        public bool waiting_room { get; set; }
        public string[] global_dial_in_countries { get; set; }
        public Global_Dial_In_Numbers[] global_dial_in_numbers { get; set; }
        public bool registrants_email_notification { get; set; }
        public bool meeting_authentication { get; set; }
    }

    public class Global_Dial_In_Numbers
    {
        public string country_name { get; set; }
        public string city { get; set; }
        public string number { get; set; }
        public string type { get; set; }
        public string country { get; set; }
    }

}