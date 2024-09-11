using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos.Zoom
{

}
public class RootobjectMeetingList
{
    public int page_count { get; set; }
    public int page_number { get; set; }
    public int page_size { get; set; }
    public int total_records { get; set; }
    public Meeting[] meetings { get; set; }
}

public class Meeting
{
    public string uuid { get; set; }
    public long id { get; set; }
    public string host_id { get; set; }
    public string topic { get; set; }
    public int type { get; set; }
    public DateTime start_time { get; set; }
    public int duration { get; set; }
    public string timezone { get; set; }
    public string agenda { get; set; }
    public DateTime created_at { get; set; }
    public string join_url { get; set; }
}
