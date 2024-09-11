using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SchoolPortal.Web.Startup))]
namespace SchoolPortal.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

          
        }
    }
}
