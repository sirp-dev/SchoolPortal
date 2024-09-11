using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos.Zoom
{
    //public class ZoomMeetingParticipant
    /// {
    //}


    public class RootobjectParticipant
    {
        public int page_count { get; set; }
        public int page_size { get; set; }
        public int total_records { get; set; }
        public string next_page_token { get; set; }
        public Participant[] participants { get; set; }
    }

    public class Participant
    {
        public string id { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string device { get; set; }
        public string ip_address { get; set; }
        public string location { get; set; }
        public string network_type { get; set; }
        public string microphone { get; set; }
        public string speaker { get; set; }
        public string data_center { get; set; }
        public string connection_type { get; set; }
        public DateTime join_time { get; set; }
        public DateTime leave_time { get; set; }
        public bool share_application { get; set; }
        public bool share_desktop { get; set; }
        public bool share_whiteboard { get; set; }
        public bool recording { get; set; }
        public string pc_name { get; set; }
        public string domain { get; set; }
        public string mac_addr { get; set; }
        public string harddisk_id { get; set; }
        public string version { get; set; }
        public string leave_reason { get; set; }
    }

}