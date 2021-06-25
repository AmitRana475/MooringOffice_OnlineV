using MenuLayer;
using Microsoft.AspNet.Identity.Owin;
using Reports;
using Shipment49Web.Areas.Setting.Models;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Shipment49Web.Areas.Setting.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class UserDetailsController : BaseController
    {
        MorringOfficeEntities context = new MorringOfficeEntities();

        public UserDetailsController()
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                var _smartmenu = sc1.SmartMenus.ToList().FirstOrDefault();
                if (_smartmenu != null)
                {
                    ViewBag.MenuContent = _smartmenu.SmartMenuContent;
                }
            }

           
        }

        //[Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> Index()
        {
            ViewBag.Role = CommonClass.UserInformation.UserRole;
            if (CommonClass.UserInformation.UserRole == "USER")
            {

                var userlist = await context.UserInfoes.Where(x => x.UserID == CommonClass.UserInformation.UserID).OrderBy(u => u.FullName).ToListAsync();
                return View(userlist);
            }
            else
            {
                var userlist = await context.UserInfoes.OrderBy(u => u.FullName).ToListAsync();
                return View(userlist);

            }

        }

        public ActionResult Create()
        {
            ModelState.Clear();
            var userinfo = new Reports.UserInfo();
            return View(userinfo);
        }

        //[Throttle(TimeUnit = TimeUnit.Minute, Count = 2)]
        //[RateLimit(Name = "TestThrottle", Message = "You must wait {n} seconds before accessing this url again.", Seconds = 10)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventSpam("CreateUser",3,1)]
        public ActionResult Create(UserInfo user)
        {
          
            if (user.UserRole.Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                user.Vessels = user.FleetNames = user.FleetTypes = "";
                user.PermissionType = "All";
                if ((user.VesselIMOs.Count > 0) && (user.FNameIDs.Count > 0) && (user.FTypeIDs.Count > 0))
                {

                    if (string.IsNullOrEmpty(user.UserID))
                        return View(user);
                    else
                        return View("Create", user);
                }
                else
                {
                    user.Vessels = user.Vessels.Trim(',');
                    user.FleetNames = user.FleetNames.Trim(',');
                    user.FleetTypes = user.FleetTypes.Trim(',');
                }

            }
            else if(user.Password != user.ConfirmPassword)
            {
                ModelState.AddModelError("", "Password and Confirm Password does not match.");
                return View("Create", user);
            }
            else if (user.UserRole.Equals("USER", StringComparison.OrdinalIgnoreCase))
            {
                if ((user.VesselIMOs.Count == 0) && (user.FNameIDs.Count == 0) && (user.FTypeIDs.Count == 0))
                {
                    ModelState.AddModelError("", "Any one from the Vessel Name/ Fleet Name/ Fleet Type is required for assigning permissions");

                    if (string.IsNullOrEmpty(user.UserID))
                        return View(user);
                    else
                        return View("Create", user);
                }
                else
                {
                    switch (string.IsNullOrEmpty(user.PermissionType) ? string.Empty : user.PermissionType.ToUpper())
                    {
                        case "VESSELS":
                            if (user.VesselIMOs.Count > 0)
                            {
                                foreach (int id in user.VesselIMOs)
                                    user.Vessels = string.Format("{0},{1}", id, user.Vessels);

                                foreach (var v in context.VesselDetails.Where(p => user.VesselIMOs.Contains(p.ImoNo)).ToList())
                                {
                                    user.FleetTypes = string.Format("{0},{1}", v.FleetTypeID, user.FleetTypes);
                                    user.FleetNames = string.Format("{0},{1}", v.FleetNameID, user.FleetNames);
                                }
                            }
                            break;
                        case "FLEETTYPES":
                            if (user.FTypeIDs.Count > 0)
                            {
                                foreach (int id in user.FTypeIDs)
                                    user.FleetTypes = string.Format("{0},{1}", id, user.FleetTypes);

                                foreach (var v in context.VesselDetails.Where(p => user.FTypeIDs.Contains(p.FleetTypeID)).ToList())
                                {
                                    user.Vessels = string.Format("{0},{1}", v.ImoNo, user.Vessels);
                                    user.FleetNames = string.Format("{0},{1}", v.FleetNameID, user.FleetNames);
                                }
                            }
                            break;
                        case "FLEETNAMES":
                            if (user.FNameIDs.Count > 0)
                            {
                                foreach (int id in user.FNameIDs)
                                    user.FleetNames = string.Format("{0},{1}", id, user.FleetNames);

                                foreach (var v in context.VesselDetails.Where(p => user.FNameIDs.Contains(p.FleetNameID)).ToList())
                                {
                                    user.Vessels = string.Format("{0},{1}", v.ImoNo, user.Vessels);
                                    user.FleetTypes = string.Format("{0},{1}", v.FleetTypeID, user.FleetTypes);
                                }
                            }
                            break;
                        default:
                            user.Vessels = user.FleetNames = user.FleetTypes = string.Empty;
                            break;
                    }

                    user.Vessels = user.Vessels.Trim(',');
                    user.FleetNames = user.FleetNames.Trim(',');
                    user.FleetTypes = user.FleetTypes.Trim(',');

                }
            }

          
            var appuser = new ApplicationUser { UserName = user.EmailAddress, Email = user.EmailAddress, EmailConfirmed = true };
            var result = UserManager.CreateAsync(appuser, user.Password);
            // var result2 = await UserManager.ChangePasswordAsync(userInfo.Id, model.CurrentPassword, model.NewPassword);
            AspNetUser aspNetUserInfo = new AspNetUser();

            if (result.Result.Succeeded)
            {
                var userDetails = context.AspNetUsers.FirstOrDefault(p => p.Email == user.EmailAddress);

                if (userDetails != null)
                {
                    //user.UserRole = "ADMIN";
                    user.UserID = userDetails.Id;
                    user.CreatedDate = DateTime.Now;

                    foreach (var v in context.tblCommons.Where(p => user.LTypeIDs.Contains(p.Id)).ToList())
                    {
                        // int dd = v.Id;
                        user.LoginType = v.Id;

                    }

                    var uscheck = context.tblCommons.Where(x => x.Id == user.LoginType).Select(x => x.Name).SingleOrDefault();

                    if (uscheck == "Vessel")
                    {
                        if (user.VesselIMOs.Count > 0)
                        {                           
                            foreach (int id in user.VesselIMOs.Distinct())
                                user.Vessels = string.Format("{0},{1}", id, user.Vessels);

                        }
                        user.Vessels = user.Vessels.Trim(',');
                        user.PermissionType = "VESSELS";
                    }




                    
                    context.UserInfoes.Add(user);
                    context.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Error creating User");
                    return View("Create", user);
                }
            }
            else
            {
                string errormessage = "";
                if (result.Result.Errors.Count() > 0)
                {
                    foreach (string err in result.Result.Errors)
                        errormessage = err + ", " + errormessage;

                    if (!string.IsNullOrEmpty(errormessage))
                        errormessage = errormessage.TrimEnd(',');
                }

                ModelState.AddModelError("", errormessage);
                return View("Create", user);
            }
        }

        [HttpGet]
        //[Authorize(Roles = "Admin,User")]
        public ActionResult Edit(string id)
        {
            ViewBag.Role = CommonClass.UserInformation.UserRole;
            var userinfo = context.UserInfoes.FirstOrDefault(p => p.UserID == id);

            if (userinfo == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                switch (string.IsNullOrEmpty(userinfo.PermissionType) ? string.Empty : userinfo.PermissionType.ToUpper())
                {
                    case "VESSELS":
                        if (!string.IsNullOrEmpty(userinfo.Vessels))
                        {
                            foreach (string v in userinfo.Vessels.Split(','))
                                userinfo.VesselIMOs.Add(Convert.ToInt32(v));
                        }
                        break;
                    case "FLEETTYPES":
                        if (!string.IsNullOrEmpty(userinfo.FleetTypes))
                        {
                            foreach (string v in userinfo.FleetTypes.Split(','))
                                userinfo.FTypeIDs.Add(Convert.ToInt32(v));
                        }
                        break;
                    case "FLEETNAMES":
                        if (!string.IsNullOrEmpty(userinfo.FleetNames))
                        {
                            foreach (string v in userinfo.FleetNames.Split(','))
                                userinfo.FNameIDs.Add(Convert.ToInt32(v));
                        }
                        break;
                }

                userinfo.ConfirmPassword = userinfo.Password;
                userinfo.EmailAddress = userinfo.AspNetUser.Email;
                return View(userinfo);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventSpam("EditUser", 3, 1)]
        public ActionResult Edit(UserInfo user)
        {
            ViewBag.Role = CommonClass.UserInformation.UserRole;
            var userinfo = context.UserInfoes.FirstOrDefault(p => p.UserID == user.UserID);
            if (string.IsNullOrEmpty(user.UserRole))
            {
                user.UserRole = userinfo.UserRole;
            }
            if (user.UserRole.Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                user.Vessels = user.FleetNames = user.FleetTypes = "";
                user.PermissionType = "ALL";
            }
            else
            {
                if ((user.VesselIMOs.Count == 0) && (user.FNameIDs.Count == 0) && (user.FTypeIDs.Count == 0))
                {
                    ModelState.AddModelError("", "Any one from the Vessel Name/ Fleet Name/ Fleet Type is required for assigning permissions");
                    return View(user);
                }
                else
                {
                    switch (string.IsNullOrEmpty(user.PermissionType) ? string.Empty : user.PermissionType.ToUpper())
                    {
                        case "VESSELS":
                            if (user.VesselIMOs.Count > 0)
                            {
                                foreach (int id in user.VesselIMOs)
                                    user.Vessels = string.Format("{0},{1}", id, user.Vessels);

                                foreach (var v in context.VesselDetails.Where(p => user.VesselIMOs.Contains(p.ImoNo)).ToList())
                                {
                                    user.FleetTypes = string.Format("{0},{1}", v.FleetTypeID, user.FleetTypes);
                                    user.FleetNames = string.Format("{0},{1}", v.FleetNameID, user.FleetNames);
                                }
                            }
                            break;
                        case "FLEETTYPES":
                            if (user.FTypeIDs.Count > 0)
                            {
                                foreach (int id in user.FTypeIDs)
                                    user.FleetTypes = string.Format("{0},{1}", id, user.FleetTypes);

                                foreach (var v in context.VesselDetails.Where(p => user.FTypeIDs.Contains(p.FleetTypeID)).ToList())
                                {
                                    user.Vessels = string.Format("{0},{1}", v.ImoNo, user.Vessels);
                                    user.FleetNames = string.Format("{0},{1}", v.FleetNameID, user.FleetNames);
                                }
                            }
                            break;
                        case "FLEETNAMES":
                            if (user.FNameIDs.Count > 0)
                            {
                                foreach (int id in user.FNameIDs)
                                    user.FleetNames = string.Format("{0},{1}", id, user.FleetNames);

                                foreach (var v in context.VesselDetails.Where(p => user.FNameIDs.Contains(p.FleetNameID)).ToList())
                                {
                                    user.Vessels = string.Format("{0},{1}", v.ImoNo, user.Vessels);
                                    user.FleetTypes = string.Format("{0},{1}", v.FleetTypeID, user.FleetTypes);
                                }
                            }
                            break;
                        default:
                            user.Vessels = user.FleetNames = user.FleetTypes = string.Empty;
                            break;

                            //case "VESSELS":
                            //    if (user.VesselIMOs.Count > 0)
                            //    {
                            //        foreach (int id in user.VesselIMOs)
                            //            user.Vessels = string.Format("{0},{1}", id, user.Vessels);
                            //    }

                            //    user.FleetNames = user.FleetTypes = string.Empty;
                            //    break;
                            //case "FLEETTYPES":
                            //    if (user.FTypeIDs.Count > 0)
                            //    {
                            //        foreach (int id in user.FTypeIDs)
                            //            user.FleetTypes = string.Format("{0},{1}", id, user.FleetTypes);

                            //        foreach (var v in context.VesselDetails.Where(p => user.FTypeIDs.Contains(p.FleetTypeID)).ToList())
                            //            user.Vessels = string.Format("{0},{1}", v.ImoNo, user.Vessels);

                            //        user.FleetNames = string.Empty;
                            //    }
                            //    break;
                            //case "FLEETNAMES":
                            //    if (user.FNameIDs.Count > 0)
                            //    {
                            //        foreach (int id in user.FNameIDs)
                            //            user.FleetNames = string.Format("{0},{1}", id, user.FleetNames);

                            //        foreach (var v in context.VesselDetails.Where(p => user.FNameIDs.Contains(p.FleetNameID)).ToList())
                            //            user.Vessels = string.Format("{0},{1}", v.ImoNo, user.Vessels);

                            //        user.FleetTypes = string.Empty;
                            //    }
                            //    break;
                            //default:
                            //    user.Vessels = user.FleetNames = user.FleetTypes = string.Empty;
                            //    break;
                    }

                    user.Vessels = string.IsNullOrEmpty(user.Vessels) ? string.Empty : user.Vessels.Trim(',');
                    user.FleetNames = string.IsNullOrEmpty(user.FleetNames) ? string.Empty : user.FleetNames.Trim(',');
                    user.FleetTypes = string.IsNullOrEmpty(user.FleetTypes) ? string.Empty : user.FleetTypes.Trim(',');
                }
            }



            if (userinfo != null)
            {
                // this is not an actual password, this is just to bypass the validations
                userinfo.Password = userinfo.ConfirmPassword = "123456";
                userinfo.EmailAddress = user.EmailAddress;
                userinfo.FullName = user.FullName;
                userinfo.ContactNumber = user.ContactNumber;
                userinfo.PermissionType = user.PermissionType;
                userinfo.UserRole = user.UserRole;
                userinfo.Vessels = user.Vessels;
                userinfo.FleetNames = user.FleetNames;
                userinfo.FleetTypes = user.FleetTypes;

                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        //[Authorize(Roles = "Admin,User")]
        public ActionResult Details(string id)
        {
            var userinfo = context.UserInfoes.FirstOrDefault(p => p.UserID == id);

            if (userinfo == null)
                return RedirectToAction("Index");
            else
            {
                if (!string.IsNullOrEmpty(userinfo.Vessels))
                {
                    userinfo.PermissionType = "VESSELS";
                    foreach (string v in userinfo.Vessels.Split(','))
                        userinfo.VesselIMOs.Add(Convert.ToInt32(v));

                    if (userinfo.VesselIMOs.Count > 0)
                        userinfo.VesselList = context.VesselDetails.Where(p => userinfo.VesselIMOs.Contains(p.ImoNo)).ToList();
                }

                if (!string.IsNullOrEmpty(userinfo.FleetNames))
                {
                    userinfo.PermissionType = "FLEETNAMES";
                    foreach (string v in userinfo.FleetNames.Split(','))
                        userinfo.FNameIDs.Add(Convert.ToInt32(v));

                    if (userinfo.FNameIDs.Count > 0)
                        userinfo.FleetNameList = context.tblCommons.Where(p => userinfo.FNameIDs.Contains(p.Id)).ToList();
                }

                if (!string.IsNullOrEmpty(userinfo.FleetTypes))
                {
                    userinfo.PermissionType = "FLEETTYPES";
                    foreach (string v in userinfo.FleetTypes.Split(','))
                        userinfo.FTypeIDs.Add(Convert.ToInt32(v));

                    if (userinfo.FTypeIDs.Count > 0)
                        userinfo.FleetTypeList = context.tblCommons.Where(p => userinfo.FTypeIDs.Contains(p.Id)).ToList();
                }

                userinfo.ConfirmPassword = userinfo.Password;
                return View(userinfo);
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventSpam("DeleteUser", 3, 1)]
        public ActionResult Delete(string id)
        {
            if (CommonClass.UserInformation.UserRole == "ADMIN")
            {
                var userInfo = context.UserInfoes.FirstOrDefault(e => e.UserID == id);
                if (userInfo != null)
                {
                    context.UserInfoes.Remove(userInfo);
                    context.Entry(userInfo).State = EntityState.Deleted;

                    var userMaster = context.AspNetUsers.FirstOrDefault(e => e.Id == id);

                    context.AspNetUsers.Remove(userMaster);
                    context.Entry(userMaster).State = EntityState.Deleted;

                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ModelState.Clear();
            ModelState.AddModelError("", "Something went wrong!");
            return View();
        }

        public ActionResult EditProfile()
        {
            EditProfileViewModel editProfile = new EditProfileViewModel();
            var userinfo = context.UserInfoes.FirstOrDefault(p => p.UserID == LoggedInUserID);

            if (userinfo != null)
            {
                editProfile.UserID = userinfo.UserID;
                editProfile.UserRole = userinfo.UserRole;
                editProfile.EmailAddress = userinfo.AspNetUser.Email;
                editProfile.FullName = userinfo.FullName;
                editProfile.ContactNumber = userinfo.ContactNumber;
            }

            return View(editProfile);
        }

        [HttpPost]
        [PreventSpam("EditProfile", 3, 1)]
        public ActionResult EditProfile(EditProfileViewModel editProfile)
        {
            var userinfo = context.UserInfoes.FirstOrDefault(p => p.UserID == LoggedInUserID);

            if (userinfo != null)
            {
                userinfo.FullName = editProfile.FullName;
                userinfo.ContactNumber = editProfile.ContactNumber;
                userinfo.ConfirmPassword = userinfo.Password = "123"; // this is a temporary password to bypass the validations

                context.SaveChanges();

                TempData["Result"] = "Profile Updated Successfully.";

                return RedirectToAction("Confirmation");
            }

            return View(editProfile);
        }

        public ActionResult ChangePassword()
        {
            return View();


            //if (TempData["Error"] != null)
            //{
            //    return View();
            //}
            //if (TempData["Success"] != null)
            //{

            //}

        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            string errormessage = string.Empty;

            var userInfo = await UserManager.FindByEmailAsync(LoggedInUserEmail);

            if (userInfo != null)
            {
                var result = await UserManager.ChangePasswordAsync(userInfo.Id, model.CurrentPassword, model.NewPassword);
                if (model.CurrentPassword == model.NewPassword)
                {
                    TempData["Error"] = "You Cannot Reuse Previous Password.";
                    return View(model);
                }
                else if (model.NewPassword != model.ConfirmPassword)
                {
                    TempData["Error"] = "New Password and Confirm Password does not match.";
                    return View(model);
                }
                else
                {
                    if (result.Succeeded)
                    {
                        TempData["Success"] = "Password updated successfully.";
                        return RedirectToAction("Confirmation");

                    }
                    else if (result.Errors.Count() > 0)
                    {
                        foreach (string err in result.Errors)
                            errormessage = err + "," + errormessage;

                        if (!string.IsNullOrEmpty(errormessage))
                        {
                            TempData["Error"] = errormessage.Trim(',');
                            return View(model);
                        }
                    }
                    else
                    {
                        TempData["Error"] = "An error has occured. Please try later.";
                        return View(model);
                    }
                }
            }
            else
                TempData["Error"] = "Invalid User.";
            return View(model);
           
        }

        public ActionResult Confirmation()
        {
            return View();
        }

        [Obsolete]
        public JsonResult sessionexp()
        {
            string anotherlogin = ""; int login = 0;
                       
            return Json(new { Result = true, AnotherLog = anotherlogin, lgn = login }, JsonRequestBehavior.AllowGet);
        }
    }
}


//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Linq.Dynamic;
//using System.Net;
//using System.Web.Mvc;
//using UserLayer;
//using MenuLayer;
//using System.Web.Security;
//using Shipment49Web.Models;
//using Microsoft.AspNet.Identity;
//using Reports;

//namespace Shipment49Web.Areas.Setting.Controllers
//{
//    [Authorize]
//    [ErrorClass]
//    public class UserDetailsController : Controller
//    {

//        //private ShipmentContaxt sc = new ShipmentContaxt();

//        private readonly IMenuRepository sc;
//        public UserDetailsController(IMenuRepository repo)
//        {
//            sc = repo;

//            if (UserRole.username == null)
//            {
//                UserRole.username = string.Join("", Roles.GetRolesForUser());
//            }
//            ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();
//        }


//        //[Authorize(Roles = "Admin,User")]
//        public ActionResult Index(int page = 1, string sort = "FullName", string sortdir = "asc", string search = "")
//        {
//            // UserRole.username
//            int pagesize = 10;
//            int totalrecord = 0;
//            if (page < 1) page = 1;
//            int skip = (page * pagesize) - pagesize;
//            var data = Getusers(search, sort, sortdir, skip, pagesize, out totalrecord);
//            ViewBag.TotalRows = totalrecord;
//            ViewBag.search = search;

//            ViewBag.Messagegrid = data.Count() > 0 ? "" : ViewBag.Messagegrid = "Data Not Available For Selected Criteria! Please Select The Other Criteria.";

//            return View(data);
//        }


//        [NonAction]
//        public List<UserClass> Getusers(string search, string sort, string sortdir, int skip, int pagesize, out int totalrecord)
//        {

//            if (UserRole.username == "Admin")
//            {

//                var user = (from a in sc.Users
//                            where
//                            a.FullName.Contains(search) ||
//                            a.EmailId.Contains(search) ||
//                            a.Designation.Contains(search)
//                            select a);

//                totalrecord = user.Count();
//                user = user.OrderBy(sort + " " + sortdir);
//                if (pagesize > 0)
//                {
//                    user = user.Skip(skip).Take(pagesize);
//                }
//                return user.ToList();
//            }
//            else
//            {
//                string usermame = User.Identity.GetUserName();
//                var Tuserlist = sc.Users.Where(x => x.EmailId == usermame).ToList();

//                var user = (from a in Tuserlist
//                            where
//                            a.FullName.Contains(search) ||
//                            a.EmailId.Contains(search) ||
//                            a.Designation.Contains(search)
//                            select a);

//                totalrecord = user.Count();
//                user = user.OrderBy(sort + " " + sortdir);
//                if (pagesize > 0)
//                {
//                    user = user.Skip(skip).Take(pagesize);
//                }

//                return Tuserlist.ToList();
//            }
//        }




//        [Authorize(Roles = "Admin,User")]
//        public ActionResult Create()
//        {
//            ModelState.Clear();
//            return View(new Reports.UserInfo());
//        }

//        [Authorize(Roles = "Admin,User")]
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create(UserInfo user)
//        {
//            using (ShipmentContaxt sc1 = new ShipmentContaxt())
//            {
//                if (RFleettype == "VesselName")
//                {
//                    sc1.Configuration.ProxyCreationEnabled = false;


//                    string vesselid = "";
//                    string vesselname1 = "";
//                    string[] bms = vesselname.TrimEnd(',').Split(',').ToArray();


//                    foreach (string bb in bms)
//                    {
//                        vesselid += sc.Vessels.Where(x => x.VesselName.ToString() == bb).Select(p => p.VesselID).Distinct().FirstOrDefault() + ",";
//                        vesselname1 += bb + ",";
//                    }

//                    user.VesselName = vesselname1.TrimEnd(',');
//                    user.VesselID = vesselid.TrimEnd(',');
//                    user.Role = "User";
//                    user.FleetType = Ftp;
//                    user.FleetName = FleetNames;
//                    user.AssignVal = RFleettype;
//                    if (string.IsNullOrEmpty(vesselname))
//                    {
//                        ModelState.AddModelError("VesselName", "VesselName is required");
//                    }

//                }
//                else if (RFleettype == "FleetType")
//                {
//                    sc1.Configuration.ProxyCreationEnabled = false;


//                    string vesselid = "";
//                    string vesselname1 = "";
//                    string[] bms = Ftp.TrimEnd(',').Split(',').ToArray();


//                    foreach (string bb in bms)
//                    {
//                        var vvcc = sc.Vessels.Where(x => x.FleetType.ToString() == bb).Distinct().ToList();

//                        foreach (var item in vvcc)
//                        {
//                            vesselid += item.VesselID + ",";
//                            vesselname1 += item.VesselName + ",";
//                        }

//                        //vesselid += sc.Vessels.Where(x => x.FleetType.ToString() == bb).Select(p => p.VesselID).Distinct().FirstOrDefault() + ",";
//                        //vesselname1 += bb + ",";
//                    }

//                    user.VesselName = vesselname1.TrimEnd(',');
//                    user.VesselID = vesselid.TrimEnd(',');
//                    user.Role = "User";
//                    user.FleetType = Ftp;
//                    user.FleetName = FleetNames;
//                    user.AssignVal = RFleettype;
//                    if (string.IsNullOrEmpty(Ftp))
//                    {
//                        ModelState.AddModelError("VesselName", "FleetType is required");
//                    }
//                }
//                else if (RFleettype == "FleetName")
//                {
//                    sc1.Configuration.ProxyCreationEnabled = false;


//                    string vesselid = "";
//                    string vesselname1 = "";
//                    string[] bms = FleetNames.TrimEnd(',').Split(',').ToArray();


//                    foreach (string bb in bms)
//                    {
//                        var vvcc = sc.Vessels.Where(x => x.FleetName.ToString() == bb).Distinct().ToList();

//                        foreach (var item in vvcc)
//                        {
//                            vesselid += item.VesselID + ",";
//                            vesselname1 += item.VesselName + ",";
//                        }

//                        //vesselid += sc.Vessels.Where(x => x.FleetType.ToString() == bb).Select(p => p.VesselID).Distinct().FirstOrDefault() + ",";
//                        //vesselname1 += bb + ",";
//                    }

//                    user.VesselName = vesselname1.TrimEnd(',');
//                    user.VesselID = vesselid.TrimEnd(',');
//                    user.Role = "User";
//                    user.FleetType = Ftp;
//                    user.FleetName = FleetNames;
//                    user.AssignVal = RFleettype;
//                    if (string.IsNullOrEmpty(FleetNames))
//                    {
//                        ModelState.AddModelError("VesselName", "FleetName is required");
//                    }
//                }

//                if (ModelState.IsValid)
//                {

//                    //var local = sc1.Set<UserClass>()
//                    // .Local
//                    // .FirstOrDefault(f => f.curid == cc.curid);
//                    //if (local != null)
//                    //{
//                    //sc1.Entry(user).State = EntityState.Detached;
//                    // }

//                    //var Updateduser = new UserClass()
//                    //{
//                    //    FullName = user.FullName,
//                    //    EmailId = user.EmailId,
//                    //    ContactNo = user.ContactNo,
//                    //    Password = user.Password,
//                    //    Designation = user.Designation,
//                    //    Role = user.Role,
//                    //    VesselName = user.VesselName,
//                    //    VesselID = user.VesselID



//                    //};



//                    sc1.Users.Add(user);
//                    sc1.SaveChanges();

//                    ViewBag.vesselnamebm = AutoCompletevessel1;
//                    ViewBag.FleetTypeR = AutoCompleteFleetType;
//                    ViewBag.FleetNameR = AutoCompleteFleetName;

//                    return RedirectToAction("index");
//                }
//                else
//                {
//                    ViewBag.vesselnamebm = AutoCompletevessel1;
//                    ViewBag.FleetTypeR = AutoCompleteFleetType;
//                    ViewBag.FleetNameR = AutoCompleteFleetName;
//                    return View(user);
//                }

//            }
//        }

//        [HttpGet]
//        [Authorize(Roles = "Admin,User")]
//        public ActionResult Edit(int? id)
//        {
//            UserClass uc = sc.Users.Where(x => x.UserId == id).FirstOrDefault();
//            uc.ConfirmPassowrd = uc.Password;
//            var AsignType = uc.AssignVal;
//            if (AsignType == "VesselName")
//            {
//                ViewBag.vesselnamebm1 = uc.VesselName;
//            }
//            else if (AsignType == "FleetType")
//            {
//                ViewBag.FleetTypeR1 = uc.FleetType;
//            }
//            else if (AsignType == "FleetName")
//            {
//                ViewBag.FleetNameR1 = uc.FleetName;
//            }

//            ViewBag.vesselnamebm = AutoCompletevessel1;

//            ViewBag.FleetTypeR = AutoCompleteFleetType;

//            ViewBag.FleetNameR = AutoCompleteFleetName;



//            return View(uc);
//        }

//        [HttpPost]
//        [Authorize(Roles = "Admin,User")]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit(UserClass us, string vesselname, string Ftp, string FleetNames, string RFleettype)
//        {
//            using (ShipmentContaxt sc1 = new ShipmentContaxt())
//            {
//                //sc1.Configuration.ProxyCreationEnabled = false;

//                ViewBag.vesselnamebm = AutoCompletevessel1;

//                ViewBag.FleetTypeR = AutoCompleteFleetType;

//                ViewBag.FleetNameR = AutoCompleteFleetName;

//                //string vesselid = "";
//                //string vesselname1 = "";
//                //string[] bms = vesselname.TrimEnd(',').Split(',').ToArray();


//                //foreach (string bb in bms)
//                //{
//                //    vesselid += sc.Vessels.Where(x => x.VesselName.ToString() == bb).Select(p => p.VesselID).Distinct().FirstOrDefault() + ",";
//                //    vesselname1 += bb + ",";
//                //}

//                //us.VesselName = vesselname1.TrimEnd(',');
//                //us.VesselID = vesselid.TrimEnd(',');
//                //us.Role = "User";
//                //if (string.IsNullOrEmpty(vesselname))
//                //{
//                //    ModelState.AddModelError("VesselName", "VesselName is required");
//                //}

//                if (RFleettype == "VesselName")
//                {
//                    sc1.Configuration.ProxyCreationEnabled = false;


//                    string vesselid = "";
//                    string vesselname1 = "";
//                    string[] bms = vesselname.TrimEnd(',').Split(',').ToArray();


//                    foreach (string bb in bms)
//                    {
//                        vesselid += sc.Vessels.Where(x => x.VesselName.ToString() == bb).Select(p => p.VesselID).Distinct().FirstOrDefault() + ",";
//                        vesselname1 += bb + ",";
//                    }

//                    us.VesselName = vesselname1.TrimEnd(',');
//                    us.VesselID = vesselid.TrimEnd(',');
//                    us.Role = "User";
//                    us.FleetType = Ftp = null;
//                    us.FleetName = FleetNames = null;
//                    us.AssignVal = RFleettype;
//                    if (string.IsNullOrEmpty(vesselname))
//                    {
//                        ModelState.AddModelError("VesselName", "VesselName is required");
//                    }

//                }
//                else if (RFleettype == "FleetType")
//                {
//                    sc1.Configuration.ProxyCreationEnabled = false;


//                    string vesselid = "";
//                    string vesselname1 = "";
//                    string[] bms = Ftp.TrimEnd(',').Split(',').ToArray();


//                    foreach (string bb in bms)
//                    {
//                        var vvcc = sc.Vessels.Where(x => x.FleetType.ToString() == bb).Distinct().ToList();

//                        foreach (var item in vvcc)
//                        {
//                            vesselid += item.VesselID + ",";
//                            vesselname1 += item.VesselName + ",";
//                        }

//                        //vesselid += sc.Vessels.Where(x => x.FleetType.ToString() == bb).Select(p => p.VesselID).Distinct().FirstOrDefault() + ",";
//                        //vesselname1 += bb + ",";
//                    }

//                    us.VesselName = vesselname1.TrimEnd(',');
//                    us.VesselID = vesselid.TrimEnd(',');
//                    us.Role = "User";
//                    us.FleetType = Ftp;
//                    us.FleetName = FleetNames = null;
//                    us.AssignVal = RFleettype;
//                    if (string.IsNullOrEmpty(Ftp))
//                    {
//                        ModelState.AddModelError("VesselName", "FleetType is required");
//                    }
//                }
//                else if (RFleettype == "FleetName")
//                {
//                    sc1.Configuration.ProxyCreationEnabled = false;


//                    string vesselid = "";
//                    string vesselname1 = "";
//                    string[] bms = FleetNames.TrimEnd(',').Split(',').ToArray();


//                    foreach (string bb in bms)
//                    {
//                        var vvcc = sc.Vessels.Where(x => x.FleetName.ToString() == bb).Distinct().ToList();

//                        foreach (var item in vvcc)
//                        {
//                            vesselid += item.VesselID + ",";
//                            vesselname1 += item.VesselName + ",";
//                        }

//                        //vesselid += sc.Vessels.Where(x => x.FleetType.ToString() == bb).Select(p => p.VesselID).Distinct().FirstOrDefault() + ",";
//                        //vesselname1 += bb + ",";
//                    }

//                    us.VesselName = vesselname1.TrimEnd(',');
//                    us.VesselID = vesselid.TrimEnd(',');
//                    us.Role = "User";
//                    us.FleetType = Ftp = null;
//                    us.FleetName = FleetNames ;
//                    us.AssignVal = RFleettype;
//                    if (string.IsNullOrEmpty(FleetNames))
//                    {
//                        ModelState.AddModelError("VesselName", "FleetName is required");
//                    }
//                }


//                if (ModelState.IsValid)
//                {
//                    sc1.Entry(us).State = EntityState.Modified;
//                    sc1.SaveChanges();

//                    return RedirectToAction("index");


//                }
//                else
//                {
//                    return View(us);
//                }
//            }
//        }
//        [Authorize(Roles = "Admin,User")]
//        public ActionResult Detail(int? id)
//        {
//            using (ShipmentContaxt sc1 = new ShipmentContaxt())
//            {
//                sc1.Configuration.ProxyCreationEnabled = false;

//                if (id == null)
//                {
//                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//                }
//                UserClass user = sc1.Users.Find(id);
//                if (user == null)
//                {
//                    return HttpNotFound();
//                }
//                return View(user);
//            }
//        }

//        [Authorize(Roles = "Admin,User")]
//        public ActionResult Delete(int? id)
//        {
//            using (ShipmentContaxt sc1 = new ShipmentContaxt())
//            {
//                sc1.Configuration.ProxyCreationEnabled = false;

//                if (id == null)
//                {
//                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

//                }
//                else
//                {
//                    UserClass uc = sc1.Users.Where(x => x.UserId == id).FirstOrDefault();
//                    sc1.Users.Remove(uc);
//                    sc1.SaveChanges();
//                }
//                return RedirectToAction("index");
//            }
//        }
//        public JsonResult checkuser(string initialProductCode, string emailid)
//        {
//            if (emailid == initialProductCode)
//            {
//                return Json(true, JsonRequestBehavior.AllowGet);
//            }
//            return Json(sc.Users.All(u => u.EmailId != emailid), JsonRequestBehavior.AllowGet);
//        }

//        //[PartialCache("OneMinuteCache")]
//        public IEnumerable<SelectListItem> AutoCompletevessel1
//        {
//            get
//            {
//                using (ShipmentContaxt ssc = new ShipmentContaxt())
//                {
//                    var studentsvv = ssc.Vessels.Select(x => new { x.ImoNo, x.VesselName }).Distinct().ToList();

//                    if (UserRole.username.ToLower() != "admin")
//                    {
//                        var vsname = ssc.Users.Where(x => x.EmailId.Equals(UserRole.username1)).FirstOrDefault();
//                        if (vsname.AssignVal.ToString() == "VesselName")
//                        {
//                            string[] vesselname = vsname.VesselName.TrimEnd(',').Split(',');
//                            studentsvv = ssc.Vessels.Where(x => vesselname.Contains(x.VesselName)).Select(x => new { x.ImoNo, x.VesselName }).Distinct().ToList();
//                        }
//                    }
//                    var sportsList = new List<SelectListItem>();
//                    foreach (var item in studentsvv)
//                    {
//                        sportsList.Add(new SelectListItem { Text = item.VesselName, Value = item.VesselName.ToString() });
//                    }

//                    return sportsList;
//                }
//            }
//        }


//        public IEnumerable<SelectListItem> AutoCompleteFleetType
//        {
//            get
//            {
//                using (ShipmentContaxt ssc = new ShipmentContaxt())
//                {

//                    var studentsvv = ssc.FleetTypes.Select(x => new { x.Tid, x.FleetType }).Distinct().ToList();

//                    if (UserRole.username.ToLower() != "admin")
//                    {
//                        var vsname = ssc.Users.Where(x => x.EmailId.Equals(UserRole.username1)).FirstOrDefault();
//                        if (vsname.AssignVal.ToString() == "FleetType")
//                        {
//                            string[] vesselname = vsname.FleetType.TrimEnd(',').Split(',');
//                            studentsvv = ssc.FleetTypes.Where(x => vesselname.Contains(x.FleetType)).Select(x => new { x.Tid, x.FleetType }).Distinct().ToList();
//                        }
//                    }
//                    var sportsList = new List<SelectListItem>();
//                    foreach (var item in studentsvv)
//                    {
//                        sportsList.Add(new SelectListItem { Text = item.FleetType, Value = item.FleetType.ToString() });
//                    }

//                    return sportsList;
//                }
//            }
//        }

//        public IEnumerable<SelectListItem> AutoCompleteFleetName
//        {
//            get
//            {
//                using (ShipmentContaxt ssc = new ShipmentContaxt())
//                {

//                    var studentsvv = ssc.FleetNames.Select(x => new { x.Fid, x.FleetName }).Distinct().ToList();

//                    if (UserRole.username.ToLower() != "admin")
//                    {
//                        var vsname = ssc.Users.Where(x => x.EmailId.Equals(UserRole.username1)).FirstOrDefault();
//                        if (vsname.AssignVal.ToString() == "FleetName")
//                        {
//                            string[] Fleetname = vsname.FleetName.TrimEnd(',').Split(',');
//                            studentsvv = ssc.FleetNames.Where(x => Fleetname.Contains(x.FleetName)).Select(x => new { x.Fid, x.FleetName }).Distinct().ToList();
//                        }
//                    }
//                    var sportsList = new List<SelectListItem>();
//                    foreach (var item in studentsvv)
//                    {
//                        sportsList.Add(new SelectListItem { Text = item.FleetName, Value = item.FleetName.ToString() });
//                    }

//                    return sportsList;
//                }
//            }
//        }




//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                //sc.Dispose();

//            }

//            base.Dispose(disposing);
//        }

//    }
//}