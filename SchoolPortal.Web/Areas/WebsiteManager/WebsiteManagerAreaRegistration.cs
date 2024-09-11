using System.Web.Mvc;

namespace SchoolPortal.Web.Areas.WebsiteManager
{
    public class WebsiteManagerAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "WebsiteManager";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "WebsiteManager_default",
                "WebsiteManager/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}