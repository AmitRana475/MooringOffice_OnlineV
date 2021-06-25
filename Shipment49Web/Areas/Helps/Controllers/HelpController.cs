using MenuLayer;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Shipment49Web.Areas.Helps.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class HelpController : BaseController
    {
        public HelpController()
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

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TestSession()
        {
            return View();
        }
    }
}