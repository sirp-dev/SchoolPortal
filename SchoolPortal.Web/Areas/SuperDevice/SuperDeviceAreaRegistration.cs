using System.Web.Mvc;

namespace SchoolPortal.Web.Areas.SuperDevice
{
    public class SuperDeviceAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SuperDevice";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SuperDevice_default",
                "SuperDevice/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}