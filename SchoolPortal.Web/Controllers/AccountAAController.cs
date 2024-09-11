using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SchoolPortal.Web.Areas.Data.IServices;
using SchoolPortal.Web.Areas.Data.Services;
using SchoolPortal.Web.Models;
using System.Data.Entity;
using System.Data;
using System.Net.NetworkInformation;
using System.Management;
using System.Runtime.InteropServices;
using SchoolPortal.Web.Models.Entities;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Web.Security;
using Google.Authenticator;
using System.Text;
using System.Net.Mail;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SchoolPortal.Web.Controllers
{



    [Authorize]
    public class AccountAAController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IUserManagerService _userService = new UserManagerService();

        public AccountAAController()
        {
        }

        public AccountAAController(ApplicationUserManager userManager, ApplicationRoleManager roleManager, ApplicationSignInManager signInManager, UserManagerService userService)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            _userService = userService;
            RoleManager = roleManager;
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

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
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

        public string GetMAC2()
        {
            string macAddresses = "";

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }
            return macAddresses;
        }

        public string GetMAC()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            return sMacAddress;
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

        [AllowAnonymous]
        public ActionResult FindProfile()
        {
            string message = "";
            bool status = false;

            var sch = db.Settings.FirstOrDefault();
            if (sch != null)
            {
                ViewBag.sch = sch.SchoolName;
            }

            var img = db.ImageModel.FirstOrDefault(x => x.Id == sch.ImageId);
            var bg = db.ImageSlider.Where(x => x.CurrentSlider == true).FirstOrDefault();
            try
            {
                ViewBag.schimage = img.ImageContent;
            }
            catch (Exception c)
            {
                ViewBag.schimage = new byte[0];
            }
            try
            {
                ViewBag.bg = bg.Content;
            }
            catch (Exception c)
            {
                ViewBag.bg = new byte[0];
            }


            //ViewBag.MAC = GetMACAddress();
            ViewBag.macId = GetMAC();
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateData(DataUserRequest model)
        {
            if (ModelState.IsValid)
            {
                model.Date = DateTime.UtcNow.AddHours(1);
                db.DataUserRequests.Add(model);
                await db.SaveChangesAsync();
                var sett = db.Settings.FirstOrDefault();

                string mass = "";
                try
                {

                    MailMessage mail = new MailMessage();

                    //set the addresses 
                    mail.From = new MailAddress("learnonline@iskools.com"); //IMPORTANT: This must be same as your smtp authentication address.
                    mail.To.Add("espErrorMail@exwhyzee.ng");
                    mail.To.Add("iskoolsportal@gmail.com");
                    
                    mail.To.Add("onwukaemeka41@gmail.com");
                    

                    //set the content 

                    mail.Subject = " Request for Login Details " + sett.SchoolName;

                    mass = sett.SchoolName + " - " + sett.PortalLink + "/Admin/DataUserRequests/Details/" + model.Id + " - visit for more info and update";

                    mail.Body = mass;
                    //send the message 
                    SmtpClient smtp = new SmtpClient("mail.iskools.com");

                    //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
                    NetworkCredential Credentials = new NetworkCredential("learnonline@iskools.com", "Exwhyzee@123");
                    smtp.Credentials = Credentials;
                    smtp.Send(mail);

                }
                catch (Exception ex)
                {


                }


                return RedirectToAction("Feedback");
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Feedback()
        {
            string message = "";
            bool status = false;

            var sch = db.Settings.FirstOrDefault();
            if (sch != null)
            {
                ViewBag.sch = sch.SchoolName;
            }
            ViewBag.url = sch.WebsiteLink;
            var img = db.ImageModel.FirstOrDefault(x => x.Id == sch.ImageId);
            var bg = db.ImageSlider.Where(x => x.CurrentSlider == true).FirstOrDefault();
            try
            {
                ViewBag.schimage = img.ImageContent;
            }
            catch (Exception c)
            {
                ViewBag.schimage = new byte[0];
            }
            try
            {
                ViewBag.bg = bg.Content;
            }
            catch (Exception c)
            {
                ViewBag.bg = new byte[0];
            }


            //ViewBag.MAC = GetMACAddress();
            ViewBag.macId = GetMAC();
            return View();
        }
        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]

        public ActionResult Login(string returnUrl)
        {
            return RedirectToAction("Login", "XYZ", new { area = "XYZ" });
        }



        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            var role = new IdentityRole("Edittor");
            var rolee1 = await RoleManager.FindByNameAsync("Edittor");
            if (rolee1 == null)
            {
                await RoleManager.CreateAsync(role);
            }

            var role1 = new IdentityRole("Delettor");
            var rolee2 = await RoleManager.FindByNameAsync("Delettor");
            if (rolee2 == null)
            {
                await RoleManager.CreateAsync(role1);
            }








            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true


            var student = await db.StudentProfiles.Include(x => x.user).Where(x => x.StudentRegNumber == model.Username).FirstOrDefaultAsync();
            if (student != null)
            {
                var studentUser = await db.Users.FirstOrDefaultAsync(x => x.Id == student.UserId);
                var removePassword = UserManager.RemovePassword(studentUser.Id);
                if (removePassword.Succeeded)
                {
                    var AddPassword = UserManager.AddPassword(studentUser.Id, model.Password);
                    if (AddPassword.Succeeded)
                    {
                        await SignInManager.PasswordSignInAsync(studentUser.UserName, model.Password, model.RememberMe, shouldLockout: false);
                        if (await SignInManager.UserManager.IsInRoleAsync(studentUser.Id, "Student"))
                        {
                            return RedirectToAction("Index", "Panel", new { area = "Student" });
                        }
                        else
                        {
                            TempData["error"] = "Try Again or Contact Your Administration";
                            return RedirectToAction("Login", "XYZ", new { area = "" });
                        }
                    }
                }
            }

            var user1 = await UserManager.FindByNameAsync(model.Username);

            if (await UserManager.IsInRoleAsync(user1.Id, "SuperAdmin"))
            {
                if (await UserManager.CheckPasswordAsync(user1, model.Password))
                {

                    return RedirectToAction("Access", new { id = user1.Id });

                }


                //string macAdd = MacAddress2();
                //var macAddress = db.ApprovedDevices.Include(x => x.User).FirstOrDefault(x => x.MacAddress == macAdd);

                //if (macAddress == null)
                //{

                //    TempData["error"] = "Access Denied";
                //    return RedirectToAction("Login", "XYZ", new { area = "" });
                //}
                //else
                //{
                //    var result2 = await SignInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, shouldLockout: false);
                //    if (await SignInManager.UserManager.IsInRoleAsync(user1.Id, "SuperAdmin") && macAddress != null)
                //    {
                //        return RedirectToAction("Index", "DashBoard", new { area = "Admin" });
                //    }



                //}

            }


            var result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, shouldLockout: false);
            var user = await UserManager.FindByNameAsync(model.Username);

            switch (result)
            {
                case SignInStatus.Success:
                    if (user.IsLocked != true)
                    {
                        if (user.Status == Models.Entities.EntityStatus.NotActive)
                        {
                            return RedirectToAction("UserLockout", new { userid = user.Id });
                        }

                        else
                        {



                            if (returnUrl != null)
                            {
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                //if(User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
                                //{
                                //    return RedirectToAction("Index", "DashBoard", new { area = "Admin" });
                                //}
                                //else if (User.IsInRole("Student"))
                                //{
                                //    return RedirectToAction("Index", "Panel", new { area = "Student" });
                                //}
                                //else if (User.IsInRole("Staff"))
                                //{
                                //    return RedirectToAction("Index", "Panel", new { area = "Staff" });
                                //}
                                //else
                                //{
                                //    TempData["error"] = "Try Again";
                                //    return RedirectToAction("Login", "XYZ", new { area = "" });
                                //}

                                if (await SignInManager.UserManager.IsInRoleAsync(user.Id, "Admin") || await SignInManager.UserManager.IsInRoleAsync(user.Id, "Read Only"))
                                {
                                    return RedirectToAction("Index", "DashBoard", new { area = "Admin" });
                                }
                                else if (await SignInManager.UserManager.IsInRoleAsync(user.Id, "Student"))
                                {
                                    return RedirectToAction("Index", "Panel", new { area = "Student" });
                                }
                                else if (await SignInManager.UserManager.IsInRoleAsync(user.Id, "Staff"))
                                {
                                    return RedirectToAction("Index", "Panel", new { area = "Staff" });
                                }
                                else
                                {
                                    TempData["error"] = "Try Again or Contact Your Administration";
                                    return RedirectToAction("Login", "XYZ", new { area = "" });
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("UserLockout", new { userid = user.Id });
                    }

                case SignInStatus.LockedOut:
                    return RedirectToAction("UserLockout", new { userid = user.Id });
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    //ModelState.AddModelError("", "Invalid login attempt.");
                    string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                    TempData["error"] = "incorrect username or password, " + messages;
                    return View(model);
            }

        }

        [AllowAnonymous]
        public async Task<ActionResult> TwoFactorLogin()
        {
            try
            {
                var token = Request["passcode"];
                string username = Request["username"];

                TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                string UserUniqueKey = Session["UserUniqueKey"].ToString();
                bool isValid = tfa.ValidateTwoFactorPIN(UserUniqueKey, token);
                if (isValid)
                {
                    Session["IsValid2Fa"] = true;
                    var user = await UserManager.FindByNameAsync(username);
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    return RedirectToAction("Index", "DashBoard", new { area = "Admin" });
                }
            }
            catch (Exception k)
            {
                TempData["err"] = "Erro! try again";
            }

            return RedirectToAction("Access", "XYZ");
        }

        [AllowAnonymous]
        public ActionResult Access(string id)
        {
            var sch = db.Settings.FirstOrDefault();
            if (sch != null)
            {
                ViewBag.sch = sch.SchoolName;
            }

            var img = db.ImageModel.FirstOrDefault(x => x.Id == sch.ImageId);
            var bg = db.ImageSlider.Where(x => x.CurrentSlider == true).FirstOrDefault();
            try
            {
                ViewBag.schimage = img.ImageContent;
            }
            catch (Exception c)
            {
                ViewBag.schimage = new byte[0];
            }
            try
            {
                ViewBag.bg = bg.Content;
            }
            catch (Exception c)
            {
                ViewBag.bg = new byte[0];
            }


            if (String.IsNullOrEmpty(id))
            {
                ViewBag.error = "Invalid. try again";
                ViewBag.idd = id;
                return View();
            }
            try
            {
                string message = "";
                bool status = false;
                var user1 = UserManager.FindById(id);
                status = true;
                message = "2FA Verification";
                Session["Username"] = user1.UserName;
                string UserUniqueKey = user1.UserName + Key;
                Session["UserUniqueKey"] = UserUniqueKey;
                TempData["iname"] = user1.UserName;
                TempData["name"] = user1.UserName;
                TempData["status"] = status;
                ViewBag.Message = message;
            }
            catch (Exception c)
            {
                ViewBag.error = "Invalid. try again";
                ViewBag.idd = id;

            }
            return View();
        }

        public async Task<ActionResult> UserLockout(string userid)
        {

            var staff = await db.StaffProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.UserId == userid);
            if (staff != null)
            {
                ViewBag.user = staff;

            }
            else
            {
                var student = await db.StudentProfiles.Include(x => x.user).FirstOrDefaultAsync(x => x.UserId == userid);
                ViewBag.user = student;

            }
            int imgid = ViewBag.user.ImageId;
            var img = await db.ImageModel.FirstOrDefaultAsync(x => x.Id == imgid);
            ViewBag.img = img.ImageContent;

            return View();
        }
        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "XYZ", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [AllowAnonymous]
        public async Task<ActionResult> ResetUserPassword()
        {
            var u = await _userService.UserAll();
            ViewBag.username = new SelectList(u, "Username", "Username");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetUserPassword(string username, string newPassword)
        {
            var user = await UserManager.FindByNameAsync(username);

            // await  _userManager.AddPasswordAsync(userId, newPassword);
            var removePassword = UserManager.RemovePassword(user.Id);
            if (removePassword.Succeeded)
            {
                //Removed Password Success
                var AddPassword = UserManager.AddPassword(user.Id, newPassword);
                if (AddPassword.Succeeded)
                {
                    //var userm = _userManager.Users.FirstOrDefault(x => x.Id == userId);
                    TempData["password"] = "Password Resset Successful.";
                    return RedirectToAction("ResetUserPassword");
                }
            }
            var u = await _userService.UserAll();
            ViewBag.username = new SelectList(u, "Username", "Username");
            TempData["passworderror"] = "Unable To Resset Password.";
            return RedirectToAction("ResetUserPassword");
        }


        public async Task<ActionResult> ChangePassword()
        {

            return View();
        }

        [HttpPost]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(string newPassword, string oldPassword)
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser user = await UserManager.FindByIdAsync(userId);
            var check = await UserManager.CheckPasswordAsync(user, oldPassword);
            if (check == true)
            {
                try
                {


                    await UserManager.ChangePasswordAsync(userId, oldPassword, newPassword);
                    TempData["password"] = "Password Change Successful.";
                }
                catch (Exception e)
                {
                    TempData["passworderror"] = "Unable To Changed Password.";

                }
                TempData["passworderror"] = "Old Password Incorrect.";
            }


            return RedirectToAction("ChangePassword", new { userId = userId });
        }
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "XYZ", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "XYZ");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "XYZ");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "XYZ");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        ////[ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "XYZ", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        ////[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "XYZ");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }
        private const string Key = "ASDFGHKLqaz123!@@()*";
        [AllowAnonymous]
        public ActionResult A2F()
        {
            string uid = "SuperAdmin";

            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            string bob = uid + Key;
            byte[] bytes = Encoding.ASCII.GetBytes(bob);
            byte[] UserUniqueKey = bytes;
            Session["UserUniqueKey"] = UserUniqueKey;
            //var setupInfo = tfa.GenerateSetupCode("HeadWay Capital Investment", user.UserName, UserUniqueKey, 300, true);
            var setupInfo = tfa.GenerateSetupCode(uid, "ISKOOL EXWHYZEE ADMIN", UserUniqueKey, 300, true);
            ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
            ViewBag.SetupCode = setupInfo.ManualEntryKey;

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}