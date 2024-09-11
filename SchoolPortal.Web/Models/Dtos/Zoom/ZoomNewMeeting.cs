using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos.Zoom
{
    //public class ZoomNewMeeting
    //{
    //}

    public class RootobjectNewMeeting
    {
        public string topic { get; set; }
        public string type { get; set; }
        public string start_time { get; set; }
        public string duration { get; set; }
        public string schedule_for { get; set; }
        public string timezone { get; set; }
        public string password { get; set; }
        public string agenda { get; set; }
        public Recurrence recurrence { get; set; }
        public Settings settings { get; set; }
    }

    public class Recurrence
    {
        public string type { get; set; }
        public string repeat_interval { get; set; }
        public string weekly_days { get; set; }
        public string monthly_day { get; set; }
        public string monthly_week { get; set; }
        public string monthly_week_day { get; set; }
        public string end_times { get; set; }
        public string end_date_time { get; set; }
    }

    public class Settings
    {
        public string host_video { get; set; }
        public string participant_video { get; set; }
        public string cn_meeting { get; set; }
        public string in_meeting { get; set; }
        public string join_before_host { get; set; }
        public string mute_upon_entry { get; set; }
        public string watermark { get; set; }
        public string use_pmi { get; set; }
        public string approval_type { get; set; }
        public string registration_type { get; set; }
        public string audio { get; set; }
        public string auto_recording { get; set; }
        public string enforce_login { get; set; }
        public string enforce_login_domains { get; set; }
        public string alternative_hosts { get; set; }
        public string[] global_dial_in_countries { get; set; }
        public string registrants_email_notification { get; set; }
    }
}