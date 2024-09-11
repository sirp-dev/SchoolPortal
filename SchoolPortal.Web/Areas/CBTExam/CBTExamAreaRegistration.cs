using System.Web.Mvc;

namespace SchoolPortal.Web.Areas.CBTExam
{
    public class CBTExamAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CBTExam";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "CBTExam_default",
                "CBTExam/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}