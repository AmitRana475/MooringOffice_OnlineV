using MenuLayer;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System.Linq;
using System.Web.Mvc;

namespace Shipment49Web.Areas.MSPS.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class shipspecattachmentController : BaseController
    {
        public shipspecattachmentController()
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

        public ActionResult index()
        {
            return View();
        }
    }
}