using System.Web.Mvc;

namespace Shipment49Web.Areas.WinchRotationSettings
{
    public class WinchRotationSettingsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "WinchRotationSettings";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "WinchRotationSettings_default",
                "WinchRotationSettings/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}