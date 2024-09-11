using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos.Zoom
{


    public class RootobjectMeetingRecord
    {
        public string uuid { get; set; }
        public long id { get; set; }
        public string account_id { get; set; }
        public string host_id { get; set; }
        public string topic { get; set; }
        public int type { get; set; }
        public DateTime start_time { get; set; }
        public string timezone { get; set; }
        public int duration { get; set; }
        public int total_size { get; set; }
        public int recording_count { get; set; }
        public string share_url { get; set; }
        public Recording_Files[] recording_files { get; set; }
    }

    public class Recording_Files
    {
        public string id { get; set; }
        public string meeting_id { get; set; }
        public DateTime recording_start { get; set; }
        public DateTime recording_end { get; set; }
        public string file_type { get; set; }
        public int file_size { get; set; }
        public string play_url { get; set; }
        public string download_url { get; set; }
        public string status { get; set; }
        public string recording_type { get; set; }
    }

}