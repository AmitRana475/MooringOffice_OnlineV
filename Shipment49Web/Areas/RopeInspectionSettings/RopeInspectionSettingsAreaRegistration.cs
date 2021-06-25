using System.Web.Mvc;

namespace Shipment49Web.Areas.RopeInspectionSettings
{
    public class RopeInspectionSettingsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "RopeInspectionSettings";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "RopeInspectionSettings_default",
                "RopeInspectionSettings/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}