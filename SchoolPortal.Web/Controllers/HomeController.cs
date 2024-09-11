using Microsoft.AspNet.Identity.Owin;
using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Net;
using System.Runtime.InteropServices;
using System.Web.UI.WebControls;
using System.Net.NetworkInformation;
using System.Data;
using System.Data.Entity;

namespace SchoolPortal.Web.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;

        private ApplicationUserManager _userManager;
        private IUserManagerService _userService = new UserManagerService();

        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, UserManagerService userService)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            _userService = userService;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int DestIP, int SrcIP, [Out] byte[] pMacAddr, ref int PhyAddrLen);
        public string MacAddress2()
        {

            IPAddress[] TempAd = Dns.GetHostEntry(Server.MachineName).AddressList;
            String sMacAddress = string.Empty;
            foreach (IPAddress a in TempAd)
            {
                if (a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    byte[] ab = new byte[6];

                    int len = ab.Length;

                    SendARP((int)TempAd[1].Address, 0, ab, ref len);

                    sMacAddress = BitConverter.ToString(ab, 0, 6);
                }
            }
            return sMacAddress;
        }


        
        public async Task<ActionResult> Index()
        {
           var user = db.Users;
            string userid = User.Identity.GetUserId();
            if (user.Count() < 1)
            {
                return RedirectToAction("CreateAccount", "AdminUser", new { area = "SuperUser" });
            }



            else
            {

                if (User.Identity.IsAuthenticated)
                {
                    if (await SignInManager.UserManager.IsInRoleAsync(userid, "SuperAdmin"))
                    {

                        return RedirectToAction("Index", "DashBoard", new { area = "Admin" });
                    }

                    if (await SignInManager.UserManager.IsInRoleAsync(userid, "Admin") || await SignInManager.UserManager.IsInRoleAsync(userid, "Read Only"))
                    {
                        return RedirectToAction("Index", "DashBoard", new { area = "Admin" });
                    }
                    else if (await UserManager.IsInRoleAsync(userid, "Student"))
                    {
                        return RedirectToAction("Index", "Panel", new { area = "Student" });
                    }
                    else if (await UserManager.IsInRoleAsync(userid, "Staff"))
                    {
                        return RedirectToAction("Index", "Panel", new { area = "Staff" });
                    }
                    else
                    {

                        return RedirectToAction("Login", "XYZ", new { area = "Account" });
                    }
                }
            }
            return RedirectToAction("Login", "XYZ", new { area = "Account" });
        }

    }
}