using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Dtos.Zoom
{
    public class OnlineZoomDto
    {
        public string ClassDate { get; set; }
        public string ClassTime { get; set; }
        public string Duration1 { get; set; }
        public string UserId1 { get; set; }
        public int ClassLevelId1 { get; set; }
        public int? SubjectId1 { get; set; }
        public string Description1 { get; set; }
        public string ClassPassword1 { get; set; }

        //
       
        public string Duration2 { get; set; }
        public string UserId2 { get; set; }
        public int ClassLevelId2 { get; set; }
        public int? SubjectId2 { get; set; }
        public string Description2 { get; set; }
        public string ClassPassword2 { get; set; }

        //
      
        public string Duration3 { get; set; }
        public string UserId3 { get; set; }
        public int ClassLevelId3 { get; set; }
        public int? SubjectId3 { get; set; }
        public string Description3 { get; set; }
        public string ClassPassword3 { get; set; }

    }
}