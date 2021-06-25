using MenuLayer;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Shipment49Web.Areas.Setting.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class LogController : BaseController
    {
        public LogController()
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
            using (Reports.MorringOfficeEntities morringOfficeContext = new Reports.MorringOfficeEntities())
            {
                var importLogs = morringOfficeContext.ImportLogs.OrderByDescending(p => p.DateImported).ToList();
                return View(importLogs);
            }
        }
    }
}