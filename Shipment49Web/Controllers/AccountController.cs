using MenuLayer;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Reports;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Shipment49Web.Controllers
{
    [Authorize]
    [ErrorClass]
    public class AccountController : BaseController
    {
        private readonly ShipmentContaxt emp = new ShipmentContaxt();
        MorringOfficeEntities entities = new MorringOfficeEntities();
        //CommonClass cls = new CommonClass();
        SqlConnection con = ConnectionBulder.con;
        public string emaillID { get; set; }

        //protected ApplicationSignInManager _signInManager;
        //protected ApplicationUserManager _userManager;

        //public ApplicationSignInManager SignInManager
        //{
        //    get
        //    {
        //        return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
        //    }
        //    protected set
        //    {
        //        _signInManager = value;
        //    }
        //}

        //public ApplicationUserManager UserManager
        //{
        //    get
        //    {
        //        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    }
        //    protected set
        //    {
        //        _userManager = value;
        //    }
        //}

        public AccountController()
        {

        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
                       
            return View();
        }

        static int logincounter = 0;
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Obsolete]
        public ActionResult Login(LoginEmp model, string returnUrl)
        {
            logincounter++;
            if ((string.IsNullOrEmpty(model.EmailAddress)) || (string.IsNullOrEmpty(model.Password)))
            {
                ModelState.AddModelError("", "Invalid login.");
                return View(model);
            }
            else
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true

                var username = AESEncrytDecry.DecryptStringAES(model.HId);
                var password = AESEncrytDecry.DecryptStringAES(model.HPass);

                switch (SignInManager.PasswordSignIn(model.EmailAddress, password, true, shouldLockout: true))
                {
                    case SignInStatus.Success:

                        string hostName = Dns.GetHostName(); // Retrive the Name of HOST
                        string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

                        myIP = model.IpAddress;




                        Guid id = Guid.NewGuid();
                        Session["IpAdd"] = id;
                        // Ipadd = id.ToString();
                        myIP = myIP + id;

                        //int status = 0;
                        //using (SqlDataAdapter adp1 = new SqlDataAdapter("select * from autologout where Email='" + model.EmailAddress + "'", con))
                        //{
                        //    DataTable dt1 = new DataTable();
                        //    adp1.Fill(dt1);
                        //    if (dt1.Rows.Count > 0)
                        //    {
                        //        status = Convert.ToInt32(dt1.Rows[0]["Status"]);

                        //        using (SqlDataAdapter adp11 = new SqlDataAdapter("delete from autologout where Email='" + model.EmailAddress + "'", con))
                        //        {
                        //            DataTable dt11 = new DataTable();
                        //            adp11.Fill(dt11);
                        //        }
                        //    }
                        //}

                        CommonClass.CheckEmailAuto(model.EmailAddress);
                        CommonClass.InsertAtuoLogout(model.EmailAddress, myIP);
                        //using (SqlDataAdapter adp = new SqlDataAdapter("INSERT INTO autologout (Email, IP_Address, Status,CreatedDate) VALUES ('" + model.EmailAddress + "', '" + myIP + "', '1','" + DateTime.Now + "')", con))
                        //{
                        //    DataTable dt = new DataTable();
                        //    adp.Fill(dt);
                        //}

                        //emaillID = model.EmailAddress;

                        Session["emailID"]= model.EmailAddress;



                        var userInfo = entities.AspNetUsers.FirstOrDefault(x => x.Email == model.EmailAddress.Trim());
                            userInfo.LockoutEndDateUtc = null;
                        entities.Entry(userInfo).State = EntityState.Modified;
                        entities.SaveChanges();

                        var userDetails = userInfo.UserInfoes.FirstOrDefault();

                        if (userDetails == null)
                        {
                            ModelState.AddModelError("", "Permissions not assigned.");
                            return View(model);
                        }

                        string fullName = userDetails.FullName;

                        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, fullName, DateTime.Now, DateTime.Now.AddMinutes(30),
                            true, userDetails.UserRole, FormsAuthentication.FormsCookiePath);

                        // Encrypt the cookie using the machine key for secure transport
                        string hash = FormsAuthentication.Encrypt(ticket);
                        HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hash);
                        // To Set Secure flag on session id cookie
                        cookie.HttpOnly = true;
                        cookie.Secure = true;

                        // Set the cookie's expiration time to the tickets expiration time
                        if (ticket.IsPersistent) cookie.Expires = ticket.Expiration;

                      
                        // Add the cookie to the list for outgoing response
                        Response.Cookies.Add(cookie);
                        if (Response.Cookies.Count > 0)
                        {
                           // Response.CacheControl = "no-store";
                            foreach (string s in Response.Cookies.AllKeys)
                            {
                                if (s == FormsAuthentication.FormsCookieName || s == "ASP.NET_SessionId" || s.ToLower() == "asp.net_sessionid")
                                {
                                    Response.Cookies[s].Secure = true;
                                }
                            }
                        }

                        var LoggedInUserVesselPermissions = new List<int>();
                        var LoggedInUserFleetNamePermissions = new List<int>();
                        var LoggedInUserFleetTypePermissions = new List<int>();

                        LoggedInUserID = userInfo.Id;
                        LoggedInUserFullName = fullName;
                        LoggedInUserPermissionType = userDetails.PermissionType;
                        LoggedInUserEmail = model.EmailAddress;

                        PermittedVessels = new List<VesselDetail>();
                        PermittedFleetNames = new List<tblCommon>();
                        PermittedFleetTypes = new List<tblCommon>();

                        CommonClass.UserInformation = userDetails;
                        switch (userDetails.UserRole)
                        {
                            case "ADMIN":
                                PermittedVessels = entities.VesselDetails.OrderBy(p => p.VesselName).ToList();
                                PermittedFleetNames = entities.tblCommons.Where(p => p.Type == (int)CommonType.FleetName).ToList();
                                PermittedFleetTypes = entities.tblCommons.Where(p => p.Type == (int)CommonType.FleetType).ToList();
                                break;
                            case "USER":
                                switch (userDetails.PermissionType)
                                {
                                    case "VESSELS":
                                        if (!string.IsNullOrEmpty(userDetails.Vessels))
                                        {
                                            foreach (string v in userDetails.Vessels.Split(','))
                                                LoggedInUserVesselPermissions.Add(Convert.ToInt32(v));

                                            PermittedVessels = entities.VesselDetails.Where(p => LoggedInUserVesselPermissions.Contains(p.ImoNo)).OrderBy(u => u.VesselName).ToList();

                                            #region License Subscription Check                                           
                                            try
                                            {
                                                var dtfrm = entities.VesselDetails.Where(p => LoggedInUserVesselPermissions.Contains(p.ImoNo)).Select(u => u.SubscriptionTo).SingleOrDefault();
                                                string dateto = dtfrm.ToShortDateString();
                                                string todayDate = DateTime.UtcNow.ToShortDateString();
                                                DateTime dt1 = DateTime.Parse(dateto);
                                                DateTime dt2 = DateTime.Now;
                                                if (dt1.Date < dt2.Date)
                                                {
                                                    ModelState.AddModelError("", "Subscription expired.");
                                                    return View(model);
                                                }
                                            }
                                            catch { }
                                            #endregion



                                            foreach (var v in PermittedVessels)
                                            {
                                                if (!LoggedInUserFleetNamePermissions.Contains(v.FleetNameID))
                                                    LoggedInUserFleetNamePermissions.Add(v.FleetNameID);

                                                if (!LoggedInUserFleetTypePermissions.Contains(v.FleetTypeID))
                                                    LoggedInUserFleetTypePermissions.Add(v.FleetTypeID);
                                            }

                                            PermittedFleetNames = entities.tblCommons.Where(p => LoggedInUserFleetNamePermissions.Contains(p.Id)).OrderBy(u => u.Name).ToList();
                                            PermittedFleetTypes = entities.tblCommons.Where(p => LoggedInUserFleetTypePermissions.Contains(p.Id)).OrderBy(u => u.Name).ToList();
                                        }
                                        break;
                                    case "FLEETNAMES":
                                        if (!string.IsNullOrEmpty(userDetails.FleetNames))
                                        {
                                            foreach (string v in userDetails.FleetNames.Split(','))
                                                LoggedInUserFleetNamePermissions.Add(Convert.ToInt32(v));

                                            PermittedVessels = entities.VesselDetails.Where(p => LoggedInUserFleetNamePermissions.Contains(p.FleetNameID)).OrderBy(u => u.VesselName).ToList();

                                            #region License Subscription Check                                           
                                            try
                                            {
                                                var dtfrm = entities.VesselDetails.Where(p => LoggedInUserVesselPermissions.Contains(p.ImoNo)).Select(u => u.SubscriptionTo).SingleOrDefault();
                                                string dateto = dtfrm.ToShortDateString();
                                                string todayDate = DateTime.UtcNow.ToShortDateString();
                                                DateTime dt1 = DateTime.Parse(dateto);
                                                DateTime dt2 = DateTime.Now;
                                                if (dt1.Date < dt2.Date)
                                                {
                                                    ModelState.AddModelError("", "Subscription expired.");
                                                    return View(model);
                                                }
                                            }
                                            catch { }
                                            #endregion

                                            foreach (var v in PermittedVessels)
                                            {
                                                if (!LoggedInUserFleetTypePermissions.Contains(v.FleetTypeID))
                                                    LoggedInUserFleetTypePermissions.Add(v.FleetTypeID);
                                            }

                                            PermittedFleetNames = entities.tblCommons.Where(p => LoggedInUserFleetNamePermissions.Contains(p.Id)).OrderBy(u => u.Name).ToList();
                                            PermittedFleetTypes = entities.tblCommons.Where(p => LoggedInUserFleetTypePermissions.Contains(p.Id)).OrderBy(u => u.Name).ToList();
                                        }
                                        break;
                                    case "FLEETTYPES":
                                        if (!string.IsNullOrEmpty(userDetails.FleetTypes))
                                        {
                                            foreach (string v in userDetails.FleetTypes.Split(','))
                                                LoggedInUserFleetTypePermissions.Add(Convert.ToInt32(v));

                                            PermittedVessels = entities.VesselDetails.Where(p => LoggedInUserFleetTypePermissions.Contains(p.FleetTypeID)).OrderBy(u => u.VesselName).ToList();

                                            #region License Subscription Check                                           
                                            try
                                            {
                                                var dtfrm = entities.VesselDetails.Where(p => LoggedInUserVesselPermissions.Contains(p.ImoNo)).Select(u => u.SubscriptionTo).SingleOrDefault();
                                                string dateto = dtfrm.ToShortDateString();
                                                string todayDate = DateTime.UtcNow.ToShortDateString();
                                                DateTime dt1 = DateTime.Parse(dateto);
                                                DateTime dt2 = DateTime.Now;
                                                if (dt1.Date < dt2.Date)
                                                {
                                                    ModelState.AddModelError("", "Subscription expired.");
                                                    return View(model);
                                                }
                                            }
                                            catch { }
                                            #endregion

                                            foreach (var v in PermittedVessels)
                                            {
                                                if (!LoggedInUserFleetNamePermissions.Contains(v.FleetNameID))
                                                    LoggedInUserFleetNamePermissions.Add(v.FleetNameID);
                                            }

                                            PermittedFleetNames = entities.tblCommons.Where(p => LoggedInUserFleetNamePermissions.Contains(p.Id)).OrderBy(u => u.Name).ToList();
                                            PermittedFleetTypes = entities.tblCommons.Where(p => LoggedInUserFleetTypePermissions.Contains(p.Id)).OrderBy(u => u.Name).ToList();
                                        }
                                        break;
                                }
                                break;
                        }

                       



                        var uscheck = entities.tblCommons.Where(x => x.Id == userDetails.LoginType).Select(x => x.Name).SingleOrDefault();

                        //if (uscheck == "Office")
                        //{
                        //    return RedirectToAction("index", "home");
                        //}

                        CommonClass.LoggedIPAddressesRemove();
                        if (uscheck == "Vessel")
                        {
                            //int Vid = Convert.ToInt32(CommonClass.VesselSessionID);
                            int vid = CommonClass.UserInformation.Vessels == "" ? 0 : Convert.ToInt32(CommonClass.UserInformation.Vessels);
                            var Vname = userDetails.VesselList.Where(x => x.ImoNo == vid).Select(x => x.VesselName).SingleOrDefault();

                            CommonMethods.VesselName = Vname;
                            // CommonClass.VesselSessionID = userDetails.Vessels;
                            Session["VesselNames"] = Vname;
                            Session["VesselSessionID"] = userDetails.Vessels;
                            return RedirectToAction("index", "ShipNotification", new { area = "NotificationInfos" });
                        }
                        else 
                        {
                            return RedirectToAction("index", "home");
                        }

                       // return RedirectToAction("index", "home");
                    default:

                        var ASPUser = entities.AspNetUsers.Where(x => x.Email == model.EmailAddress).FirstOrDefault();
                        if (ASPUser != null)
                        {
                            if (ASPUser.LockoutEndDateUtc != null)
                            {
                                ModelState.AddModelError("", "Due to excessive invalid login attempts, your account has been locked, please try after some time.");

                            }
                            else
                            {
                                ModelState.AddModelError("", "Invalid login.");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Invalid User Name Or Password.");
                        }
                        return View(model);
                }
            }
        }

        //if (model.UserName != null)
        //{
        //    if (model.UserName.ToLower() == "admin")
        //    {
        //        var dataitem = entities.UserInfoes.FirstOrDefault(x => x.UserName == model.UserName.Trim() && x.Password == model.Password);
        //        if (dataitem != null)
        //        {
        //            //FormsAuthentication.SetAuthCookie(dataitem.UserName, false);
        //            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, dataitem.UserName, DateTime.Now, DateTime.Now.AddMinutes(30),
        //             true, dataitem.UserRole, FormsAuthentication.FormsCookiePath);

        //            // Encrypt the cookie using the machine key for secure transport
        //            string hash = FormsAuthentication.Encrypt(ticket);
        //            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hash);

        //            // Set the cookie's expiration time to the tickets expiration time
        //            if (ticket.IsPersistent) cookie.Expires = ticket.Expiration;

        //            // Add the cookie to the list for outgoing response
        //            Response.Cookies.Add(cookie);

        //            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && returnUrl.StartsWith("//") && returnUrl.StartsWith("/\\"))
        //            {
        //                return Redirect(returnUrl);
        //            }
        //            else
        //            {
        //                return RedirectToAction("index", "home");
        //            }
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Invalid Email / password");
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        UserRole.username1 = model.UserName.Trim();

        //        var usercheck = emp.Memberships.Where(x => x.UserName == model.UserName.Trim()).FirstOrDefault();
        //        bool bbm = usercheck != null && usercheck.IsConfirmed == false ? false : true;

        //        var dataitem = emp.Users.Where(x => x.EmailId == model.UserName.Trim() && x.Password == model.Password).FirstOrDefault();
        //        if (dataitem != null)
        //        {
        //            if (bbm)
        //            {
        //                FormsAuthentication.SetAuthCookie(dataitem.EmailId, false);
        //                // ViewBag.Fullname = dataitem.FullName;
        //                //FormsAuthentication.SetAuthCookie(dataitem.UserName, false);
        //                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && returnUrl.StartsWith("//") && returnUrl.StartsWith("/\\"))
        //                {
        //                    return Redirect(returnUrl);
        //                }
        //                else
        //                {
        //                    return RedirectToAction("index", "home");
        //                }
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("", "Invalid Email / password");
        //                return View();
        //            }
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Invalid Email / password");
        //            return View();
        //        }
        //    }
        //}
        //else
        //{
        //    ModelState.AddModelError("", "Invalid Email / password");
        //    return View();
        //}

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {

           // string Domain = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                //string code1 = UserManager.GeneratePasswordResetToken(user.Id);
                //var code11 = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                //var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                string Domain = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                string callbackUrl = Domain + "/Account/ResetPassword/resetpassword?email=" + user.Email + "&code=" + HttpUtility.UrlEncode(code);

                string resetLink = "<a href='"+ Domain + "/Account/ResetPassword/?rt=" + HttpUtility.UrlEncode(code) + "&userId=" + user.Id + "'>Click this link to reset your password.</a>";

                SendEmail("", "Update Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public void SendEmail(string receiver, string subject, string message)
        {



            //string strMailMessageBody;
            try
            {


                string strMailMessageBody;
                try
                {



                    MailMessage msg = new MailMessage();
                    msg.From = new MailAddress("noreply@work-ship.com");
                    msg.Subject = subject;
                    msg.Body = message;
                    msg.IsBodyHtml = true;

                    msg.To.Add(new MailAddress("ws1.49web@gmail.com"));
                    msg.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");
                    msg.SubjectEncoding = System.Text.Encoding.Default;
                    //message(mailmsg, MailSendTo, Subject, strMailMessageBody);
                    //MailMessage msg = new MailMessage(mailmsg, MailSendTo, Subject, strMailMessageBody);
                    msg.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient("mail.work-ship.com");
                    //msg.CC.Add(CCEmail);
                    //msg.Bcc.Add(bcc);
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    // smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new NetworkCredential("noreply@work-ship.com", "4949WEB$treet");
                    smtp.Send(msg);



                    // return false;
                }
                catch (Exception ex) { throw ex; }




                //< !--< network host = "208.91.198.99" port = "587" username = "cs@work-ship.com" password = "49web@#0" enableSsl = "false" /> -->




                //var senderEmail = new mailaddress("noreply@work-ship.com", "4949WEB$treet");
                //var receiverEmail = new mailaddress("ws1.49web@gmail.com", "Receiver");
                //var password = "Your Email Password here";
                //var sub = subject;
                //var body = message;
                //var smtp = new SmtpClient
                //{
                // Host = "smtp.gmail.com",
                // Port = 587,
                // EnableSsl = true,
                // DeliveryMethod = SmtpDeliveryMethod.Network,
                // UseDefaultCredentials = false,
                // Credentials = new NetworkCredential(senderEmail.Address, password)
                //};
                //using (var mess = new MailMessage(senderEmail, receiverEmail)
                //{
                // Subject = subject,
                // Body = body
                //})
                //{
                // smtp.Send(mess);
                //}

                // return false;
            }
            catch (Exception ex) { throw ex; }
        }


        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //// POST: /Account/ForgotPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ForgotPassword(string Emailid, Membership1 mbp)
        //{

        //    if (string.IsNullOrEmpty(Emailid))
        //    {

        //        ViewBag.Forgotpassword = "Please enter the Email";
        //        return View("Forgotpassword");
        //    }
        //    else
        //    {
        //        var data = emp.Users.Where(p => p.EmailId.Equals(Emailid, StringComparison.Ordinal)).FirstOrDefault();
        //        if (data != null)
        //        {

        //            string confirmationToken =
        //               data.UserId + "," + data.EmailId + "," + data.FullName;
        //            dynamic email = new Email("RegEmail");
        //            email.To = data.EmailId;
        //            email.UserName = data.EmailId;
        //            string tokens = SwichButton.Encode(confirmationToken);
        //            email.ConfirmationToken = SwichButton.Encode(confirmationToken);

        //            email.Send();

        //            //................

        //            mbp.UserName = data.EmailId;
        //            mbp.CreateDate = Convert.ToDateTime(DateTime.Now.ToString());
        //            mbp.ConfirmationToken = tokens;
        //            mbp.IsConfirmed = false;



        //            var all = from c in emp.Memberships select c;
        //            emp.Memberships.RemoveRange(all);
        //            emp.SaveChanges();


        //            emp.Memberships.Add(mbp);
        //            emp.SaveChanges();



        //            ViewBag.Forgotpassword = "To complete the reset password look for an email in your inbox that provides further instructions";
        //            return View("Forgotpassword");
        //        }
        //        else
        //        {
        //            ViewBag.Forgotpassword = "This e-mail id does not exist in our System";
        //            //ViewBag.success = "failed";
        //            return View("forgotpassword");
        //        }
        //    }
        //}


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
        [ValidateAntiForgeryToken]
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
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            //user.HasResetPassword = true;
            //user.IsFirstLoginAfterPasswordReset = false;


            //var ss = "rKoEmmUzZbVT2+ZS9TawPXAjc2+aoLJBU/tDtwBkCFp9ImCFWKV82D9pQsr2MZZaViVzdPqpNVqvsGRVXiX56zG0/jJTfBkLjDtJJR/6SjutK/UC1mP7q5DYyiCd6xntS1ttc3aKzH0kFe5R1lc17d8aSJranIIawrf4Nbl42RzJM7BCaviUtK7lJKd/vjF3d6jU8xs/WvCE6DpVSev1Gw==";


            //var code = model.Code.Replace(" ", "+");
            //And then change the following line
            // UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);

            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);

            // var result = await _userManager.ResetPasswordAsync(user.Id, model.Code, model.Password);

            //string ds = "vuLmLoEhtVMP/984RbZURwrcHyXOiYdvtwWYeZEowhijIUS5o1v4DJr2qb0uQSmPUYyUjH49S85940Sb8QtbaARWfNTv881MZYySe9LN9TKFJMK22P85VVTfiqzDIY7Aqm/v/qS5nLyN8cby1HIoGsxFXeDPiy/SIAv3rIq3pEwXK8XIFy0mnnFkki6sTVsf";

            //var result = await UserManager.ResetPasswordAsync(user.Id, HttpUtility.UrlDecode(ds), model.Password);
            if (result.Succeeded)
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("ResetPasswordConfirmation", "Account");
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

        //[AllowAnonymous]
        //public ActionResult newpassword(string Id)
        //{
        //    string bmsid = SwichButton.Decode(Id);

        //    string[] s2 = null;
        //    s2 = bmsid.Split(',');
        //    int userid = Convert.ToInt32(s2[0].ToString());
        //    string emailid = s2[1].ToString();
        //    //BMSID.bmsuser = s2[2].ToString();


        //    UserClass urs = emp.Users.Single(ex => ex.UserId == userid && ex.EmailId == emailid);

        //    return View(urs);

        //}


        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult newpassword(UserClass usd, int userId, string password)
        //{

        //    if (!string.IsNullOrEmpty(usd.Password) && !string.IsNullOrEmpty(usd.ConfirmPassowrd) && usd.Password == usd.ConfirmPassowrd && usd.Password.Length >= 6)
        //    {

        //        //emp.updatepassword(usd);

        //        //......
        //        //emp.updatemembership(usd.UserName);
        //        //.........

        //        var usercheck = emp.Memberships.Where(x => x.UserName == usd.EmailId.Trim()).FirstOrDefault();

        //        if (usercheck != null)
        //        {
        //            DateTime dt1 = DateTime.Now;
        //            DateTime dt2 = usercheck.CreateDate.AddHours(24);

        //            if (dt1 < dt2)
        //            {


        //                var local = emp.Set<UserClass>()
        //                .Local
        //                .FirstOrDefault(f => f.UserId == userId);
        //                if (local != null)
        //                {
        //                    emp.Entry(local).State = EntityState.Detached;
        //                }



        //                var user = new UserClass() { UserId = userId, Password = password };
        //                using (var db = new ShipmentContaxt())
        //                {
        //                    db.Configuration.ValidateOnSaveEnabled = false;

        //                    db.Users.Attach(user);
        //                    db.Entry(user).Property(x => x.Password).IsModified = true;
        //                    db.SaveChanges();
        //                }

        //                //................

        //                var all = from c in emp.Memberships select c;
        //                emp.Memberships.RemoveRange(all);
        //                emp.SaveChanges();

        //                //..................

        //                ViewBag.passwordreset = "Renew password has been successful";
        //                return RedirectToAction("login", "account");
        //            }
        //            else
        //            {
        //                ViewBag.passwordreset = "Session is time out! please renew password to continew ";
        //                return View(usd);
        //            }
        //        }
        //        else
        //        {
        //            Session["passwordreset"] = "Session is time out! please renew password to continew";
        //            return View(usd);
        //        }


        //    }
        //    else
        //    {
        //        ViewBag.passwordreset = "";
        //        return View(usd);
        //    }
        //}


        /*
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
        [ValidateAntiForgeryToken]
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
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
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


         POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                     Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                 For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                 Send an email with this link
                 string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                 var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                 await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                 return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

             If we got this far, something failed, redisplay form
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
        [ValidateAntiForgeryToken]
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
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
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
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
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
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
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

        */
        //
        // POST: /Account/LogOff

        // [HttpPost]
        [Authorize]
        //[ValidateAntiForgeryToken]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();

            var urlToRemove = Url.Action("Index", "home");
            HttpResponse.RemoveOutputCacheItem(urlToRemove);
            UserRole.username = string.Empty;
            ViewBag.GetMenu = null;

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Clear();
            return RedirectToAction("login", "account");
        }

        [Obsolete]
        public JsonResult AutoLogout(string Ip)
        {
            string anotherlogin = ""; int login = 0;

            try
            {

                //string hostName = Dns.GetHostName(); // Retrive the Name of HOST
                // // Get the IP
                //string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

                if (Ip != "")
                {
                    string myIP = Ip + Session["IpAdd"];

                    // myIP = myIP + Ipadd;
                    //if (Session["IpAdd"] != null)
                    // myIP = Session["IpAdd"].ToString();

                    using (SqlDataAdapter adp1 = new SqlDataAdapter("SP_Autologout", con))
                    {
                        adp1.SelectCommand.CommandType = CommandType.StoredProcedure;
                        adp1.SelectCommand.Parameters.AddWithValue("@Email", Session["emailID"]);
                        adp1.SelectCommand.Parameters.AddWithValue("@myIP", myIP);

                        DataTable dt1 = new DataTable();
                        adp1.Fill(dt1);
                        if (dt1.Rows.Count > 0)
                        {
                            login = 1;
                            anotherlogin = myIP;
                        }
                        else
                        {
                            FormsAuthentication.SignOut();

                            var urlToRemove = Url.Action("Index", "home");
                            HttpResponse.RemoveOutputCacheItem(urlToRemove);
                            UserRole.username = string.Empty;
                            ViewBag.GetMenu = null;

                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            Session.Clear();


                            login = 2;
                            anotherlogin = myIP;
                        }
                    }
                }

            }
            catch
            {

            }
            return Json(new { Result = true, AnotherLog = anotherlogin, lgn = login }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
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