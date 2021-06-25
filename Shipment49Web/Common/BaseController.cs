using Microsoft.AspNet.Identity.Owin;
using Reports;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Shipment49Web.Common
{
    public class BaseController : Controller
    {
        protected ApplicationSignInManager _signInManager;
        protected ApplicationUserManager _userManager;

        public BaseController()
        { }

        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

        //    if (controllerName.IndexOf("ACCOUNT", System.StringComparison.OrdinalIgnoreCase) < 0 && UserManager == null)
        //    {
        //        filterContext.Result = RedirectToAction("login", "account");
        //    }
        //}

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            protected set
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
            protected set
            {
                _userManager = value;
            }
        }

        public List<VesselDetail> PermittedVessels
        {
            get
            {
                if (Session["PermittedVessels"] == null)
                    return new List<VesselDetail>();
                else
                    return (List<VesselDetail>)Session["PermittedVessels"];
            }
            set
            {
                Session["PermittedVessels"] = value;
            }
        }

        public List<tblCommon> PermittedFleetTypes
        {
            get
            {
                if (Session["PermittedFleetTypes"] == null)
                    return new List<tblCommon>();
                else
                    return (List<tblCommon>)Session["PermittedFleetTypes"];
            }
            set
            {
                Session["PermittedFleetTypes"] = value;
            }
        }

        public List<tblCommon> PermittedFleetNames
        {
            get
            {
                if (Session["PermittedFleetNames"] == null)
                    return new List<tblCommon>();
                else
                    return (List<tblCommon>)Session["PermittedFleetNames"];
            }
            set
            {
                Session["PermittedFleetNames"] = value;
            }
        }

        public List<int> LoggedInUserVesselPermissions
        {
            get
            {
                if (Session["LoggedInUserVesselPermissions"] == null)
                    return new List<int>();
                else
                    return (List<int>)Session["LoggedInUserVesselPermissions"];
            }
            set
            {
                Session["LoggedInUserVesselPermissions"] = value;
            }
        }

        public List<int> LoggedInUserFleetNamePermissions
        {
            get
            {
                if (Session["LoggedInUserFleetNamePermissions"] == null)
                    return new List<int>();
                else
                    return (List<int>)Session["LoggedInUserFleetNamePermissions"];
            }
            set
            {
                Session["LoggedInUserFleetNamePermissions"] = value;
            }
        }

        public List<int> LoggedInUserFleetTypePermissions
        {
            get
            {
                if (Session["LoggedInUserFleetTypePermissions"] == null)
                    return new List<int>();
                else
                    return (List<int>)Session["LoggedInUserFleetTypePermissions"];
            }
            set
            {
                Session["LoggedInUserFleetTypePermissions"] = value;
            }
        }

        public string LoggedInUserPermissionType
        {
            get
            {
                if (Session["LoggedInUserPermissionType"] == null)
                    return string.Empty;
                else
                    return (string)Session["LoggedInUserPermissionType"];
            }
            set
            {
                Session["LoggedInUserPermissionType"] = value;
            }
        }

        public string LoggedInUserID
        {
            get
            {
                if (Session["LoggedInUserID"] == null)
                    return string.Empty;
                else
                    return (string)Session["LoggedInUserID"];
            }
            set
            {
                Session["LoggedInUserID"] = value;
            }
        }

        public string LoggedInUserFullName
        {
            get
            {
                if (Session["LoggedInUserFullName"] == null)
                    return string.Empty;
                else
                    return (string)Session["LoggedInUserFullName"];
            }
            set
            {
                Session["LoggedInUserFullName"] = value;
            }
        }

        public string LoggedInUserEmail
        {
            get
            {
                if (Session["LoggedInUserEmail"] == null)
                    return string.Empty;
                else
                    return (string)Session["LoggedInUserEmail"];
            }
            set
            {
                Session["LoggedInUserEmail"] = value;
            }
        }
    }
}