using System.Web.Mvc;

namespace Shipment49Web.Areas.WinchBrakeTestRecord
{
    public class WinchBrakeTestRecordAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "WinchBrakeTestRecord";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "WinchBrakeTestRecord_default",
                "WinchBrakeTestRecord/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}