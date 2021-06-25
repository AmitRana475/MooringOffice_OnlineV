using Shipment49Web.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shipment49Web.Areas.UserHelp.Controllers
{
    [Authorize]
    public class UserHelpController : Controller
    {
        // GET: UserHelp/UserHelp
        public UserHelpController()
        {
            CommonClass.TopeMenuID = "Menu5";
        }
        public ActionResult Index()
        {
            //  string url = "HelpManual/1.0_ABOUT.htm";
           
            //var path = Server.MapPath("~/WebHelp/index.htm");
            //var fullpath = Path.Combine(path, "myfile.txt");
           // string url = "WebHelp/index.htm";
           // ViewBag.help = path;

             return View();

            
        }
    }
}